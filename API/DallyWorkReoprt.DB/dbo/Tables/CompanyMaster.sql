CREATE TABLE [dbo].[CompanyMaster] (
    [CompanyID]          INT              IDENTITY (1, 1) NOT NULL,
    [CompanyName]        NVARCHAR (200)   NOT NULL,
    [Email]              VARCHAR (200)    NOT NULL,
    [IsEmailVerified]    TINYINT          NOT NULL,
    [PhoneNo]            VARCHAR (100)    NULL,
    [IsMobileNoVerified] TINYINT          NOT NULL,
    [Website]            VARCHAR (200)    NULL,
    [PreferredSubDomain] VARCHAR (100)    NULL,
    [FullAddress]        NVARCHAR (300)   NULL,
    [CountryID]          INT              NULL,
    [StateID]            INT              NULL,
    [CityName]           VARCHAR (100)    NULL,
    [Pincode]            VARCHAR (10)     NULL,
    [ActiveStatus]       TINYINT          NOT NULL,
    [CreateDate]         DATETIME         NOT NULL,
    [Guids]              UNIQUEIDENTIFIER NOT NULL,
    [LogoUrl]            VARCHAR (200)    NULL,
    CONSTRAINT [PK_CompanyMaster] PRIMARY KEY CLUSTERED ([CompanyID] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_CompanyMaster_CountryMaster] FOREIGN KEY ([CountryID]) REFERENCES [dbo].[CountryMaster] ([CountryID]),
    CONSTRAINT [FK_CompanyMaster_StateMaster] FOREIGN KEY ([StateID]) REFERENCES [dbo].[StateMaster] ([StateID])
);

