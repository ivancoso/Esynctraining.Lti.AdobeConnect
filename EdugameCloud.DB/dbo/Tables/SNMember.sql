CREATE TABLE [dbo].[SNMember] (
    [snMemberId]         INT            IDENTITY (1, 1) NOT NULL,
    [acSessionId]        INT            NOT NULL,
    [participant]        NVARCHAR (255) NOT NULL,
    [participantProfile] NTEXT          NULL,
    [dateCreated]        DATETIME       NULL,
    [isBlocked]          BIT            CONSTRAINT [DF_SNSessionMember_isBlocked] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_SNSessionMember] PRIMARY KEY CLUSTERED ([snMemberId] ASC),
    CONSTRAINT [FK_SNMember_ACSession] FOREIGN KEY ([acSessionId]) REFERENCES [dbo].[ACSession] ([acSessionId])
);

