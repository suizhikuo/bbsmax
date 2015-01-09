
EXEC bx_Drop 'FK_bx_QuestionRewards_ThreadID';

ALTER TABLE [bx_QuestionRewards] ADD 
   CONSTRAINT [FK_bx_QuestionRewards_ThreadID]        FOREIGN KEY ([ThreadID])         REFERENCES [bx_Questions]         ([ThreadID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO

