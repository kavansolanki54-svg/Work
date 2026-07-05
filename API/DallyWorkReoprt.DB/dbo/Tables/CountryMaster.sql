CREATE TABLE [dbo].[CountryMaster] (
    [CountryID]       INT              NOT NULL,
    [CountryName]     NVARCHAR (50)    NOT NULL,
    [CountryCode]     NVARCHAR (10)    NULL,
    [CurrencyCode]    NVARCHAR (10)    NULL,
    [CurrencySymbole] NVARCHAR (10)    NULL,
    [ActiveStatus]    TINYINT          NOT NULL,
    [Guids]           UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_CountryMaster] PRIMARY KEY CLUSTERED ([CountryID] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'softtable', @value = N'1', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'CountryMaster';

