CREATE VIEW bx_ImpressionRecordsWithTypeInfo
AS
	SELECT A.RecordID, A.TypeID, A.UserID, A.TargetUserID, A.CreateDate, B.Text, B.KeywordVersion FROM bx_ImpressionRecords AS A LEFT JOIN bx_ImpressionTypes B ON B.TypeID = A.TypeID