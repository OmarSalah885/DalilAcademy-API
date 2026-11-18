using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DalilAcademy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentsController(IEnrollmentService enrollmentService,
    IStudentService studentService,
    IPaymentService paymentService) : ControllerBase
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

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(int id, UpdateEnrollmentDto dto)
    {
        await enrollmentService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await enrollmentService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id:int}/pay")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PaymentOrderResponseDto>> Pay(int id)
    {
        var result = await paymentService.PayAsync(id);
        return Ok(result);
    }

    [HttpPost("{id:int}/capture")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EnrollmentResponseDto>> Capture(int id)
    {
        var result = await paymentService.CaptureAsync(id);
        return Ok(result);
    }

    [HttpGet("{id:int}/payments")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<PaymentResponseDto>>> GetPayments(int id)
    {
        var payments = await paymentService.GetPaymentsByEnrollmentIdAsync(id);
        return Ok(payments);
    }
}