CREATE TABLE [dbo].[RoleMasterSoftwareModules] (
    [RoleMasterSoftwareModulesID] INT              IDENTITY (1, 1) NOT NULL,
    [RoleMasterID]                INT              NOT NULL,
    [SoftwareModulesMasterID]     INT              NOT NULL,
    [View]                        BIT              NULL,
    [Add]                         BIT              NULL,
    [Edit]                        BIT              NULL,
    [Delete]                      BIT              NULL,
    [ActiveStatus]                TINYINT          CONSTRAINT [DF_RoleMasterSoftwareModules_ActiveStatus] DEFAULT ((1)) NOT NULL,
    [CreatedByID]                 VARCHAR (20)     NOT NULL,
    [CreateDate]                  DATETIME         CONSTRAINT [DF_RoleMasterSoftwareModules_CreateDate] DEFAULT (getdate()) NOT NULL,
    [ModifiedByID]                VARCHAR (20)     NULL,
    [ModifiedDate]                DATETIME         NULL,
    [Guids]                       UNIQUEIDENTIFIER CONSTRAINT [DF_RoleMasterSoftwareModules_Guids] DEFAULT (newid()) NOT NULL,
    CONSTRAINT [PK_RoleMasterSoftwareModules] PRIMARY KEY CLUSTERED ([RoleMasterSoftwareModulesID] ASC),
    CONSTRAINT [FK_RoleMasterSoftwareModules_RoleMaster] FOREIGN KEY ([RoleMasterID]) REFERENCES [dbo].[RoleMaster] ([RoleMasterId]),
    CONSTRAINT [FK_RoleMasterSoftwareModules_SoftwareModulesMaster] FOREIGN KEY ([SoftwareModulesMasterID]) REFERENCES [dbo].[SoftwareModulesMaster] ([SoftwareModulesMasterID])
);


GO
ALTER TABLE [dbo].[RoleMasterSoftwareModules] NOCHECK CONSTRAINT [FK_RoleMasterSoftwareModules_RoleMaster];


GO
ALTER TABLE [dbo].[RoleMasterSoftwareModules] NOCHECK CONSTRAINT [FK_RoleMasterSoftwareModules_SoftwareModulesMaster];

