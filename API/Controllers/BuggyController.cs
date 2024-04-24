using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class BuggyController : BaseApiController
{
    private readonly DataContext _Context;

    public BuggyController(DataContext context)
    {
        _Context = context;
    }

    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetSecret()
    {
        return Unauthorized();
    }
    [HttpGet("not-found")]
    public ActionResult<AppUser> GetNotFound()
    {
        var thing = _Context.Users.Find(-1);
        if (thing == null) return NotFound();
        return thing;
    }
    [HttpGet("server-error")]
    public ActionResult<string> ServerError()
    {
        var thing = _Context.Users.Find(-1);
        var thingToReturn = thing.ToString();
        return thingToReturn;
    }
    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest()
    {
        return BadRequest("This was not a good request");
    }

    
}
