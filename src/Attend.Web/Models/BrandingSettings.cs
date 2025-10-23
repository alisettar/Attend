namespace Attend.Web.Models;

public class BrandingSettings
{
    public string AppName { get; set; } = "Attend";
    public string CompanyName { get; set; } = string.Empty;
    public string ContactEmailUser { get; set; } = string.Empty;
    public string ContactEmailDomain { get; set; } = string.Empty;
    public string PrimaryColor { get; set; } = "#6f42c1";
    public string SecondaryColor { get; set; } = "#20c997";
    public string LogoUrl { get; set; } = "/images/logo.png";
    public string FaviconUrl { get; set; } = "/favicon.svg";
}
