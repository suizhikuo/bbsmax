EXEC bx_Drop 'bx_ImpressionRecords_AfterInsert';

GO

CREATE TRIGGER [bx_ImpressionRecords_AfterInsert]
ON [bx_ImpressionRecords] AFTER INSERT
AS
BEGIN

        UPDATE bx_Impressions SET [Count] = [Count] + 1, [UpdateDate] = GETDATE() FROM [INSERTED] WHERE bx_Impressions.UserID = [INSERTED].TargetUserID AND bx_Impressions.TypeID = [INSERTED].TypeID;
        
		UPDATE bx_ImpressionTypes SET RecordCount = RecordCount + 1 FROM [INSERTED] WHERE bx_ImpressionTypes.TypeID = [INSERTED].TypeID;
		
		UPDATE bx_UserVars SET [LastImpressionDate] = GETDATE() FROM [INSERTED] WHERE bx_UserVars.UserID = [INSERTED].TargetUserID;
END