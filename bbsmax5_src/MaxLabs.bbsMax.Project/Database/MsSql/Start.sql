IF EXISTS (SELECT [name] FROM sysobjects WHERE [name]='bx_Drop' AND [type]='P')
   DROP PROCEDURE bx_Drop
GO

CREATE PROCEDURE bx_Drop @Name sysname
AS
BEGIN

	IF OBJECT_ID(@Name) IS NULL
		RETURN;


	/* 删除指定名称的表 */
	IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'U' AND [name] = @Name) BEGIN
		
		DECLARE cs_trgs CURSOR FOR SELECT [name] FROM [sysobjects] WHERE [type] = 'F' and parent_obj = OBJECT_ID(@Name) FOR READ ONLY
		DECLARE @t_name NVARCHAR(100)

		OPEN cs_trgs

		FETCH NEXT FROM cs_trgs INTO @t_name
		WHILE @@FETCH_STATUS = 0 BEGIN
			EXEC('ALTER TABLE [' + @Name + '] DROP CONSTRAINT [' + @t_name + '];');
			FETCH NEXT FROM cs_trgs INTO @t_name;
		END

		CLOSE cs_trgs;
		DEALLOCATE cs_trgs;
		
		EXEC('DROP TABLE ' + @Name);
    END

	/* 删除指定名称的外键 */
	ELSE IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'F' AND [name] = @Name) BEGIN

		DECLARE @TableName nvarchar(200)
		SELECT @TableName = OBJECT_NAME(fkeyid) FROM [sysforeignkeys] WHERE constid = OBJECT_ID(@Name)
		EXEC('ALTER TABLE [' + @TableName + '] DROP CONSTRAINT [' + @Name + '];');

	END

	/* 删除指定名称的存储过程 */
	ELSE IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = @Name)
		EXEC('DROP PROCEDURE [' + @Name + '];');

	/* 删除指定名称的自定义函数 */
	ELSE IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'TF' AND [name] = @Name)
		EXEC('DROP FUNCTION [' + @Name + '];');


END

GO