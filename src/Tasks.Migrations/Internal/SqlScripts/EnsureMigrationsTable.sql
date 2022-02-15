IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE [name] = '__TasksMigration') 
	CREATE TABLE dbo.[__TasksMigration] (
		ScriptsVersion [int] NOT NULL
	)

INSERT INTO dbo.[__TasksMigration] (ScriptsVersion) 
	SELECT 0 
	WHERE NOT EXISTS(SELECT 1 FROM dbo.[__TasksMigration]) 