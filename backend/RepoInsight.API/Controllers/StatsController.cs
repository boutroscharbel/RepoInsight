using Microsoft.AspNetCore.Mvc;
using RepoInsight.API.Models;

[Route("api/[controller]")]
[ApiController]
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
