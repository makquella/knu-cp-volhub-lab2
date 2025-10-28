using System.ComponentModel.DataAnnotations;
using VolHub.Mvc.Models.Validation;

namespace VolHub.Mvc.Models.ViewModels;

public class RegisterViewModel
{
    [Display(Name = "Логін")]
    [Required(ErrorMessage = "Логін є обов'язковим.")]
    [StringLength(50, ErrorMessage = "Логін не може перевищувати 50 символів.")]
    public string Username { get; set; } = string.Empty;

    [Display(Name = "Повне ім'я (ПІБ)")]
    [Required(ErrorMessage = "ПІБ є обов'язковим.")]
    [StringLength(500, ErrorMessage = "ПІБ не може перевищувати 500 символів.")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "E-mail")]
    [Required(ErrorMessage = "E-mail є обов'язковим.")]
    [EmailAddress(ErrorMessage = "Вкажіть коректний e-mail.")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Телефон")]
    [Required(ErrorMessage = "Телефон є обов'язковим.")]
    [RegularExpression(@"^\+380\d{9}$", ErrorMessage = "Телефон має бути у форматі +380XXXXXXXXX.")]
    public string Phone { get; set; } = string.Empty;

    [Display(Name = "Пароль")]
    [Required(ErrorMessage = "Пароль є обов'язковим.")]
    [DataType(DataType.Password)]
    [PasswordComplexity]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Підтвердження пароля")]
    [Required(ErrorMessage = "Підтвердження пароля є обов'язковим.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Паролі не співпадають.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class ProfileEditViewModel
{
    [Display(Name = "Логін")]
    public string Username { get; init; } = string.Empty;

    [Display(Name = "E-mail")]
    public string Email { get; init; } = string.Empty;

    [Display(Name = "Повне ім'я (ПІБ)")]
    [Required(ErrorMessage = "ПІБ є обов'язковим.")]
    [StringLength(500, ErrorMessage = "ПІБ не може перевищувати 500 символів.")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Телефон")]
    [Required(ErrorMessage = "Телефон є обов'язковим.")]
    [RegularExpression(@"^\+380\d{9}$", ErrorMessage = "Телефон має бути у форматі +380XXXXXXXXX.")]
    public string Phone { get; set; } = string.Empty;
}
