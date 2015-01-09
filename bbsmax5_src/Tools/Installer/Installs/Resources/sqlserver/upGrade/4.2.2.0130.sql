
IF NOT EXISTS (SELECT * FROM sysobjects WHERE [name]='bx_Serials' AND type='U') BEGIN

	CREATE TABLE [bx_Serials] (
	   [Serial]           uniqueidentifier                                 NOT NULL    CONSTRAINT [DF_bx_bx_Serials_Serial]          DEFAULT (NEWID())
	  
	  ,[UserID]           int                                              NOT NULL
	  
	  ,[CreateDate]       datetime                                         NOT NULL    CONSTRAINT [DF_bx_bx_Serials_CreateDate]      DEFAULT (GETDATE())
	  ,[ExpiresDate]      datetime                                         NOT NULL
	  ,[Type]			  int											   NOT NULL    CONSTRAINT [DF_bx_bx_Serials_Type]			 DEFAULT (0)
	  ,[Data]             nvarchar(1000)                                   NULL 
	  ,CONSTRAINT [PK_bx_Serials] PRIMARY KEY ([Serial])
	  );

END

GO

IF EXISTS (SELECT * FROM sysobjects WHERE [name]='bx_RecoverPasswordSerials' AND type='U')
	INSERT INTO bx_Serials(Serial, UserID, CreateDate,ExpiresDate, Type, Data) SELECT Serial, UserID, CreateDate, ExpiresDate, 0, N'' FROM bx_RecoverPasswordSerials;

GO

IF EXISTS (SELECT * FROM sysobjects WHERE [name]='bx_RecoverPasswordSerials' AND type='U')
	DROP TABLE bx_RecoverPasswordSerials;