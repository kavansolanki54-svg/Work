CREATE TABLE [dbo].[StateMaster] (
    [StateID]      INT              NOT NULL,
    [CountryID]    INT              NOT NULL,
    [StateName]    NVARCHAR (50)    NOT NULL,
    [ActiveStatus] TINYINT          NOT NULL,
    [Guids]        UNIQUEIDENTIFIER NOT NULL,
    [StateCode]    VARCHAR (10)     NOT NULL,
    CONSTRAINT [PK_StateMaster] PRIMARY KEY CLUSTERED ([StateID] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'softtable', @value = N'1', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'StateMaster';

