EXEC bx_Drop 'bx_GetStringTable_varchar';

GO

CREATE FUNCTION [bx_GetStringTable_varchar]
(
	@text varchar,
	@separator varchar(2) = ','
)
RETURNS @ItemTable TABLE 
(
	 id   int IDENTITY(1, 1) 
	,item varchar(8000)
)
AS
BEGIN

	IF (@text = '')
		RETURN;

	INSERT @ItemTable
	SELECT SUBSTRING(@text, I ,CHARINDEX(@separator, @text + @separator, I) - I)   
	FROM [bx_Identities_8000] WITH (NOLOCK)
	WHERE I <= LEN(@text) + 1 AND CHARINDEX(@separator, @separator + @text, I) - I = 0   

	RETURN;

END

GO