# 📝 Slutprojekt - Backend API

Detta är den centrala backend-applikationen för mitt fullstack-projekt. API:et är utvecklat i **C# / ASP.NET Core** och hanterar all affärslogik och databaslagring för projektets två klienter: en React-baserad webbapp och en React Native-baserad mobilapp.

Genom att använda en centraliserad backend synkroniseras all data sömlöst mellan plattformarna i realtid.

## 🚀 Teknisk Stack
* **Språk & Ramverk:** C#, ASP.NET Core
* **Databas:** SQLite
* **ORM:** Entity Framework Core
* **Filhantering:** `app.UseStaticFiles()` för att servera uppladdade bilder

## ✨ Huvudfunktioner
* **Gemensam Databas:** Säkerställer att användare, uppgifter och uppladdade filer är identiska oavsett vilken enhet som används.
* **CORS-konfiguration:** Speciellt anpassad (`AllowAll`) för att acceptera anrop från både lokala webbläsare och externa mobila enheter på nätverket.
* **Dynamisk filhantering:** Tar emot bilder via `FormData` och skapar automatiskt nödvändiga mappar (`wwwroot/uploads`) vid uppstart.

## 🛠️ Kom igång & Starta servern

För att köra detta projekt lokalt behöver du ha .NET SDK installerat på din dator.

### 1. Förberedelser
Klona projektet och navigera in i projektets rotmapp via din terminal. Systemet kommer automatiskt att skapa databasen när det startas.

### 2. Starta för Webbappen (Lokal åtkomst)
Om appen enbart ska testas mot frontend-webbappen, starta API:et med standardkommandot:
```bash
dotnet run