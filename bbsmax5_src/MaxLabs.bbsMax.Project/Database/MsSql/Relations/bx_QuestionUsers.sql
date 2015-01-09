
EXEC bx_Drop 'FK_bx_QuestionUsers_ThreadID';

ALTER TABLE [bx_QuestionUsers] ADD 
   CONSTRAINT [FK_bx_QuestionUsers_ThreadID]        FOREIGN KEY ([ThreadID])         REFERENCES [bx_Questions]         ([ThreadID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO

