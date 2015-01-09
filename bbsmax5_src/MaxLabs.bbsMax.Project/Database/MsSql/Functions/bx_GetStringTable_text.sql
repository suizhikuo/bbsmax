EXEC bx_Drop 'bx_GetStringTable_text';

GO

CREATE FUNCTION [bx_GetStringTable_text]
(
	@text text,
	@separator varchar(2) = ','
)
RETURNS @ItemTable TABLE 
(
	id int identity(1,1),
	item varchar(8000) 
)
AS
BEGIN

	DECLARE @s varchar(8000), @i int, @j int
	SELECT @s = SUBSTRING(@text, 1, 8000), @i=1;

	--IF (@s = '')
		--INSERT @ItemTable VALUES ('');
	--ELSE BEGIN

	IF (@s <> '') BEGIN
		WHILE @s <> '' BEGIN

			IF LEN(@s) = 8000   
				SELECT @j = 8000 - CHARINDEX(@separator, REVERSE(@s)), @i = @i + @j + 1, @s = LEFT(@s, @j)
			ELSE     
				SELECT @i = @i + 8000, @j = LEN(@s)

			INSERT @ItemTable
			SELECT SUBSTRING(@s, I ,CHARINDEX(@separator, @s + @separator, I) - I)
				FROM bx_Identities_8000 WITH (NOLOCK)
				WHERE I <= @j + 1 AND CHARINDEX(@separator, @separator + @s, I) - I = 0;

			SELECT @s = SUBSTRING(@text, @i, 8000);

		END
	END
	RETURN;
END

GO
