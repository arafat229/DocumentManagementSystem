using AutoMapper;
using DocumentManagementSystem.Data;
using DocumentManagementSystem.DTOs.DocumentsDtos;
using DocumentManagementSystem.Helpers;
using DocumentManagementSystem.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;

namespace DocumentManagementSystem.Mapper;




[Route("api/[controller]")]
[ApiController]
public class DocumentController : ControllerBase
{

    private readonly ApplicationDBContext dbContext;
    private readonly IMapper mapper;
    
   

    public DocumentController(ApplicationDBContext dbContext, IMapper mapper )
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
       
    }

    [HttpGet("Search")]
    public async Task<IActionResult>SearchFile([FromQuery]FileForSerachDto fileForSerach)
    {
        try
        {
            
            IQueryable<MyFile> file = dbContext.MyFiles;
            if (!string.IsNullOrEmpty(fileForSerach.Name))
            {
                 file = file.Where(x => x.Name.Contains(fileForSerach.Name));

            }
            if (!string.IsNullOrEmpty(fileForSerach.Description))
            {
                 file = file.Where(x => x.Description.Contains(fileForSerach.Description));
            }
            if (fileForSerach.Id > 0)
            {
                 file = file.Where(x => x.Id == fileForSerach.Id);
            }
            if (fileForSerach.CreatedAt!= null)
            {
                 file = file.Where(x => x.CreatedAt.Equals(fileForSerach.CreatedAt));

            }

            return Ok(CallResult.Ok(data: file));

          

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
           return  BadRequest(CallResult.Error("Cannot find the file you are searching for "));
            
        }
      
        

    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateFile([FromForm]FileForCreationDto fileForCreationDto)
    {
        fileForCreationDto.Name = fileForCreationDto.Name.Trim();
        fileForCreationDto.Description = fileForCreationDto.Description.Trim();
     
        long size = fileForCreationDto.File.Length;
        if (size <= 0)
            return BadRequest(CallResult.Error("empty File"));

        List<string> validExtension = new List<string> { ".pdf",".docx" };

        var extension = Path.GetExtension(fileForCreationDto.File.FileName);

        if (!validExtension.Contains(extension))
            return BadRequest(CallResult.Error($"wrong File,system only support {string.Join(",",validExtension)}"));

        var fileName = Guid.NewGuid().ToString() + extension;
        var path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
       using  FileStream stream = new FileStream (Path.Combine(path, fileName),FileMode.Create);

        fileForCreationDto.File.CopyTo(stream);


        var document = mapper.Map<MyFile>(fileForCreationDto);

        document.CreatedAt = DateTime.Now;
     

        await dbContext.MyFiles.AddAsync(document);
      await dbContext.SaveChangesAsync();

        return Ok(CallResult.Ok(data: fileName, message: "File created sucessfully"));
    }

  
    [HttpPut("UpdateFile/{id}")]
    public async Task<IActionResult>UpdateFile(int id,[FromForm]FileForUpdateDto fileForUpdateDto )
    {


        try
        {
            fileForUpdateDto.Description = fileForUpdateDto.Description.Trim();
            fileForUpdateDto.Name = fileForUpdateDto.Name.Trim();



            var fileFromDB = await dbContext.MyFiles.FirstOrDefaultAsync(x => x.Id == id);

            if (fileFromDB != null)
            {
                // fileFromDB.Name = fileForUpdateDto.Name;
                //fileFromDB.Description = fileForUpdateDto.Description;

                var size = fileForUpdateDto.file.Length;

                if (size <= 0)
                    return BadRequest(CallResult.Error("File is empty"));

                List<string> validExtension = new List<string> { ".pdf", ".docx" };

                var extension = Path.GetExtension(fileForUpdateDto.file.FileName);

                if (!validExtension.Contains(extension))
                {
                    BadRequest(CallResult.Error($"wrong file, system only support,{string.Join(",", validExtension)}"));
                }

                var fileName = Guid.NewGuid().ToString() + extension;

                var path = Path.Combine(Directory.GetCurrentDirectory(), "FileUpdates");

                using FileStream stream = new FileStream(Path.Combine(fileName, path), FileMode.Create);
                fileForUpdateDto.file.CopyTo(stream);

                var updatedFiles = mapper.Map<MyFile>(fileForUpdateDto);



                fileFromDB.File = updatedFiles.File;
                fileFromDB.Description = updatedFiles.Description;
                fileFromDB.Name = updatedFiles.Name;
                updatedFiles.CreatedAt = DateTime.Now;


            }

            else
            {
                BadRequest(CallResult.Error("Can't find the File you are trying to access"));

            }


            await dbContext.SaveChangesAsync();

            return Ok(CallResult.Ok("file updated succesfully"));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
            return BadRequest(CallResult.Error("Something went wrong"));

        }



    }


    [HttpDelete("DeleteFile/{id}")]
    public async Task<IActionResult> DeleteFile(int id)
    {
        var fileFromDB = await dbContext.MyFiles.FirstOrDefaultAsync(x => x.Id == id);

        if (fileFromDB is null)
            BadRequest(CallResult.Error("Can't find the File you are trying to access"));

         dbContext.MyFiles.Remove(fileFromDB);

        return Ok(CallResult.Ok("FileDeleted successfully"));
       

    }
}

