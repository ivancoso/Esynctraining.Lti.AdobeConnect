CREATE TABLE [dbo].[LmsMeetingType] (
    [lmsMeetingTypeId]   INT           IDENTITY (1, 1) NOT NULL,
    [lmsMeetingTypeName] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_LmsMeetingType] PRIMARY KEY CLUSTERED ([lmsMeetingTypeId] ASC)
);

