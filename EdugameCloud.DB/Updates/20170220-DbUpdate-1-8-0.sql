alter table dbo.Company
add useEventMapping bit
GO
update dbo.Company
set useEventMapping = 0
GO
alter table dbo.Company add constraint Company_UseEventMappingDefault DEFAULT 0 FOR useEventMapping
