using System.ComponentModel.DataAnnotations;

namespace VolHub.Mvc.Models;

// -------- BMI --------
public class BmiInput
{
    [Display(Name = "Зріст, см")]
    [Range(50, 260, ErrorMessage = "Зріст має бути в діапазоні 50–260 см.")]
    public double HeightCm { get; set; }

    [Display(Name = "Вага, кг")]
    [Range(10, 400, ErrorMessage = "Вага має бути в діапазоні 10–400 кг.")]
    public double WeightKg { get; set; }

    [Display(Name = "Індекс маси тіла (BMI)")]
    public double? Bmi { get; set; }

    [Display(Name = "Категорія")]
    public string? Category { get; set; }
}

// -------- TextTool --------
public class TextToolInput
{
    [Display(Name = "ПІБ (через пробіл)")]
    [Required(ErrorMessage = "ПІБ є обов'язковим.")]
    [StringLength(500, ErrorMessage = "ПІБ не може перевищувати 500 символів.")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Нормалізований ПІБ")]
    public string? NormalizedName { get; set; }

    [Display(Name = "Телефон")]
    [RegularExpression(@"^\+380\d{9}$", ErrorMessage = "Телефон має бути у форматі +380XXXXXXXXX.")]
    public string? Phone { get; set; }

    [Display(Name = "E-mail")]
    [EmailAddress(ErrorMessage = "Вкажіть коректний e-mail.")]
    public string? Email { get; set; }

    [Display(Name = "Кількість символів")]
    public int CharacterCount { get; set; }

    [Display(Name = "Кількість слів")]
    public int WordCount { get; set; }
}

// -------- Geo (Haversine) --------
public class GeoInput
{
    [Display(Name = "Широта 1 (lat1, градуси)")]
    [Range(-90, 90, ErrorMessage = "Широта повинна бути в межах -90...90 градусів.")]
    public double Lat1 { get; set; }

    [Display(Name = "Довгота 1 (lon1, градуси)")]
    [Range(-180, 180, ErrorMessage = "Довгота повинна бути в межах -180...180 градусів.")]
    public double Lon1 { get; set; }

    [Display(Name = "Широта 2 (lat2, градуси)")]
    [Range(-90, 90, ErrorMessage = "Широта повинна бути в межах -90...90 градусів.")]
    public double Lat2 { get; set; }

    [Display(Name = "Довгота 2 (lon2, градуси)")]
    [Range(-180, 180, ErrorMessage = "Довгота повинна бути в межах -180...180 градусів.")]
    public double Lon2 { get; set; }

    [Display(Name = "Відстань, км")]
    public double? Km { get; set; }
}
