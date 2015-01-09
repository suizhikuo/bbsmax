EXEC bx_Drop bx_ThreadWords;

GO

CREATE TABLE [bx_ThreadWords](
    [ThreadID]         int							                    NOT NULL    CONSTRAINT  [DF_bx_ThreadWords_ThreadID]           DEFAULT(0)
   ,[Word]             nvarchar(50)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_ThreadWords_Words]              DEFAULT('')
)

/*
Name:标签
Columns:
    [ThreadID]          主题ID
    [Word]				关键字
*/

GO


CREATE INDEX [IX_bx_ThreadWords_ThreadID] ON [bx_ThreadWords]([ThreadID])
CREATE INDEX [IX_bx_ThreadWords_Word] ON [bx_ThreadWords]([Word])

