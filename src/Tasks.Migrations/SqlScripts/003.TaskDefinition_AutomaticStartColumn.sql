IF NOT EXISTS
(
  SELECT 1
  FROM sys.columns
  WHERE Name=N'AutomaticStart'
        AND Object_ID=OBJECT_ID( N'TaskDefinition' )
)
BEGIN
ALTER TABLE TaskDefinition
ADD AutomaticStart BIT;
END;