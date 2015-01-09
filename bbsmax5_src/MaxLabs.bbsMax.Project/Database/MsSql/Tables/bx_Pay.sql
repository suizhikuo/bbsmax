EXEC bx_Drop 'bx_Pay';

CREATE TABLE [bx_Pay] (
	 [PayID]		   int IDENTITY (1, 1)		NOT NULL    CONSTRAINT [PK_bx_Pay]   PRIMARY KEY ([PayID])
    ,[UserID]          int						NOT NULL
    ,[BuyerEmail]      nvarchar(50)
    ,[OrderNo]         varchar(50)		    COLLATE Chinese_PRC_CI_AS_WS  NOT NULL
    ,[TransactionNo]   nvarchar(200)			COLLATE Chinese_PRC_CI_AS_WS 
	,[OrderAmount]	   decimal(18, 2)			NOT NULL	
	,[Payment]         tinyint                  NOT NULL	          
	,[PayType]	       tinyint					NOT NULL
	,[PayValue]	       int						NOT NULL
	,[CreateDate]	   datetime				    NOT NULL	CONSTRAINT [DF_bx_Pay_CreateDate]   DEFAULT (GETDATE())
	,[PayDate]		   datetime
	,[SubmitIp]        varchar(50)              COLLATE Chinese_PRC_CI_AS_WS   NOT NULL    
	,[PayIp]           varchar(50)              COLLATE Chinese_PRC_CI_AS_WS 
	,[Remarks]		   nvarchar(50)             COLLATE Chinese_PRC_CI_AS_WS
	,[State]           bit                      CONSTRAINT [DF_bx_Pay_State]  DEFAULT ((0))
) 
GO


CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Pay_OrderNo] ON [bx_Pay] ([OrderNo]);
CREATE INDEX [IX_bx_Pay_TransactionNo] ON [bx_Pay] ([TransactionNo]);
CREATE INDEX [IX_bx_Pay_UserID] ON [bx_Pay] ([UserID],[State]);
GO
