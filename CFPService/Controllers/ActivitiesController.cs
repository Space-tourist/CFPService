using CFPService.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CFPService.Controllers;

[ApiController]
[Route("[controller]")]
public class ActivitiesController : Controller
{
    private readonly ApplicationContext _applicationContext;

    public ActivitiesController(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    [HttpGet]
    public ActivityDto[] Get()
    {
        var activities = _applicationContext.Activities.ToArray();

        return activities.Select(t => new ActivityDto
            {
                Activity = t.Activity,
                Description = t.Description
            })
            .ToArray();
    }
}