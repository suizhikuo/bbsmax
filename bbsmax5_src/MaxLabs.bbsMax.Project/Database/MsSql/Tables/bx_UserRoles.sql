--用户用户组关系表
EXEC bx_Drop 'bx_UserRoles';

CREATE TABLE [bx_UserRoles](
     [UserID]         int                 NOT NULL
    ,[RoleID]         uniqueidentifier    NOT NULL

    ,[BeginDate]      datetime            NOT NULL
    ,[EndDate]        datetime            NOT NULL
    
    ,CONSTRAINT [PK_bx_UserRoles] PRIMARY KEY ([UserID],[RoleID])
);

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_UserRoles_UserRoles] ON [bx_UserRoles] ([RoleID], [UserID]);
CREATE NONCLUSTERED INDEX [IX_bx_UserRoles_EndDate] ON [bx_UserRoles] ([EndDate]);
GO

