-- yourdatabase.database.windows.net,1433

CREATE LOGIN sdp2017 WITH PASSWORD='abc123!@'

USE LeaderboardNETCore
GO

CREATE USER sdp2017
	FOR LOGIN sdp2017
	WITH DEFAULT_SCHEMA = dbo
GO
-- Add user to the database owner role
EXEC sp_addrolemember N'db_owner', N'sdp2017'
GO
