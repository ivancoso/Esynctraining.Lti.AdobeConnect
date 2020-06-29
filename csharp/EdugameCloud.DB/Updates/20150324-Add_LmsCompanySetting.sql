set xact_abort on;
go

begin transaction;
go

create table LmsCompanySetting(
  lmsCompanySettingId int           not null identity constraint PK_LmsCompanySetting primary key,
  lmsCompanyId        int           not null,
  [name]              nvarchar(100) not null,
  value               nvarchar(200) not null
);
go

alter table LmsCompanySetting add
  constraint FK_LmsCompanySetting_LmsCompany foreign key(lmsCompanyId) references CompanyLms(companyLmsId) on delete cascade;
go

INSERT INTO dbo.LmsCompanySetting
  (lmsCompanyId, [name], value)
  VALUES (43, N'DenyACUserCreation', N'true')
go

commit;
go


