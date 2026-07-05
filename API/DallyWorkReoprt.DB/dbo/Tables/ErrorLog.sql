CREATE TABLE [dbo].[ErrorLog] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [Message]    NVARCHAR (MAX) NULL,
    [StackTrace] NVARCHAR (MAX) NULL,
    [Controller] NVARCHAR (200) NULL,
    [Action]     NVARCHAR (200) NULL,
    [CreatedOn]  DATETIME       DEFAULT (getdate()) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

