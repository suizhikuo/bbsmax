--标签表外键关系
EXEC bx_Drop 'FK_bx_TagRelation_TagID';

ALTER TABLE [bx_TagRelation] ADD 
     CONSTRAINT [FK_bx_TagRelation_TagID]            FOREIGN KEY ([TagID])            REFERENCES [bx_Tags]            ([ID])    ON UPDATE CASCADE  ON DELETE CASCADE
GO
