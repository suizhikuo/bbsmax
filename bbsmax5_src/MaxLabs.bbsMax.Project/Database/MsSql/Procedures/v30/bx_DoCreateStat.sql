CREATE PROCEDURE [bx_DoCreateStat] 
	@ForumID int,
	@StatType int,
	@Count int
AS
BEGIN
SET NOCOUNT ON;
IF EXISTS (SELECT * FROM bx_Stats WHERE StatType=@StatType AND [Year]=year(getdate()) AND [Month]=month(getdate()) AND [Day]=day(getdate()) AND [Hour]=DATEPART(hh, GETDATE()) AND [Param]=@ForumID )
		UPDATE bx_Stats SET [Count]=[Count]+@Count WHERE StatType=@StatType AND [Year]=year(getdate()) AND [Month]=month(getdate()) AND [Day]=day(getdate()) AND [Hour]=DATEPART(hh, GETDATE()) AND [Param]=@ForumID
		ELSE
		INSERT INTO bx_Stats(StatType,[Year],[Month],[Day],[Hour],[Count],[Param])
		VALUES (@StatType,year(getdate()),month(getdate()),day(getdate()),DATEPART(hh, GETDATE()),@Count,@ForumID)
END