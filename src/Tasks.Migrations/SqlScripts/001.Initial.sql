CREATE TABLE [Application]
	(
	ApplicationId int IDENTITY(1,1) NOT NULL,
	[Name] nvarchar(450) NOT NULL	
	CONSTRAINT [PK_Application] PRIMARY KEY CLUSTERED (ApplicationId ASC),
	CONSTRAINT [UQ_Application_Name] UNIQUE NONCLUSTERED ([Name] ASC)
	)  ON [PRIMARY]

CREATE TABLE EventDefinition
	(
	EventDefinitionId int IDENTITY(1,1) NOT NULL,
	[Name] nvarchar(400) NOT NULL,
	Topic nvarchar(500) NULL,
	ApplicationId int NOT NULL,
	CONSTRAINT PK_EventDefinition PRIMARY KEY CLUSTERED (EventDefinitionId ASC),
	CONSTRAINT UQ_EventDefinition_ApplicationId_Name UNIQUE NONCLUSTERED (ApplicationId ASC, [Name] ASC),
	CONSTRAINT FK_EventDefinition_Application FOREIGN KEY (ApplicationId) REFERENCES [Application] (ApplicationId) ON UPDATE  NO ACTION ON DELETE  NO ACTION
	)  ON [PRIMARY]

CREATE TABLE ProcessDefinition
	(
	ProcessDefinitionId int IDENTITY(1,1) NOT NULL,
	[Name] nvarchar(400) NOT NULL,
	ProcessIdentifierEventProps nvarchar(500) NULL,
	ApplicationId int NOT NULL,
	[Enabled] BIT NOT NULL,
	CONSTRAINT PK_ProcessDefinition PRIMARY KEY CLUSTERED (ProcessDefinitionId ASC),
	CONSTRAINT UQ_ProcessDefinition_Name UNIQUE NONCLUSTERED (ApplicationId ASC,[Name] ASC),
	CONSTRAINT FK_ProcessDefinition_Application FOREIGN KEY (ApplicationId) REFERENCES [Application] (ApplicationId) ON UPDATE  NO ACTION ON DELETE  NO ACTION
	)  ON [PRIMARY]

CREATE TABLE ProcessEventDefinition
	(
	ProcessDefinitionId int NOT NULL,
	EventDefinitionId int NOT NULL,
	ProcessIdentifierProps nvarchar(500) NULL,
	CONSTRAINT PK_ProcessEventDefinition PRIMARY KEY CLUSTERED (ProcessDefinitionId, EventDefinitionId),
	CONSTRAINT FK_ProcessEventDefinition_EventDefinition FOREIGN KEY (EventDefinitionId) REFERENCES EventDefinition	(EventDefinitionId) ON UPDATE  NO ACTION ON DELETE  NO ACTION ,
	CONSTRAINT FK_ProcessEventDefinition_ProcessDefinition FOREIGN KEY	(ProcessDefinitionId) REFERENCES ProcessDefinition (ProcessDefinitionId) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
	)  ON [PRIMARY]

CREATE TABLE TaskDefinition
	(
	TaskDefinitionId int IDENTITY(1,1) NOT NULL,
	ProcessDefinitionId int NOT NULL,
	Name nvarchar(500) NOT NULL,
	StartEventDefinitionId int NULL,
	EndEventDefinitionId int NULL,
	CancelEventDefinitionId int NULL,
	StartExpression NVARCHAR(500) NULL,
	EndExpression NVARCHAR(500) NULL,
	CancelExpression NVARCHAR(500) NULL,
	GroupAllocationExpression NVARCHAR(500) NULL,
	UserAllocationExpression NVARCHAR(500) NULL,
	CONSTRAINT	PK_TaskDefinition PRIMARY KEY CLUSTERED (TaskDefinitionId ASC),
	CONSTRAINT FK_TaskDefinition_ProcessDefinition FOREIGN KEY (ProcessDefinitionId) REFERENCES ProcessDefinition	(ProcessDefinitionId) ON UPDATE  NO ACTION ON DELETE  NO ACTION,
	--CONSTRAINT FK_TaskDefinition_EventDefinition_Start FOREIGN KEY (StartEventDefinitionId) REFERENCES EventDefinition	(EventDefinitionId) ON UPDATE  NO ACTION ON DELETE  NO ACTION,
	CONSTRAINT FK_TaskDefinition_EventDefinition_Start FOREIGN KEY (ProcessDefinitionId, StartEventDefinitionId) REFERENCES ProcessEventDefinition	(ProcessDefinitionId, EventDefinitionId) ON UPDATE  NO ACTION ON DELETE  NO ACTION,
	--CONSTRAINT FK_TaskDefinition_EventDefinition_End FOREIGN KEY (EndEventDefinitionId) REFERENCES EventDefinition	(EventDefinitionId) ON UPDATE  NO ACTION ON DELETE  NO ACTION,
	CONSTRAINT FK_TaskDefinition_EventDefinition_End FOREIGN KEY (ProcessDefinitionId, EndEventDefinitionId) REFERENCES ProcessEventDefinition	(ProcessDefinitionId, EventDefinitionId) ON UPDATE  NO ACTION ON DELETE  NO ACTION,
	--CONSTRAINT FK_TaskDefinition_EventDefinition_Cancel FOREIGN KEY (CancelEventDefinitionId) REFERENCES EventDefinition	(EventDefinitionId) ON UPDATE  NO ACTION ON DELETE  NO ACTION
	CONSTRAINT FK_TaskDefinition_EventDefinition_Cancel FOREIGN KEY (ProcessDefinitionId, CancelEventDefinitionId) REFERENCES ProcessEventDefinition	(ProcessDefinitionId, EventDefinitionId) ON UPDATE  NO ACTION ON DELETE  NO ACTION
	)  ON [PRIMARY]

CREATE TABLE [dbo].[EventStoreEvents](
	[EventId] [uniqueidentifier] NOT NULL,
	[EventData] [nvarchar](max) NOT NULL,
	[EventType] [varchar](300) NOT NULL,
	[CorrelationId] [uniqueidentifier] NULL,
	[StreamId] [varchar](200) NOT NULL,
	[StreamVersion] int NOT NULL
 CONSTRAINT [PK_EventStoreEvents] PRIMARY KEY NONCLUSTERED([EventId] ASC)
);

CREATE CLUSTERED INDEX [IX_EventStoreEvents_StreamId] 
	ON [dbo].[EventStoreEvents](StreamId);

CREATE UNIQUE NONCLUSTERED INDEX [IX_EventStoreEvents_StreamId_StreamVersion] 
	ON [dbo].[EventStoreEvents](StreamId, StreamVersion); 


CREATE TYPE dbo.NewEventStoreEvents AS TABLE (
        OrderNo             INT IDENTITY                            NOT NULL,
        EventId             UNIQUEIDENTIFIER                        NOT NULL,
        EventData           NVARCHAR(max)                           NOT NULL,
        EventType           VARCHAR(300)                            NOT NULL,
		CorrelationId       UNIQUEIDENTIFIER                        NULL
);

