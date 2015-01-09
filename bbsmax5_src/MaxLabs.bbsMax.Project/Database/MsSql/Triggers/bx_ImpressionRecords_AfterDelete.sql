EXEC bx_Drop 'bx_ImpressionRecords_AfterDelete';

GO

CREATE TRIGGER [bx_ImpressionRecords_AfterDelete]
ON [bx_ImpressionRecords] AFTER DELETE
AS
BEGIN

        UPDATE bx_Impressions SET [Count] = [Count] - 1 FROM [DELETED] WHERE bx_Impressions.UserID = [DELETED].TargetUserID AND bx_Impressions.TypeID = [DELETED].TypeID;
     
		UPDATE bx_ImpressionTypes SET RecordCount = RecordCount - 1 FROM [DELETED] WHERE bx_ImpressionTypes.TypeID = [DELETED].TypeID;
END