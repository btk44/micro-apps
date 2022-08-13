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

    [HttpGet]
    public async Task<string> Get()
    {
        return "works!";
    }
}
