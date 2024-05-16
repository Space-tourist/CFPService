using System.Net.Mime;
using App;
using App.Contracts;
using App.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CFPService.Controllers;

[ApiController]
[Route("[controller]")]
public class ApplicationsController : Controller
{
    private readonly ApplicationService _applicationService;
    public ApplicationsController(ApplicationService applicationService)
    {
        _applicationService = applicationService;
    }
    
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApplicationResponseDto>> Post(CreateApplicationRequestDto app)
    {
        try
        {
            return await _applicationService.CreateApplication(app);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{applicationId}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApplicationResponseDto>> Put(Guid applicationId, EditApplicationRequestDto app)
    {
        try
        {
            return await _applicationService.UpdateApplication(applicationId, app);
        }
        catch (NotFoundApplicationException e)
        {
            return NotFound(e.Message);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{applicationId}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Delete(Guid applicationId)
    {
        try
        {
            await _applicationService.DeleteApplication(applicationId);
        }
        catch (NotFoundApplicationException e)
        {
            return NotFound(e.Message);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }

        return Ok();
    }

    [HttpPost("{applicationId}/submit")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Submit(Guid applicationId)
    {
        try
        {
            await _applicationService.SubmitApplication(applicationId);
        }
        catch (NotFoundApplicationException e)
        {
            return NotFound(e.Message);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }

        return Ok();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationResponseDto[]>> Get(DateTime? submittedAfter, DateTime? unsubmittedOlder) 
    {
        try
        {
            return await _applicationService.GetApplicationsByFilter(submittedAfter, unsubmittedOlder);
        }
        catch (NotFoundApplicationException e)
        {
            return NotFound(e.Message);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{applicationId}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApplicationResponseDto>> Get(Guid applicationId)
    {
        try
        {
            return await _applicationService.GetApplication(applicationId);
        }
        catch (NotFoundApplicationException e)
        {
            return NotFound(e.Message);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
    }
}