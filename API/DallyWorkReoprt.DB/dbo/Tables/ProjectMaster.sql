CREATE TABLE [dbo].[ProjectMaster] (
    [ProjectID]    INT              IDENTITY (1, 1) NOT NULL,
    [ProjectName]  NVARCHAR (200)   NOT NULL,
    [ProjectColor] VARCHAR (10)     NULL,
    [CompanyID]    INT              NOT NULL,
    [ActiveStatus] TINYINT          DEFAULT ((1)) NOT NULL,
    [CreateDate]   DATETIME         DEFAULT (getdate()) NOT NULL,
    [CreatedByID]  NVARCHAR (100)   NOT NULL,
    [ModifiedByID] NVARCHAR (100)   NULL,
    [ModifiedDate] DATETIME         NULL,
    [Guids]        UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    CONSTRAINT [PK_ProjectMaster] PRIMARY KEY CLUSTERED ([ProjectID] ASC),
    CONSTRAINT [FK_ProjectMaster_CompanyMaster] FOREIGN KEY ([CompanyID]) REFERENCES [dbo].[CompanyMaster] ([CompanyID])
);




GO
CREATE NONCLUSTERED INDEX [IX_ProjectMaster_CompanyID]
    ON [dbo].[ProjectMaster]([CompanyID] ASC);

