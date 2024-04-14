using System.Net.Mime;
using App;
using App.Contracts;
using App.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CFPService.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : Controller
{
    private readonly ApplicationService _applicationService;

    public UsersController(ApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    [HttpGet("{authorId}/currentapplication")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationResponseDto>> Get(Guid authorId)
    {
        try
        {
            return await _applicationService.GetCurrentAuthorsApplication(authorId);
        }
        catch (NotFoundApplicationException e)
        {
            return NotFound(e.Message);
        }
    }
}