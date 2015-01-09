EXEC bx_Drop 'bx_GetStringTable_ntext';

GO

CREATE FUNCTION [bx_GetStringTable_ntext]
(
	@text ntext,
	@separator nvarchar(2) = N','
)
RETURNS @ItemTable TABLE 
(
	id int identity(1,1),
	item nvarchar(4000) 
)
AS
BEGIN

	DECLARE @s nvarchar(4000), @i int, @j int   
	SELECT @s = SUBSTRING(@text, 1, 4000), @i=1;  
	--IF (@s = '')
		--INSERT @ItemTable VALUES ('')
	--ELSE BEGIN
	IF (@s <> N'') BEGIN
		WHILE @s <> N'' BEGIN

			IF LEN(@s) = 4000   
				SELECT @j = 4000 - CHARINDEX(@separator, REVERSE(@s)), @i = @i + @j + 1, @s = LEFT(@s, @j)
			ELSE     
				SELECT @i = @i + 4000, @j = LEN(@s)

			INSERT @ItemTable
			SELECT SUBSTRING(@s, I ,CHARINDEX(@separator, @s + @separator, I) - I)
			FROM bx_Identities_4000 WITH (NOLOCK)
			WHERE I <= @j + 1 AND CHARINDEX(@separator, @separator + @s, I) - I = 0

			SELECT @s = SUBSTRING(@text, @i, 4000)

		END  
	END 
	RETURN;
END

GO


