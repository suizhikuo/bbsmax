
EXEC bx_Drop 'bx_UserExtendedValues_AfterUpdate';

GO


CREATE TRIGGER [bx_UserExtendedValues_AfterUpdate]
	ON [bx_UserExtendedValues]
	AFTER INSERT, UPDATE
AS
BEGIN

/* include:Trigger_BuildExtendedData.sql
	$table$=[INSERTED]
*/

END