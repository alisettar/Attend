# Azure Transfer - Windows PowerShell Komutları

## Resource Group
```powershell
az group create --name attend-rg --location "West Europe"
```

## App Service Plan
```powershell
az appservice plan create --name attend-plan --resource-group attend-rg --sku B1 --is-linux
```

## Web App
```powershell
az webapp create --name gencligianlamasanati --resource-group attend-rg --plan attend-plan --runtime "DOTNETCORE:9.0"
```

## API App
```powershell
az webapp create --name api-gencligianlamasanati --resource-group attend-rg --plan attend-plan --runtime "DOTNETCORE:9.0"
```

## Service Principal
```powershell
az ad sp create-for-rbac --name "attend-github-deploy" --role contributor --scopes /subscriptions/{subscription-id}/resourceGroups/attend-rg --json-auth
```

## API Environment Variables
```powershell
az webapp config appsettings set --name api-gencligianlamasanati --resource-group attend-rg --settings TenantSettings__Tenant1__Name="Erkekler" TenantSettings__Tenant1__Hash="abc123xyz" TenantSettings__Tenant1__ConnectionString="Data Source=/home/site/wwwroot/AttendDb_Erkekler.db" TenantSettings__Tenant2__Name="Kadinlar" TenantSettings__Tenant2__Hash="def456uvw" TenantSettings__Tenant2__ConnectionString="Data Source=/home/site/wwwroot/AttendDb_Kadinlar.db"
```

## Web Environment Variables
```powershell
az webapp config appsettings set --name gencligianlamasanati --resource-group attend-rg --settings ApiSettings__BaseUrl="https://api-gencligianlamasanati.azurewebsites.net/" GoogleReCaptcha__SiteKey="YOUR_SITE_KEY" GoogleReCaptcha__SecretKey="YOUR_SECRET_KEY" BrandingSettings__AppName="Gençliği Anlama Sanatı" BrandingSettings__CompanyName="Gençliği Anlama Sanatı" BrandingSettings__ContactEmailUser="gencligianlamasanati" BrandingSettings__ContactEmailDomain="outlook.com" BrandingSettings__PrimaryColor="#667eea" BrandingSettings__SecondaryColor="#764ba2"
```

## CORS
```powershell
az webapp cors add --name api-gencligianlamasanati --resource-group attend-rg --allowed-origins "https://gencligianlamasanati.azurewebsites.net"
```

## Custom Domain (Opsiyonel)
```powershell
az webapp config hostname add --webapp-name gencligianlamasanati --resource-group attend-rg --hostname gencligianlamasanati.com
```

```powershell
az webapp config hostname add --webapp-name api-gencligianlamasanati --resource-group attend-rg --hostname api.gencligianlamasanati.com
```

---

**Not:** Service Principal komutunda `{subscription-id}` yerine kendi subscription ID'nizi yazın.
