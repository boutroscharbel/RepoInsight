using Microsoft.AspNetCore.Mvc;

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
   /// Get the frequency of each letter in the JavaScript and TypeScript files of a GitHub repository.
   /// </summary>
   /// <param name="repositoryUrl"></param>
   /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<Dictionary<string, int>>> GetStats(string repositoryUrl = "https://github.com/lodash/lodash")
    {
        var (owner, repo) = ExtractOwnerAndRepo(repositoryUrl);

        var stats = await _gitHubService.GetLetterFrequencyAsync(repositoryUrl);

        return Ok(stats);
    }

    private static (string owner, string repo) ExtractOwnerAndRepo(string url)
    {
        Uri uri = new Uri(url);
        string[] pathSegments = uri.AbsolutePath.Split('/');
        string owner = pathSegments[1]; // The first part after '/github.com'
        string repo = pathSegments[2];  // The second part after the owner's name
        return (owner, repo);
    }
}
