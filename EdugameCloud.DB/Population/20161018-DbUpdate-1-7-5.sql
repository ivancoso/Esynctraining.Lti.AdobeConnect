sp_rename 'LmsCalendarEvent', 'LmsMeetingSession';
GO
sp_rename 'LmsMeetingSession.lmsCalendarEventId', 'lmsMeetingSessionId', 'COLUMN';
GO
sp_rename 'PK_LmsCalendarEvent', 'PK_LmsMeetingSession';
GO
sp_rename 'FK_LmsCalendarEvent_LmsCourseMeeting', 'FK_LmsMeetingSession_LmsCourseMeeting';
GO

alter table LmsMeetingSession
  alter column eventId nvarchar(50) null;
alter table LmsMeetingSession 
  add summary nvarchar(2000);

update questionforsinglemultiplechoice set restrictions='multi_choice' where questionid in
	(select qfs.questionid from
		questionforsinglemultiplechoice qfs
		inner join question q on qfs.questionid=q.questionid
		inner join distractor d on d.questionid=q.questionid and d.isCorrect=1
		inner join submoduleitem smi on q.submoduleitemid=smi.submoduleitemid
		inner join submodulecategory smc on smi.submodulecategoryid=smc.submodulecategoryid
	where smc.companylmsid is null
	group by qfs.questionid
	having count(*)>1);

update question
set randomizeanswers=1
where questiontypeid=8;