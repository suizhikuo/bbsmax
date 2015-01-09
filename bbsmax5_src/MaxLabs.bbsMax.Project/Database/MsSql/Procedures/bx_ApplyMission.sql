--申请任务

CREATE PROCEDURE bx_ApplyMission
     @UserID       int
    ,@MissionID    int
    ,@Percent      float
AS
BEGIN
	SET NOCOUNT ON;

    DECLARE @CycleTime int;
    SELECT @CycleTime = [CycleTime] FROM [bx_Missions] WHERE [ID] = @MissionID AND getdate() > [BeginDate] AND getdate() < [EndDate] AND IsEnable = 1;

    IF @CycleTime IS NOT NULL BEGIN
        DECLARE @Status tinyint,@IsPrized bit;
        IF @Percent = 1 BEGIN
            SET @Status = 1;
            SET @IsPrized = 1;
        END
        ELSE BEGIN 
            SET @Status = 0;
            SET @IsPrized = 0;
        END

        IF NOT EXISTS (SELECT * FROM [bx_UserMissions] WHERE [UserID] = @UserID AND [MissionID] = @MissionID) BEGIN
            INSERT INTO [bx_UserMissions](
                         [UserID]
                        ,[MissionID]
                        ,[Status]
                        ,[FinishPercent]
                        ,[IsPrized]
                        ) VALUES (
                         @UserID
                        ,@MissionID
                        ,@Status
                        ,@Percent
                        ,@IsPrized
                        );
            UPDATE [bx_Missions] SET [TotalUsers] = [TotalUsers] + 1 WHERE [ID] = @MissionID
            RETURN 0;
        END
        ELSE IF @CycleTime = 0  BEGIN--不是周期性任务
			DECLARE @TempStatus tinyint;
            SELECT @TempStatus = [Status]
				 FROM [bx_UserMissions] WHERE [UserID] = @UserID AND [MissionID] = @MissionID;
				 
			IF @TempStatus = 3 BEGIN--放弃的 可以重新申请
				UPDATE [bx_UserMissions] SET 
                          [Status] = @Status
                        , [FinishPercent] = @Percent
                        , [IsPrized] = @IsPrized
                        , [FinishDate] = getdate()
                        , [CreateDate] = getdate()
                        WHERE [UserID] = @UserID AND [MissionID] = @MissionID;
				UPDATE [bx_Missions] SET [TotalUsers] = [TotalUsers] + 1 WHERE [ID] = @MissionID
				RETURN 0;
			END	
			ELSE
				RETURN 2; --已经申请过  
		END
        ELSE BEGIN --周期性任务
            DECLARE @TempIsPrized bit,@TempCreateDate datetime,@TempFinishPercent float;
            SELECT @TempStatus = [Status]
				 , @TempFinishPercent = [FinishPercent]
				 , @TempIsPrized = [IsPrized]
				 , @TempCreateDate = [CreateDate] 
				 FROM [bx_UserMissions] WHERE [UserID] = @UserID AND [MissionID] = @MissionID;
            
			IF @TempStatus<>3 AND DATEADD(second,@CycleTime,@TempCreateDate)>getdate() --下次申请时间未到
				RETURN 2; --已经申请过  

            DECLARE @ReturnValue int,@UpdateUserMission bit
            SET @ReturnValue = 0;
            SET @UpdateUserMission = 0;

            IF @TempIsPrized = 1 BEGIN
                SET @ReturnValue = 0;
                SET @UpdateUserMission = 1;
            END
            ELSE IF @TempFinishPercent = 1 BEGIN --已经完成 未领取奖励
                SET @UpdateUserMission = 1;
                SET @ReturnValue = 3;
            END
            ELSE IF @TempStatus = 0
                RETURN 2;--已经申请过
            ELSE IF @TempStatus > 1 BEGIN --放弃 或者 失败的任务  重新开始
                SET @UpdateUserMission = 1;
                SET @ReturnValue = 0;
            END
            
            IF @UpdateUserMission = 1 BEGIN
                UPDATE [bx_UserMissions] SET 
                          [Status] = @Status
                        , [FinishPercent] = @Percent
                        , [IsPrized] = @IsPrized
                        , [FinishDate] = getdate()
                        , [CreateDate] = getdate()
                        WHERE [UserID] = @UserID AND [MissionID] = @MissionID;
				UPDATE [bx_Missions] SET [TotalUsers] = [TotalUsers] + 1 WHERE [ID] = @MissionID
            END
            RETURN @ReturnValue;
        END
    END
    ELSE
        RETURN 1;--失败 
END