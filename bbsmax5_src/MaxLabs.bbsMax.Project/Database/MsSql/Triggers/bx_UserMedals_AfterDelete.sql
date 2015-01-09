
EXEC bx_Drop 'bx_UserMedals_AfterDelete';

GO


CREATE TRIGGER [bx_UserMedals_AfterDelete]
	ON [bx_UserMedals]
	AFTER DELETE
AS
BEGIN

/* include:Trigger_BuildExtendedData.sql
	$table$=[DELETED]
*/

END