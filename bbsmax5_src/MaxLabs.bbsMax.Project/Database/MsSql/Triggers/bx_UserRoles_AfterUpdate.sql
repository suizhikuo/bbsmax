
EXEC bx_Drop 'bx_UserRoles_AfterUpdate';

GO


CREATE TRIGGER [bx_UserRoles_AfterUpdate]
	ON [bx_UserRoles]
	AFTER INSERT, UPDATE
AS
BEGIN

/* include:Trigger_BuildExtendedData.sql
	$table$=[INSERTED]
*/

END