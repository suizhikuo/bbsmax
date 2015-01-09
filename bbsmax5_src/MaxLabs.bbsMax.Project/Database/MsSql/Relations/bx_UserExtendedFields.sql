--用户的扩展字段表外键关系
EXEC bx_Drop 'FK_bx_UserExtendedValues_UserID';
EXEC bx_Drop 'FK_bx_UserExtendedValues_ExtendedFieldID';

ALTER TABLE [bx_UserExtendedValues] ADD 
     CONSTRAINT [FK_bx_UserExtendedValues_UserID]            FOREIGN KEY ([UserID])          REFERENCES [bx_Users]            ([UserID])  ON UPDATE CASCADE ON DELETE CASCADE

GO

