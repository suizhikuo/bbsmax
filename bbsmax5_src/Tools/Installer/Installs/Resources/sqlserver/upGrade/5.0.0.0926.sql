--ignore error
DECLARE @IsFullTextEnabled bit,@NowIsEnabled bit;
IF (SELECT DATABASEPROPERTY (db_name(),'IsFulltextEnabled'))<>1
	SET @IsFullTextEnabled = 0;
ELSE
	SET @IsFullTextEnabled = 1;

SET @NowIsEnabled = 0;

IF EXISTS(SELECT * FROM sysfulltextcatalogs WHERE [name]='FTCatalog_bbsMax_Threads') BEGIN
	SET @NowIsEnabled = 1;
	IF @IsFullTextEnabled = 0
		EXEC sp_fulltext_database 'enable'

	exec sp_fulltext_catalog N'FTCatalog_bbsMax_Threads', N'stop' 
	
    IF EXISTS(SELECT t.name FROM sysobjects t INNER JOIN sysfulltextcatalogs ftc ON ftc.ftcatid=objectproperty(t.id,'TableFulltextCatalogID') WHERE t.name='bbsMax_Threads') BEGIN
		exec sp_fulltext_table N'[bbsMax_Threads]', N'drop'
		exec sp_fulltext_catalog N'FTCatalog_bbsMax_Threads', N'drop'
    END ELSE IF EXISTS(SELECT t.name FROM sysobjects t INNER JOIN sysfulltextcatalogs ftc ON ftc.ftcatid=objectproperty(t.id,'TableFulltextCatalogID') WHERE t.name='bx_Threads') BEGIN
		exec sp_fulltext_table N'[bx_Threads]', N'drop'
		exec sp_fulltext_catalog N'FTCatalog_bbsMax_Threads', N'drop'
    END
END

IF EXISTS(SELECT * FROM sysfulltextcatalogs WHERE [name]='FTCatalog_bbsMax_Posts') BEGIN
	IF @IsFullTextEnabled = 0 AND @NowIsEnabled = 0
		EXEC sp_fulltext_database 'enable'
	SET @NowIsEnabled = 1;
	exec sp_fulltext_catalog N'FTCatalog_bbsMax_Posts', N'stop' 
		
    IF EXISTS(SELECT t.name FROM sysobjects t INNER JOIN sysfulltextcatalogs ftc ON ftc.ftcatid=objectproperty(t.id,'TableFulltextCatalogID') WHERE t.name='bbsMax_Posts') BEGIN
		exec sp_fulltext_table N'[bbsMax_Posts]', N'drop'
		exec sp_fulltext_catalog N'FTCatalog_bbsMax_Posts', N'drop'
    END ELSE IF EXISTS(SELECT t.name FROM sysobjects t INNER JOIN sysfulltextcatalogs ftc ON ftc.ftcatid=objectproperty(t.id,'TableFulltextCatalogID') WHERE t.name='bx_Posts') BEGIN
		exec sp_fulltext_table N'[bx_Posts]', N'drop'
		exec sp_fulltext_catalog N'FTCatalog_bbsMax_Posts', N'drop'
    END
END
IF @IsFullTextEnabled = 0 AND @NowIsEnabled = 1
	EXEC sp_fulltext_database 'disable';

GO

DELETE bx_Polemizes WHERE ThreadID NOT IN(SELECT ThreadID FROM bx_Threads)