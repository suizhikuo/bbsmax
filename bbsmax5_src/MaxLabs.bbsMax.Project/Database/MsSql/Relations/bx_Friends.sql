--好友 好友组关系表外键关系
EXEC bx_Drop 'FK_bx_Friends_GroupID';

ALTER TABLE [bx_Friends] ADD 
       CONSTRAINT [FK_bx_Friends_GroupID]        FOREIGN KEY ([GroupID])       REFERENCES [bx_FriendGroups]    ([GroupID])    ON UPDATE CASCADE  ON DELETE CASCADE

GO