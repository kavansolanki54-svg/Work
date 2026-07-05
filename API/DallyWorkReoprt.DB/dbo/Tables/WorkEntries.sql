CREATE TABLE [dbo].[WorkEntries] (
    [Id]           INT              IDENTITY (1, 1) NOT NULL,
    [WorkReportId] INT              NOT NULL,
    [SrNo]         INT              NOT NULL,
    [Title]        NVARCHAR (500)   NOT NULL,
    [ProjectId]    INT              NOT NULL,
    [StatusId]     INT              NOT NULL,
    [Description]  NVARCHAR (MAX)   NULL,
    [ActiveStatus] TINYINT          DEFAULT ((1)) NOT NULL,
    [CreatedByID]  NVARCHAR (20)    NOT NULL,
    [CreateDate]   DATETIME         NOT NULL,
    [ModifiedBYID] NVARCHAR (20)    NULL,
    [ModifiedDate] DATETIME         NULL,
    [Guids]        UNIQUEIDENTIFIER NOT NULL,
    [ModuleId]     INT              NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_WorkEntries_ModuleMaster] FOREIGN KEY ([ModuleId]) REFERENCES [dbo].[ModuleMaster] ([ModuleID]),
    CONSTRAINT [FK_WorkEntries_WorkReports] FOREIGN KEY ([WorkReportId]) REFERENCES [dbo].[WorkReports] ([Id]) ON DELETE CASCADE
);

