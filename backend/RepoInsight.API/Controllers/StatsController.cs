using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepoInsight.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Add this attribute to require authentication
public class StatsController : ControllerBase
{
    private readonly GitHubService _gitHubService;

    public StatsController(GitHubService gitHubService)
    {
        _gitHubService = gitHubService;
    }

    /// <summary>
    /// Retrieves the letter frequency statistics for a given repository URL.
    /// </summary>
    /// <param name="repositoryRequest"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<Dictionary<string, int>>> GetStats([FromBody] RepositoryRequest repositoryRequest)
    {
        var stats = await _gitHubService.GetLetterFrequencyAsync(repositoryRequest.RepositoryUrl);

        return Ok(stats);
    }
}