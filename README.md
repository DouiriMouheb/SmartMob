# SmartMob API - Docker Deployment (Remote Database)

This project is a .NET 9.0 Web API with Entity Framework Core that provides CRUD operations for database management. It's containerized with Docker and connects to a remote database.

## Quick Start (New Machine Setup)

1. **Clone the repository:**
   ```bash
   git clone <your-repo-url>
   cd api
   ```

2. **Create appsettings.json with your database connection:**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Data Source=your-server-ip;Initial Catalog=SMARTMOB;User Id=sa;Password=your-password;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
     },
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning"
       }
     },
     "AllowedHosts": "*"
   }
   ```

3. **Start the application:**
   ```bash
   docker-compose up --build
   ```

4. **Access the API:**
   - API: http://localhost:5000
   - Swagger UI: http://localhost:5000/swagger

## What Gets Created

The Docker setup creates:
- **API container** (port 5000) with your .NET application
- **Uses your appsettings.json** for database connection (no environment variables needed)

## Database Setup

Since you're using a remote database, ensure that:
1. **Database exists**: The SmartMob database should exist on your remote server
2. **Tables exist**: Run the `init-db.sql` script on your remote database to create tables
3. **Network access**: The Docker container can reach your remote database
4. **Firewall rules**: Database server allows connections from your deployment environment

## API Endpoints

### Database Records (TCFG_PARAMETRI_GENERALI)
- `GET /api/DatabaseRecords` - Get all records
- `GET /api/DatabaseRecords/{name}` - Get record by name
- `POST /api/DatabaseRecords` - Create new record
- `PUT /api/DatabaseRecords/{name}` - Update record
- `DELETE /api/DatabaseRecords/{name}` - Delete record

### Tipologie (TCFG_SIGNIFICATI_TIPOLOGIE_PARAMETRI)
- `GET /api/Tipologie` - Get all tipologie
- `GET /api/Tipologie/{id}` - Get tipologia by ID

### Controllo Qualità (ARTICOLI_CONTROLLO_QUALITA)
- `GET /api/ControlloQualita` - Get all articoli
- `GET /api/ControlloQualita/{id}` - Get articolo by ID
- `GET /api/ControlloQualita/by-code/{codArticolo}` - Get by article code
- `GET /api/ControlloQualita/by-linea/{codLinea}` - Get by production line
- `POST /api/ControlloQualita` - Create new articolo
- `PUT /api/ControlloQualita/{id}` - Update articolo
- `DELETE /api/ControlloQualita/{id}` - Delete articolo

## Docker Configuration

### Simple Configuration
- **Just create appsettings.json** with your connection string
- **No environment variables needed** - Docker uses the appsettings.json file directly
- **API Port:** 5000 (mapped from container's 8080)

### Connection String Format
Update the `DefaultConnection` in your `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=YOUR_SERVER_IP;Initial Catalog=SMARTMOB;User Id=sa;Password=YOUR_PASSWORD;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
  }
}
```

## Development

### Local Development (without Docker)
1. Ensure SQL Server is running locally
2. Update connection string in `appsettings.Development.json`
3. Run migrations: `dotnet ef database update`
4. Start API: `dotnet run`

### Database Migrations
The Docker setup automatically initializes the database. For local development:
```bash
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

## Troubleshooting

### Container Issues
```bash
# View container logs
docker-compose logs api

# Restart container
docker-compose down
docker-compose up --build
```

### Database Connection
- Ensure your remote database is accessible from the container
- Check firewall rules and network connectivity
- Verify connection string format in appsettings.json
- Test connection from your local machine first

### Port Conflicts
If port 5000 is in use, modify the port in `docker-compose.yml`:
```yaml
ports:
  - "5001:8080"  # Change API port to 5001
```

## Production Deployment

1. **Set up remote database** with proper security and access rules
2. **Configure connection string** in `.env` file with production credentials
3. **Use environment-specific configuration** files
4. **Configure proper SSL certificates** for HTTPS
5. **Set up proper logging and monitoring**
6. **Use Docker secrets** for sensitive connection strings in production

## Database Migration

Since you're using a remote database, you'll need to apply the database schema manually:

1. **Run the initialization script** on your remote database:
   ```sql
   -- Execute the contents of init-db.sql on your remote database
   ```

2. **For ongoing migrations** (during development):
   ```bash
   # Update connection string in appsettings.Development.json to point to remote DB
   dotnet ef migrations add <MigrationName>
   dotnet ef database update
   ```

## File Structure

```
api/
├── Controllers/           # API Controllers
├── Data/                 # Entity Framework DbContext
├── DTOs/                 # Data Transfer Objects
├── Models/               # Entity Models
├── Properties/           # Launch settings
├── Dockerfile           # Container build instructions
├── docker-compose.yml   # Multi-container setup
├── init-db.sql         # Database initialization script
├── .dockerignore       # Docker build exclusions
└── README.md           # This file
```

## Support

For issues or questions, check:
1. Container logs: `docker-compose logs api`
2. API health: http://localhost:5000/swagger
3. Database connectivity: Ensure remote database is accessible and connection string is correct
4. Network issues: Test database connection from local machine first
