using AutoMapper;
using DocumentManagementSystem.Data;
using DocumentManagementSystem.DTOs.DocumentsDtos;
using DocumentManagementSystem.Helpers;
using DocumentManagementSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Xml;
using System;
using System.IO;
using IronOcr;
using Patagames.Ocr;
using Patagames.Ocr.Enums;
using SautinSoft.Document;
using TesseractOCR;
using TesseractOCR.Enums;
using SixLabors.ImageSharp.Formats.Png;


namespace DocumentManagementSystem.Mapper;




[Route("api/[controller]")]
[ApiController]
public class DocumentController : ControllerBase
{

    private readonly ApplicationDBContext dbContext;
    private readonly IMapper mapper;
    private readonly UserManager<User> userManager;
    
   

    public DocumentController(ApplicationDBContext dbContext, IMapper mapper, UserManager<User> usermanager)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.userManager = usermanager;
    }

    [HttpGet("Search")]
    public async Task<IActionResult> SearchFile([FromQuery] FileForSerachDto fileForSerach)
    {
        try
        {

   

            List<MyFile> fileList = new();

            if (string.IsNullOrEmpty(fileForSerach.SearchingTerm))
            {
                List<MyFile> filesFromDb = await dbContext.MyFiles.ToListAsync();

                fileList.AddRange(filesFromDb);
              
            }

              
            if (!string.IsNullOrEmpty(fileForSerach.SearchingTerm))
            {
             

                fileForSerach.SearchingTerm = fileForSerach.SearchingTerm.ToLower().Trim();

                List<MyFile> files = await dbContext.MyFiles.Where(f =>
                f.Name.ToLower().Contains(fileForSerach.SearchingTerm) ||
                f.Description.ToLower().Contains(fileForSerach.SearchingTerm) ||
                f.Extractedtext.ToLower().Contains(fileForSerach.SearchingTerm)).ToListAsync();

                foreach(var file in files)
                   fileList.Add(file);
                   
            }
            

            var res = fileList.Select(x => new
            {
                x.Name,
                x.Description,
                x.File,
                x.CreatedAt
                
            }).ToList();
           
            
            return Ok(CallResult.Ok(data:res));
            

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
            return BadRequest(CallResult.Error("Something went wrong"));

        }



    }

    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> CreateFile([FromForm]FileForCreationDto dto)
    {
        dto.Name = dto.Name.Trim();
        dto.Description = dto.Description?.Trim();
     
        long size = dto.File.Length;
        if (size == 0)
            return BadRequest(CallResult.Error("empty File"));

        List<string> validExtension = [".pdf", ".jpg"];

        var extension = Path.GetExtension(dto.File.FileName);

        if (!validExtension.Contains(extension))
            return BadRequest(CallResult.Error($"wrong File,system only support {string.Join(",",validExtension)}"));

       
       
        var path = Path.Combine(Directory.GetCurrentDirectory(),"Uploads");
        
        var name = $"{Guid.NewGuid()}{extension}"; 
        var destination = Path.Combine(path, name);

        using var stream = dto.File.OpenReadStream();
        using var bufferStream = new BufferedStream(stream);
        using MemoryStream ms = new();
        await bufferStream.CopyToAsync(ms);
        var fileByte = ms.ToArray();
       // var fileString = Convert.ToBase64String(fileByte);

        await System.IO.File.WriteAllBytesAsync(destination, fileByte);


        if (!Path.GetExtension(destination).Contains(".png"))
        {
            byte[] png = Freeware.Pdf2Png.Convert(stream,1);

            await System.IO.File.WriteAllBytesAsync(destination, png);

        }


        using var engine = new Engine(@"C:\Users\ASAKE\source\repos\DocumentManagementSystem\DocumentManagementSystem\bin\Debug\net9.0\tessdata", Language.English,EngineMode.Default);
        using var img = TesseractOCR.Pix.Image.LoadFromFile(destination);
        using var page = engine.Process(img);

        string text = page.Text;



        var document = new MyFile
        {
            Name = dto.Name,
            Description = dto.Description,
            CreatedAt = DateTime.Now,
            userId = Convert.ToInt64(HttpContext.User.FindFirstValue("UserId")),
            File = destination,
            Extractedtext = text

        };


      dbContext.Add(document);
      await dbContext.SaveChangesAsync();

        return Ok(CallResult.Ok(message: "File created sucessfully"));
    }

  
    [HttpPut("UpdateFile/{id}")]
    public async Task<IActionResult>UpdateFile(long id,[FromForm]FileForUpdateDto dto )
    {


        try
        {

            var fileFromDb = await dbContext.MyFiles.FirstOrDefaultAsync(x => x.Id == id);

            if (fileFromDb is null)
                return BadRequest(CallResult.Error("non-existing UploadedFile"));


            if (dto.Name is null && dto.File is null && dto.Description is null)
                return BadRequest(CallResult.Error("No content for update"));



            if (!string.IsNullOrEmpty(dto.Name))
                fileFromDb.Name = dto.Name.Trim();



            if (!string.IsNullOrEmpty(dto.Description))   
                fileFromDb.Description = dto.Description.Trim();
            
               
  
            if (dto.File is not null)
            {

                if (dto.File.Length == 0)
                    return BadRequest(CallResult.Error("Empty UploadedFile"));


                List<string> validExtension = [".pdf", ".jpg", ".jpeg"];

                var extension = Path.GetExtension(dto.File.FileName);

                if (!validExtension.Contains(extension))
                    return BadRequest(CallResult.Error($"Wrong UploadedFile format,{string.Join(",", validExtension)} are supported files"));


                  System.IO.File.Delete(fileFromDb.File);
                

                var fileName = $"{Guid.NewGuid()}{extension}";

                var path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

                var destination = Path.Combine(path,fileName);

            ;

                 
                 fileFromDb.File = destination;
              
                using var stream = dto.File.OpenReadStream();
                using var bufferStream = new BufferedStream(stream);
                using MemoryStream ms = new();
                await bufferStream.CopyToAsync(ms);
                var fileByte = ms.ToArray();

                await System.IO.File.WriteAllBytesAsync(destination, fileByte);

                if (!Path.GetExtension(destination).Contains(".png"))
                {
                    byte[] png = Freeware.Pdf2Png.Convert(stream, 1);

                    await System.IO.File.WriteAllBytesAsync(destination, png);

                }

                using var engine = new Engine(@"C:\Users\ASAKE\source\repos\DocumentManagementSystem\DocumentManagementSystem\bin\Debug\net9.0\tessdata", Language.English, EngineMode.Default);
                using var img = TesseractOCR.Pix.Image.LoadFromFile(destination);
                using var page = engine.Process(img);

                string text = page.Text;

                fileFromDb.Extractedtext = text;
            }

            fileFromDb.UpdatedAt = DateTime.UtcNow;

            dbContext.MyFiles.Update(fileFromDb);

                await dbContext.SaveChangesAsync();
              
            
            return Ok(CallResult.Ok("fileFromDB updated succesfully"));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
            return BadRequest(CallResult.Error("Something went wrong"));

        }



    }


    [HttpDelete("DeleteFile/{id}")]
    public async Task<IActionResult> DeleteFile(string id )
    {
        var fileFromDB = await dbContext.MyFiles.FirstOrDefaultAsync(x => x.Id.ToString() == id);

        if (fileFromDB is null)
         return BadRequest(CallResult.Error("Can't find the File you are trying to access"));


            dbContext.MyFiles.Remove(fileFromDB);
      

         await dbContext.SaveChangesAsync();

        return Ok(CallResult.Ok("File deleted succesfully"));
        
    }
}

