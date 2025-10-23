# Azure Transfer Rehberi - Yeni Hesaba Taşıma

## Adım 1: Azure Kaynakları Oluştur

### Resource Group
```bash
az group create --name attend-rg --location "West Europe"
```

### App Service Plan
```bash
az appservice plan create \
  --name attend-plan \
  --resource-group attend-rg \
  --sku B1 \
  --is-linux
```

### App Services
```bash
# Web (ana domain)
az webapp create \
  --name gencligianlamasanati \
  --resource-group attend-rg \
  --plan attend-plan \
  --runtime "DOTNETCORE:9.0"

# API (subdomain)
az webapp create \
  --name api-gencligianlamasanati \
  --resource-group attend-rg \
  --plan attend-plan \
  --runtime "DOTNETCORE:9.0"
```

**Sonuç URL'ler:**
- Web: https://gencligianlamasanati.azurewebsites.net
- API: https://api-gencligianlamasanati.azurewebsites.net

## Adım 2: Service Principal Oluştur

```bash
az ad sp create-for-rbac \
  --name "attend-github-deploy" \
  --role contributor \
  --scopes /subscriptions/{subscription-id}/resourceGroups/attend-rg \
  --json-auth
```

Çıktıyı kopyala ve GitHub Secret'a ekle.

## Adım 3: GitHub Secrets Güncelle

Repository → Settings → Secrets → Actions

**Güncelle:**
- `AZURE_CREDENTIALS`: Service Principal JSON (Adım 2'den)

## Adım 4: Workflow Dosyalarını Güncelle

`.github/workflows/deploy-api.yml`:
```yaml
with:
  app-name: 'api-gencligianlamasanati'
```

`.github/workflows/deploy-web.yml`:
```yaml
with:
  app-name: 'gencligianlamasanati'
```

## Adım 5: Environment Variables

### API App Service
```bash
az webapp config appsettings set \
  --name api-gencligianlamasanati \
  --resource-group attend-rg \
  --settings \
    TenantSettings__Tenant1__Name="Erkekler" \
    TenantSettings__Tenant1__Hash="abc123xyz" \
    TenantSettings__Tenant1__ConnectionString="Data Source=/home/site/wwwroot/AttendDb_Erkekler.db" \
    TenantSettings__Tenant2__Name="Kadinlar" \
    TenantSettings__Tenant2__Hash="def456uvw" \
    TenantSettings__Tenant2__ConnectionString="Data Source=/home/site/wwwroot/AttendDb_Kadinlar.db"
```

### Web App Service
```bash
az webapp config appsettings set \
  --name gencligianlamasanati \
  --resource-group attend-rg \
  --settings \
    ApiSettings__BaseUrl="https://api-gencligianlamasanati.azurewebsites.net/" \
    GoogleReCaptcha__SiteKey="YOUR_SITE_KEY" \
    GoogleReCaptcha__SecretKey="YOUR_SECRET_KEY" \
    BrandingSettings__AppName="Gençliği Anlama Sanatı" \
    BrandingSettings__CompanyName="Gençliği Anlama Sanatı" \
    BrandingSettings__ContactEmailUser="gencligianlamasanati" \
    BrandingSettings__ContactEmailDomain="outlook.com" \
    BrandingSettings__PrimaryColor="#667eea" \
    BrandingSettings__SecondaryColor="#764ba2"
```

## Adım 6: CORS Yapılandırması

```bash
az webapp cors add \
  --name api-gencligianlamasanati \
  --resource-group attend-rg \
  --allowed-origins "https://gencligianlamasanati.azurewebsites.net"
```

## Adım 7: Database Taşıma

### Kudu Console
1. API: https://api-gencligianlamasanati.scm.azurewebsites.net
2. Debug Console → PowerShell
3. `cd D:\home\site\wwwroot`
4. Upload database files:
   - `AttendDb_Erkekler.db`
   - `AttendDb_Kadinlar.db`

## Adım 8: Deploy ve Test

```bash
git add .
git commit -m "Update Azure deployment configuration"
git push origin master
```

**Test URL'leri:**
- API: https://api-gencligianlamasanati.azurewebsites.net/swagger
- Web: https://gencligianlamasanati.azurewebsites.net

## Hızlı Checklist

- [ ] Resource group: `attend-rg`
- [ ] App Service Plan: `attend-plan`
- [ ] Web App: `gencligianlamasanati`
- [ ] API App: `api-gencligianlamasanati`
- [ ] Service Principal oluştur
- [ ] GitHub Secret: `AZURE_CREDENTIALS`
- [ ] Workflow app-name güncelle (2 dosya)
- [ ] API environment variables
- [ ] Web environment variables (`ApiSettings__BaseUrl` kritik!)
- [ ] CORS: Web URL ekle
- [ ] Database dosyaları taşı
- [ ] Git push → otomatik deploy
- [ ] Test: Login + QR scan

## Custom Domain (Opsiyonel)

```bash
# DNS CNAME records:
# gencligianlamasanati.com → gencligianlamasanati.azurewebsites.net
# api.gencligianlamasanati.com → api-gencligianlamasanati.azurewebsites.net

az webapp config hostname add \
  --webapp-name gencligianlamasanati \
  --resource-group attend-rg \
  --hostname gencligianlamasanati.com

az webapp config hostname add \
  --webapp-name api-gencligianlamasanati \
  --resource-group attend-rg \
  --hostname api.gencligianlamasanati.com
```

## Sorun Giderme

**Deploy başarısız:**
- GitHub Actions logs
- Service Principal permissions

**Database bulunamıyor:**
- Connection string path doğru mu?
- Dosyalar `/home/site/wwwroot` altında mı?

**CORS hatası:**
- Web URL API CORS listesinde?
- HTTPS kullanılıyor?

**API bağlantı hatası:**
- `ApiSettings__BaseUrl` doğru mu?
- SSL sertifikası aktif mi?

---

**Kritik:** Web app'te `ApiSettings__BaseUrl` mutlaka `https://api-gencligianlamasanati.azurewebsites.net/` olmalı.
