CREATE TABLE [dbo].[lego_parts]
(
	[part_num] VARCHAR(100) NOT NULL PRIMARY KEY,
	[name] VARCHAR(500) NULL,
	[part_cat_id] INT NULL--, 
    --CONSTRAINT [FK_lego_parts_lego_part_categories] FOREIGN KEY ([part_cat_id]) REFERENCES [lego_part_categories]([id]) 
)
