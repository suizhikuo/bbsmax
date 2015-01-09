--点击记录
EXEC bx_Drop 'bx_ClickLogs';

CREATE TABLE [bx_ClickLogs] (
     [ID]            int                    IDENTITY(1,1)                               NOT NULL

	,[UserIdentify]         varchar(200)                                                NOT NULL    CONSTRAINT [DF_bx_ClickLogs_UserIdentify]          DEFAULT(0)
	,[Ip]                   varchar(50)                                                 NULL    
	,[ClickDate]            datetime                                                    NOT NULL    CONSTRAINT [DF_bx_ClickLogs_ClickDate]			   DEFAULT(getdate())
	,[SourceType]           int                                                         NOT NULL    CONSTRAINT [DF_bx_ClickLogs_SourceType]         DEFAULT(0)
	,[TargetID]             int                                                         NOT NULL    CONSTRAINT [DF_bx_ClickLogs_TargetID]              DEFAULT(0)

	,CONSTRAINT [PK_bx_ClickLogs] PRIMARY KEY ([ID])
);

/*
点击记录：
[UserIdentify]:可能是游客的GuestID 或者用户的 UserID
[SourceType]  :被点击的对象枚举
[TargetID]    :被点击的对象ID

*/

GO
