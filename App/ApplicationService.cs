using App.Contracts;
using App.Exceptions;
using DataAccess;
using Domain;

namespace App;

public class ApplicationService
{
    private readonly ApplicationRepository _applicationRepository;
    private readonly RequestValidator _requestValidator;

    public ApplicationService(ApplicationRepository applicationRepository, RequestValidator requestValidator)
    {
        _applicationRepository = applicationRepository;
        _requestValidator = requestValidator;
    }

    public async Task<ApplicationResponseDto> CreateApplication(CreateApplicationRequestDto request)
    {
        _requestValidator.ValidateCreateApplicationRequest(request);
     
        var parsedActivityType = TryParseActivityType(request.Activity);

        Applications createdApp = new Applications
        {
            Author = request.Author.Value,
            Activity = parsedActivityType,
            Name = request.Name,
            Description = request.Description,
            Outline = request.Outline,
            CreatedTime = DateTime.UtcNow,
            LastModificationTime = DateTime.UtcNow,
            Status = ApplicationStatus.Created
        };

        await _applicationRepository
            .CreateApplicationAsync(createdApp);

        ApplicationResponseDto responseApp = new ApplicationResponseDto
        {
            Id = createdApp.Id,
            Author = createdApp.Author,
            Activity = createdApp.Activity?.ToString() ?? string.Empty,
            Name = createdApp.Name,
            Description = createdApp.Description,
            Outline = createdApp.Outline
        };

        return responseApp;
    }

    public async Task<ApplicationResponseDto> UpdateApplication(Guid applicationId, EditApplicationRequestDto app)
    {
        var editableApp = await _applicationRepository
            .GetApplicationAsync(applicationId);

        RequestValidator.ValidateEditApplicationRequest(editableApp);

        var parsedActivityType = TryParseActivityType(app.Activity ?? string.Empty);

        editableApp.Activity = parsedActivityType;
        editableApp.Name = app.Name;
        editableApp.Description = app.Description;
        editableApp.Outline = app.Outline;
        
        editableApp.LastModificationTime = DateTime.UtcNow;

        RequestValidator.ValidateEditedApplication(editableApp);

        await _applicationRepository.UpdateApplicationAsync();
        
        ApplicationResponseDto responseApp = new ApplicationResponseDto
        {
            Id = editableApp.Id,
            Author = editableApp.Author,
            Activity = editableApp.Activity?.ToString() ?? string.Empty,
            Name = editableApp.Name,
            Description = editableApp.Description,
            Outline = editableApp.Outline
        };
        return responseApp;
    }

    public async Task DeleteApplication(Guid applicationId)
    {
        var appForDelete = await _applicationRepository
            .GetApplicationAsync(applicationId);

        RequestValidator.ValidateEditApplicationRequest(appForDelete);

        await _applicationRepository
            .RemoveApplicationAsync(appForDelete);
    }

    public async Task SubmitApplication(Guid applicationId)
    {
        var appForSubmit = await _applicationRepository
            .GetApplicationAsync(applicationId);

        RequestValidator.ValidateSubmitRequest(appForSubmit);
        
        appForSubmit.Status = ApplicationStatus.OnSubmitting;
        appForSubmit.SubmittedTime = DateTime.UtcNow;

        await _applicationRepository.UpdateApplicationAsync();
    }

    public async Task<ApplicationResponseDto[]> GetApplicationsByFilter(DateTime? submittedAfter,
        DateTime? unsubmittedOlder)
    {
       RequestValidator.ValidateFilterRequest(submittedAfter, unsubmittedOlder);

        if (submittedAfter is not null)
        {
            var submittedUtcTime = submittedAfter.Value.ToUniversalTime();

            var appsSubmittedAfter = await _applicationRepository
                .GetSubmittedApplications(submittedUtcTime);

            return appsSubmittedAfter
                .Select(t => new ApplicationResponseDto
                {
                    Id = t.Id,
                    Author = t.Author,
                    Activity = t.Activity?.ToString() ?? string.Empty,
                    Name = t.Name,
                    Description = t.Description,
                    Outline = t.Outline
                })
                .ToArray();
        }

        if (unsubmittedOlder is not null)
        {
            var unsubmittedUtcTime = unsubmittedOlder.Value.ToUniversalTime();

            var appsUnsubmittedOlder = await _applicationRepository
                .GetUnsubmittedApplications(unsubmittedUtcTime);

            return appsUnsubmittedOlder
                .Select(t => new ApplicationResponseDto
                {
                    Id = t.Id,
                    Author = t.Author,
                    Activity = t.Activity?.ToString() ?? string.Empty,
                    Name = t.Name,
                    Description = t.Description,
                    Outline = t.Outline
                }).ToArray();
        }
        throw new NotFoundApplicationException("Заявка не найдена");
    }

    public async Task<ApplicationResponseDto> GetApplication(Guid applicationId)
    {
        var application = await _applicationRepository
            .GetApplicationAsync(applicationId);

        RequestValidator.ValidateApplicationExistence(application);

        ApplicationResponseDto responseApp = new ApplicationResponseDto
        {
            Id = application.Id,
            Author = application.Author,
            Activity = application.Activity?.ToString() ?? string.Empty,
            Name = application.Name,
            Description = application.Description,
            Outline = application.Outline
        };
        return responseApp;
    }

    public async Task<ApplicationResponseDto> GetCurrentAuthorsApplication(Guid authorId)
    {
        var currentApp = await _applicationRepository.GetCurrentAuthorsApplication(authorId);

        RequestValidator.ValidateApplicationExistence(currentApp);

        ApplicationResponseDto responseApp = new ApplicationResponseDto
        {
            Id = currentApp.Id,
            Author = currentApp.Author,
            Activity = currentApp.Activity?.ToString() ?? string.Empty,
            Name = currentApp.Name,
            Description = currentApp.Description,
            Outline = currentApp.Outline
        };

        return responseApp;
    }

    private static ActivityType? TryParseActivityType(string? activity)
    {
        var activityExists = Enum.TryParse(activity, out ActivityType parsedActivityType);

        if (!string.IsNullOrWhiteSpace(activity) && !activityExists)
        {
            throw new BadRequestException($"Активность {activity} не найдена!");
        }

        return activityExists ? parsedActivityType : null;
    }
}