CREATE TABLE [dbo].[Webinar] (
    [webinar_id]          INT           IDENTITY (1, 1) NOT NULL,
    [webinar_date]        SMALLDATETIME NULL,
    [webinar_host]        NCHAR (50)    NULL,
    [webinar_description] NCHAR (255)   NULL,
    CONSTRAINT [PK_Webinar] PRIMARY KEY CLUSTERED ([webinar_id] ASC)
);

