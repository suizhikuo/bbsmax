
EXEC bx_Drop 'FK_bx_StickThreads_ThreadID';
EXEC bx_Drop 'FK_bx_StickThreads_ForumID';

ALTER TABLE [bx_StickThreads] ADD 
     CONSTRAINT [FK_bx_StickThreads_ThreadID]        FOREIGN KEY ([ThreadID])         REFERENCES [bx_Threads]         ([ThreadID])    ON UPDATE CASCADE  ON DELETE CASCADE
     ,CONSTRAINT [FK_bx_StickThreads_ForumID]        FOREIGN KEY ([ForumID])         REFERENCES [bx_Forums]         ([ForumID])

GO
