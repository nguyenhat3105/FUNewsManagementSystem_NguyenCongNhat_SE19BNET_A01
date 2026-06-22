using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace NguyenCongNhatMVC.Models;

public class Category
{
    public int CategoryID { get; set; }

    [Required, StringLength(100)]
    public string CategoryName { get; set; } = string.Empty;

    [Required, StringLength(250)]
    public string CategoryDescription { get; set; } = string.Empty;

    [Display(Name = "Parent Category")]
    public int? ParentCategoryID { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    [ValidateNever] public Category? ParentCategory { get; set; }
    [ValidateNever] public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    [ValidateNever] public ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
}
