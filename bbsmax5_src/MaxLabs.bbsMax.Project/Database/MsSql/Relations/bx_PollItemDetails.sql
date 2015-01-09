
EXEC bx_Drop 'FK_bx_PollItemDetails_ItemID';

ALTER TABLE [bx_PollItemDetails] ADD 
   CONSTRAINT [FK_bx_PollItemDetails_ItemID]        FOREIGN KEY ([ItemID]) REFERENCES [bx_PollItems] ([ItemID]) ON DELETE CASCADE ON UPDATE CASCADE

GO

