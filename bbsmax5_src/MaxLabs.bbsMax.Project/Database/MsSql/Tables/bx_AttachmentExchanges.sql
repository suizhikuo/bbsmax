EXEC bx_Drop 'bx_AttachmentExchanges';

CREATE TABLE [bx_AttachmentExchanges]
(
[AttachmentID] [int] NOT NULL,
[UserID] [int] NOT NULL,
[Price] [int] NOT NULL,
[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_bx_AttachmentExchanges_CreateDate] DEFAULT (getdate()),
CONSTRAINT [PK_bx_AttachmentExchanges] PRIMARY KEY ([AttachmentID], [UserID])
)
GO

