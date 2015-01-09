

--BEGIN DROP TRIGGERS
	
BEGIN
	DECLARE @username NVARCHAR(256)
	DECLARE @t_name NVARCHAR(100)
	DECLARE @trg_name NVARCHAR(100)
		DECLARE cs_trgs CURSOR FOR SELECT DISTINCT user_name(t.uid),t.name table_name,trg.name trigger_name FROM sysobjects t INNER JOIN sysobjects trg ON trg.parent_obj=t.id WHERE t.type='U' AND trg.type='TR' FOR READ ONLY


		OPEN cs_trgs

		FETCH NEXT FROM cs_trgs INTO @username,@t_name,@trg_name
		WHILE @@FETCH_STATUS = 0
		BEGIN
			IF @username IS NOT NULL BEGIN
				IF @t_name LIKE 'Max_%' OR @t_name LIKE 'bbsMax_%' OR @t_name LIKE 'System_Max_%' OR @t_name LIKE 'System_bbsMax_%' OR @t_name LIKE 'bx_%'
				BEGIN
					EXEC('DROP TRIGGER ['+@username+'].['+@trg_name+']')
				END
			END
			FETCH NEXT FROM cs_trgs INTO @username,@t_name,@trg_name
		END

		CLOSE cs_trgs
		DEALLOCATE cs_trgs
END	

--END DROP TRIGGERS

--BEGIN DROP STORE PROCEDURES

BEGIN
	--DECLARE @username NVARCHAR(256)
	DECLARE @p_name NVARCHAR(100)
	
		DECLARE cs_ps CURSOR FOR SELECT DISTINCT user_name(p.uid),p.name FROM sysobjects p WHERE p.type = 'P' FOR READ ONLY

		OPEN cs_ps

		FETCH NEXT FROM cs_ps INTO @username,@p_name
		WHILE @@FETCH_STATUS = 0
		BEGIN
			IF @username IS NOT NULL BEGIN
				IF @p_name LIKE 'Max_%' OR @p_name LIKE 'bbsMax_%' OR @p_name LIKE 'System_Max_%' OR @p_name LIKE 'System_bbsMax_%' OR @p_name LIKE 'bx_%'
				BEGIN
					EXEC('DROP PROCEDURE ['+@username+'].['+@p_name+']')
				END
			END
			FETCH NEXT FROM cs_ps INTO @username,@p_name
		END

		CLOSE cs_ps
		DEALLOCATE cs_ps
	
END

--END DROP STORE PROCEDURES

--BEGIN DROP FUNCTIONS

BEGIN

	--DECLARE @username NVARCHAR(256)
	DECLARE @f_name NVARCHAR(100)
	
		DECLARE cs_fs CURSOR FOR SELECT DISTINCT user_name(f.uid),f.name FROM sysobjects f WHERE f.type IN ('FN','TF') FOR READ ONLY

		OPEN cs_fs

		FETCH NEXT FROM cs_fs INTO @username,@f_name
		WHILE @@FETCH_STATUS = 0
		BEGIN
			IF @username IS NOT NULL BEGIN
				IF @f_name LIKE 'Max_%' OR @f_name LIKE 'bbsMax_%' OR @f_name LIKE 'System_Max_%' OR @f_name LIKE 'System_bbsMax_%' OR @f_name LIKE 'bx_%'
				BEGIN
					EXEC('DROP FUNCTION ['+@username+'].['+@f_name+']');
				END
			END
			FETCH NEXT FROM cs_fs INTO @username,@f_name
		END
	    
		CLOSE cs_fs
		DEALLOCATE cs_fs
	
END

--END DROP FUNCTIONS