CREATE TABLE [dbo].[lego_sets]
(
	[set_num] VARCHAR(100) NOT NULL PRIMARY KEY,
	[name] VARCHAR(500) NULL,
	[year] INT NULL,
	[theme_id] INT NULL,	
	[num_parts] INT NULL--, 
    --CONSTRAINT [FK_lego_sets_lego_themes] FOREIGN KEY ([theme_id]) REFERENCES [lego_themes]([id])
)