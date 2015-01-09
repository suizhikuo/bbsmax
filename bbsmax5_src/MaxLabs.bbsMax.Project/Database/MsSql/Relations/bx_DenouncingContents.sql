EXEC bx_Drop 'FK_bx_DenouncingContents_DenouncingID';

ALTER TABLE [bx_DenouncingContents]  WITH CHECK ADD  CONSTRAINT [FK_bx_DenouncingContents_DenouncingID] FOREIGN KEY([DenouncingID])
REFERENCES [bx_Denouncings] ([DenouncingID])
ON UPDATE CASCADE
ON DELETE CASCADE