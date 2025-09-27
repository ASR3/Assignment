## Customer Subscriptions Web (ASP.NET Core 9, SQL Server LocalDB)

### Prerequisites
- .NET SDK 9.0+
- SQL Server LocalDB (bundled with Visual Studio) or full SQL Server

### Setup
1. Update connection string in `appsettings.json` if not using LocalDB:
   - `Server=(localdb)\\MSSQLLocalDB;Database=CustomerSubscriptionsDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True`
2. Restore packages:
   - `dotnet restore`
3. Create EF Core migrations and update database:
   - `dotnet ef migrations add InitialCreate`
   - `dotnet ef database update`
4. Run the app:
   - `dotnet run`

### Default Admin
- Email: `admin@example.com`
- Password: `Admin#12345`

### Features
- Identity login/register, password reset (email sender stubbed to console)
- Roles: Admin, User (Admin can create/update/delete)
- MVC UI under `Customer Subscription` menu
- APIs under `/api/subscriptions`

#### API Examples
- GET `/api/subscriptions?customerId=C123&subscriptionName=Pro&start=2025-01-01&end=2025-12-31`
- POST `/api/subscriptions` (Admin)
```json
{
  "customerId": "C123",
  "customerName": "Acme Co",
  "subscriptionName": "Pro",
  "subscriptionCount": 5,
  "startDate": "2025-01-01",
  "endDate": "2025-12-31",
  "isActive": true
}
```
- PUT `/api/subscriptions/{id}` (Admin)
- DELETE `/api/subscriptions/{id}` (Admin)
- POST `/api/subscriptions/count`
```json
{ "customerId": "C123", "subscriptionName": "Pro", "delta": 1 }
```

### Notes
- For production, replace `EmailSender` with a real SMTP/SendGrid implementation.
- You can switch to a full SQL Server by updating the connection string.

