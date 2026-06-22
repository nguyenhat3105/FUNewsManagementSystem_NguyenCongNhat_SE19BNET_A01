using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using NguyenCongNhatMVC.Models;

namespace NguyenCongNhatMVC.Data;

public static class DbInitializer
{
    public static void Initialize(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FUNewsDbContext>();
        try
        {
            context.Database.EnsureCreated();
            ApplySchemaUpgrades(context);
        }
        catch (Exception ex) when (ex is SqlException or InvalidOperationException)
        {
            return;
        }

        if (!context.SystemAccounts.Any())
        {
            context.SystemAccounts.AddRange(
                new SystemAccount { AccountName = "Staff User", AccountEmail = "staff@funews.org", AccountPassword = "123456", AccountRole = 1 },
                new SystemAccount { AccountName = "Lecturer User", AccountEmail = "lecturer@funews.org", AccountPassword = "123456", AccountRole = 2 });
        }

        if (!context.Categories.Any())
        {
            context.Categories.AddRange(
                new Category { CategoryName = "Campus", CategoryDescription = "Campus activities", IsActive = true },
                new Category { CategoryName = "Academic", CategoryDescription = "Academic announcements", IsActive = true });
        }

        if (!context.Tags.Any())
        {
            context.Tags.AddRange(
                new Tag { TagName = "Event", Note = "Events and workshops" },
                new Tag { TagName = "Notice", Note = "Official notices" });
        }

        context.SaveChanges();

        if (!context.NewsArticles.Any())
        {
            var staff = context.SystemAccounts.First(a => a.AccountRole == 1);
            var category = context.Categories.First();
            var tag = context.Tags.First();
            var article = new NewsArticle
            {
                NewsArticleID = "N000000001",
                NewsTitle = "Welcome to FUNews",
                Headline = "The news management system is ready.",
                CreatedDate = DateTime.Now,
                NewsContent = "This is the first active article in the FUNews Management System.",
                NewsSource = "FUNews",
                NewsStatus = true,
                CategoryID = category.CategoryID,
                CreatedByID = staff.AccountID
            };
            context.NewsArticles.Add(article);
            context.NewsTags.Add(new NewsTag { NewsArticleID = article.NewsArticleID, TagID = tag.TagID });
            context.SaveChanges();
        }
    }

    private static void ApplySchemaUpgrades(FUNewsDbContext context)
    {
        context.Database.ExecuteSqlRaw("""
            IF OBJECT_ID(N'dbo.AuditLog', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.AuditLog
                (
                    AuditLogID INT IDENTITY(1,1) PRIMARY KEY,
                    ActorEmail NVARCHAR(80) NOT NULL,
                    ActorRole NVARCHAR(30) NOT NULL,
                    Action NVARCHAR(40) NOT NULL,
                    EntityName NVARCHAR(80) NOT NULL,
                    EntityKey NVARCHAR(80) NULL,
                    Description NVARCHAR(500) NULL,
                    CreatedAt DATETIME2 NOT NULL
                );
            END
            """);

        context.Database.ExecuteSqlRaw("""
            IF OBJECT_ID(N'dbo.ArticleState', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.ArticleState
                (
                    ArticleStateID INT NOT NULL PRIMARY KEY,
                    StateName NVARCHAR(50) NOT NULL,
                    Description NVARCHAR(200) NULL
                );
            END
            """);

        context.Database.ExecuteSqlRaw("""
            IF NOT EXISTS (SELECT 1 FROM dbo.ArticleState WHERE ArticleStateID = 1)
            BEGIN
                INSERT INTO dbo.ArticleState (ArticleStateID, StateName, Description)
                VALUES
                (1, N'Draft', N'Article is being written.'),
                (2, N'Pending Review', N'Article is waiting for approval.'),
                (3, N'Published', N'Article is visible when scheduled time is due.'),
                (4, N'Archived', N'Article is hidden but retained.');
            END
            """);

        context.Database.ExecuteSqlRaw("""
            IF COL_LENGTH(N'dbo.NewsArticle', N'ArticleStateID') IS NULL
            BEGIN
                ALTER TABLE dbo.NewsArticle ADD ArticleStateID INT NOT NULL CONSTRAINT DF_NewsArticle_ArticleStateID DEFAULT 3;
            END
            """);

        context.Database.ExecuteSqlRaw("""
            IF COL_LENGTH(N'dbo.NewsArticle', N'ScheduledPublishDate') IS NULL
            BEGIN
                ALTER TABLE dbo.NewsArticle ADD ScheduledPublishDate DATETIME2 NULL;
            END
            """);

        context.Database.ExecuteSqlRaw("""
            IF COL_LENGTH(N'dbo.NewsArticle', N'ImageUrl') IS NULL
            BEGIN
                ALTER TABLE dbo.NewsArticle ADD ImageUrl NVARCHAR(500) NULL;
            END
            """);

        context.Database.ExecuteSqlRaw("""
            IF COL_LENGTH(N'dbo.SystemAccount', N'PhoneNumber') IS NULL
            BEGIN
                ALTER TABLE dbo.SystemAccount ADD PhoneNumber NVARCHAR(30) NULL;
            END
            """);

        context.Database.ExecuteSqlRaw("""
            IF COL_LENGTH(N'dbo.SystemAccount', N'AvatarUrl') IS NULL
            BEGIN
                ALTER TABLE dbo.SystemAccount ADD AvatarUrl NVARCHAR(500) NULL;
            END
            """);

        context.Database.ExecuteSqlRaw("""
            IF COL_LENGTH(N'dbo.SystemAccount', N'Bio') IS NULL
            BEGIN
                ALTER TABLE dbo.SystemAccount ADD Bio NVARCHAR(500) NULL;
            END
            """);

        context.Database.ExecuteSqlRaw("""
            UPDATE dbo.NewsArticle
            SET ArticleStateID = CASE WHEN NewsStatus = 1 THEN 3 ELSE 1 END
            WHERE ArticleStateID NOT IN (1,2,3,4);
            """);

        context.Database.ExecuteSqlRaw("""
            IF OBJECT_ID(N'dbo.ApprovalHistory', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.ApprovalHistory
                (
                    HistoryID INT IDENTITY(1,1) PRIMARY KEY,
                    NewsArticleID NVARCHAR(20) NOT NULL,
                    AccountID INT NULL,
                    Action NVARCHAR(30) NOT NULL,
                    Note NVARCHAR(500) NULL,
                    Timestamp DATETIME2 NOT NULL
                );
            END
            """);

        context.Database.ExecuteSqlRaw("""
            IF OBJECT_ID(N'dbo.ArticleLike', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.ArticleLike
                (
                    ArticleLikeID INT IDENTITY(1,1) PRIMARY KEY,
                    NewsArticleID NVARCHAR(20) NOT NULL,
                    AccountID INT NOT NULL,
                    CreatedAt DATETIME2 NOT NULL,
                    CONSTRAINT UQ_ArticleLike UNIQUE (NewsArticleID, AccountID)
                );
            END
            """);

        context.Database.ExecuteSqlRaw("""
            IF OBJECT_ID(N'dbo.ArticleBookmark', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.ArticleBookmark
                (
                    ArticleBookmarkID INT IDENTITY(1,1) PRIMARY KEY,
                    NewsArticleID NVARCHAR(20) NOT NULL,
                    AccountID INT NOT NULL,
                    CreatedAt DATETIME2 NOT NULL,
                    CONSTRAINT UQ_ArticleBookmark UNIQUE (NewsArticleID, AccountID)
                );
            END
            """);

        context.Database.ExecuteSqlRaw("""
            IF OBJECT_ID(N'dbo.ArticleComment', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.ArticleComment
                (
                    ArticleCommentID INT IDENTITY(1,1) PRIMARY KEY,
                    NewsArticleID NVARCHAR(20) NOT NULL,
                    AccountID INT NULL,
                    DisplayName NVARCHAR(100) NOT NULL,
                    Content NVARCHAR(1000) NOT NULL,
                    ParentCommentID INT NULL,
                    CreatedAt DATETIME2 NOT NULL,
                    IsDeleted BIT NOT NULL CONSTRAINT DF_ArticleComment_IsDeleted DEFAULT 0
                );
            END
            """);

        context.Database.ExecuteSqlRaw("""
            IF OBJECT_ID(N'dbo.Notification', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.Notification
                (
                    NotificationID      INT IDENTITY(1,1) PRIMARY KEY,
                    RecipientAccountID  INT NOT NULL,
                    NewsArticleID       NVARCHAR(20) NOT NULL,
                    Type                NVARCHAR(30) NOT NULL,
                    ActorAccountID      INT NULL,
                    ActorName           NVARCHAR(100) NOT NULL DEFAULT '',
                    Excerpt             NVARCHAR(200) NULL,
                    IsRead              BIT NOT NULL CONSTRAINT DF_Notification_IsRead DEFAULT 0,
                    CreatedAt           DATETIME2 NOT NULL
                );
            END
            """);
    }
}
