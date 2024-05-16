using Domain;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class ApplicationRepository
{
    private readonly ApplicationContext _applicationContext;

    public ApplicationRepository(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public Task<Applications?> GetApplicationAsync(Guid applicationId)
    {
        return _applicationContext
            .Applications
            .FirstOrDefaultAsync(t => t.Id == applicationId);
    }

    public async Task CreateApplicationAsync(Applications application)
    {
        await _applicationContext.Applications.AddAsync(application);
        await _applicationContext.SaveChangesAsync();
    }

    public async Task UpdateApplicationAsync()
    {
        await _applicationContext.SaveChangesAsync();
    }

    public async Task RemoveApplicationAsync(Applications application)
    {
        _applicationContext.Applications.Remove(application);
        
        await _applicationContext.SaveChangesAsync();
    }
    public bool CheckAuthorsApplications(Guid authorId)
    {
        return _applicationContext
            .Applications
            .Any(t => t.Author == authorId && t.Status == ApplicationStatus.Created);
    }

    public Task<Applications[]> GetSubmittedApplications(DateTime submittedUtcTime)
    {
        return _applicationContext.Applications
            .Where(t => t.SubmittedTime >= submittedUtcTime && t.Status == ApplicationStatus.OnSubmitting)
            .ToArrayAsync();
    }

    public Task<Applications[]> GetUnsubmittedApplications(DateTime unsubmittedUtcTime)
    {
        return _applicationContext.Applications
            .Where(t => t.Status == ApplicationStatus.Created)
            .Where(t => t.CreatedTime < unsubmittedUtcTime)
            .ToArrayAsync();
    }

    public Task<Applications?> GetCurrentAuthorsApplication(Guid id)
    {
        return _applicationContext.Applications
            .FirstOrDefaultAsync(t => t.Author == id && t.Status == ApplicationStatus.Created);
    }
    
}