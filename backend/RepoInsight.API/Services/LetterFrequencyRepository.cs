using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class LetterFrequencyRepository : ILetterFrequencyRepository
{
    private readonly RepoInsightDbContext _dbContext;

    public LetterFrequencyRepository(RepoInsightDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveLetterFrequenciesAsync(string repositoryUrl, Dictionary<string, int> frequencies)
    {
        foreach (var kvp in frequencies)
        {
            var letterFrequency = new LetterFrequency
            {
                RepositoryUrl = repositoryUrl,
                Letter = kvp.Key,
                Count = kvp.Value
            };

            _dbContext.LetterFrequencies.Add(letterFrequency);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<Dictionary<string, int>> GetLetterFrequenciesAsync(string repositoryUrl)
    {
        var frequencies = await _dbContext.LetterFrequencies
            .Where(lf => lf.RepositoryUrl == repositoryUrl)
            .OrderByDescending(lf => lf.Count)
            .ToDictionaryAsync(lf => lf.Letter, lf => lf.Count);

        return frequencies;
    }

}
