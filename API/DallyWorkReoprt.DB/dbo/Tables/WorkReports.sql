CREATE TABLE [dbo].[WorkReports] (
    [Id]           INT              IDENTITY (1, 1) NOT NULL,
    [ReportDate]   DATE             NOT NULL,
    [EmployeeId]   INT              NOT NULL,
    [ActiveStatus] TINYINT          DEFAULT ((1)) NOT NULL,
    [CreatedByID]  NVARCHAR (20)    NOT NULL,
    [CreateDate]   DATETIME         NOT NULL,
    [ModifiedBYID] NVARCHAR (20)    NULL,
    [ModifiedDate] DATETIME         NULL,
    [Guids]        UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_WorkReports_EmployeeMaster] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[EmployeeMaster] ([EmployeeID])
);

