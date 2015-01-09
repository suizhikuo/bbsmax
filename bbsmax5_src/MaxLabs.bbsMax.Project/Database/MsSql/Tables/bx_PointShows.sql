EXEC bx_Drop 'bx_PointShows';

CREATE TABLE bx_PointShows (
   [UserID]        int              NOT NULL
   ,[ShowPoints]    int              NOT NULL
   ,[Price]         int              NOT NULL
   ,[Content]       nvarchar(100)  COLLATE Chinese_PRC_CI_AS_WS   NULL
   
   ,[CreateDate]    datetime         NOT NULL    CONSTRAINT  [DF_bx_PointShows_CreateDate]    DEFAULT (GETDATE())
)


CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_PointShows_UserID] ON [bx_PointShows] ([UserID]);
CREATE INDEX [IX_bx_PointShows_ShowPoints] ON [bx_PointShows] ([ShowPoints]);

/*
Name:上榜
Columns:
	[Points]            积分
    [UserID]           用户ID
    
    [Content]          内容
    
    [CreateDate]       时间
*/

GO
