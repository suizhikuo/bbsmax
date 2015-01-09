
EXEC bx_Drop 'FK_bx_PolemizeUsers_ThreadID';

ALTER TABLE [bx_PolemizeUsers] ADD 
   CONSTRAINT [FK_bx_PolemizeUsers_ThreadID] FOREIGN KEY([ThreadID]) REFERENCES [bx_Threads] ([ThreadID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO

