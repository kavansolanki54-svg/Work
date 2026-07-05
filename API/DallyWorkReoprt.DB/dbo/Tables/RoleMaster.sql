CREATE TABLE [dbo].[RoleMaster] (
    [RoleMasterId] INT              IDENTITY (1, 1) NOT NULL,
    [CompanyId]    INT              NOT NULL,
    [RoleName]     NVARCHAR (100)   NOT NULL,
    [RoleTypeId]   INT              NOT NULL,
    [Descriptions] NVARCHAR (300)   NULL,
    [ActiveStatus] TINYINT          NOT NULL,
    [CreatedByID]  NVARCHAR (100)   NOT NULL,
    [CreateDate]   DATETIME         NOT NULL,
    [ModifiedByID] NVARCHAR (100)   NULL,
    [ModifiedDate] DATETIME         NULL,
    [Guids]        UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_RoleMaster] PRIMARY KEY CLUSTERED ([RoleMasterId] ASC),
    CONSTRAINT [FK_RoleMaster_InstituteMaster] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[CompanyMaster] ([CompanyID]),
    CONSTRAINT [FK_RoleMaster_Lookup] FOREIGN KEY ([RoleTypeId]) REFERENCES [dbo].[Lookup] ([Id])
);

