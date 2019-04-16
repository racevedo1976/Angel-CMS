



UPDATE cms.Notification
SET ProcId = 'TEST_0001', Status = 'processing', ProcStartUTC = GETUTCDATE(), RetryCount = 0
WHERE Id in (
	SELECT TOP(10) note.Id
	FROM cms.Notification note
	 WHERE note.ScheduledUTC < GETUTCDATE()
	AND (note.Status = 'scheduled' OR note.Status = 'error')
	AND note.RetryCount < 1000
	ORDER BY note.RetryCount
	)


UPDATE TOP(1) n
SET ProcId = 'TEST_0001', Status = 'processing', ProcStartUTC = GETUTCDATE(), RetryCount = n.RetryCount % 100 + 1
FROM cms.Notification n
WHERE n.ScheduledUTC < GETUTCDATE()
AND (n.Status = 'scheduled' OR n.Status = 'error')
ORDER BY RetryCount








SELECT TOP(10) *
FROM cms.Notification n
WHERE n.ScheduledUTC < GETUTCDATE()
AND n.Status = 'scheduled'
AND n.ProcId is null
Order by n.RetryCount


select * from cms.Notification
Order by RetryCount

select GETUTCDATE()

update cms.Notification 
set ProcId = NULL, Status = 'scheduled'
where Status != 'draft'
