CREATE TABLE [dbo].[ModuleMaster] (
    [ModuleID]       INT              IDENTITY (1, 1) NOT NULL,
    [ModuleName]     NVARCHAR (200)   NOT NULL,
    [CompanyID]      INT              NOT NULL,
    [ActiveStatus]   TINYINT          DEFAULT ((1)) NOT NULL,
    [CreateDate]     DATETIME         DEFAULT (getdate()) NOT NULL,
    [CreatedByID]    VARCHAR (100)    NOT NULL,
    [ModifiedByID]   VARCHAR (100)    NULL,
    [ModifiedDate]   DATETIME         NULL,
    [Guids]          UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [ParentModuleID] INT              NULL,
    CONSTRAINT [PK_ModuleMaster] PRIMARY KEY CLUSTERED ([ModuleID] ASC),
    CONSTRAINT [FK_ModuleMaster_CompanyMaster] FOREIGN KEY ([CompanyID]) REFERENCES [dbo].[CompanyMaster] ([CompanyID]),
    CONSTRAINT [FK_ModuleMaster_ParentModule] FOREIGN KEY ([ParentModuleID]) REFERENCES [dbo].[ModuleMaster] ([ModuleID])
);




GO
CREATE NONCLUSTERED INDEX [IX_ModuleMaster_CompanyID]
    ON [dbo].[ModuleMaster]([CompanyID] ASC);

