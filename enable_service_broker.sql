-- Run this in SQL Server Management Studio to enable Service Broker
-- Replace 'SMARTMOB' with your actual database name if different

USE master;
GO

-- Check if Service Broker is enabled
SELECT name, is_broker_enabled 
FROM sys.databases 
WHERE name = 'SMARTMOB';

-- Enable Service Broker (if not already enabled)
ALTER DATABASE [SMARTMOB] SET ENABLE_BROKER;

-- Verify it's enabled
SELECT name, is_broker_enabled 
FROM sys.databases 
WHERE name = 'SMARTMOB';

-- Also check that we have permissions
USE SMARTMOB;
GO

-- Check if the current user has necessary permissions
SELECT 
    dp.permission_name,
    dp.state_desc,
    p.name AS principal_name
FROM sys.database_permissions dp
JOIN sys.database_principals p ON dp.grantee_principal_id = p.principal_id
WHERE dp.major_id = 0 AND dp.permission_name IN ('SUBSCRIBE QUERY NOTIFICATIONS', 'CONTROL', 'ALTER');
