CREATE TABLE [dbo].[parts]
(
	[part_num] VARCHAR(100) NOT NULL PRIMARY KEY,
	[name] VARCHAR(500) NULL,
	[part_cat_id] INT NULL--, 
    --CONSTRAINT [FK_parts_part_categories] FOREIGN KEY ([part_cat_id]) REFERENCES [part_categories]([id]) 
)
