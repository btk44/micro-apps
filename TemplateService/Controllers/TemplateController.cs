using Microsoft.AspNetCore.Mvc;

namespace TemplateService.Controllers;

[ApiController]
[Route("[controller]")]
public class TemplateController : ControllerBase
{
    private readonly ILogger<TemplateController> _logger;

    public TemplateController(ILogger<TemplateController> logger)
    {
        _logger = logger;
    }

    [HttpGet("path1")]
    public async Task<string> Get1()
    {
        return "works!";
    }

    [HttpGet("path2")]
    public async Task<string> Get2()
    {
        throw new Exception("error handler should work and you should not see a callstack");
        return "works!";
    }
}
