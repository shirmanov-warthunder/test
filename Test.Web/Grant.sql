IF NOT EXISTS (SELECT name FROM sys.server_principals WHERE name = 'IIS APPPOOL\test')
BEGIN
    CREATE LOGIN [IIS APPPOOL\test] 
      FROM WINDOWS WITH DEFAULT_DATABASE=[master], 
      DEFAULT_LANGUAGE=[us_english]
END
GO
CREATE USER [TrustedUser] 
  FOR LOGIN [IIS APPPOOL\test]
GO
EXEC sp_addrolemember 'db_owner', 'TrustedUser'
GO