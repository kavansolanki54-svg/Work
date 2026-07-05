CREATE TABLE [dbo].[SoftwareModulesMaster] (
    [SoftwareModulesMasterID] INT              IDENTITY (1, 1) NOT NULL,
    [ParentId]                INT              NULL,
    [ModulesName]             VARCHAR (200)    NOT NULL,
    [ControllersName]         VARCHAR (100)    NOT NULL,
    [ActionName]              VARCHAR (200)    NOT NULL,
    [HasCreate]               BIT              NULL,
    [FullURL]                 VARCHAR (500)    NULL,
    [DisplayOrder]            INT              NULL,
    [ActiveStatus]            TINYINT          CONSTRAINT [DF_SoftwareModulesMaster_ActiveStatus] DEFAULT ((1)) NOT NULL,
    [CrDate]                  DATETIME         CONSTRAINT [DF_SoftwareModulesMaster_CrDate] DEFAULT (getdate()) NOT NULL,
    [Guids]                   UNIQUEIDENTIFIER CONSTRAINT [DF_SoftwareModulesMaster_Guids] DEFAULT (newid()) NOT NULL,
    [ImagePath]               NVARCHAR (200)   NULL,
    [Icon]                    NVARCHAR (200)   NULL,
    [Description]             NVARCHAR (500)   NULL,
    [IsNew]                   BIT              NULL,
    [ExternalURI]             VARCHAR (1000)   NULL,
    [DisplayLevel]            TINYINT          CONSTRAINT [DF_SoftwareModulesMaster_DisplayLevel] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ModulesMaster] PRIMARY KEY NONCLUSTERED ([SoftwareModulesMasterID] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_SoftwareModulesMaster_SoftwareModulesMaster] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[SoftwareModulesMaster] ([SoftwareModulesMasterID])
);


GO
ALTER TABLE [dbo].[SoftwareModulesMaster] NOCHECK CONSTRAINT [FK_SoftwareModulesMaster_SoftwareModulesMaster];


GO
EXECUTE sp_addextendedproperty @name = N'softtable', @value = N'1', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SoftwareModulesMaster';


GO
EXECUTE sp_addextendedproperty @name = N'tbl_Description', @value = N'soft table for module menu data', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SoftwareModulesMaster';

