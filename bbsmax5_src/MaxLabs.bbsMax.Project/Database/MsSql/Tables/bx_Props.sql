EXEC bx_Drop 'bx_Props';

CREATE TABLE [bx_Props] (
   [PropID]            int           IDENTITY(1,1)                NOT NULL
  ,[Icon]              nvarchar(255) COLLATE Chinese_PRC_CI_AS_WS NOT NULL CONSTRAINT [DF_bx_Props_Icon]                DEFAULT('')
  ,[Name]              nvarchar(100) COLLATE Chinese_PRC_CI_AS_WS NOT NULL CONSTRAINT [DF_bx_Props_Name]                DEFAULT('')
  ,[Price]             int                                        NOT NULL CONSTRAINT [DF_bx_Props_Price]               DEFAULT(0)
  ,[PriceType]         int                                        NOT NULL CONSTRAINT [DF_bx_Props_PriceType]           DEFAULT('')
  ,[PropType]          nvarchar(512) COLLATE Chinese_PRC_CI_AS_WS NOT NULL CONSTRAINT [DF_bx_Props_PropType]            DEFAULT('')
  ,[PropParam]         ntext         COLLATE Chinese_PRC_CI_AS_WS NOT NULl CONSTRAINT [DF_bx_Props_PropParam]           DEFAULT('') 
  ,[Description]       nvarchar(255) COLLATE Chinese_PRC_CI_AS_WS NOT NULL CONSTRAINT [DF_bx_Props_Description]         DEFAULT(0)
  ,[PackageSize]       int                                        NOT NULL CONSTRAINT [DF_bx_Props_PackageSize]         DEFAULT(0)
  ,[TotalNumber]       int                                        NOT NULL CONSTRAINT [DF_bx_Props_TotalNumber]         DEFAULT(0)
  ,[SaledNumber]       int                                        NOT NULL CONSTRAINT [DF_bx_Props_SaledNumber]			DEFAULT(0)
  ,[AllowExchange]     bit                                        NOT NULL CONSTRAINT [DF_bx_Props_AllowExchange]       DEFAULT(0)
  ,[AutoReplenish]     bit                                        NOT NULl CONSTRAINT [DF_bx_Props_AutoReplenish]       DEFAULT(0)
  ,[ReplenishNumber]   int                                        NOT NULL CONSTRAINT [DF_bx_Props_ReplenishNumber]     DEFAULT(0)
  ,[ReplenishTimeSpan] int                                        NOT NULL CONSTRAINT [DF_bx_Props_ReplenishTimeSpan]   DEFAULT(0)
  ,[LastReplenishTime] datetime                                   NOT NULL CONSTRAINT [DF_bx_Props_LastReplenishTime]	DEFAULT(GETDATE())
  ,[BuyCondition]      ntext        COLLATE Chinese_PRC_CI_AS_WS  NOT NULL CONSTRAINT [DF_bx_Props_BuyCondition]        DEFAULT('')
  ,[Enable]            bit                                        NOT NULL CONSTRAINT [DF_bx_Props_Enable]				DEFAULT(1)
  ,[ReplenishLimit]    int                                        NOT NULL CONSTRAINT [DF_bx_Props_ReplenishLimit]		DEFAULT(0)
  ,[SortOrder]         int                                        NOT NULL CONSTRAINT [DF_bx_Props_SortOrder]			DEFAULT(0)
  
  ,CONSTRAINT [PK_bx_Props] PRIMARY KEY ([PropID])
);