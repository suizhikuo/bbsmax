EXEC bx_Drop 'bx_GetStringTable_nvarchar';

GO

CREATE FUNCTION [bx_GetStringTable_nvarchar]
(
	@text nvarchar(4000),
	@separator nvarchar(2) = N','
)
RETURNS @ItemTable TABLE 
(
	 id   int IDENTITY(1, 1) 
	,item nvarchar(4000)
)
AS
BEGIN

	IF (@text = N'')
		RETURN;

	INSERT @ItemTable
	SELECT SUBSTRING(@text, I ,CHARINDEX(@separator, @text + @separator, I) - I)   
	FROM [bx_Identities_8000] WITH (NOLOCK)
	WHERE I <= LEN(@text) + 1 AND CHARINDEX(@separator, @separator + @text, I) - I = 0   

	RETURN;

END

GO