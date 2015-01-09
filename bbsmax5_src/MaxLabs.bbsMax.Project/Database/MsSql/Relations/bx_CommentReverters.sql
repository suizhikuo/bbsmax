EXEC bx_Drop 'FK_bx_CommentReverters_CommentID';

ALTER TABLE [bx_CommentReverters]  WITH CHECK ADD  CONSTRAINT [FK_bx_CommentReverters_CommentID] FOREIGN KEY([CommentID])
REFERENCES [bx_Comments] ([CommentID])
ON UPDATE CASCADE
ON DELETE CASCADE