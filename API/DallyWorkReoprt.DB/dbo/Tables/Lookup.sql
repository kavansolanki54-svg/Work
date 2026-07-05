CREATE TABLE [dbo].[Lookup] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [TypeId]       SMALLINT       NOT NULL,
    [LookupName]   NVARCHAR (200) NOT NULL,
    [Icon]         NVARCHAR (200) NULL,
    [ActiveStatus] BIT            CONSTRAINT [DF_Table_1_ActiveStatus] DEFAULT ((1)) NOT NULL,
    [DisplayOrder] INT            NOT NULL,
    CONSTRAINT [PK_Table_1] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Lookup_LookupType] FOREIGN KEY ([TypeId]) REFERENCES [dbo].[LookupType] ([Id])
);


GO
EXECUTE sp_addextendedproperty @name = N'softtable', @value = N'1', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Lookup';

