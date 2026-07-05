CREATE TABLE [dbo].[WorkTimeLogs] (
    [Id]           INT              IDENTITY (1, 1) NOT NULL,
    [WorkEntryId]  INT              NOT NULL,
    [InTime]       NVARCHAR (10)    NOT NULL,
    [OutTime]      NVARCHAR (10)    NOT NULL,
    [Is30MinBreak] BIT              DEFAULT ((0)) NOT NULL,
    [ActiveStatus] TINYINT          DEFAULT ((1)) NOT NULL,
    [CreatedByID]  NVARCHAR (20)    NOT NULL,
    [CreateDate]   DATETIME         DEFAULT (getdate()) NOT NULL,
    [ModifiedBYID] NVARCHAR (20)    NULL,
    [ModifiedDate] DATETIME         NULL,
    [Guids]        UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_WorkTimeLogs_WorkEntries] FOREIGN KEY ([WorkEntryId]) REFERENCES [dbo].[WorkEntries] ([Id]) ON DELETE CASCADE
);

