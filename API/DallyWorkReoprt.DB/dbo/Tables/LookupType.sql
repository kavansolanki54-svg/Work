CREATE TABLE [dbo].[LookupType] (
    [Id]           SMALLINT       IDENTITY (1, 1) NOT NULL,
    [TypeName]     NVARCHAR (200) NOT NULL,
    [Icon]         NVARCHAR (200) NULL,
    [ActiveStatus] BIT            CONSTRAINT [DF_LookupType_ActiveStatus] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_LookupType] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'softtable', @value = N'1', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LookupType';

