--用户用户任务关系表外键关系
EXEC bx_Drop 'FK_bx_UserMissions_UserID';
EXEC bx_Drop 'FK_bx_UserMissions_MissionID';

ALTER TABLE [bx_UserMissions] ADD 
     CONSTRAINT [FK_bx_UserMissions_UserID]        FOREIGN KEY ([UserID])         REFERENCES [bx_Users]         ([UserID])  ON UPDATE CASCADE  ON DELETE CASCADE
    ,CONSTRAINT [FK_bx_UserMissions_MissionID]     FOREIGN KEY ([MissionID])      REFERENCES [bx_Missions]      ([ID])  ON UPDATE CASCADE  ON DELETE CASCADE

GO

