
EXEC bx_Drop 'FK_TopicStatus_ThreadID';

ALTER TABLE [bx_TopicStatus] ADD 
     CONSTRAINT [FK_bx_TopicStatus_ThreadID]        FOREIGN KEY ([ThreadID])         REFERENCES [bx_Threads]         ([ThreadID])    ON UPDATE CASCADE  ON DELETE CASCADE

GO

