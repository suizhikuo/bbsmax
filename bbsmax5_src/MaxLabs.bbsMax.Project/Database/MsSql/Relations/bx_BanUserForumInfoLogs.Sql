EXEC bx_Drop 'FK_bx_BanUserForumInfoLogs_LogID';

ALTER TABLE [bx_BanUserLogForumInfos] ADD
	CONSTRAINT [FK_bx_BanUserLogForumInfos_LogID]			FOREIGN	KEY([LogID])			REFERENCES [bx_BanUserLogs]		([LogID])	ON UPDATE CASCADE	ON DELETE CASCADE