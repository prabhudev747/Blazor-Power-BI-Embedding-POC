using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace PowerBIEmbededProject.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AzureADTokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public AzureADTokenController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        [HttpGet]
        public async Task<ActionResult<string>> GetADToken() 
        {
            var tenantId = _configuration["AzureADinfo:TenantId"];
            var clientId = _configuration["AzureADinfo:ClientId"];
            var clientSecret = _configuration["AzureADinfo:ClientSecreat"];
            var authorityUri = new Uri($"https://login.microsoftonline.com/{tenantId}");
            var app = ConfidentialClientApplicationBuilder
                        .Create(clientId)
                        .WithClientSecret(clientSecret)
                        .WithAuthority(authorityUri)
                        .Build();
            var powerbiApiDefaultsxope = "https://analysis.windows.net/powerbi/api/.default";
            var scopes = new string[] { powerbiApiDefaultsxope };

            try
            {
                var authResult = await app.AcquireTokenForClient(scopes).ExecuteAsync();
                return Ok(authResult.AccessToken);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
