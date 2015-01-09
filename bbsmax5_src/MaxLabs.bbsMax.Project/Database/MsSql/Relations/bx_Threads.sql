
EXEC bx_Drop 'FK_bx_Threads_ForumID';

ALTER TABLE [bx_Threads] ADD 
   CONSTRAINT [FK_bx_Threads_ForumID]        FOREIGN KEY ([ForumID])         REFERENCES [bx_Forums]         ([ForumID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO

