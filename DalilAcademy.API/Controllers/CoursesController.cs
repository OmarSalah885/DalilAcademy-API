using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Service;
using DalilAcademy.Infra.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace DalilAcademy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CoursesController(ICourseService courseService,IImageService images) : ControllerBase
{
    [HttpGet("all")]   // → GET /api/courses/all
    [AllowAnonymous]
    public async Task<ActionResult<List<CourseResponseDto>>> GetAll()
    {
        var result = await courseService.GetAllAsync();
        return Ok(result);
    }
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<CourseResponseDto>>> GetCourses(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] decimal? minPrice = null,
    [FromQuery] decimal? maxPrice = null)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var result = await courseService.GetPagedAsync(page, pageSize, minPrice, maxPrice);
        return Ok(result);
    }
    [HttpGet("count")]
    [AllowAnonymous]
    public async Task<ActionResult<int>> GetCoursesCount(
    [FromQuery] decimal? minPrice = null,
    [FromQuery] decimal? maxPrice = null)
    {
        var count = await courseService.GetCountAsync(minPrice, maxPrice);
        return Ok(count);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<CourseResponseDto>> GetById(int id)
    {
        var course = await courseService.GetByIdAsync(id);
        return Ok(course);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<CourseResponseDto>> Create(CreateCourseDto dto)
    {
        var created = await courseService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Instructor}")]
    public async Task<IActionResult> Update(int id, UpdateCourseDto dto)
    {
        await courseService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]

    public async Task<IActionResult> Delete(int id)
    {
        await courseService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("upload-image")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult> UploadImage(IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        var url = await images.SaveCourseImageAsync(stream, file.FileName, file.Length);
        return Ok(new { imageUrl = url });
    }

    [HttpGet("with-enrollments")]
    [Authorize]   
    public async Task<ActionResult<List<CourseWithEnrollmentsDto>>> GetAllWithEnrollments()
    {
        var result = await courseService.GetAllWithEnrollmentsAsync();
        return Ok(result);
    }
}