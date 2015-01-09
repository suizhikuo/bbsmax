 --BEGIN DROP TRIGGERS

BEGIN

	DECLARE cs_trgs CURSOR FOR SELECT DISTINCT t.name table_name,trg.name trigger_name FROM sysobjects t INNER JOIN sysobjects trg ON trg.parent_obj=t.id WHERE t.type='U ' AND trg.type='TR' FOR READ ONLY
	DECLARE @t_name NVARCHAR(100)
	DECLARE @trg_name NVARCHAR(100)

	OPEN cs_trgs

	FETCH NEXT FROM cs_trgs INTO @t_name,@trg_name
	WHILE @@FETCH_STATUS = 0
	BEGIN
		IF @t_name LIKE 'Max_%' OR @t_name LIKE 'bbsMax_%' OR @t_name LIKE 'System_Max_%' OR @t_name LIKE 'System_bbsMax_%' OR @t_name LIKE 'bx_%'
		BEGIN
			EXEC('DROP TRIGGER ['+@trg_name+']')
		END

		FETCH NEXT FROM cs_trgs INTO @t_name,@trg_name
	END

	CLOSE cs_trgs
	DEALLOCATE cs_trgs
END	
GO
--END DROP TRIGGERS



DELETE [bx_ThreadCatalogsInForums] WHERE [ThreadCatalogID] NOT IN (SELECT [ThreadCatalogID] FROM [bx_ThreadCatalogs] WITH(NOLOCK));
DELETE [bx_ThreadCatalogsInForums] WHERE [ForumID] NOT IN (SELECT [ForumID] FROM [bx_Forums] WITH(NOLOCK));

DELETE [bx_Moderators] WHERE [UserID] NOT IN (SELECT [UserID] FROM [bx_Users] WITH(NOLOCK));
DELETE [bx_AdminSessions] WHERE [UserID] NOT IN (SELECT [UserID] FROM [bx_Users] WITH(NOLOCK));
DELETE [bx_BannedUsers] WHERE [UserID] NOT IN (SELECT [UserID] FROM [bx_Users] WITH(NOLOCK));
DELETE [bx_BlogCategoryReverters] WHERE [CategoryID] NOT IN (SELECT [CategoryID] FROM [bx_BlogCategories] WITH(NOLOCK));
DELETE [bx_ChatMessages] WHERE [UserID] NOT IN (SELECT [UserID] FROM [bx_Users] WITH(NOLOCK));
DELETE [bx_UserReverters] WHERE [UserID] NOT IN (SELECT [UserID] FROM [bx_Users] WITH(NOLOCK));
DELETE [bx_UserInfos] WHERE [UserID] NOT IN (SELECT [UserID] FROM [bx_Users] WITH(NOLOCK));
DELETE [bx_TempUploadFiles] WHERE [UserID] NOT IN (SELECT [UserID] FROM [bx_Users] WITH(NOLOCK));

DELETE [bx_Serials] WHERE [UserID] NOT IN (SELECT [UserID] FROM [bx_Users] WITH(NOLOCK));
DELETE [bx_PointShows] WHERE [UserID] NOT IN (SELECT [UserID] FROM [bx_Users] WITH(NOLOCK));
DELETE [bx_Notifies] WHERE [UserID] NOT IN (SELECT [UserID] FROM [bx_Users] WITH(NOLOCK));
DELETE [bx_InviteSerials] WHERE [UserID] NOT IN (SELECT [UserID] FROM [bx_Users] WITH(NOLOCK));
DELETE [bx_ActivationSerials] WHERE [UserID] NOT IN (SELECT [UserID] FROM [bx_Users] WITH(NOLOCK));

DELETE [bx_PostMarks] WHERE [PostID] NOT IN (SELECT [PostID] FROM [bx_Posts] WITH(NOLOCK));

DELETE [bx_DenouncingContents] WHERE [DenouncingID] NOT IN (SELECT [DenouncingID] FROM [bx_Denouncings] WITH(NOLOCK));
DELETE [bx_CommentReverters] WHERE [CommentID] NOT IN (SELECT [CommentID] FROM [bx_Comments] WITH(NOLOCK));
DELETE [bx_ChatSessions] WHERE [UserID] NOT IN (SELECT [UserID] FROM [bx_Users] WITH(NOLOCK));
DELETE [bx_ChatMessageReverters] WHERE [MessageID] NOT IN (SELECT [MessageID] FROM [bx_ChatMessages] WITH(NOLOCK));
DELETE [bx_FeedFilters] WHERE [UserID] NOT IN (SELECT [UserID] FROM [bx_Users] WITH(NOLOCK));
DELETE [bx_UserNoAddFeedApps] WHERE [UserID] NOT IN (SELECT [UserID] FROM [bx_Users] WITH(NOLOCK));

