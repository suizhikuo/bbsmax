-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bx_GetAllModerators]
AS
BEGIN

	SET NOCOUNT ON;
   
    SELECT * FROM bx_Moderators WITH (NOLOCK)  ORDER BY ForumID, SortOrder;
END


