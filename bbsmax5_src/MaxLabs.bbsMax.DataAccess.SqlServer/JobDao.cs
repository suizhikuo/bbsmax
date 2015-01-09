//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Entities;

using MaxLabs.bbsMax.ValidateCodes;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    class JobDao : Jobs.JobDao
    {
        #region 存储过程 bx_SetJobExecuteTime
        [StoredProcedure(Name = "bx_SetJobExecuteTime", Script = @"
CREATE PROCEDURE {name}
     @Type        varchar(200)
    ,@ExecuteTime dateTime
AS
BEGIN
	SET NOCOUNT ON;
    UPDATE [bx_JobStatus] SET [LastExecuteTime] = @ExecuteTime WHERE [Type] = @Type;

    IF @@ROWCOUNT < 1
        INSERT INTO [bx_JobStatus] ([Type], [LastExecuteTime]) VALUES (@Type, @ExecuteTime);
END
"
            )]
        #endregion
        public override void SetJobLastExecuteTime(string type, DateTime executeTime)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_SetJobExecuteTime";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<string>("@Type", type, SqlDbType.VarChar, 200);
                query.CreateParameter<DateTime>("@ExecuteTime", executeTime, SqlDbType.DateTime);

                query.ExecuteNonQuery();
            }
        }

        #region 存储过程 bx_GetAllJobLastExecuteTimes
        [StoredProcedure(Name = "bx_GetAllJobStatus", Script = @"
CREATE PROCEDURE {name}
AS
BEGIN
	SET NOCOUNT ON;
    SELECT * FROM [bx_JobStatus];
END
"
            )]
        #endregion
        public override Dictionary<string, DateTime> GetAllJobStatus()
        {
            Dictionary<string, DateTime> times = new Dictionary<string, DateTime>(StringComparer.CurrentCultureIgnoreCase);
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetAllJobStatus";
                query.CommandType = CommandType.StoredProcedure;

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        times.Add(reader.Get<string>("Type"), reader.Get<DateTime>("LastExecuteTime"));
                    }
                }

                return times;
            }
        }

        #if !Passport

        /// <summary>
        /// 检查过期的投票和过期的提问，并进行相应处理
        /// </summary>
        [StoredProcedure(Name = "bx_QueryForBeforeRequestIn3M", Script = @"
CREATE PROCEDURE {name}
AS
BEGIN

    SET NOCOUNT ON;
	
    --找出所有需要处理的主题状态变更（例如：置顶到期、高亮到期）
    SELECT * FROM [bx_TopicStatus] WHERE EndDate <= GETDATE();

    --恢复已经被锁定的头像
    --DECLARE @LockAvatarTable table(UserID int, OldAvatarSrc nvarchar(200));

    --INSERT INTO @LockAvatarTable SELECT UserID, OldAvatarSrc FROM bx_UserAvatarLocks WHERE AvatarLockTo <= GETDATE();

    --UPDATE bx_Users SET AvatarSrc = OldAvatarSrc FROM @LockAvatarTable T WHERE bx_Users.UserID = T.UserID;

    --DELETE bx_UserAvatarLocks FROM @LockAvatarTable T WHERE bx_UserAvatarLocks.UserID = T.UserID;

    --SELECT 'ResetUser' AS XCMD, UserID, OldAvatarSrc AS AvatarSrc FROM @LockAvatarTable;

	--查出过期的提问，交给服务器代码处理
	SELECT Q.*, T.PostUserID,T.ForumID FROM [bx_Questions] Q INNER JOIN [bx_Threads] T ON Q.ThreadID = T.ThreadID WHERE Q.IsClosed = 0 AND Q.ExpiresDate < getdate();

END")]
        public override void QueryForBeforeRequestIn3M(out TopicStatusCollection experiesTopicStatus, out List<int> autoFinalQuestionThreadIds, out Dictionary<int, Dictionary<int, int>> autoFinalQuestionForumIDAndRewards)
        {

            //供放置查出的已经过期的提问
            //List<Question> expiresQuestions = new List<Question>();

            List<QuestionThread> expiresQuestions = new List<QuestionThread>();

            Dictionary<int, int> threadIDAndPostUserIDs = new Dictionary<int, int>();
            Dictionary<int, int> threadIDAndForumIDs = new Dictionary<int, int>();

            autoFinalQuestionThreadIds = new List<int>();

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_QueryForBeforeRequestIn3M";
                query.CommandType = CommandType.StoredProcedure;
                query.CommandTimeout = 30;

                try
                {
                    if (IsAutoFinalQuestion == true)
                    {
                        experiesTopicStatus = new TopicStatusCollection();
                        autoFinalQuestionForumIDAndRewards = new Dictionary<int, Dictionary<int, int>>();
                        return;
                    }
                    IsAutoFinalQuestion = true;

                    using (XSqlDataReader reader = query.ExecuteReader())
                    {
                        experiesTopicStatus = new TopicStatusCollection(reader);

                        reader.NextResult();

                        while (reader.Read())
                        {
                            //Question question = ConvertData.ConvertToQuestion(reader);
                            QuestionThread question = new QuestionThread();
                            question.ThreadID = reader.Get<int>(reader.GetOrdinal("ThreadID"));
                            question.FillQuestion(reader);

                            expiresQuestions.Add(question);

                            int forumID = reader.Get<int>("ForumID");

                            threadIDAndForumIDs.Add(question.ThreadID, forumID);

                            threadIDAndPostUserIDs.Add(question.ThreadID, reader.GetInt32(reader.GetOrdinal("PostUserID")));
                        }
                    }


                    query.CommandText = "bx_AutoFinalQuestion";
                    query.CommandType = CommandType.StoredProcedure;
                    query.CommandTimeout = 30;

                    autoFinalQuestionForumIDAndRewards = new Dictionary<int, Dictionary<int, int>>();
                    foreach (QuestionThread q in expiresQuestions)
                    {
                        int forumID;
                        if (threadIDAndForumIDs.ContainsKey(q.ThreadID))
                            forumID = threadIDAndForumIDs[q.ThreadID];
                        else
                            continue;

                        query.Parameters.Clear();
                        query.CreateParameter<int>("@ThreadID", q.ThreadID, SqlDbType.Int);
                        query.CreateParameter<int>("@UserID", threadIDAndPostUserIDs[q.ThreadID], SqlDbType.Int);
                        query.CreateParameter<int>("@RewardCount", q.RewardCount, SqlDbType.Int);
                        query.CreateParameter<int>("@TotalReward", q.Reward, SqlDbType.Int);

                        if (autoFinalQuestionForumIDAndRewards.ContainsKey(forumID) == false)
                            autoFinalQuestionForumIDAndRewards.Add(forumID, new Dictionary<int, int>());

                        using (XSqlDataReader reader = query.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int userID = reader.Get<int>("UserID");
                                int reward = reader.Get<int>("Reward");

                                if (autoFinalQuestionForumIDAndRewards[forumID].ContainsKey(userID))
                                {
                                    autoFinalQuestionForumIDAndRewards[forumID][userID] = autoFinalQuestionForumIDAndRewards[forumID][userID] + reward;
                                }
                                else
                                {
                                    autoFinalQuestionForumIDAndRewards[forumID].Add(userID, reward);
                                }
                            }
                        }
                        if (!autoFinalQuestionThreadIds.Contains(q.ThreadID))
                            autoFinalQuestionThreadIds.Add(q.ThreadID);
                    }

                    IsAutoFinalQuestion = false;
                }
                catch (Exception ex)
                {
                    IsAutoFinalQuestion = false;
                    throw ex;
                }
            }


        }

        private static bool IsAutoFinalQuestion = false;

#if DEBUG

        [StoredProcedure(Name = "bx_AutoFinalQuestion", Script = @"
CREATE PROCEDURE {name}
	@ThreadID int, 
	@UserID int,
	@RewardCount int,
	@TotalReward int
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @Reward int,@PostCount int
	SELECT @PostCount = COUNT(*) FROM [bx_Posts] WITH (NOLOCK) WHERE ThreadID = @ThreadID AND SortOrder < 4000000000000000 AND UserID <> @UserID;
	
	IF(@PostCount = 0)
	BEGIN
		UPDATE [bx_Questions] SET IsClosed = 1 WHERE ThreadID = @ThreadID
	END
	ELSE BEGIN
        IF EXISTS(SELECT * FROM [bx_Questions] WHERE IsClosed = 1 AND ThreadID = @ThreadID)
            RETURN;
---------------------------------------
		IF(@PostCount < @RewardCount)
			SET @RewardCount = @PostCount;

		SET @Reward = @TotalReward / @RewardCount;
		
		BEGIN TRANSACTION
		EXEC('INSERT [bx_QuestionRewards](PostID,ThreadID,Reward) SELECT TOP '+@RewardCount+' PostID,'+@ThreadID+','+@Reward+' FROM [bx_Posts] WITH(NOLOCK) WHERE ThreadID='+@ThreadID+' AND SortOrder < 4000000000000000 AND UserID <> ' + @UserID + ' ORDER BY PostID')
		IF(@@ERROR <> 0) BEGIN
		   ROLLBACK TRANSACTION
		   RETURN
		END
		
		UPDATE [bx_QuestionRewards] SET Reward = Reward + @TotalReward % @RewardCount WHERE PostID = (SELECT MIN(PostID) FROM [bx_QuestionRewards] WHERE ThreadID = @ThreadID)
		IF(@@ERROR <> 0) BEGIN
		   ROLLBACK TRANSACTION
		   RETURN
		END
		
		UPDATE [bx_Questions] SET IsClosed = 1 WHERE ThreadID = @ThreadID
		IF(@@ERROR <> 0) BEGIN
		   ROLLBACK TRANSACTION
		   RETURN
		END
		
		COMMIT TRANSACTION
		
		SELECT UserID, SUM(Q.Reward) AS Reward FROM bx_Posts P INNER JOIN bx_QuestionRewards Q ON P.PostID = Q.PostID
			WHERE P.ThreadID = @ThreadID GROUP BY UserID;
--------------------------------------------
	END
END")]
        private void p1()
        { }

#endif

#endif
    }
}