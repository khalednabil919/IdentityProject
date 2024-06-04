using BusinessLogic.CustomAuthorization;
using Core.Entities;
using Core.Interfaces;
using DataTransferObject.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdentityProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles ="edit,Visitor")]
    public class RegionsController : ControllerBase
    {
        private readonly IRegionService _regionService;
        public RegionsController(IRegionService regionService)
        {
            _regionService = regionService;
        }
        [HttpPost("CreateRegion")]
        //[Authorization("")]
        [Authorize(Policy = "CreateRegionPolicy")]
        //[Authorize(Policy = "FirstPolicy")]
        //[Authorize(Roles = "Create,Khaled")]
        //[Authorize(Roles ="Visitor")]
        //[Authorize(Roles ="edit")]
        //[Authorize(Roles ="Create")]
        public async Task<IActionResult> CreateRegion(Region region)
        {
             if (!ModelState.IsValid)
                return BadRequest(new APIResult { message = ModelState.ToString() });
            var regionResult = await _regionService.Create(region);
            return Ok(regionResult);
        }

        [HttpGet("GetAll")]
        [Authorize(Policy = "SuperAdminandVisitorandCreate")]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid)
                return BadRequest(new APIResult { message = ModelState.ToString() });
            var region = await _regionService.GetAll();
            return Ok(region);
        }

        [Authorize(Policy = "SuperAdminORVisitorandCreate")]
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
