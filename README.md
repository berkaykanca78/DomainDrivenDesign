# 🚀 Domain Driven Design (DDD) Projesi

<div align="center">
  <img src="https://cdn2.iconfinder.com/data/icons/microservices-soft-fill/60/Domain-Driven-Design-domain-driven-design-512.png" alt="DDD Architecture" width="600"/>
</div>

## 📝 Proje Hakkında

Bu proje, .NET 9.0 kullanılarak geliştirilmiş bir Domain Driven Design (DDD) uygulamasıdır. Proje, modern yazılım mimarisi prensiplerini takip ederek, ölçeklenebilir ve sürdürülebilir bir yapı sunmaktadır.

## 🏗️ Proje Yapısı

Proje aşağıdaki katmanlardan oluşmaktadır:

- **Domain Layer**: İş mantığının ve domain modellerinin bulunduğu katman
- **Application Layer**: Uygulama servislerinin ve iş mantığının bulunduğu katman
- **Infrastructure Layer**: Veritabanı, harici servisler ve altyapı kodlarının bulunduğu katman
- **WebApi Layer**: API endpoint'lerinin ve controller'ların bulunduğu katman

## 🛠️ Kullanılan Teknolojiler

### Core Framework
- .NET 9.0

### NuGet Paketleri
- **AspNetCoreRateLimit (5.0.0)**: API rate limiting için
- **AWSSDK.S3 (4.0.0.3)**: AWS S3 entegrasyonu için
- **Microsoft.AspNetCore.Authentication.JwtBearer (9.0.4)**: JWT tabanlı kimlik doğrulama
- **Microsoft.AspNetCore.OpenApi (9.0.3)**: OpenAPI/Swagger desteği
- **Scalar.AspNetCore (2.2.7)**: GraphQL desteği
- **Microsoft.EntityFrameworkCore.Design (9.0.3)**: Entity Framework Core tasarım araçları
- **Serilog.AspNetCore (9.0.0)**: Gelişmiş loglama
- **Serilog.Sinks.Graylog (3.1.1)**: Graylog entegrasyonu
- **System.IdentityModel.Tokens.Jwt (8.9.0)**: JWT token işlemleri

## 🚀 Başlangıç

1. Projeyi klonlayın
```bash
git clone https://github.com/berkaykanca/DomainDrivenDesign.git
```

2. Gerekli bağımlılıkları yükleyin
```bash
dotnet restore
```

3. Projeyi çalıştırın
```bash
dotnet run --project DomainDrivenDesign.WebApi
```

## 📚 API Dokümantasyonu

API dokümantasyonuna Swagger UI üzerinden erişebilirsiniz:
```
https://localhost:5001/swagger
```

## 🔐 Güvenlik

- JWT tabanlı kimlik doğrulama
- Rate limiting ile API koruması
- Güvenli veri transferi

## 📝 Lisans

Bu proje MIT lisansı altında lisanslanmıştır.

## 👨‍💻 Yazar

**Berkay KANCA**

---

<div align="center">
  <sub>Built with ❤️ by Berkay KANCA</sub>
</div>
