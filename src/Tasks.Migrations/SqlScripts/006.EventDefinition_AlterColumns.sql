IF NOT EXISTS
(
  SELECT 1
  FROM sys.columns
  WHERE Name=N'Schema'
        AND Object_ID=OBJECT_ID( N'EventDefinition' )
)
BEGIN
ALTER TABLE EventDefinition
ADD [Schema] nvarchar(max);
END;