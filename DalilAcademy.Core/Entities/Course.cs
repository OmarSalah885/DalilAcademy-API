namespace DalilAcademy.Core.Entities;

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string Level { get; set; } = "Beginner";
    public string? ImageUrl { get; set; }
    public int? MaxStudents { get; set; }
    public int InstructorId { get; set; }
    public Instructor Instructor { get; set; } = null!;



    public List<Enrollment> Enrollments { get; set; } = [];
}