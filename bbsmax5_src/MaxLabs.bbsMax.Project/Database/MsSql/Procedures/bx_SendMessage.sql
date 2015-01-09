--发送短消息

CREATE PROCEDURE [bx_SendMessage]
	 @UserID                 int
	,@MessageID              int
	,@RelatedUserIDs         varchar(500)
	,@SenderIP               varchar(50)
	,@ParentID               int
	,@ContentType            tinyint
	,@Content                ntext
	,@ContentReverter        nvarchar(1000)

AS
BEGIN
	SET NOCOUNT ON; 
	
	BEGIN TRANSACTION

	--每个RelatedUsers里面的用户均接收一条数据
	INSERT INTO [bx_Messages](
				  [IsRead]
				, [Type]
				, [ContentType]
				, [UserID]
				, [RelatedUserIDs]
				, [ParentID]
				, [SenderIP]
				, [Content]
				, [ContentReverter]
				, [PostDate]
				)
		SELECT    0                         --新消息都是未读的
				, 1                         --收件箱类型
				, @ContentType
				, item AS RelatedUsers    --RelatedUsers每个ID对应一个用户ID,都会接收一条新消息在收件箱中
				, @UserID
				, @ParentID
				, @SenderIP
				, @Content
				, @ContentReverter
				, GETDATE()
		  FROM  bx_GetIntTable(@RelatedUserIDs, ',');
		  
	IF(@@error<>0)
		GOTO Cleanup;	 
			   
	--如果ID参数不为零,表示为编辑草稿后发送消息,需要删除掉原草稿消息
	IF (@MessageID <> 0) BEGIN
		DELETE FROM 
			[bx_Messages] 
		WHERE 
			 [MessageID] = @MessageID 
		AND 
			 [UserID] = @UserID;
			
	  END
	  
	--发件人的发件箱添加一条数据
	INSERT INTO [bx_Messages](
			  [IsRead]
			, [Type]
			, [ContentType]
			, [UserID]
			, [RelatedUserIDs]
			, [ParentID]
			, [SenderIP]
			, [Content]
			, [ContentReverter]
			, [PostDate]
			
		   ) VALUES(
		      1                    --发件箱都标记为已读
		    , 2                    --发件箱类型
		    , @ContentType     
		    , @UserID
		    , @RelatedUserIDs
		    , @ParentID
		    , @SenderIP
		    , @Content
		    , @ContentReverter
		    , GETDATE()
		   ) ; 
	
	IF(@@error<>0)
		GOTO Cleanup;
	ELSE BEGIN
		GOTO CommitTrans;
	END
			   
	
CommitTrans:
	BEGIN
		COMMIT TRANSACTION
		RETURN (0);
	END
                    
Cleanup:
	BEGIN
		ROLLBACK TRANSACTION
		RETURN (-1)
	END
	  
END