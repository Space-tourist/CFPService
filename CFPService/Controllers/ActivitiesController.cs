using App;
using App.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CFPService.Controllers;

[ApiController]
[Route("[controller]")]
public class ActivitiesController : Controller
{
    [HttpGet]
    public ActionResult<ActivityDto[]> Get()
    {
        return ActivitiesService.GetAllActivities();
    }
}