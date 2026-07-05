CREATE TABLE [dbo].[EmployeeMaster] (
    [EmployeeID]        INT              IDENTITY (1, 1) NOT NULL,
    [CompanyID]         INT              NOT NULL,
    [RoleMasterID]      INT              NULL,
    [FirstName]         NVARCHAR (100)   NOT NULL,
    [MiddleName]        NVARCHAR (100)   NULL,
    [LastName]          NVARCHAR (100)   NULL,
    [Designation]       NVARCHAR (100)   NULL,
    [Email]             VARCHAR (200)    NOT NULL,
    [Passwords]         VARCHAR (300)    NOT NULL,
    [MobileNo]          VARCHAR (50)     NULL,
    [EmployeePhotoFile] NVARCHAR (200)   NULL,
    [IsAllowLogin]      TINYINT          CONSTRAINT [DF_EmployeeMaster_IsAllowLogin] DEFAULT ((1)) NOT NULL,
    [EmployeeCode]      INT              NULL,
    [BirthDate]         DATE             NULL,
    [DOJ]               DATE             NULL,
    [GenderID]          INT              NULL,
    [Address]           NVARCHAR (MAX)   NULL,
    [SignInAttempt]     TINYINT          CONSTRAINT [DF_EmployeeMaster_SignInAttempt] DEFAULT ((0)) NOT NULL,
    [EmployeeName]      AS               (concat_ws(' ',nullif(ltrim(rtrim([FirstName])),''),nullif(ltrim(rtrim([MiddleName])),''),nullif(ltrim(rtrim([LastName])),''))),
    [ActiveStatus]      TINYINT          CONSTRAINT [DF_EmployeeMaster_ActiveStatus] DEFAULT ((1)) NOT NULL,
    [CreatedByID]       NVARCHAR (100)   NOT NULL,
    [CreateDate]        DATETIME         CONSTRAINT [DF_EmployeeMaster_CreateDate] DEFAULT (getdate()) NOT NULL,
    [ModifiedByID]      NVARCHAR (100)   NULL,
    [ModifiedDate]      DATETIME         CONSTRAINT [DF_EmployeeMaster_ModifiedDate] DEFAULT (getdate()) NULL,
    [Guids]             UNIQUEIDENTIFIER CONSTRAINT [DF_EmployeeMaster_Guids] DEFAULT (newid()) NOT NULL,
    [Tenant]            BIT              CONSTRAINT [DF_EmployeeMaster_Tenant] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_StaffMaster] PRIMARY KEY CLUSTERED ([EmployeeID] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_EmployeeMaster_Lookup] FOREIGN KEY ([GenderID]) REFERENCES [dbo].[Lookup] ([Id]),
    CONSTRAINT [FK_EmployeeMaster_RoleMaster] FOREIGN KEY ([RoleMasterID]) REFERENCES [dbo].[RoleMaster] ([RoleMasterId])
);


GO
ALTER TABLE [dbo].[EmployeeMaster] NOCHECK CONSTRAINT [FK_EmployeeMaster_Lookup];


GO
ALTER TABLE [dbo].[EmployeeMaster] NOCHECK CONSTRAINT [FK_EmployeeMaster_RoleMaster];




GO
ALTER TABLE [dbo].[EmployeeMaster] NOCHECK CONSTRAINT [FK_EmployeeMaster_Lookup];


GO
ALTER TABLE [dbo].[EmployeeMaster] NOCHECK CONSTRAINT [FK_EmployeeMaster_RoleMaster];

