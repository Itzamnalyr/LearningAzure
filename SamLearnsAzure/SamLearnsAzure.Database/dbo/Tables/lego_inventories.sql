CREATE TABLE [dbo].[lego_inventories]
(
	[id] INT NOT NULL PRIMARY KEY,
	[version] INT NULL,
	[set_num] VARCHAR(100) NULL--, 
    --CONSTRAINT [FK_lego_inventories_lego_sets] FOREIGN KEY ([set_num]) REFERENCES [lego_sets]([set_num])
)