




select top 100 *
from log.LogEvent
where ResourceId = 'NotificationProcessor'
order by Created desc


-- delete from log.LogEvent

