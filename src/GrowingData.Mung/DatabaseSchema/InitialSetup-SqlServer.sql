
CREATE SCHEMA mung


DROP TABLE mung.Component 
DROP TABLE mung.Dashboard 
DROP TABLE mung.Munger
GO

DELETE FROM mung.Component

SELECT * FROM mung.Component

CREATE TABLE mung.Munger(
	MungerId [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](256) NULL,
	[Name] [nvarchar](256) NOT NULL,
	[IsAdmin] [bit] NOT NULL,

	[PasswordHash] [nvarchar](max) NULL,
	[PasswordSalt] [nvarchar](max) NULL,

	[CreatedAt] [datetime] NOT NULL DEFAULT (getutcdate()),
	[LastSeenAt] [datetime] NOT NULL DEFAULT (getutcdate()),
	CONSTRAINT [PK_app.Users] PRIMARY KEY CLUSTERED (MungerId ASC)
) 

GO

CREATE TABLE mung.Dashboard (
	DashboardId INT NOT NULL IDENTITY(1,1),
	Url VARCHAR(256) NOT NULL,
	Title VARCHAR(256) NULL,
	Css NVARCHAR(MAX) NULL,
	CreatedAt DATETIME NOT NULL,
	UpdatedAt DATETIME NOT NULL,
	CreatedByUserId INT NOT NULL,
	ModifiedByUserId INT NOT NULL,
	CONSTRAINT PK_Dashboard PRIMARY KEY (DashboardId),
	CONSTRAINT FK_Dashboard_CreatedBy FOREIGN KEY (CreatedByUserId) REFERENCES mung.Munger(MungerId),
	CONSTRAINT UQ_Dashboard_Url UNIQUE (Url)
)

CREATE TABLE mung.Component (
	ComponentId INT NOT NULL IDENTITY(1,1),
	DashboardId INT NOT NULL,
	Html NVARCHAR(MAX),
	Sql NVARCHAR(MAX),
	Js NVARCHAR(MAX) NULL,
	PositionX FLOAT NOT NULL,
	PositionY FLOAT NOT NULL,
	Width FLOAT NOT NULL,
	Height FLOAT NOT NULL

	CONSTRAINT PK_Component PRIMARY KEY (ComponentId),
	CONSTRAINT FK_Component_Dashboard FOREIGN KEY (DashboardId) REFERENCES mung.Dashboard(DashboardId)
)
GO
