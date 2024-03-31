using CFPService.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CFPService.Controllers;

[ApiController]
[Route("[controller]")]
public class ApplicationsController : Controller
{
    private readonly ApplicationContext _applicationContext;

    public ApplicationsController(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }
    
    [HttpPost]
    public async Task<ApplicationResponseDto> PostApplication(CreateApplicationRequestDto app)
    {
        if (_applicationContext.Applications.Count(t => t.Author == app.Author && t.Status == Statuses.Created) > 1)
        {
            throw new ArgumentException();
        }

        if (app.Author == Guid.Empty || (string.IsNullOrWhiteSpace(app.Activity) 
                                         && string.IsNullOrWhiteSpace(app.Name)
                                         && string.IsNullOrWhiteSpace(app.Description)
                                         && string.IsNullOrWhiteSpace(app.Outline)))
        {
            throw new ArgumentException();
        }
        
        var act = await _applicationContext.Activities
            .FirstOrDefaultAsync(t => t.Activity == app.Activity);
        if (act is  null && !string.IsNullOrWhiteSpace(app.Activity))
        {
            throw new ArgumentException();
        }
        
        Applications createdApp = new Applications
        {
            Author = app.Author,
            Activity = act,
            Name = app.Name,
            Description = app.Description,
            Outline = app.Outline,
            CreatedTime = DateTime.UtcNow,
            LastModificationTime = DateTime.UtcNow,
            Status = Statuses.Created
        };
        await _applicationContext.Applications.AddAsync(createdApp);
        await _applicationContext.SaveChangesAsync();
        ApplicationResponseDto responseApp = new ApplicationResponseDto
        {
            Id = createdApp.Id,
            Author = createdApp.Author,
            Activity = createdApp.Activity?.Activity ?? string.Empty,
            Name = createdApp.Name,
            Description = createdApp.Description,
            Outline = createdApp.Outline
        };

        return responseApp;
    }

    [HttpPut("{applicationId}")]
    public async Task<ApplicationResponseDto> PutApplication(Guid applicationId, EditApplicationRequestDto app)
    {
        var editableApp = await _applicationContext.Applications
            .FirstOrDefaultAsync(t => t.Id == applicationId);

        if (editableApp is null || editableApp.Status == Statuses.OnSubmitting)
        {
            throw new ArgumentException();
        }

        var act = await _applicationContext.Activities
            .FirstOrDefaultAsync(t => t.Activity == app.Activity);

        if (act is null)
        {
            throw new ArgumentException();
        }
        editableApp.Activity = act;
        editableApp.Name = !string.IsNullOrWhiteSpace(app.Name) ? app.Name : editableApp.Name;
        editableApp.Description = !string.IsNullOrWhiteSpace(app.Description) ? app.Description : editableApp.Description;
        editableApp.Outline = !string.IsNullOrWhiteSpace(app.Outline) ? app.Outline : editableApp.Outline;
        editableApp.LastModificationTime=DateTime.UtcNow;

        if (editableApp.Activity is null 
            && string.IsNullOrWhiteSpace(editableApp.Name)
            && string.IsNullOrWhiteSpace(editableApp.Description)
            && string.IsNullOrWhiteSpace(editableApp.Outline))
        {
            throw new ArgumentException();
        }
        
        await _applicationContext.SaveChangesAsync();
        
        ApplicationResponseDto responseApp = new ApplicationResponseDto
        {
            Id = editableApp.Id,
            Author = editableApp.Author,
            Activity = editableApp.Activity?.Activity ?? string.Empty,
            Name = editableApp.Name,
            Description = editableApp.Description,
            Outline = editableApp.Outline
        };
        return responseApp;
    }

    [HttpDelete("{applicationId}")]
    public async Task DeleteApplication(Guid applicationId)
    {
        var appForDelete = await _applicationContext
            .Applications
            .Include(applications => applications.Activity)
            .FirstOrDefaultAsync(t => t.Id == applicationId);

        if (appForDelete is null || appForDelete.Status == Statuses.OnSubmitting)
        {
            throw new ArgumentException();
        }
        _applicationContext.Applications.Remove(appForDelete);
        
        await _applicationContext.SaveChangesAsync();
    }

    [HttpPost("{applicationId}/submit")]
    public async void SubmitApplication(Guid applicationId)
    {
        var appForSubmit = await _applicationContext
            .Applications
            .Include(applications => applications.Activity)
            .FirstOrDefaultAsync(t => t.Id == applicationId);

        if (appForSubmit is  null || (appForSubmit.Activity is null
                                        && string.IsNullOrWhiteSpace(appForSubmit.Name)
                                        && string.IsNullOrWhiteSpace(appForSubmit.Outline)))
        {
            throw new ArgumentException();
        }
        appForSubmit.Status = Statuses.OnSubmitting;
        appForSubmit.SubmittedTime = DateTime.UtcNow;
        await _applicationContext.SaveChangesAsync();
    }

    [HttpGet]
    public ApplicationResponseDto[] GetSubmittedAfter(DateTime? submittedAfter, DateTime? unsubmittedOlder) 
    {
        if ((submittedAfter is not null && unsubmittedOlder is not null)
            || (submittedAfter is null && unsubmittedOlder is null))
        {
            throw new ArgumentException();
        }

        if (submittedAfter is not null)
        {
            if (submittedAfter.Value.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException();
            }

            var appsSubmittedAfter = _applicationContext.Applications
                .Where(t => t.SubmittedTime >= submittedAfter && t.Status == Statuses.OnSubmitting)
                .ToArray();

            return appsSubmittedAfter
                .Select(t => new ApplicationResponseDto
                {
                    Id = t.Id,
                    Author = t.Author,
                    Activity = t.Activity?.Activity ?? string.Empty,
                    Name = t.Name,
                    Description = t.Description,
                    Outline = t.Outline
                })
                .ToArray();
        }

        if (unsubmittedOlder is not null)
        {
            if (unsubmittedOlder.Value.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException();
            }

            var appsUnsubmittedOlder = _applicationContext.Applications
                .Include(t => t.Activity)
                .Where(t => t.Status == Statuses.Created)
                .Where(t => t.CreatedTime < unsubmittedOlder)
                .ToArray();

            return appsUnsubmittedOlder
                .Select(t => new ApplicationResponseDto
                {
                    Id = t.Id,
                    Author = t.Author,
                    Activity = t.Activity?.Activity ?? string.Empty,
                    Name = t.Name,
                    Description = t.Description,
                    Outline = t.Outline
                }).ToArray();
        }

        return Array.Empty<ApplicationResponseDto>();
    }

    [HttpGet("{applicationId}")]
    public async Task<ApplicationResponseDto> GetApplication(Guid applicationId)
    {
        var application = _applicationContext.Applications
            .Include(applications => applications.Activity)
            .FirstOrDefault(t => t.Id==applicationId);

        if (application is null)
        {
            throw new ArgumentException();
        }

        ApplicationResponseDto responseApp = new ApplicationResponseDto
        {
            Id = application.Id,
            Author = application.Author,
            Activity = application.Activity?.Activity ?? string.Empty,
            Name = application.Name,
            Description = application.Description,
            Outline = application.Outline
        };
        return responseApp;
        
    }
}
