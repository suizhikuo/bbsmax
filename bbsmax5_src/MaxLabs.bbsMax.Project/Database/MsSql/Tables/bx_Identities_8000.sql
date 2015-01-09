EXEC bx_Drop 'bx_Identities_8000';

SELECT TOP 8000 I = IDENTITY(int,1,1) INTO [bx_Identities_8000]
FROM syscolumns a, syscolumns b
ALTER TABLE [bx_Identities_8000] ADD CONSTRAINT PK_bx_Identities_8000 PRIMARY KEY(I)

/*
Name: 序列表1-8000，此表可用作高效分割字符串
Columns:
    [I]      序列，存储1-8000的8000个顺序数字
*/

GO
