CREATE TABLE [dbo].[EmailRecipients] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [Email]         NVARCHAR (200) NOT NULL,
    [Name]          NVARCHAR (100) NULL,
    [RecipientType] NVARCHAR (10)  NOT NULL,
    [ActiveStatus]  BIT            NOT NULL,
    [CreateDate]    DATETIME       NOT NULL,
    [EmployeeID]    INT            NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EmailRecipients_EmployeeMaster] FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[EmployeeMaster] ([EmployeeID])
);

