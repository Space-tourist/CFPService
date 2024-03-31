using CFPService.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CFPService.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : Controller
{
    private readonly ApplicationContext _applicationContext;

    public UsersController(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    [HttpGet("{authorId}/currentapplication")]
    public async Task<ApplicationResponseDto> Get(Guid authorId)
    {
        var currentApp = await _applicationContext
            .Applications
            .Include(applications => applications.Activity)
            .FirstOrDefaultAsync(t => t.Author == authorId && t.Status == Statuses.Created);

        if (currentApp is null)
        {
            throw new ArgumentException();
        }

        ApplicationResponseDto responseApp = new ApplicationResponseDto
        {
            Id = currentApp.Id,
            Author = currentApp.Author,
            Activity = currentApp.Activity?.Activity ?? string.Empty,
            Name = currentApp.Name,
            Description = currentApp.Description,
            Outline = currentApp.Outline
        };

        return responseApp;
    }
}