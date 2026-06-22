using System.ComponentModel.DataAnnotations;

namespace NguyenCongNhatMVC.Models.ViewModels;

public class ReportViewModel
{
    [DataType(DataType.Date)]
    public DateTime? StartDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }

    public List<NewsArticle> Articles { get; set; } = new();
}
