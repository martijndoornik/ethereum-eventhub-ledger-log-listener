DROP TABLE IF EXISTS [dbo.Events]
DROP TABLE IF EXISTS [dbo.Errors]

CREATE TABLE [dbo.Events] (
	[id] BIGINT PRIMARY KEY,
	[name] VARCHAR(255) NOT NULL,
	[body] TEXT,
	[date] DATETIME NOT NULL,
	[listener] VARCHAR(100) NOT NULL,
	[listener_version] VARCHAR(20) NOT NULL
)

CREATE TABLE [dbo.Errors] (
	[id] INT PRIMARY KEY,
	[event_id] BIGINT NOT NULL,
	[message] VARCHAR(255) NOT NULL
)