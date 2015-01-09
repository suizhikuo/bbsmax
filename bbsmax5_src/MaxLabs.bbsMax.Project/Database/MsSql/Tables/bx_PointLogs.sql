
CREATE TABLE [bx_PointLogs](
	[LogID] bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_bx_PointLogs    PRIMARY KEY (LogID),
	[UserID] int NOT NULL,
	[OperateID] int NOT NULL,
	[Remarks] nvarchar(200) NULL,
	[Point0] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_P0]              DEFAULT (0),
	[Point1] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_P1]              DEFAULT (0),
	[Point2] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_P2]              DEFAULT (0),
	[Point3] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_P3]              DEFAULT (0),
	[Point4] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_P4]              DEFAULT (0),
	[Point5] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_P5]              DEFAULT (0),
	[Point6] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_P6]              DEFAULT (0),
	[Point7] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_P7]              DEFAULT (0),
	[Current0] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_CP0]              DEFAULT (0),
	[Current1] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_CP1]              DEFAULT (0),
	[Current2] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_CP2]              DEFAULT (0),
	[Current3] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_CP3]              DEFAULT (0),
	[Current4] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_CP4]              DEFAULT (0),
	[Current5] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_CP5]              DEFAULT (0),
	[Current6] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_CP6]              DEFAULT (0),
	[Current7] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_CP7]              DEFAULT (0),
	[CreateTime] datetime NOT NULL    CONSTRAINT [DF_bx_PointLogs_CreateTime]              DEFAULT (GETDATE())
);
GO