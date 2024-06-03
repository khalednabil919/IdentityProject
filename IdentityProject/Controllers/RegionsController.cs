using BusinessLogic.CustomAuthorization;
using Core.Entities;
using Core.Interfaces;
using DataTransferObject.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly IRegionService _regionService;
        public RegionsController(IRegionService regionService)
        {
            _regionService = regionService;
        }
        [HttpPost("CreateRegion")]
        //[Authorization("")]
        [Authorize(Roles ="edit,delete")]
        public async Task<IActionResult> CreateRegion(Region region)
        {
            if (!ModelState.IsValid)
                return BadRequest(new APIResult { message = ModelState.ToString() });
            var regionResult = await _regionService.Create(region);
            return Ok(regionResult);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid)
                return BadRequest(new APIResult { message = ModelState.ToString() });
            var region = await _regionService.GetAll();
            return Ok(region);
        } 
        [HttpGet("GetRegion/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(new APIResult { message = ModelState.ToString() });

            return Ok(await _regionService.GetByID(id));
        }
        [HttpPut]
        public async Task<IActionResult> Update(Region region)
        {
            if (!ModelState.IsValid)
                return BadRequest(new APIResult { message = ModelState.ToString() });

            return Ok(await _regionService.Update(region));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(new APIResult { message = ModelState.ToString() });

            return Ok(await _regionService.Delete(id));
        }
    }
}
