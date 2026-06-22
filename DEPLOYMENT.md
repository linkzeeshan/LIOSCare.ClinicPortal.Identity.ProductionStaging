# LIOSCare Clinic Portal - Production Deployment Guide

## 🚀 Quick Start

### 1. Environment Setup
```bash
# Copy environment template
cp .env.example .env

# Edit .env with your production values
nano .env
```

### 2. Database Setup
```sql
-- Apply TimeOnly column fix if not already applied
ALTER TABLE portal.doctor_profiles 
ALTER COLUMN "WorkStartTime" 
TYPE time without time zone 
USING "WorkStartTime"::time without time zone;

ALTER TABLE portal.doctor_profiles 
ALTER COLUMN "WorkEndTime" 
TYPE time without time zone 
USING "WorkEndTime"::time without time zone;
```

### 3. Docker Deployment (Recommended)
```bash
# Build and start all services
docker-compose up -d

# Check logs
docker-compose logs -f lioscare-app

# Health check
curl http://localhost:8080/health
```

### 4. Manual Deployment
```bash
# Build the application
cd src/LIOSCare.ClinicPortal.Web
dotnet build -c Release

# Run migrations
dotnet ef database update

# Start the application
dotnet run --environment Production
```

## 🔧 Configuration

### Environment Variables
| Variable | Required | Description |
|----------|----------|-------------|
| `DB_HOST` | ✅ | Database host |
| `DB_PORT` | ✅ | Database port (default: 5432) |
| `DB_NAME` | ✅ | Database name |
| `DB_USER` | ✅ | Database username |
| `DB_PASSWORD` | ✅ | Database password |
| `ALLOWED_HOSTS` | ✅ | Comma-separated allowed hosts |
| `ASPNETCORE_ENVIRONMENT` | ✅ | Environment (Production) |

### Security Features Implemented
- ✅ Environment variable configuration
- ✅ Rate limiting (100 req/sec, 1000 req/min)
- ✅ Security headers (CSP, XSS protection, etc.)
- ✅ Health checks at `/health`
- ✅ Structured logging with Serilog
- ✅ HTTPS enforcement in production

## 📊 Monitoring

### Health Check Endpoint
```bash
curl http://localhost:8080/health
```

Response:
```json
{
  "status": "Healthy",
  "checks": [
    {
      "name": "ApplicationDbContext",
      "status": "Healthy",
      "description": "Database connection successful",
      "duration": 45.2
    }
  ],
  "totalDuration": 45.2
}
```

### Log Files
- Application logs: `logs/lioscare-*.log`
- Docker logs: `docker-compose logs lioscare-app`

## 🔒 Security Checklist

- [ ] Database credentials stored in environment variables
- [ ] SSL/TLS configured for HTTPS
- [ ] Rate limiting rules appropriate for traffic
- [ ] Security headers verified
- [ ] Database access restricted to application
- [ ] Regular backups configured
- [ ] Monitoring and alerting set up

## 🐳 Docker Commands

```bash
# Build image
docker build -t lioscare-portal ./src/LIOSCare.ClinicPortal.Web

# Run container
docker run -d \
  --name lioscare-app \
  -p 8080:80 \
  --env-file .env \
  lioscare-portal

# View logs
docker logs -f lioscare-app

# Stop container
docker stop lioscare-app
```

## 🚨 Troubleshooting

### Common Issues

1. **Database Connection Failed**
   - Check environment variables
   - Verify database is running
   - Check network connectivity

2. **Migration Issues**
   - Run: `dotnet ef database update`
   - Apply TimeOnly column fix manually

3. **Health Check Failing**
   - Check application logs
   - Verify database connectivity
   - Check rate limiting configuration

### Performance Tuning

1. **Database**
   - Add connection pooling
   - Optimize queries
   - Add indexes

2. **Application**
   - Adjust rate limits
   - Configure caching
   - Monitor memory usage

## 📈 Scaling

### Horizontal Scaling
- Use load balancer (nginx included)
- Deploy multiple instances
- Configure sticky sessions if needed

### Database Scaling
- Read replicas for reporting
- Connection pooling
- Regular maintenance

## 🔄 CI/CD Pipeline

### GitHub Actions Example
```yaml
name: Deploy
on:
  push:
    branches: [main]
jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Deploy to production
        run: |
          docker-compose up -d
```

## 📞 Support

For deployment issues:
1. Check logs: `docker-compose logs -f`
2. Verify health: `curl /health`
3. Review configuration files
4. Check environment variables
