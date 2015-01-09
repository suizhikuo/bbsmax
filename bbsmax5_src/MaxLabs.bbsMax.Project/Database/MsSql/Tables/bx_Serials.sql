EXEC bx_Drop '[bx_Serials]';

CREATE TABLE [bx_Serials] (
   [Serial]           uniqueidentifier                                 NOT NULL    CONSTRAINT [DF_bx_bx_Serials_Serial]          DEFAULT (NEWID())
  
  ,[UserID]           int                                              NOT NULL
  
  ,[CreateDate]       datetime                                         NOT NULL    CONSTRAINT [DF_bx_bx_Serials_CreateDate]      DEFAULT (GETDATE())
  ,[ExpiresDate]      datetime                                         NOT NULL    
  ,[Type]			  tinyint										   NOT NULL    CONSTRAINT [DF_bx_bx_Serials_Type]			 DEFAULT (0)
  ,[Data]             nvarchar(1000)                                   NULL 
  ,CONSTRAINT [PK_bx_bx_Serials] PRIMARY KEY ([Serial])
  );
  
  CREATE INDEX [IX_bx_Serials_UserType] ON [bx_Serials] ([UserID],[Type]);