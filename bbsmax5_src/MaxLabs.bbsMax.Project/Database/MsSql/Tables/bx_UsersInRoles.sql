EXEC bx_Drop 'bx_UsersInRoles';

CREATE TABLE [bx_UsersInRoles](
	[UserID]         int                 NOT NULL
    ,[RoleID]         int    NOT NULL

    ,[BeginDate]      datetime            NOT NULL
    ,[EndDate]        datetime            NOT NULL
    
    ,CONSTRAINT [PK_bx_UsersInRoles] PRIMARY KEY ([UserID],[RoleID])
);

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_UsersInRoles_RoleUsers] ON [bx_UsersInRoles] ([RoleID], [UserID]);
CREATE NONCLUSTERED INDEX [IX_bx_UsersInRoles_EndDate] ON [bx_UsersInRoles] ([EndDate]);
GO


