using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace NguyenCongNhatMVC.Models;

public class Tag
{
    public int TagID { get; set; }

    [Required, StringLength(100)]
    public string TagName { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Note { get; set; }

    [ValidateNever]
    public ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();
}
