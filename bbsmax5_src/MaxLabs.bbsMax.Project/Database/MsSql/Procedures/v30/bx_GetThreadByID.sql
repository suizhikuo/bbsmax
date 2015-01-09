-- =============================================
-- Author:		<zzbird>
-- Create date: <2006/12/8>
-- Description:	<获得主题和回复，并获得附件和评分记录>
-- =============================================
CREATE PROCEDURE [bx_GetThreadByID]
    @ThreadID int
AS
BEGIN
    SET NOCOUNT ON;

	SELECT *,IsClosed = CASE
		WHEN ThreadType=1 THEN ( SELECT CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END FROM bx_Polls p1 WITH (NOLOCK) WHERE p1.ThreadID = bx_Threads.ThreadID )
		WHEN ThreadType=2 THEN ( SELECT CASE WHEN IsClosed=1 THEN 1 ELSE (CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END) END FROM bx_Questions q1 WITH (NOLOCK) WHERE q1.ThreadID = bx_Threads.ThreadID )
		WHEN ThreadType=4 THEN ( SELECT CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END FROM bx_Polemizes p1 WITH (NOLOCK) WHERE p1.ThreadID = bx_Threads.ThreadID )
		ELSE 0
		END FROM [bx_Threads] WITH (NOLOCK) WHERE ThreadID = @ThreadID;
     
END

