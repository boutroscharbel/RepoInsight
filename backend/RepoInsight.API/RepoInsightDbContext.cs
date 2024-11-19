using Microsoft.EntityFrameworkCore;

public class RepoInsightDbContext : DbContext
{
    public DbSet<LetterFrequency> LetterFrequencies { get; set; }

    public RepoInsightDbContext(DbContextOptions<RepoInsightDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LetterFrequency>().HasKey(lf => new { lf.Letter, lf.RepositoryUrl });
    }
}
