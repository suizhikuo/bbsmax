--广告表
EXEC bx_Drop 'bx_Adverts';

CREATE TABLE bx_Adverts(
	 [ADID]				int													NOT NULL											IDENTITY (1, 1)												
	,[CategoryID]		int													NOT NULL								
	,[Index]			int													NULL		CONSTRAINT [DF_bx_Adverts_Index]		DEFAULT(0)
	,[Position]			tinyint												NULL		CONSTRAINT [DF_bx_Adverts_Position]		DEFAULT(0)
	,[Targets]			ntext				COLLATE Chinese_PRC_CI_AS_WS	NULL		CONSTRAINT [DF_bx_Adverts_Targets]		DEFAULT('')
	,[ADType]			tinyint												NOT NULL				
	,[Available]		bit													NOT NULL	CONSTRAINT [DF_bx_Adverts_Available]	DEFAULT(1)
	,[Color]			varchar(50)			COLLATE Chinese_PRC_CI_AS_WS	NULL		CONSTRAINT [DF_bx_Adverts_Color]		DEFAULT('0')
	,[Title]			nvarchar(50)		COLLATE Chinese_PRC_CI_AS_WS	NOT NULL	CONSTRAINT [DF_bx_Adverts_Title]		DEFAULT('')
	,[Code]				ntext				COLLATE Chinese_PRC_CI_AS_WS	NOT NULL    CONSTRAINT [DF_bx_Adverts_CODE]			DEFAULT('')
	,[Text]				nvarchar(200)		COLLATE Chinese_PRC_CI_AS_WS	NOT NULL	CONSTRAINT [DF_bx_Adverts_Text]			DEFAULT('')
	,[Href]				nvarchar(500)		COLLATE Chinese_PRC_CI_AS_WS	NOT NULL	CONSTRAINT [DF_bx_Adverts_Href]		    DEFAULT('')
	,[FontSize]			int													NOT NULL	CONSTRAINT [DF_bx_Adverts_FontSize]		DEFAULT(14)
	,[ResourceHref]		nvarchar(500)		COLLATE Chinese_PRC_CI_AS_WS	NOT NULL	CONSTRAINT [DF_bx_Adverts_ResourceHref]	DEFAULT('')
	,[Height]			int													NOT NULL	CONSTRAINT [DF_bx_Adverts_Width]		DEFAULT(0)
	,[Width]			int													NOT NULL	CONSTRAINT [DF_bx_Adverts_Height]		DEFAULT(0)
	,[BeginDate]		datetime											NULL		CONSTRAINT [DF_bx_Adverts_BeginDate]	DEFAULT(GETDATE())
	,[EndDate]			datetime											NULL
	,[Floor]			varchar(1000)		COLLATE Chinese_PRC_CI_AS_WS	NULL		CONSTRAINT [DF_bx_Adverts_Floor]		DEFAULT(',0,')
	CONSTRAINT [PK_bx_Adverts] PRIMARY KEY ([ADID])
); 





/*
Name:     广告表
Columns:
        [ADID]
        
        [CategoryID]            广告类别
        [Index]                 显示顺序
        [Position]              显示位置（贴内广告  上、下右）
        
        [Targets]               投放目标（用逗号(,)隔开的字符串）
        
        [ADType]				广告形式（文字链接、图片、flash、HTML代码）
        [Available]				是否启用 					
        [Color]					颜色（针对文字链接）
        [Title]					标题    
        [Code]					广告代码   
        [Text]					文本（针对文字链接和图片的alt属性）   
        [Href]					广告目标地址   
        [FontSize]				字体大小（针对文字链接） 
        [ResourceHref]			广告资源地址（指图片或者FLASH的src）
        [Height]				高度（图片或者flash）   
        [Width]					宽度（同上）   
        [BeginDate]				开始日期 
        [EndDate]				结束日期
*/

GO