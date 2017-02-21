if not exists (
    select 1 from QuestionType 
	where [Type] = N'Multiple Answer'
)
begin
	SET IDENTITY_INSERT QuestionType ON
	insert into QuestionType (questionTypeId,type, questionTypeOrder, isActive,iconSource)  values (23, N'Multiple Answer', 18, 1, N'ICON_MULTI_CHOICE'  )
	SET IDENTITY_INSERT QuestionType OFF
end

GO

  declare @sakaiId int
  select @sakaiId = lmsProviderId
  from LMSProvider
  where lmsProvider = N'Sakai'

  --print @sakaiId

  if not exists (
      select 1 from [LmsQuestionType]
	  where lmsProviderId = @sakaiId and questionTypeId = 2
	  )
	  begin
		insert into [LmsQuestionType] values(@sakaiId, 2, N'True/False', NULL)
	  end
	  else
	  begin
		print 'already in db'
	  end




	    if not exists (
      select 1 from [LmsQuestionType]
	  where lmsProviderId = @sakaiId and questionTypeId = 17
	  )
	  begin
		insert into [LmsQuestionType] values(@sakaiId, 17, N'Test Numeric', NULL)
	  end
	  else
	  begin
		print 'already in db'
	  end





	  if not exists (
      select 1 from [LmsQuestionType]
	  where lmsProviderId = @sakaiId and questionTypeId = 23
	  )
	  begin
		insert into [LmsQuestionType] values(@sakaiId, 23, N'Multiple Answer', NULL)
	  end
	  else
	  begin
		print 'already in db'
	  end





	  if not exists (
      select 1 from [LmsQuestionType]
	  where lmsProviderId = @sakaiId and questionTypeId = 13
	  )
	  begin
		insert into [LmsQuestionType] values(@sakaiId, 13, N'Rate Scale (Likert)', NULL)
	  end
	  else
	  begin
		print 'already in db'
	  end



	  if not exists (
      select 1 from [LmsQuestionType]
	  where lmsProviderId = @sakaiId and questionTypeId = 18
	  )
	  begin
		insert into [LmsQuestionType] values(@sakaiId, 18, N'Calculated', NULL)
	  end
	  else
	  begin
		print 'already in db'
	  end