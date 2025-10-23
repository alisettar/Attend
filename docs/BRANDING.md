# Branding Kullanım Rehberi

## Yapılandırma

### appsettings.json / appsettings.Production.json

```json
{
  "BrandingSettings": {
    "AppName": "Attend",
    "CompanyName": "Gençliği Anlama Sanatı",
    "ContactEmailUser": "gencligianlamasanati",
    "ContactEmailDomain": "outlook.com",
    "PrimaryColor": "#6f42c1",
    "SecondaryColor": "#20c997",
    "LogoUrl": "/images/logo.png",
    "FaviconUrl": "/favicon.svg"
  }
}
```

## Entegre Edilen Sayfalar

✅ **_Layout.cshtml**
- Title: `@branding.AppName`
- Favicon: `@branding.FaviconUrl`
- Navigation brand: `@branding.AppName`
- Footer copyright: `@branding.CompanyName`
- Footer email: `@branding.ContactEmailUser<i class="fa-solid fa-at"></i>@branding.ContactEmailDomain`
- CSS Variables: `--primary-color` ve `--secondary-color`

✅ **PrivacyPolicy.cshtml**
- Veri sorumlusu: `@branding.CompanyName`
- İletişim: Email icon formatında

✅ **ConsentText.cshtml**
- Rıza geri alma: Email icon formatında

✅ **Register/Success.cshtml**
- Title: `@branding.AppName`
- Favicon: `@branding.FaviconUrl`
- Gradient: Primary/Secondary colors
- Footer: `@branding.CompanyName`

## Yeni Müşteri İçin Adımlar

1. **appsettings.Production.json güncelle:**
   ```json
   {
     "BrandingSettings": {
       "AppName": "YeniMarka",
       "CompanyName": "Yeni Şirket A.Ş.",
       "ContactEmailUser": "info",
       "ContactEmailDomain": "yenimarka.com",
       "PrimaryColor": "#ff5733",
       "SecondaryColor": "#33c3ff",
       "LogoUrl": "/images/yeni-logo.png",
       "FaviconUrl": "/favicon-yeni.svg"
     }
   }
   ```

2. **Logo/Favicon dosyalarını ekle:**
   - `wwwroot/images/yeni-logo.png`
   - `wwwroot/favicon-yeni.svg`

3. **Deploy et** - Değişiklikler otomatik yansır

## E-posta Spam Koruması

E-posta adresleri @ işareti yerine Font Awesome icon ile gösterilir:
```html
contact<i class="fa-solid fa-at"></i>domain.com
```

Bu format web scraper'ların e-posta toplamalarını zorlaştırır.

## Test

Development ortamında:
```bash
dotnet run
```

Tarayıcıda kontrol edin:
- Header: AppName görünüyor mu?
- Footer: CompanyName + email icon formatında mı?
- Legal sayfalar: Dinamik veriler doğru mu?
- Success page: Renkler ve branding uyumlu mu?
