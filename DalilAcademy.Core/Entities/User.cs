public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty; // never the real password
    public string Role { get; set; } = Roles.Student;
}

public static class Roles
{
    public const string Admin = "Admin";
    public const string Instructor = "Instructor";
    public const string Student = "Student";
}