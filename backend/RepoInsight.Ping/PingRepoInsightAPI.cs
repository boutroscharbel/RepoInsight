using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace RepoInsight.Ping
{
    public class PingRepoInsightAPI
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly string _hostEndpoint;

        public PingRepoInsightAPI(ILoggerFactory loggerFactory, IConfiguration configuration, HttpClient httpClient)
        {
            _logger = loggerFactory.CreateLogger<PingRepoInsightAPI>();
            _httpClient = httpClient;
            _hostEndpoint = configuration["HostEndpoint"];
        }

        [Function("PingRepoInsightAPI")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            _logger.LogInformation($"HostEndpoint: {_hostEndpoint}");

            if (myTimer.ScheduleStatus != null)
            {
                _logger.LogInformation($"Last timer schedule at: {myTimer.ScheduleStatus.Last}");
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
                _logger.LogInformation($"LastUpdated timer schedule at: {myTimer.ScheduleStatus.LastUpdated}");
            }

            try
            {
                var response = await _httpClient.GetAsync(_hostEndpoint);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"GET request to {_hostEndpoint} succeeded.");
                }
                else
                {
                    _logger.LogError($"GET request to {_hostEndpoint} failed with status code {response.StatusCode}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET request to {_hostEndpoint} failed with exception: {ex.Message}");
            }
        }
    }
}