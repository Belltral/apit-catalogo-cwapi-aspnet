using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers.Versioning.URL;

[Route("api/v{version:apiVersion}/teste")]
[ApiController]
[ApiVersion("1.0", Deprecated = true)]
[ApiExplorerSettings(IgnoreApi = true)]
public class TesteURLV1Controller : ControllerBase
{
    [HttpGet]
    public string GetVersion()
    {
        return "TesteV1 - GET - API Versão 1.0";
    }
}
