CREATE TABLE [dbo].[inventories]
(
	[id] INT NOT NULL PRIMARY KEY,
	[version] INT NULL,
	[set_num] VARCHAR(100) NULL--, 
    --CONSTRAINT [FK_inventories_sets] FOREIGN KEY ([set_num]) REFERENCES [sets]([set_num])
)