CREATE TABLE [dbo].[feature_flags]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[Name] VARCHAR(50),
	[EnabledInDev] BIT,
	[EnabledInQA] BIT,
	[EnabledInProd] BIT,
	[LastUpdated] DATETIME
)
