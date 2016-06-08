  declare @companyId int
  select @companyId = CompanyLmsId
  from CompanyLms
  where consumerKey = 'fbf4fed1-375f-42a1-9d2c-163d566f4dd1'
  --print @companyId

  insert into LmsCompanySetting values (@companyId, 'useSakaiEvents', 'True')