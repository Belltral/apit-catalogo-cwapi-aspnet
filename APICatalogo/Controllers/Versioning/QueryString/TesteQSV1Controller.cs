using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers.Versioning.QueryString;

[Route("api/teste")]
[ApiController]
[ApiVersion("1.0", Deprecated = true)]
[ApiExplorerSettings(IgnoreApi = true)]
public class TesteQSV1Controller : ControllerBase
{
    [HttpGet]
    public string GetVersion()
    {
        return "TesteV1 - GET - API Versão 1.0";
    }
}
