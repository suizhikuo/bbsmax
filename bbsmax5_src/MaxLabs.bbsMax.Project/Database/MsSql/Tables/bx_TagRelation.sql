EXEC bx_Drop bx_TagRelation;

GO

CREATE TABLE [bx_TagRelation](
    [TagID]               int									       NOT NULL    CONSTRAINT  [DF_bx_TagRelation_TagID]              DEFAULT(0)
   ,[Type]                tinyint                                      NOT NULL    CONSTRAINT  [DF_bx_TagRelation_Type]               DEFAULT(0)
   ,[TargetID]            int                                          NOT NULL    CONSTRAINT  [DF_bx_TagRelation_TargetID]           DEFAULT(0)
   
)

/*
Name:标签
Columns:
    [TagID]               标签ID
    [Type]                标签类型
	[TargetID]            使用该标签的对象ID
*/

GO


--创建标签使用对象索引
CREATE INDEX  [IX_bx_TagRelation_TargetID] ON [bx_TagRelation]([TargetID]);

--创建标签类型索引
CREATE INDEX  [IX_bx_TagRelation_Type] ON [bx_TagRelation]([Type]);


