EXEC bx_Drop 'bx_UsersInRoles_AfterInsert';

GO

CREATE TRIGGER [bx_UsersInRoles_AfterInsert] ON [bx_UsersInRoles] AFTER INSERT

AS
BEGIN

	SET NOCOUNT ON;
	
	DECLARE @tempTable table(RoleID int,TotalUser int)
	
	INSERT INTO @tempTable
		SELECT DISTINCT RoleID,
			ISNULL((SELECT COUNT(*) FROM [bx_UsersInRoles] as r WITH (NOLOCK) WHERE r.RoleID=T.RoleID),0)
		FROM [INSERTED]	T;
		
	UPDATE [bx_Roles]
		SET
			bx_Roles.UserCount=T.TotalUser
		FROM @tempTable	 T
		WHERE
			T.RoleID=bx_Roles.RoleID;
END			