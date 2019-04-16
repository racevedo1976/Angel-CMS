

select *
from auth.[User]
where UserName = 'admin'


select uc.* 
from auth.[User] u
inner join auth.UserClaim uc on u.Id = uc.UserId
where u.UserName = 'admin'

select r.PoolId as pool_id, r.Name as role_name, rc.ClaimType as claim, rc.ClaimValue as claim_value
from auth.[User] u
inner join auth.UserRole ur on u.Id = ur.UserId
inner join auth.[Role] r on ur.RoleId = r.Id
inner join auth.RoleClaim rc on r.Id = rc.RoleId
where u.UserName = 'admin'



