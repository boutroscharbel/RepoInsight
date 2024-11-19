using Octokit;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IGitHubService
{
    Task<Dictionary<string, int>> GetLetterFrequencyAsync(string repositoryUrl);
    Task ProcessDirectory(string owner, string repo, string path, ConcurrentDictionary<string, int> frequency);
    Task ProcessRepositoryContents(string owner, string repo, ConcurrentDictionary<string, int> frequency, IReadOnlyList<RepositoryContent> contents);
    (string owner, string repo) ExtractOwnerAndRepo(string repositoryUrl);

}
