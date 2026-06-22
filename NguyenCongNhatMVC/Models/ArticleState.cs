using System.ComponentModel.DataAnnotations;

namespace NguyenCongNhatMVC.Models;

public class ArticleState
{
    public int ArticleStateID { get; set; }

    [Required, StringLength(50)]
    public string StateName { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Description { get; set; }

    public ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
}
