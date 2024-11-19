public interface ILetterFrequencyRepository
{
    Task SaveLetterFrequenciesAsync(string repositoryUrl, Dictionary<string, int> frequencies);
    Task<Dictionary<string, int>> GetLetterFrequenciesAsync(string repositoryUrl);
}
