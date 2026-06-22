using System.ComponentModel.DataAnnotations;

namespace NguyenCongNhatMVC.Models.ViewModels;

public class AccountViewModel
{
    public int AccountID { get; set; }

    [Required, StringLength(100)]
    public string AccountName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(100)]
    public string AccountEmail { get; set; } = string.Empty;

    [Required, Range(1, 3)]
    public int AccountRole { get; set; } = 1;

    [Required, StringLength(100, MinimumLength = 6), DataType(DataType.Password)]
    public string AccountPassword { get; set; } = string.Empty;
}
