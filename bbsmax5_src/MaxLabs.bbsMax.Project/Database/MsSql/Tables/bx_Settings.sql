EXEC bx_Drop 'bx_Settings';

CREATE TABLE [bx_Settings] (
     [NodeID]      int                                              NULL
     
    ,[Key]         nvarchar(200)    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Settings_Key]      DEFAULT ('*')
    ,[TypeName]    nvarchar(200)    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL
    
    ,[Value]       ntext            COLLATE Chinese_PRC_CI_AS_WS    NOT NULL
);

/*
Name: 系统设置表
Columns:
    [NodeID]      目标节点ID，用在版块等，允许为空

    [Key]         设置项Key，如果是序列化保存，则Key都是*，如果是非序列化保存，则此项是属性名
    [TypeName]    设置项类型名，使用FullName

    [Value]       设置值，如果是序列化保存，保存的值是整个设置对象序列化所得的字符串，如果是非序列化保存，则保存的是对应Key的属性ToString得到的值
*/

GO

