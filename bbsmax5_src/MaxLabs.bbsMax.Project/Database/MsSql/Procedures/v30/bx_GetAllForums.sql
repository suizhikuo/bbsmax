CREATE PROCEDURE [bx_GetAllForums]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM bx_Forums WITH (NOLOCK) ORDER BY [ParentID],[SortOrder] ASC;
END