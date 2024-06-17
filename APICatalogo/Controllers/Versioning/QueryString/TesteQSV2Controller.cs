
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers.Versioning.QueryString;

[Route("api/teste")]
[ApiController]
[ApiVersion("2.0")]
[ApiExplorerSettings(IgnoreApi = true)]
public class TesteQSV2Controller : ControllerBase
{
    [HttpGet]
    public string GetVersion()
    {
        return "TesteV2 - GET - API Versão 2.0";
    }
}
