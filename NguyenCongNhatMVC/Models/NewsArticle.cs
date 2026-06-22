using System.ComponentModel.DataAnnotations;

namespace NguyenCongNhatMVC.Models;

public class NewsArticle
{
    [Required, StringLength(20)]
    public string NewsArticleID { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string NewsTitle { get; set; } = string.Empty;

    [Required, StringLength(250)]
    public string Headline { get; set; } = string.Empty;

    [DataType(DataType.DateTime)]
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    [Required]
    public string NewsContent { get; set; } = string.Empty;

    [StringLength(200)]
    public string? NewsSource { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    [Required]
    public int CategoryID { get; set; }

    [Display(Name = "Active")]
    public bool NewsStatus { get; set; } = true;

    public int ArticleStateID { get; set; } = 3;

    [DataType(DataType.DateTime)]
    public DateTime? ScheduledPublishDate { get; set; }

    public int CreatedByID { get; set; }
    public int? UpdatedByID { get; set; }
    public DateTime? ModifiedDate { get; set; }

    public Category? Category { get; set; }
    public ArticleState? ArticleState { get; set; }
    public SystemAccount? CreatedBy { get; set; }
    public SystemAccount? UpdatedBy { get; set; }
    public ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();
    public ICollection<ApprovalHistory> ApprovalHistories { get; set; } = new List<ApprovalHistory>();
    public ICollection<ArticleLike> Likes { get; set; } = new List<ArticleLike>();
    public ICollection<ArticleBookmark> Bookmarks { get; set; } = new List<ArticleBookmark>();
    public ICollection<ArticleComment> Comments { get; set; } = new List<ArticleComment>();
}
