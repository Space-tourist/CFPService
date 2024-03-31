using System.ComponentModel.DataAnnotations;
namespace CFPService.Contracts;

public class EditApplicationRequestDto
{
    public string? Activity { get; set; }
    [MaxLength(100)]
    public string? Name { get; set; }
    [MaxLength(300)]
    public string? Description { get; set; }
    [MaxLength(1000)]
    public string? Outline { get; set; }
}