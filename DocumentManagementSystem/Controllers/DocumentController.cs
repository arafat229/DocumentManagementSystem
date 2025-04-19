using AutoMapper;
using DocumentManagementSystem.Data;
using DocumentManagementSystem.DTOs.DocumentsDtos;
using DocumentManagementSystem.Helpers;
using DocumentManagementSystem.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DocumentManagementSystem.Mapper;




[Route("api/[controller]")]
[ApiController]
public class DocumentController : ControllerBase
{

    private readonly ApplicationDBContext dbContext;
    private readonly IMapper mapper;
    private readonly IFormFile file;

    public DocumentController(ApplicationDBContext dbContext, IMapper mapper, IFormFile file)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.file = file;
    }

    [HttpGet("GetFile/{id}")]
    public async Task<IActionResult>GetFile(int id)
    {

        var fileFromDB = await dbContext.MyFiles.FirstOrDefaultAsync(x => x.Id == id);

        var file = mapper.Map<FileForReturnDto>(fileFromDB);

        return Ok(CallResult.Ok(data: file));

    }

    [HttpPost("File/create/{userId}")]
    public async Task<IActionResult> CreateFileForUser(int id, [FromForm]FileForCreationDto fileForCreationDto )
    {
        var fileFromDB = await dbContext.MyFiles.FirstOrDefaultAsync(x => x.Id == id);

        if (fileFromDB is null)
            BadRequest(CallResult.Error("Can't find the file you are trying to access"));


        var uploadedfile = new UploadHandler().Upload(file);

        fileForCreationDto.CreatedAt = DateTime.UtcNow;

        fileForCreationDto.file = uploadedfile.ToString();
         

        var document = mapper.Map<MyFile>(fileForCreationDto);

       
        await dbContext.MyFiles.AddAsync(document);

        

        if (await dbContext.SaveChangesAsync() <= 0)
            return BadRequest(CallResult.Error("unable to update the file"));


        var fileToReturn = mapper.Map<FileForReturnDto>(document);
        return CreatedAtRoute("GetFile", new {id = document.Id},fileToReturn);
    }

  
    [HttpPut("UpdateFile/{id}")]
    public async Task<IActionResult>UpdateFile(int id,[FromForm]FileForUpdateDto fileForUpdateDto )
    {
        fileForUpdateDto.Description = fileForUpdateDto.Description.ToLower().Trim();
        fileForUpdateDto.Name = fileForUpdateDto.Name.ToLower().Trim();
        fileForUpdateDto.Categories = fileForUpdateDto.Categories.ToLower().Trim();
        fileForUpdateDto.file = fileForUpdateDto.file.Trim();
       

         var fileFromDB = await dbContext.MyFiles.FirstOrDefaultAsync(x => x.Id == id);

        if (fileFromDB is null)
            BadRequest(CallResult.Error("Can't find the file you are trying to access"));

        fileForUpdateDto.CreatedAt = DateTime.UtcNow;
        mapper.Map(fileForUpdateDto, fileFromDB);

         dbContext.Update(fileFromDB);



        if (await dbContext.SaveChangesAsync() <= 0)
            return BadRequest(CallResult.Error("unable to update user"));

        return Ok(CallResult.Ok());

    }


    [HttpDelete("DeleteFile/{id}")]
    public async Task<IActionResult> DeleteFile(int id)
    {
        var fileFromDB = await dbContext.MyFiles.FirstOrDefaultAsync(x => x.Id == id);

        if (fileFromDB is null)
            BadRequest(CallResult.Error("Can't find the file you are trying to access"));

        var result = dbContext.MyFiles.Remove(fileFromDB);
        return Ok(CallResult.Ok());
       

    }
}

