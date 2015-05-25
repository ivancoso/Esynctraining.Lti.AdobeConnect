set xact_abort on;
go

begin transaction;
go

alter table LmsUser add
  [name] nvarchar(100),
  email nvarchar(100),
  userIdExtended nvarchar(50);
go

create table LmsUserMeetingRole(
  lmsUserMeetingRoleId int          not null identity constraint PK_LmsUserMeetingRole_1 primary key,
  lmsUserId            int          not null,
  lmsCourseMeetingId   int          not null,
  lmsRole              nvarchar(50)
);
go

alter table LmsUserMeetingRole add
  constraint FK_LmsUserMeetingRole_LmsUser foreign key(lmsUserId) references LmsUser(lmsUserId) on update cascade on delete cascade,
  constraint FK_LmsUserMeetingRole_LmsCourseMeeting foreign key(lmsCourseMeetingId) references LmsCourseMeeting(lmsCourseMeetingId) on update cascade on delete cascade;
go

commit;
go
