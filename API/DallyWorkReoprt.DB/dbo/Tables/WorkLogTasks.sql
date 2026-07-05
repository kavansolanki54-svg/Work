CREATE TABLE [dbo].[WorkLogTasks] (
    [WorkLogTaskId] INT              IDENTITY (1, 1) NOT NULL,
    [WorkLogId]     INT              NOT NULL,
    [Description]   NVARCHAR (MAX)   NOT NULL,
    [StatusId]      INT              NOT NULL,
    [IsCompleted]   BIT              NOT NULL,
    [CreateDate]    DATETIME2 (7)    DEFAULT (getdate()) NOT NULL,
    [CreatedById]   NVARCHAR (100)   NOT NULL,
    [Guids]         UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    CONSTRAINT [PK_WorkLogTasks] PRIMARY KEY CLUSTERED ([WorkLogTaskId] ASC),
    CONSTRAINT [FK_WorkLogTasks_StatusMaster_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [dbo].[StatusMaster] ([StatusID]),
    CONSTRAINT [FK_WorkLogTasks_WorkLogs_WorkLogId] FOREIGN KEY ([WorkLogId]) REFERENCES [dbo].[WorkLogs] ([WorkLogId]) ON DELETE CASCADE
);

