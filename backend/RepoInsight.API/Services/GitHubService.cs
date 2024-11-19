using Octokit;
using Serilog;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            await ProcessDirectory(owner, repo, "", frequency);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred: {ErrorMessage}", ex.Message);
        }

        return frequency.OrderByDescending(kv => kv.Value).ToDictionary(k => k.Key, v => v.Value);
    }

    private async Task ProcessDirectory(string owner, string repo, string path, Dictionary<string, int> frequency)
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

        foreach (var content in contents)
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
                        if (frequency.ContainsKey(lowerLetter))
                            frequency[lowerLetter]++;
                        else
                            frequency[lowerLetter] = 1;
                    }
                }
            }
        }
    }

    private (string owner, string repo) ExtractOwnerAndRepo(string repositoryUrl)
    {
        var uri = new Uri(repositoryUrl);
        var segments = uri.AbsolutePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        return (segments[0], segments[1]);
    }
}