CREATE TABLE [dbo].[School] (
    [SchoolId]                INT            IDENTITY (1, 1) NOT NULL,
    [StateId]                 INT            NOT NULL,
    [SchoolNumber]            NVARCHAR (20)  NULL,
    [OnsiteOperator]          NVARCHAR (500) NULL,
    [FirstDirector]           NVARCHAR (200) NULL,
    [MainPhone]               NVARCHAR (20)  NULL,
    [Fax]                     NVARCHAR (20)  NULL,
    [SpeedDialNumber]         NVARCHAR (20)  NULL,
    [SchoolEmail]             NVARCHAR (100) NULL,
    [CorporateName]           NVARCHAR (100) NULL,
    [FBCRepresentative]       NVARCHAR (200) NULL,
    [ESSRepresentative]       NVARCHAR (200) NULL,
    [StandardsRepresentative] NVARCHAR (200) NULL,
    [AdvRepresentative]       NVARCHAR (200) NULL,
    [MktgRepresentative]      NVARCHAR (200) NULL,
    [AccountName]             NVARCHAR (100) NULL,
    CONSTRAINT [PK_School] PRIMARY KEY CLUSTERED ([SchoolId] ASC),
    FOREIGN KEY ([StateId]) REFERENCES [dbo].[State] ([stateId])
);

