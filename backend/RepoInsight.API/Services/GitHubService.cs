using Octokit;
using Serilog;

public class GitHubService
{
    private readonly IConfiguration _configuration;
    private readonly GitHubClient _client;

    public GitHubService(IConfiguration configuration)
    {
        _configuration = configuration;
        _client = new GitHubClient(new ProductHeaderValue("RepoInsight.API"));
        _client.Credentials = new Credentials(_configuration["GitHub:Token"]);
    }

    public async Task<Dictionary<string, int>> GetLetterFrequencyAsync(string repositoryUrl)
    {
        var (owner, repo) = ExtractOwnerAndRepo(repositoryUrl);

        var frequency = new Dictionary<string, int>();

        try
        {
            var contents = await _client.Repository.Content.GetAllContents(owner, repo);

            foreach (var content in contents.Where(c => c.Name.EndsWith(".js") || c.Name.EndsWith(".ts")))
            {
                var fileContent = await _client.Repository.Content.GetRawContent(owner, repo, content.Path);
                var fileText = fileContent.ToString();

                foreach (var letter in fileText)
                {
                    if (char.IsLetter(letter))
                    {
                        var lowerLetter = char.ToLower(letter).ToString();
                        if (frequency.ContainsKey(lowerLetter))
                            frequency[lowerLetter]++;
                        else
                            frequency[lowerLetter] = 1;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred: {ErrorMessage}", ex.Message);
        }

        return frequency.OrderByDescending(kv => kv.Value).ToDictionary(k => k.Key, v => v.Value);
    }

    private (string owner, string repo) ExtractOwnerAndRepo(string url)
    {
        Uri uri = new Uri(url);
        string[] pathSegments = uri.AbsolutePath.Split('/');
        string owner = pathSegments[1]; // The first part after '/github.com'
        string repo = pathSegments[2];  // The second part after the owner's name
        return (owner, repo);
    }
}
