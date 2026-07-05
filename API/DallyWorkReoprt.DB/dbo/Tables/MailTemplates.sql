CREATE TABLE [dbo].[MailTemplates] (
    [Id]              INT              IDENTITY (1, 1) NOT NULL,
    [TemplateName]    NVARCHAR (50)    NOT NULL,
    [SubjectFormat]   NVARCHAR (200)   NOT NULL,
    [HeaderHtml]      NVARCHAR (MAX)   NULL,
    [BodyHtml]        NVARCHAR (MAX)   NOT NULL,
    [FooterHtml]      NVARCHAR (MAX)   NULL,
    [CompanyId]       INT              NOT NULL,
    [ActiveStatus]    TINYINT          DEFAULT ((1)) NOT NULL,
    [CreatedByID]     VARCHAR (20)     NULL,
    [CreateDate]      DATETIME         DEFAULT (getdate()) NULL,
    [ModifiedByID]    VARCHAR (20)     NULL,
    [ModifiedDate]    DATETIME         NULL,
    [Guids]           UNIQUEIDENTIFIER DEFAULT (newid()) NULL,
    [EmployeeId]      INT              NULL,
    [TableConfigJson] NVARCHAR (MAX)   NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_MailTemplates_Employee]
    ON [dbo].[MailTemplates]([EmployeeId] ASC, [ActiveStatus] ASC);

