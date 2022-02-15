CREATE TABLE [dbo].[EventStoreSnapshots](
	[SnapshotData] [nvarchar](max) NOT NULL,
	[SnapshotType] [varchar](300) NOT NULL,
	[StreamId] [varchar](200) NOT NULL,
	[StreamVersion] int NOT NULL
 CONSTRAINT [PK_EventStoreSnapshots] PRIMARY KEY ([StreamId], [StreamVersion] ASC)
);