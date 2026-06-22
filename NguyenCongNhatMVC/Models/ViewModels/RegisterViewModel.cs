using System.ComponentModel.DataAnnotations;

namespace NguyenCongNhatMVC.Models.ViewModels;

public class RegisterViewModel
{
    [Required, StringLength(100)]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(100)]
    [Display(Name = "Email address")]
    public string Email { get; set; } = string.Empty;

    [StringLength(30)]
    [Display(Name = "Phone number")]
    public string? PhoneNumber { get; set; }

    [StringLength(500)]
    public string? Bio { get; set; }

    [Required, DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Confirm password does not match.")]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
