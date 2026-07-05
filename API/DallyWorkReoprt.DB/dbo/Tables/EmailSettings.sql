CREATE TABLE [dbo].[EmailSettings] (
    [EmailSettingsId] INT            IDENTITY (1, 1) NOT NULL,
    [SmtpServer]      NVARCHAR (255) NOT NULL,
    [Port]            INT            NOT NULL,
    [SenderName]      NVARCHAR (255) NOT NULL,
    [SenderEmail]     NVARCHAR (255) NOT NULL,
    [Password]        NVARCHAR (255) NOT NULL,
    [ActiveStatus]    BIT            NOT NULL,
    [CreatedAt]       DATETIME2 (7)  NOT NULL,
    [CreateDate]      DATETIME2 (7)  NULL,
    [EmployeeID]      INT            NULL,
    PRIMARY KEY CLUSTERED ([EmailSettingsId] ASC),
    CONSTRAINT [FK_EmailSettings_EmployeeMaster] FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[EmployeeMaster] ([EmployeeID])
);

