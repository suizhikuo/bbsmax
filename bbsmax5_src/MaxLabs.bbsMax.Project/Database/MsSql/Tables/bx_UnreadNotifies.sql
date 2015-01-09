exec bx_Drop 'bx_UnreadNotifies';

CREATE TABLE bx_UnreadNotifies
(
	[UserID]			INT		NOT NULL				
	,[TypeID]			INT		NOT NULL				
	,[UnreadCount]		INT		NOT NULL				CONSTRAINT  [DF_bx_UnreadNotifies_UnreadCount]			DEFAULT(1)
	,CONSTRAINT [PK_bx_UnreadNotifies] PRIMARY KEY ([UserID],[TypeID]) 
);

