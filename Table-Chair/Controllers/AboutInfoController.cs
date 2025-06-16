using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos.AboutInfoDtos;
using Table_Chair_Application.Services.InterfaceServices;

namespace Table_Chair.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AboutInfoController : ControllerBase
    {
        private readonly IAboutInfoService _aboutInfoService;

        public AboutInfoController(IAboutInfoService aboutInfoService)
        {
            _aboutInfoService = aboutInfoService;
        }
        //https://localhost:7179/api/AboutInfo/getall
        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _aboutInfoService.GetAllAsync();
            return Ok(result);
        }
        //https://localhost:7179/api/AboutInfo/GetById/Id
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _aboutInfoService.GetByIdAsync(id);
            return Ok(result);
        }
        //https://localhost:7179/api/AboutInfo/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] AboutInfoCreateDto dto)
        {
            await _aboutInfoService.CreateAsync(dto);
            return Ok(new { message = "AboutInfo successfully created!" });
        }
        //https://localhost:7179/api/AboutInfo/Update/Id
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AboutInfoCreateDto dto)
        {
            await _aboutInfoService.UpdateAsync(id, dto);
            return Ok(new { message = "AboutInfo successfully updated!" });
        }
        //https://localhost:7179/api/AboutInfo/delete/Id
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _aboutInfoService.DeleteAsync(id);
            return Ok(new { message = "AboutInfo successfully deleted!" });
        }
    }
}

