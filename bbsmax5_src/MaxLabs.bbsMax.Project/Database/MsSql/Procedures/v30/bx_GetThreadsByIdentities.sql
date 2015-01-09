-- =============================================
-- Author:		zzbird
-- Create date: 2006/11/31
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bx_GetThreadsByIdentities]
	@ThreadIdentities varchar(8000)
AS
BEGIN
	SET NOCOUNT ON;

	EXEC ('SELECT *,IsClosed=CASE
WHEN ThreadType=1 THEN ( SELECT CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END FROM bx_Polls p1 WITH (NOLOCK) WHERE p1.ThreadID = bx_Threads.ThreadID )
WHEN ThreadType=2 THEN ( SELECT CASE WHEN IsClosed=1 THEN 1 ELSE (CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END) END FROM bx_Questions q1 WITH (NOLOCK) WHERE q1.ThreadID = bx_Threads.ThreadID )
WHEN ThreadType=4 THEN ( SELECT CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END FROM bx_Polemizes p1 WITH (NOLOCK) WHERE p1.ThreadID = bx_Threads.ThreadID )
ELSE 0
END FROM [bx_Threads] WITH (NOLOCK) WHERE ThreadID in ( ' + @ThreadIdentities + ' )')
END