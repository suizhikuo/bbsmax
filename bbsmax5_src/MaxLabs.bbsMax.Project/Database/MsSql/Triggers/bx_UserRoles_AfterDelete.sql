
EXEC bx_Drop 'bx_UserRoles_AfterDelete';

GO


CREATE TRIGGER [bx_UserRoles_AfterDelete]
	ON [bx_UserRoles]
	AFTER DELETE
AS
BEGIN

/* include:Trigger_BuildExtendedData.sql
	$table$=[DELETED]
*/

END