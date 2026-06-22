using Microsoft.EntityFrameworkCore;
using NguyenCongNhatMVC.Models;

namespace NguyenCongNhatMVC.Data;

public class FUNewsDbContext : DbContext
{
    public FUNewsDbContext(DbContextOptions<FUNewsDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<SystemAccount> SystemAccounts => Set<SystemAccount>();
    public DbSet<NewsArticle> NewsArticles => Set<NewsArticle>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<NewsTag> NewsTags => Set<NewsTag>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<ArticleState> ArticleStates => Set<ArticleState>();
    public DbSet<ApprovalHistory> ApprovalHistories => Set<ApprovalHistory>();
    public DbSet<ArticleLike> ArticleLikes => Set<ArticleLike>();
    public DbSet<ArticleBookmark> ArticleBookmarks => Set<ArticleBookmark>();
    public DbSet<ArticleComment> ArticleComments => Set<ArticleComment>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().ToTable("Category").HasKey(c => c.CategoryID);
        modelBuilder.Entity<SystemAccount>().ToTable("SystemAccount").HasKey(a => a.AccountID);
        modelBuilder.Entity<NewsArticle>().ToTable("NewsArticle").HasKey(n => n.NewsArticleID);
        modelBuilder.Entity<Tag>().ToTable("Tag").HasKey(t => t.TagID);
        modelBuilder.Entity<NewsTag>().ToTable("NewsTag").HasKey(nt => new { nt.NewsArticleID, nt.TagID });
        modelBuilder.Entity<AuditLog>().ToTable("AuditLog").HasKey(a => a.AuditLogID);
        modelBuilder.Entity<ArticleState>().ToTable("ArticleState").HasKey(s => s.ArticleStateID);
        modelBuilder.Entity<ApprovalHistory>().ToTable("ApprovalHistory").HasKey(h => h.HistoryID);
        modelBuilder.Entity<ArticleLike>().ToTable("ArticleLike").HasKey(l => l.ArticleLikeID);
        modelBuilder.Entity<ArticleBookmark>().ToTable("ArticleBookmark").HasKey(b => b.ArticleBookmarkID);
        modelBuilder.Entity<ArticleComment>().ToTable("ArticleComment").HasKey(c => c.ArticleCommentID);

        modelBuilder.Entity<ArticleState>().HasData(
            new ArticleState { ArticleStateID = 1, StateName = "Draft", Description = "Article is being written." },
            new ArticleState { ArticleStateID = 2, StateName = "Pending Review", Description = "Article is waiting for approval." },
            new ArticleState { ArticleStateID = 3, StateName = "Published", Description = "Article is visible when scheduled time is due." },
            new ArticleState { ArticleStateID = 4, StateName = "Archived", Description = "Article is hidden but retained." });

        modelBuilder.Entity<Category>()
            .HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<NewsArticle>()
            .HasOne(n => n.Category)
            .WithMany(c => c.NewsArticles)
            .HasForeignKey(n => n.CategoryID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<NewsArticle>()
            .HasOne(n => n.ArticleState)
            .WithMany(s => s.NewsArticles)
            .HasForeignKey(n => n.ArticleStateID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<NewsArticle>()
            .HasOne(n => n.CreatedBy)
            .WithMany(a => a.CreatedNewsArticles)
            .HasForeignKey(n => n.CreatedByID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<NewsArticle>()
            .HasOne(n => n.UpdatedBy)
            .WithMany(a => a.UpdatedNewsArticles)
            .HasForeignKey(n => n.UpdatedByID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<NewsTag>()
            .HasOne(nt => nt.NewsArticle)
            .WithMany(n => n.NewsTags)
            .HasForeignKey(nt => nt.NewsArticleID);

        modelBuilder.Entity<NewsTag>()
            .HasOne(nt => nt.Tag)
            .WithMany(t => t.NewsTags)
            .HasForeignKey(nt => nt.TagID);

        modelBuilder.Entity<ApprovalHistory>()
            .HasOne(h => h.NewsArticle)
            .WithMany(n => n.ApprovalHistories)
            .HasForeignKey(h => h.NewsArticleID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ApprovalHistory>()
            .HasOne(h => h.Account)
            .WithMany(a => a.ApprovalHistories)
            .HasForeignKey(h => h.AccountID)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ArticleLike>()
            .HasIndex(l => new { l.NewsArticleID, l.AccountID })
            .IsUnique();

        modelBuilder.Entity<ArticleBookmark>()
            .HasIndex(b => new { b.NewsArticleID, b.AccountID })
            .IsUnique();

        modelBuilder.Entity<ArticleComment>()
            .HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentCommentID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Notification>().ToTable("Notification").HasKey(n => n.NotificationID);
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.NewsArticle)
            .WithMany()
            .HasForeignKey(n => n.NewsArticleID)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Recipient)
            .WithMany()
            .HasForeignKey(n => n.RecipientAccountID)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Actor)
            .WithMany()
            .HasForeignKey(n => n.ActorAccountID)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
