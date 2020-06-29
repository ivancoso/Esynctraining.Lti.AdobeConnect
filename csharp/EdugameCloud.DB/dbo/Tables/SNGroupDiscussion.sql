CREATE TABLE [dbo].[SNGroupDiscussion] (
    [snGroupDiscussionId]  INT            IDENTITY (1, 1) NOT NULL,
    [acSessionId]          INT            NOT NULL,
    [groupDiscussionData]  NTEXT          NOT NULL,
    [groupDiscussionTitle] NVARCHAR (255) NULL,
    [dateCreated]          DATETIME       NOT NULL,
    [dateModified]         DATETIME       NULL,
    [isActive]             BIT            NOT NULL,
    CONSTRAINT [PK_SNGroupDiscussion] PRIMARY KEY CLUSTERED ([snGroupDiscussionId] ASC),
    CONSTRAINT [FK_SNGroupDiscussion_ACSession] FOREIGN KEY ([acSessionId]) REFERENCES [dbo].[ACSession] ([acSessionId])
);



