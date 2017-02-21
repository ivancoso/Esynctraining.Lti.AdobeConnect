alter table dbo.Company
add useEventMapping bit
GO
update dbo.Company
set useEventMapping = 0
GO
alter table dbo.Company add constraint Company_UseEventMappingDefault DEFAULT 0 FOR useEventMapping


alter table dbo.Quiz
add isPostQuiz bit
GO
update dbo.Quiz
set isPostQuiz = 0
GO
alter table dbo.Quiz add constraint Quiz_IsPostQuiz DEFAULT 0 FOR isPostQuiz