using System.ComponentModel.DataAnnotations;

namespace NguyenCongNhatMVC.Models.ViewModels;

public class ArticleViewModel
{
    public string? NewsArticleID { get; set; }

    [Required, StringLength(200)]
    public string NewsTitle { get; set; } = string.Empty;

    [Required, StringLength(250)]
    public string Headline { get; set; } = string.Empty;

    [Required]
    public string NewsContent { get; set; } = string.Empty;

    [StringLength(200)]
    public string? NewsSource { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    [Required]
    public int CategoryID { get; set; }

    public bool NewsStatus { get; set; } = true;

    [Required]
    public int ArticleStateID { get; set; } = 1;

    [DataType(DataType.DateTime)]
    public DateTime? ScheduledPublishDate { get; set; }

    [Display(Name = "Tags")]
    public List<int> TagIDs { get; set; } = new();
}
