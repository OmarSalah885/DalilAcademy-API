using DalilAcademy.Core.Entities;
using DalilAcademy.Core.Service;
using Microsoft.AspNetCore.Mvc;

namespace DalilAcademy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController(ICourseService service) : ControllerBase
{
    [HttpGet]
    public ActionResult<List<Course>> GetAll() => Ok(service.GetAll());

    [HttpGet("{id:int}")]
    public ActionResult<Course> GetById(int id)
    {
        var course = service.GetById(id);
        return course is null ? NotFound() : Ok(course);
    }

    [HttpPost]
    public ActionResult<Course> Create(Course course)
    {
        try
        {
            var created = service.Create(course);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public ActionResult<Course> Update(int id, Course course)
    {
        try
        {
            var updated = service.Update(id, course);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var deleted = service.Delete(id);
        return deleted ? NoContent() : NotFound();
    }
}