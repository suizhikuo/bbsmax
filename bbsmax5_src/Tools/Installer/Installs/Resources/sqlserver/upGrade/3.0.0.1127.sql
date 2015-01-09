IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[bbsMax_Identities_4000]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [bbsMax_Identities_4000]


SELECT TOP 4000 ID = IDENTITY(int,1,1) INTO [bbsMax_Identities_4000]
FROM syscolumns a, syscolumns b
ALTER TABLE [bbsMax_Identities_4000] ADD CONSTRAINT PK_bbsMax_Identities_4000 PRIMARY KEY(ID)


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[bbsMax_Identities_8000]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [bbsMax_Identities_8000]


SELECT TOP 8000 ID = IDENTITY(int,1,1) INTO [bbsMax_Identities_8000]
FROM syscolumns a, syscolumns b
ALTER TABLE [bbsMax_Identities_8000] ADD CONSTRAINT PK_bbsMax_Identities_8000 PRIMARY KEY(ID)

GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[Max_Identities_4000]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [Max_Identities_4000]


SELECT TOP 4000 ID = IDENTITY(int,1,1) INTO [Max_Identities_4000]
FROM syscolumns a, syscolumns b
ALTER TABLE [Max_Identities_4000] ADD CONSTRAINT PK_Max_Identities_4000 PRIMARY KEY(ID)



IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[Max_Identities_8000]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [Max_Identities_8000]


SELECT TOP 8000 ID = IDENTITY(int,1,1) INTO [Max_Identities_8000]
FROM syscolumns a, syscolumns b
ALTER TABLE [Max_Identities_8000] ADD CONSTRAINT PK_Max_Identities_8000 PRIMARY KEY(ID)

GO

UPDATE [System_bbsMax_Settings] SET SettingValue=N'point1+point2' WHERE Catalog=N'PointSetting' AND SettingKey=N'PointFormula' AND (CAST([SettingValue] AS NVARCHAR(1000))=N'convert([bigint],[ExtendedPoints_1],0)+convert([bigint],[ExtendedPoints_2],0)' OR SettingValue IS NULL)
GO


