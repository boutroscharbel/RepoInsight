using Microsoft.Extensions.Configuration;
using Moq;
using Octokit;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RepoInsight.Tests
{
    public class GitHubServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IGitHubClient> _mockGitHubClient;
        private readonly IGitHubService _gitHubService;
        private readonly string _token = "fake-github-token"; // Replace with a valid test token or mock token

        public GitHubServiceTests()
        {
            // Mocking IConfiguration to return a fake GitHub token
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(config => config["GitHub:Token"]).Returns(_token);

            // Mocking GitHubClient
            _mockGitHubClient = new Mock<IGitHubClient>();

            // Initialize GitHubService with the mocked configuration and GitHubClient
            _gitHubService = new GitHubService(_mockConfiguration.Object, _mockGitHubClient.Object);
        }

        [Fact]
        public async Task ProcessRepositoryContents_ProcessesFilesCorrectly()
        {
            // Arrange
            var mockRepositoryContentsClient = new Mock<IRepositoryContentsClient>();
            _mockGitHubClient.SetupGet(c => c.Repository.Content).Returns(mockRepositoryContentsClient.Object);

            var contents = new List<RepositoryContent>
            {
                new RepositoryContent("file1.js", "sha", "url", 0, ContentType.File, "downloadUrl", "gitUrl", "htmlUrl", "name", "path", "sha", "size", "type"),
            };

            var fileContent = Encoding.UTF8.GetBytes("abcABC");

            mockRepositoryContentsClient
                .Setup(c => c.GetRawContent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(fileContent);

            var frequency = new ConcurrentDictionary<string, int>();

            // Act
            await _gitHubService.ProcessRepositoryContents("owner", "repo", frequency, contents);

            // Assert
            Assert.Equal(2, frequency["a"]);
            Assert.Equal(2, frequency["b"]);
            Assert.Equal(2, frequency["c"]);
        }

        [Fact]
        public void ExtractOwnerAndRepo_ExtractsCorrectly()
        {
            // Arrange
            var repositoryUrl = "https://github.com/owner/repo";

            // Act
            var (owner, repo) = _gitHubService.ExtractOwnerAndRepo(repositoryUrl);

            // Assert
            Assert.Equal("owner", owner);
            Assert.Equal("repo", repo);
        }
    }
}
