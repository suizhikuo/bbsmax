EXEC bx_Drop 'bx_UserProps';

CREATE TABLE [bx_UserProps] (
   [UserPropID]   int IDENTITY(1,1) NOT NULL
  ,[UserID]       int               NOT NULL CONSTRAINT [DF_bx_UserProps_UserID]		DEFAULT (0)
  ,[PropID]       int               NOT NULL CONSTRAINT [DF_bx_UserProps_PropID]		DEFAULT (0)
  ,[Count]        int               NOT NULL CONSTRAINT [DF_bx_UserProps_Count]			DEFAULT (0)
  ,[SellingCount] int               NOT NULL CONSTRAINT [DF_bx_UserProps_SellingCount]	DEFAULT (0)
  ,[SellingPrice] int               NOT NULL CONSTRAINT [DF_bx_UserProps_SellingPrice]	DEFAULT (0)
  ,[SellingDate]  datetime          NOT NULL CONSTRAINT [DF_bx_UserProps_SellingDate]	DEFAULT (GETDATE())
  
  ,CONSTRAINT [PK_bx_UserProps] PRIMARY KEY ([UserPropID])
);

CREATE INDEX [IX_bx_UserProps_UserID_PropID_SellingCount] ON [bx_UserProps] ([UserID],[PropID],[SellingCount]);