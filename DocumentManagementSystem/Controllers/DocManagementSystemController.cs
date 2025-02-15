using DocumentManagementSystem.Helpers;
using DocumentManagementSystem.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DocumentManagementSystem.Mapper;




    [Route("api/[controller]")]
    [ApiController]
    public class DocManagementSystemController : ControllerBase
    {
        private readonly IDocManagementSystem _docManagementSystemRepo;

        public DocManagementSystemController(IDocManagementSystem docManagementSystemRepo)
        {
            _docManagementSystemRepo = docManagementSystemRepo;

        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        
        [HttpGet]
       [Authorize]
    public async Task<IActionResult>GetDocument([FromQuery]QueryObject query)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
       
            var document = await _docManagementSystemRepo.GetDocument(query);  
            if (document == null )
            {
                return NotFound("Document not found"); 
            }

            return Ok(document.ToDocManagementDto());
        }

    }

