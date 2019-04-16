


select p.PoolId, p.name, r.Name, rc.*
from auth.RoleClaim rc
inner join auth.Role r on rc.RoleId = r.Id
inner join auth.UserPool p on r.PoolId = p.PoolId
order by p.PoolId, r.Name



