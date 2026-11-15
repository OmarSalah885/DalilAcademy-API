using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Service;
using DalilAcademy.Infra.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DalilAcademy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class StudentsController(IStudentService studentService) : ControllerBase
{
    [HttpGet("all")]


    public async Task<ActionResult<List<StudentResponseDto>>> GetAll()
    {
        var students = await studentService.GetAllAsync();
        return Ok(students);
    }
    [HttpGet]


    public async Task<ActionResult<PagedResult<StudentResponseDto>>> GetStudents(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
    {
        page = Math.Max(1, page);       
        pageSize = Math.Clamp(pageSize, 1, 50);      

        var result = await studentService.GetPagedAsync(page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id:int}")]

    public async Task<ActionResult<StudentResponseDto>> GetById(int id)
    {
        var student = await studentService.GetByIdAsync(id);
        return Ok(student);
    }

    [HttpGet("{studentId}/courses")]

    public async Task<ActionResult<List<CourseResponseDto>>> GetCoursesByStudentId(int studentId)
    {
        var courses = await studentService.GetCoursesByStudentIdAsync(studentId);

        return Ok(courses);
    }

    [HttpPost]


    public async Task<ActionResult<StudentResponseDto>> Create(CreateStudentDto dto)
    {
        var created = await studentService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]


    public async Task<IActionResult> Update(int id, UpdateStudentDto dto)
    {
        await studentService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]


    public async Task<IActionResult> Delete(int id)
    {
        await studentService.DeleteAsync(id);
        return NoContent();
    }
}