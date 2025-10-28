using System.ComponentModel.DataAnnotations;

namespace VolHub.Mvc.Models;

public class AppUserProfile
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Логін є обов'язковим.")]
    [StringLength(50, ErrorMessage = "Логін не може перевищувати 50 символів.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "ПІБ є обов'язковим.")]
    [StringLength(500, ErrorMessage = "ПІБ не може перевищувати 500 символів.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-mail є обов'язковим.")]
    [EmailAddress(ErrorMessage = "Вкажіть коректний e-mail.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Телефон є обов'язковим.")]
    [RegularExpression(@"^\+380\d{9}$", ErrorMessage = "Телефон має бути у форматі +380XXXXXXXXX.")]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;
}
