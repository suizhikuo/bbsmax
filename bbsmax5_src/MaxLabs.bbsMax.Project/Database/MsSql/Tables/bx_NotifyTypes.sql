exec bx_Drop 'bx_NotifyTypes';

CREATE TABLE bx_NotifyTypes(
 [TypeID]       int IDENTITY(1,1)    not null    
,[TypeName]     nvarchar(50)		 not null
,[Keep]         bit                  not null    CONSTRAINT [DF_bx_NotifyTypes_Keep] DEFAULT (1)
,[Description]  nvarchar(200)        null  
,CONSTRAINT [PK_bx_NotifyTypes] PRIMARY KEY ([TypeID])       
);