CREATE TABLE [dbo].[StatusMaster] (
    [StatusID]     INT              IDENTITY (1, 1) NOT NULL,
    [StatusName]   NVARCHAR (200)   NOT NULL,
    [StatusColor]  VARCHAR (10)     NULL,
    [CompanyID]    INT              NOT NULL,
    [ActiveStatus] TINYINT          DEFAULT ((1)) NOT NULL,
    [CreateDate]   DATETIME         DEFAULT (getdate()) NOT NULL,
    [CreatedByID]  NVARCHAR (100)   NOT NULL,
    [ModifiedByID] NVARCHAR (100)   NULL,
    [ModifiedDate] DATETIME         NULL,
    [Guids]        UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    CONSTRAINT [PK_StatusMaster] PRIMARY KEY CLUSTERED ([StatusID] ASC),
    CONSTRAINT [FK_StatusMaster_CompanyMaster] FOREIGN KEY ([CompanyID]) REFERENCES [dbo].[CompanyMaster] ([CompanyID])
);




GO
CREATE NONCLUSTERED INDEX [IX_StatusMaster_CompanyID]
    ON [dbo].[StatusMaster]([CompanyID] ASC);

