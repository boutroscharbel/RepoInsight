using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Octokit;
using System.Threading.Tasks;
using Xunit;

namespace RepoInsight.Tests
{
    public class GitHubServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly GitHubService _gitHubService;
        private readonly string _token = "fake-github-token"; // Replace with a valid test token or mock token

        public GitHubServiceTests()
        {
            // Mocking IConfiguration to return a fake GitHub token
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(config => config["GitHub:Token"]).Returns(_token);

            // Initialize GitHubService with the mocked configuration
            _gitHubService = new GitHubService(_mockConfiguration.Object);
        }

        [Fact]
        public async Task GetRepositoryDetailsAsync_ShouldReturnRepository_WhenCalled()
        {
            // Arrange
            var owner = "lodash";
            var repoName = "lodash";
            
            // Mock the actual GitHub API call (this can be done with a mock of Octokit)
            var expectedRepository = new Repository("lodash", "lodash", "A modern JavaScript utility library...", 56000);
            
            // You can mock the client or set expectations here
            var mockClient = new Mock<GitHubClient>(new ProductHeaderValue("RepoInsight.API"));
            mockClient.Setup(client => client.Repository.Get(owner, repoName)).ReturnsAsync(expectedRepository);

            // Act
            var result = await _gitHubService.GetRepositoryDetailsAsync(owner, repoName);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(expectedRepository.Name);
            result.StargazersCount.Should().Be(expectedRepository.StargazersCount);
        }

        [Fact]
        public async Task GetRepositoryIssuesAsync_ShouldReturnIssues_WhenCalled()
        {
            // Arrange
            var owner = "lodash";
            var repoName = "lodash";

            // Mock issues for the repo
            var mockIssues = new[]
            {
                new Issue(1, "Issue 1", "Description of Issue 1"),
                new Issue(2, "Issue 2", "Description of Issue 2")
            };

            // Mock the client or set expectations here
            var mockClient = new Mock<GitHubClient>(new ProductHeaderValue("RepoInsight.API"));
            mockClient.Setup(client => client.Issue.GetAllForRepository(owner, repoName)).ReturnsAsync(mockIssues);

            // Act
            var result = await _gitHubService.GetRepositoryIssuesAsync(owner, repoName);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result[0].Title.Should().Be("Issue 1");
        }
    }
}
