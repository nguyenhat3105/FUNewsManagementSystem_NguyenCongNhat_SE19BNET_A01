using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace NguyenCongNhatMVC.Models;

public class SystemAccount
{
    public int AccountID { get; set; }

    [Required, StringLength(100)]
    public string AccountName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(100)]
    public string AccountEmail { get; set; } = string.Empty;

    [Required, Range(1, 3)]
    public int AccountRole { get; set; }

    [Required, StringLength(100, MinimumLength = 6)]
    public string AccountPassword { get; set; } = string.Empty;

    [StringLength(30)]
    public string? PhoneNumber { get; set; }

    [StringLength(500)]
    public string? AvatarUrl { get; set; }

    [StringLength(500)]
    public string? Bio { get; set; }

    [ValidateNever] public ICollection<NewsArticle> CreatedNewsArticles { get; set; } = new List<NewsArticle>();
    [ValidateNever] public ICollection<NewsArticle> UpdatedNewsArticles { get; set; } = new List<NewsArticle>();
    [ValidateNever] public ICollection<ApprovalHistory> ApprovalHistories { get; set; } = new List<ApprovalHistory>();
    [ValidateNever] public ICollection<ArticleLike> ArticleLikes { get; set; } = new List<ArticleLike>();
    [ValidateNever] public ICollection<ArticleBookmark> ArticleBookmarks { get; set; } = new List<ArticleBookmark>();
    [ValidateNever] public ICollection<ArticleComment> ArticleComments { get; set; } = new List<ArticleComment>();

    public string RoleName => AccountRole == 1 ? "Staff" : AccountRole == 2 ? "Lecturer" : "Reader";
}
