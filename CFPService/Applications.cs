using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CFPService;

public class Applications
{
    public Guid Id { get; set; }
    public Guid Author { get; set; }
    public string? ActivityName { get; set; }
    [ForeignKey("ActivityName")]
    public Activities? Activity { get; set; }
    [MaxLength(100)]
    public string? Name { get; set; }
    [MaxLength(300)]
    public string? Description { get; set; }
    [MaxLength(1000)]
    public string? Outline { get; set; }
    [MaxLength(1000)]
    public DateTime CreatedTime { get; set; }
    public DateTime LastModificationTime { get; set; }
    public DateTime SubmittedTime { get; set; }
    public Statuses Status { get; set; }
    
}

public class Activities
{
    [Key]
    public string Activity { get; set; }
    [MaxLength(300)]
    public string Description { get; set; }
}

public enum Statuses
{
    Created,
    OnSubmitting,
}

