using Octokit;
using Serilog;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class GitHubService : IGitHubService
{
    private readonly IConfiguration _configuration;
    private readonly IGitHubClient _client;
    private readonly ILetterFrequencyRepository _repository;

    public GitHubService(IConfiguration configuration, IGitHubClient client = null, ILetterFrequencyRepository repository = null)
    {
        _configuration = configuration;
        _client = client ?? new GitHubClient(new ProductHeaderValue("RepoInsight.API"))
        {
            Credentials = new Credentials(_configuration["GitHub:Token"])
        };
        _repository = repository;
    }

    public async Task<Dictionary<string, int>> GetLetterFrequencyAsync(string repositoryUrl)
    {
        bool useDatabase = _configuration.GetValue<bool>("Database:Enabled");
        Log.Information("Database enabled: {UseDatabase}", useDatabase);

        Dictionary<string, int> cachedFrequencies = null;

        if (useDatabase)
        {
            try
            {
                cachedFrequencies = await _repository.GetLetterFrequenciesAsync(repositoryUrl);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to connect to the database: {ErrorMessage}", ex.Message);
            }
        }

        if (cachedFrequencies != null && cachedFrequencies.Any())
        {
            return cachedFrequencies;
        }

        var (owner, repo) = ExtractOwnerAndRepo(repositoryUrl);

        var frequency = new ConcurrentDictionary<string, int>();

        try
        {
            await ProcessDirectory(owner, repo, string.Empty, frequency);

            if (useDatabase)
            {
                if (cachedFrequencies == null)
                {
                    Log.Warning("Skipping saving frequencies to the database due to previous connection failure.");
                }
                else
                {
                    await _repository.SaveLetterFrequenciesAsync(repositoryUrl, frequency.ToDictionary(kv => kv.Key, kv => kv.Value));
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred: {ErrorMessage}", ex.Message);
        }

        return frequency.OrderByDescending(kv => kv.Value).ToDictionary(k => k.Key, v => v.Value);
    }

    public async Task ProcessDirectory(string owner, string repo, string path, ConcurrentDictionary<string, int> frequency)
    {
        IReadOnlyList<RepositoryContent> contents;

        if (string.IsNullOrEmpty(path))
        {
            contents = await _client.Repository.Content.GetAllContents(owner, repo);
        }
        else
        {
            contents = await _client.Repository.Content.GetAllContents(owner, repo, path);
        }

        await ProcessRepositoryContents(owner, repo, frequency, contents);
    }

    public async Task ProcessRepositoryContents(string owner, string repo, ConcurrentDictionary<string, int> frequency, IReadOnlyList<RepositoryContent> contents)
    {
        var tasks = contents.Select(async content =>
        {
            if (content.Type == ContentType.Dir)
            {
                await ProcessDirectory(owner, repo, content.Path, frequency);
            }
            else if (content.Name.EndsWith(".js") || content.Name.EndsWith(".ts"))
            {
                var fileContent = await _client.Repository.Content.GetRawContent(owner, repo, content.Path);
                var fileText = System.Text.Encoding.UTF8.GetString(fileContent);

                Log.Information("Processing file: {FilePath}", content.Path);
                Log.Information("File content length: {FileContentLength}", fileText.Length);

                foreach (var letter in fileText)
                {
                    if (char.IsLetter(letter))
                    {
                        var lowerLetter = char.ToLower(letter).ToString();
                        frequency.AddOrUpdate(lowerLetter, 1, (key, oldValue) => oldValue + 1);
                    }
                }
            }
        });

        await Task.WhenAll(tasks);
    }

    public (string owner, string repo) ExtractOwnerAndRepo(string repositoryUrl)
    {
        var uri = new Uri(repositoryUrl);
        var segments = uri.AbsolutePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        return (segments[0], segments[1]);
    }
}
