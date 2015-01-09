EXEC bx_Drop 'FK_bx_ChatMessageReverters_MessageID';

ALTER TABLE [bx_ChatMessageReverters]  WITH CHECK ADD  CONSTRAINT [FK_bx_ChatMessageReverters_MessageID] FOREIGN KEY([MessageID])
REFERENCES [bx_ChatMessages] ([MessageID])
ON UPDATE CASCADE
ON DELETE CASCADE