
  UPDATE [CompanyLms]
  SET [acServer] = SUBSTRING ([acServer], 0, len([acServer]))
  where [acServer] like '%/'

  --SELECT *
  --FROM [CompanyLms]
  --WHERE [acServer] like '%/'
