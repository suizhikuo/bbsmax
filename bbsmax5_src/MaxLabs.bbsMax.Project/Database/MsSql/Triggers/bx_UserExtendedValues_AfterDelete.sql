
EXEC bx_Drop 'bx_UserExtendedValues_AfterDelete';

GO


CREATE TRIGGER [bx_UserExtendedValues_AfterDelete]
	ON [bx_UserExtendedValues]
	AFTER DELETE
AS
BEGIN

/* include:Trigger_BuildExtendedData.sql
	$table$=[DELETED]
*/

END