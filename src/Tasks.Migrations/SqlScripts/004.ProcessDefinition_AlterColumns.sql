BEGIN
ALTER TABLE ProcessDefinition
ALTER COLUMN ProcessIdentifierEventProps NVARCHAR(500) NOT NULL
END;