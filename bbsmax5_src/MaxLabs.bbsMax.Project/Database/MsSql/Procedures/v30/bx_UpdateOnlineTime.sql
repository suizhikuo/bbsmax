-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bx_UpdateOnlineTime] 
 	@OnlineTimes varchar(8000),
 	@LastUpdateOnlineTimes varchar(8000),
 	@UserIDs varchar(8000)
--	@OnlineTime int,
--	@UserID int
AS
BEGIN
	SET NOCOUNT ON;

--    -- Insert statements for procedure here
--	declare @TotalOnlineTime int,@MonthOnlineTime int,@LastUpdateTime dateTime;
--	SELECT @TotalOnlineTime=TotalOnlineTime, @MonthOnlineTime=MonthOnlineTime,@LastUpdateTime=LastUpdateOnlineTime FROM bbsMax_UserProfiles WITH(NOLOCK) WHERE UserID=@UserID;
--	if(year(@LastUpdateTime)= year(getdate()) and month(@LastUpdateTime)= month(getdate()))
--		SET @MonthOnlineTime=@MonthOnlineTime+@OnlineTime;
--	else
--		SET @MonthOnlineTime=@OnlineTime;
--	Update bbsMax_UserProfiles SET TotalOnlineTime=TotalOnlineTime+@OnlineTime,MonthOnlineTime=@MonthOnlineTime,LastUpdateOnlineTime=getdate() WHERE UserID=@UserID
--
--	SELECT @TotalOnlineTime+@OnlineTime,@MonthOnlineTime;

	DECLARE @i int,@j int,@OnlineTime int,@UserID int,@k int
	DECLARE @NewOnlineTimes varchar(8000),@NewMonthOnlineTimes varchar(8000),@NewWeekOnlineTimes varchar(8000),@NewDayOnlineTimes varchar(8000)
	SET @NewOnlineTimes=''
	SET @NewMonthOnlineTimes=''
	SET @NewWeekOnlineTimes=''
	SET @NewDayOnlineTimes=''

	SET @OnlineTimes = @OnlineTimes + N','
	SELECT @i = CHARINDEX(',', @OnlineTimes)

	SET @UserIDs = @UserIDs + N','
	SELECT @j = CHARINDEX(',', @UserIDs)

	SET @LastUpdateOnlineTimes = @LastUpdateOnlineTimes +N','
	SELECT @k = CHARINDEX(',', @LastUpdateOnlineTimes)
	
	--计算本周1的时间
	DECLARE @Monday DateTime,@Now DateTime;
	SELECT @Now = GETDATE();
	DECLARE @m int;
	SELECT @m = DATEPART(weekday, @Now);
	IF @m = 1
		SELECT @m = 8;
	SELECT @Monday = CONVERT(varchar(12) , DATEADD(day, 2-@m, @Now),111);
	
	
	WHILE ( @i > 1 ) BEGIN
			SELECT @OnlineTime = SUBSTRING(@OnlineTimes,0, @i)	
			SELECT @UserID = SUBSTRING(@UserIDs,0, @j)	
			
			declare @TotalOnlineTime int,@MonthOnlineTime int,@LastUpdateTime dateTime,@LastVisitDate datetime,@WeekOnlineTime int,@DayOnlineTime int;
			SELECT @LastUpdateTime = SUBSTRING(@LastUpdateOnlineTimes,0, @k)	
			SELECT @TotalOnlineTime=TotalOnlineTime, @MonthOnlineTime=MonthOnlineTime, @WeekOnlineTime = WeekOnlineTime, @DayOnlineTime = DayOnlineTime, @LastVisitDate = LastVisitDate FROM bx_Users WITH(NOLOCK) WHERE UserID=@UserID;
			
			IF (month(@LastVisitDate) <> month(@LastUpdateTime) OR year(@LastVisitDate) <> year(@LastUpdateTime))
				SET @MonthOnlineTime=@OnlineTime;
			ELSE IF(year(@LastUpdateTime)= year(getdate()) and month(@LastUpdateTime)= month(getdate()))
				SET @MonthOnlineTime=@MonthOnlineTime+@OnlineTime;
			ELSE
				SET @MonthOnlineTime=@OnlineTime;
				
			IF (day(@LastVisitDate) <> day(@LastUpdateTime) OR month(@LastVisitDate) <> month(@LastUpdateTime) OR year(@LastVisitDate) <> year(@LastUpdateTime))
				SET @DayOnlineTime=@OnlineTime;
			ELSE IF(year(@LastUpdateTime)= year(getdate()) and month(@LastUpdateTime)= month(getdate()) and day(@LastUpdateTime) = day(getdate()))
				SET @DayOnlineTime=@DayOnlineTime+@OnlineTime;
			else
				SET @DayOnlineTime=@OnlineTime;

			--if(year(@LastUpdateTime)= year(@Monday) and month(@LastUpdateTime) = month(@Monday) and day(@LastUpdateTime) >= day(@Monday)
				--and year(@LastVisitDate)= year(@Monday) and month(@LastVisitDate)>= month(@Monday) and day(@LastVisitDate) >= day(@Monday))
			if @LastVisitDate >= @Monday AND @LastUpdateTime>=@Monday
				SET @WeekOnlineTime=@WeekOnlineTime+@OnlineTime;
			else
				SET @WeekOnlineTime=@OnlineTime;
				
				
			Update bx_Users SET TotalOnlineTime=TotalOnlineTime+@OnlineTime
								,MonthOnlineTime = @MonthOnlineTime
								,WeekOnlineTime = @WeekOnlineTime
								,DayOnlineTime = @DayOnlineTime
								,LastVisitDate=@LastUpdateTime WHERE UserID=@UserID;

			SET @NewOnlineTimes=@NewOnlineTimes+str(@TotalOnlineTime+@OnlineTime)+','
			SET @NewMonthOnlineTimes=@NewMonthOnlineTimes+str(@MonthOnlineTime)+','
			SET @NewWeekOnlineTimes = @NewWeekOnlineTimes + str(@WeekOnlineTime)+','
			SET @NewDayOnlineTimes = @NewDayOnlineTimes + str(@DayOnlineTime)+','
			
			SELECT @OnlineTimes = SUBSTRING(@OnlineTimes, @i + 1, LEN(@OnlineTimes) - @i)
			SELECT @UserIDs = SUBSTRING(@UserIDs, @j + 1, LEN(@UserIDs) - @j)

			SELECT @i = CHARINDEX(',',@OnlineTimes)
			SELECT @j = CHARINDEX(',',@UserIDs)
	END
	
	SELECT @NewOnlineTimes,@NewMonthOnlineTimes,@NewWeekOnlineTimes,@NewDayOnlineTimes
END


