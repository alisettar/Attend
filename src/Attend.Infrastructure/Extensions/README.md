# Türkçe Karakter Desteği - Arama İyileştirmesi

## 🎯 Yapılan Değişiklikler

Attend projesi'ndeki tüm arama işlemlerine **Türkçe karakter** ve **büyük/küçük harf duyarsız** arama desteği eklendi.

## 📁 Oluşturulan Dosyalar

### Extensions/StringExtensions.cs
```csharp
public static string NormalizeTurkish(this string? text)
```
- Türkçe karakterleri (ı, ş, ğ, ü, ö, ç) Latin karşılıklarına dönüştürür
- Büyük/küçük harf duyarsızlığı sağlar
- `null` ve boş string kontrolü yapar

## 🔧 Güncellenen Repository'ler

### 1. UserRepository.cs
- `GetPaginatedAsync` metodunda arama iyileştirildi
- Name, Email, Phone alanlarında Türkçe karakter desteği

### 2. EventRepository.cs
- `GetPaginatedAsync` metodunda arama iyileştirildi
- Title, Description alanlarında Türkçe karakter desteği

### 3. AttendanceRepository.cs
- `GetPaginatedByEventAsync` metodunda arama iyileştirildi (User Name, Email)
- `GetPaginatedByUserAsync` metodunda arama iyileştirildi (Event Title, Description)

## 🚀 Nasıl Çalışır?

### Öncesi:
```csharp
var searchUpper = request.SearchText.ToUpper();
query = query.Where(u => u.Name.ToUpper().Contains(searchUpper));
```
❌ "Şaban" aranınca "Saban" bulunamıyordu
❌ "Gökhan" aranınca "gokhan" bulunamıyordu

### Sonrası:
```csharp
var normalizedSearch = request.SearchText.NormalizeTurkish();
var filteredUsers = allUsers.Where(u => 
    u.Name.NormalizeTurkish().Contains(normalizedSearch)
);
```
✅ "Şaban" = "saban" = "SABAN" = "ŞaBaN" → Hepsi bulunur
✅ "Gökhan" = "gokhan" = "GÖKHAN" → Hepsi bulunur

## 📊 Teknik Detaylar

### Client-Side Evaluation
Türkçe karakter dönüşümü SQL tarafında yapılamadığı için:
1. Tüm kayıtlar önce DB'den çekilir
2. Filtreleme memory'de (client-side) yapılır
3. Pagination memory'de uygulanır

### Performans Notu
⚠️ Çok fazla kayıt varsa (10,000+), performans etkilenebilir.

**Alternatif Çözümler:**
1. ✅ Şu anki: Basit implementation, migration gerektirmez
2. 🔄 Normalized column + Index: Daha performanslı ama migration gerektirir

## 💡 Kullanım Örnekleri

### Extension method'u başka yerlerde kullanma:
```csharp
using Attend.Infrastructure.Extensions;

// Herhangi bir string'i normalize et
var normalized = "Şaban Gökhan".NormalizeTurkish(); // "saban gokhan"

// Manuel karşılaştırma
if (userName.NormalizeTurkish().Contains(searchTerm.NormalizeTurkish()))
{
    // Match bulundu
}
```

## 🎨 Kapsam

✅ **User araması**: Name, Email, Phone
✅ **Event araması**: Title, Description  
✅ **Attendance araması**: User Name, Email, Event Title, Description

## 📅 Tarih
22 Kasım 2025

## 👨‍💻 Geliştirici Notları
- Extension method pattern kullanıldı (SOLID - Open/Closed Principle)
- Tüm repository'lerde tutarlı implementasyon
- Migration gerektirmeyen çözüm
- Gelecekte normalized column'a geçiş kolay
