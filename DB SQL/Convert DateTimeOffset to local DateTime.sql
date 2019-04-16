

-- Convert to local time
select convert(DATETIME, SWITCHOFFSET(ScheduledDT, DATEPART(tz,SYSDATETIMEOFFSET()))), ScheduledDT, *
FROM cms.Notification
where ScheduledDT > '2017-01-01'

-- Convert to UTC time
select convert(DATETIME, SWITCHOFFSET(ScheduledDT, '+00:00')), ScheduledDT, *
FROM cms.Notification
where ScheduledDT > '2017-01-01'


select ScheduledDT, *
FROM cms.Notification
where ScheduledDT > '2017-09-08 07:00:00'


select convert(DATETIMEOFFSET, GETDATE()), GETDATE()

select convert(DATETIMEOFFSET, '2017-01-01 06:00')
