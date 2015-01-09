--忽略错误 修改 所有对象的所有者为当前登陆用户 
declare @s nvarchar(4000)
declare tb cursor local for
select 'sp_changeobjectowner ''['+replace(user_name(uid),']',']]')+'].['+replace(name,']',']]')+']'','''+user_name()+''''
from sysobjects
where xtype in('U','V','P','FN','IF','TF') and status>=0 and user_name(uid)<>user_name()
and (name LIKE 'Max_%' OR name LIKE 'bbsMax_%' OR name LIKE 'System_Max_%' OR name LIKE 'System_bbsMax_%' OR name LIKE 'bx_%')
open tb
fetch tb into @s
while @@fetch_status=0
begin
exec(@s)
fetch tb into @s
end
close tb
deallocate tb