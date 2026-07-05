CREATE TABLE [dbo].[RefreshToken] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [Token]      VARCHAR (1000) NOT NULL,
    [JwtId]      VARCHAR (MAX)  NOT NULL,
    [UserId]     INT            NOT NULL,
    [ExpiryDate] DATETIME       NOT NULL,
    [IsUsed]     BIT            NOT NULL,
    [IsRevoked]  BIT            NOT NULL,
    [CreatedAt]  DATETIME       CONSTRAINT [DF_RefreshToken_CreatedAt] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_RefreshToken] PRIMARY KEY CLUSTERED ([Id] ASC)
);

