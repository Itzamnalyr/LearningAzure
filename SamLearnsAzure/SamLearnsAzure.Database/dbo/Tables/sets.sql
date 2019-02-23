CREATE TABLE [dbo].[sets]
(
	[set_num] VARCHAR(100) NOT NULL PRIMARY KEY,
	[name] VARCHAR(500) NULL,
	[year] INT NULL,
	[theme_id] INT NULL,	
	[num_parts] INT NULL--, 
    CONSTRAINT [FK_sets_themes] FOREIGN KEY ([theme_id]) REFERENCES [themes]([id])
)