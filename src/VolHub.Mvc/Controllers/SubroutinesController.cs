using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolHub.Mvc.Models;

namespace VolHub.Mvc.Controllers;

[Authorize]
public class SubroutinesController : Controller
{
    [HttpGet]
    public IActionResult Index() => View();

    // BMI
    [HttpGet]
    public IActionResult Bmi() => View(new BmiInput());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Bmi(BmiInput model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var heightMeters = model.HeightCm / 100.0;
        var bmi = model.WeightKg / (heightMeters * heightMeters);

        model.Bmi = Math.Round(bmi, 2);
        model.Category = bmi switch
        {
            < 18.5 => "Недовага",
            < 25 => "Норма",
            < 30 => "Надмірна вага",
            _ => "Ожиріння"
        };

        return View(model);
    }

    // TextTool
    [HttpGet]
    public IActionResult TextTool() => View(new TextToolInput());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult TextTool(TextToolInput model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var cleaned = Regex.Replace(model.FullName.Trim(), @"\s+", " ");
        var words = cleaned.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var normalized = string.Join(' ', words.Select(Capitalize));

        model.FullName = cleaned;
        model.NormalizedName = normalized;
        model.CharacterCount = cleaned.Length;
        model.WordCount = words.Length;

        return View(model);

        static string Capitalize(string value)
            => string.IsNullOrWhiteSpace(value)
                ? value
                : char.ToUpper(value[0]) + value[1..].ToLower();
    }

    // Geo
    [HttpGet]
    public IActionResult Geo() => View(new GeoInput());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Geo(GeoInput model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        model.Km = Math.Round(Haversine(model.Lat1, model.Lon1, model.Lat2, model.Lon2), 3);
        return View(model);

        static double Haversine(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371.0;
            double ToRadians(double degrees) => Math.PI * degrees / 180.0;

            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
    }
}
