CREATE PROCEDURE [bx_GetDisabledTriggerForumIDs]
				@ForumIDs nvarchar(64) output
			AS
			BEGIN
				SET NOCOUNT ON;
				set @ForumIDs='';
			END