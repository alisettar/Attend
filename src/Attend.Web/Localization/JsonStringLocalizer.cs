using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Text.Json;

namespace Attend.Web.Localization;

public class JsonStringLocalizer : IStringLocalizer
{
    private readonly Dictionary<string, string> _localizations = new();

    public JsonStringLocalizer(string resourcePath, CultureInfo culture)
    {
        var filePath = $"{resourcePath}.{culture.Name}.json";
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            _localizations = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
        }
    }

    public LocalizedString this[string name]
    {
        get
        {
            var value = _localizations.TryGetValue(name, out var result) ? result : name;
            return new LocalizedString(name, value, !_localizations.ContainsKey(name));
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var format = this[name];
            var value = string.Format(format.Value, arguments);
            return new LocalizedString(name, value, format.ResourceNotFound);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        return _localizations.Select(x => new LocalizedString(x.Key, x.Value, false));
    }
}

public class JsonStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly string _resourcesPath;

    public JsonStringLocalizerFactory(IWebHostEnvironment env)
    {
        _resourcesPath = Path.Combine(env.ContentRootPath, "Resources");
    }

    public IStringLocalizer Create(Type resourceSource)
    {
        return new JsonStringLocalizer(
            Path.Combine(_resourcesPath, resourceSource.Name),
            CultureInfo.CurrentUICulture);
    }

    public IStringLocalizer Create(string baseName, string location)
    {
        return new JsonStringLocalizer(
            Path.Combine(_resourcesPath, baseName),
            CultureInfo.CurrentUICulture);
    }
}
