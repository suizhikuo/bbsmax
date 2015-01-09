-- =============================================
-- Author:		<sek>
-- Create date: <2007/2/13>
-- Description:	<排序>
-- =============================================
CREATE PROCEDURE bx_Common_UpdateSortOrder
	@Identity int,
	@IdentityColumn varchar(256),--主键字段
	@GroupColumn varchar(256),--分组字段（例如，"GroupID","DirectoryID"）,没有分组时用''表示
	@GroupID int,--分组ID值
	@TableName varchar(256),--表名
	@IsUp bit--是否向上
AS
	SET NOCOUNT ON
	DECLARE @SQLString nvarchar(4000),@GroupCondition nvarchar(512),@TempOrder int,@SortOrder int,@TempIdentity int
	IF(@GroupColumn<>'')
	BEGIN
	-------
		IF @GroupID>0
			SET @SQLString='SELECT @GroupID='+@GroupColumn+',@SortOrder=SortOrder FROM '+@TableName+' WITH (NOLOCK) where '+@IdentityColumn+'='+STR(@Identity)+' and '+@GroupColumn+'='+STR(@GroupID)
		ELSE
	-------
			SET @SQLString='SELECT @GroupID='+@GroupColumn+',@SortOrder=SortOrder FROM '+@TableName+' WITH (NOLOCK) where '+@IdentityColumn+'='+STR(@Identity)
		EXECUTE sp_executesql @SQLString,N'@GroupID int output,@SortOrder int output',@GroupID output,@SortOrder output
		IF(@GroupID IS NULL)
			RETURN (-1)
		
		SET @GroupCondition=' and '+@GroupColumn+'='+STR(@GroupID)
	END
	ELSE
	BEGIN
		SET @SQLString=N'SELECT @SortOrder=SortOrder FROM '+@TableName+' WITH (NOLOCK) where '+@IdentityColumn+'='+STR(@Identity)
		EXECUTE sp_executesql @SQLString,N'@SortOrder int output',@SortOrder output
		IF(@SortOrder IS NULL)--SEK 修改 2007.3.23 ---原：IF(@SortOrder=0 OR @SortOrder IS NULL)
			RETURN (-1)
			
		SET @GroupCondition=''
	END
	
	
	
	IF(@IsUp=1)
		SET @SQLString=N'SELECT TOP 1 @TempOrder=SortOrder,@TempIdentity='+@IdentityColumn+' FROM '+@TableName+' WITH (NOLOCK) WHERE SortOrder<'+str(@SortOrder)+@GroupCondition+' ORDER BY SortOrder DESC'
	ELSE
		SET @SQLString=N'SELECT TOP 1 @TempOrder=SortOrder,@TempIdentity='+@IdentityColumn+' FROM '+@TableName+' WITH (NOLOCK) WHERE SortOrder>'+str(@SortOrder)+@GroupCondition+' ORDER BY SortOrder'
		
		
		EXECUTE sp_executesql @SQLString,N'@TempOrder int output,@TempIdentity int output',@TempOrder output,@TempIdentity output
	
	IF(@TempOrder IS NULL )
		RETURN (0)
	
	BEGIN TRANSACTION 	
	
	IF @SortOrder<>0 
		EXECUTE('UPDATE '+@TableName+' SET SortOrder=-'+@SortOrder+' WHERE '+@IdentityColumn+'='+@TempIdentity+@GroupCondition)-- 
	ELSE
		EXECUTE('UPDATE '+@TableName+' SET SortOrder=-'+@TempOrder+' WHERE '+@IdentityColumn+'='+@TempIdentity+@GroupCondition)-- 
		
	EXECUTE('UPDATE '+@TableName+' SET SortOrder='+@TempOrder+' WHERE '+@IdentityColumn+'='+@Identity+@GroupCondition)


	IF @@error<>0
		BEGIN
			ROLLBACK TRANSACTION
			RETURN (-1)
		END
		
	EXECUTE('UPDATE '+@TableName+' SET SortOrder='+@SortOrder+' WHERE '+@IdentityColumn+'='+@TempIdentity+@GroupCondition)
	IF @@error<>0
		BEGIN
			ROLLBACK TRANSACTION
			RETURN (-1)
		END
	
		COMMIT TRANSACTION
		RETURN (0)


