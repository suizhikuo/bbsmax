
EXEC bx_Drop 'bx_UserMedals_AfterUpdate';

GO


CREATE TRIGGER [bx_UserMedals_AfterUpdate]
	ON [bx_UserMedals]
	AFTER INSERT, UPDATE
AS
BEGIN

/* include:Trigger_BuildExtendedData.sql
	$table$=[INSERTED]
*/

END