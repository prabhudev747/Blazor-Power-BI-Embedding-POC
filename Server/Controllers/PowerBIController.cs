using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;
using PowerBIEmbededProject.Shared;

namespace PowerBIEmbededProject.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PowerBIController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public PowerBIController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<EmbeddedReportViewModel>> GetReportEmbedding()
        {
            var tenantId = _configuration["AzureADinfo:TenantId"];
            var clientId = _configuration["AzureADinfo:ClientId"];
            var clientSecret = _configuration["AzureADinfo:ClientSecreat"];
            var authorityUri = new Uri($"https://login.microsoftonline.com/{tenantId}");
            //var app = PublicClientApplicationBuilder
            //.Create(clientId)
            //.WithAuthority(authorityUri)
            //.WithRedirectUri("https://localhost:7025/authentication/login-callback") // Updated redirect URI
            //.Build();
            var app = ConfidentialClientApplicationBuilder
                        .Create(clientId)
                        .WithClientSecret(clientSecret)
                        .WithAuthority(authorityUri)
                        .Build();
            var scopes = new string[] { "https://analysis.windows.net/powerbi/api/.default" };

            try
            {
                //var accounts = await app.GetAccountsAsync();
                //var account = accounts.FirstOrDefault(); // Get the first available account

                //if (account == null)
                //{
                //    return Unauthorized("User account not found. Please log in.");
                //}

                var authResult = await app.AcquireTokenForClient(scopes).ExecuteAsync();
                var tokenCredentials = new TokenCredentials(authResult.AccessToken, "Bearer");
                var urlPowerBiServiceApiRoot = "https://api.powerbi.com/";
                var pbiClient = new PowerBIClient(new Uri(urlPowerBiServiceApiRoot), tokenCredentials);

                var workfspaceId = new Guid(_configuration["PowerBi:WorkspaceId"]);
                var reportId = new Guid(_configuration["PowerBi:ReportId"]);
                var report = pbiClient.Reports.GetReportInGroup(workfspaceId, reportId);

                var tokenRequest = new GenerateTokenRequest(TokenAccessLevel.View, report.DatasetId);
                var embedTokenResponse = await pbiClient.Reports.GenerateTokenAsync(workfspaceId, reportId, tokenRequest);


                var reportviewModel = new EmbeddedReportViewModel(
                    report.Id.ToString(),
                    report.Name,
                    report.EmbedUrl,
                    embedTokenResponse.Token);

                return Ok(reportviewModel);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
