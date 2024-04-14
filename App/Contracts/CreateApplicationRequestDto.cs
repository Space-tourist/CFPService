using System.ComponentModel.DataAnnotations;

namespace App.Contracts;

public class CreateApplicationRequestDto
{
    public Guid? Author { get; set; }
    public string? Activity { get; set; }
    [MaxLength(100)]
    public string? Name { get; set; }
    [MaxLength(300)]
    public string? Description { get; set; }
    [MaxLength(1000)]
    public string? Outline { get; set; }
}