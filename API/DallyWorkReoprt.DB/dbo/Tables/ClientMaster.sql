CREATE TABLE [dbo].[ClientMaster] (
    [ClientID]        INT              IDENTITY (1, 1) NOT NULL,
    [ClientName]      NVARCHAR (200)   NOT NULL,
    [ClientShortCode] VARCHAR (50)     NULL,
    [CompanyID]       INT              NOT NULL,
    [ActiveStatus]    TINYINT          DEFAULT ((1)) NOT NULL,
    [CreateDate]      DATETIME         DEFAULT (getdate()) NOT NULL,
    [CreatedByID]     VARCHAR (100)    NOT NULL,
    [ModifiedByID]    VARCHAR (100)    NULL,
    [ModifiedDate]    DATETIME         NULL,
    [Guids]           UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    CONSTRAINT [PK_ClientMaster] PRIMARY KEY CLUSTERED ([ClientID] ASC),
    CONSTRAINT [FK_ClientMaster_CompanyMaster] FOREIGN KEY ([CompanyID]) REFERENCES [dbo].[CompanyMaster] ([CompanyID])
);




GO
CREATE NONCLUSTERED INDEX [IX_ClientMaster_CompanyID]
    ON [dbo].[ClientMaster]([CompanyID] ASC);

