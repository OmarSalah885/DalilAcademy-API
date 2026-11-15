using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Entities;
using DalilAcademy.Core.Repository;
using DalilAcademy.Core.Service;
using DalilAcademy.Infra.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DalilAcademy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentsController(IEnrollmentService enrollmentService,IStudentService studentService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<EnrollmentResponseDto>>> GetAll()
        => Ok(await enrollmentService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EnrollmentResponseDto>> GetById(int id)
        => Ok(await enrollmentService.GetByIdAsync(id));

    [HttpGet("me")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<List<CourseResponseDto>>> MyCourses()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdClaim) ||
            !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var courses = await studentService.GetCoursesByUserIdAsync(userId);

        return Ok(courses);
    }

    [HttpPost]
    public async Task<ActionResult<EnrollmentResponseDto>> Create(CreateEnrollmentDto dto)
    {
        var created = await enrollmentService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await enrollmentService.DeleteAsync(id);
        return NoContent();
    }
}