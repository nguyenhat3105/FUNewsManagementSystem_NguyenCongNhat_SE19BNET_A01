using System.ComponentModel.DataAnnotations;

namespace NguyenCongNhatMVC.Models;

public class ArticleComment
{
    public int ArticleCommentID { get; set; }

    [Required, StringLength(20)]
    public string NewsArticleID { get; set; } = string.Empty;

    public int? AccountID { get; set; }

    [Required, StringLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    [Required, StringLength(1000)]
    public string Content { get; set; } = string.Empty;

    public int? ParentCommentID { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsDeleted { get; set; }

    public NewsArticle? NewsArticle { get; set; }
    public SystemAccount? Account { get; set; }
    public ArticleComment? ParentComment { get; set; }
    public ICollection<ArticleComment> Replies { get; set; } = new List<ArticleComment>();
}
