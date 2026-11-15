using DalilAcademy.Core.Exceptions;

public class LocalImageService : IImageService
{
    private readonly string _folder = Path.Combine(
        Directory.GetCurrentDirectory(), "wwwroot", "images", "courses");

    private static readonly string[] Allowed = [".jpg", ".jpeg", ".png", ".webp"];

    public async Task<string> SaveCourseImageAsync(Stream content, string fileName, long length)
    {
        if (length == 0 || length > 2 * 1024 * 1024)
            throw new InvalidFileException("Image must be less than 2 MB.");

        var ext = Path.GetExtension(fileName).ToLower();
        if (!Allowed.Contains(ext))
            throw new InvalidFileException("Only jpg, jpeg, png and webp are allowed.");

        if (!await HasValidSignatureAsync(content, ext))
            throw new InvalidFileException("File content does not match its extension.");

        Directory.CreateDirectory(_folder);

        var name = $"{Guid.NewGuid()}{ext}";
        await using var file = File.Create(Path.Combine(_folder, name));
        await content.CopyToAsync(file);

        return "/images/courses/" + name;
    }

    public void DeleteCourseImage(string? imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl)) return;
        var path = Path.Combine(_folder, Path.GetFileName(imageUrl));
        if (File.Exists(path)) File.Delete(path);
    }

    private static async Task<bool> HasValidSignatureAsync(Stream content, string ext)
    {
        var header = new byte[12];
        var read = await content.ReadAsync(header, 0, header.Length);
        content.Position = 0;

        if (read < 4) return false;

        return ext switch
        {
            ".jpg" or ".jpeg" => header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
            ".png" => header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47,
            ".webp" => header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46,
            _ => false
        };
    }
}