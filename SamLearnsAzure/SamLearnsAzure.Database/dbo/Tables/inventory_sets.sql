CREATE TABLE [dbo].[inventory_sets]
(
	[inventory_id] INT NOT NULL,
	[set_num] VARCHAR(100) NOT NULL,
	[quantity] INT NULL,
	[inventory_set_id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    CONSTRAINT [FK_inventory_sets_inventories] FOREIGN KEY ([inventory_id]) REFERENCES [inventories]([id]), 
    CONSTRAINT [FK_inventory_sets_sets] FOREIGN KEY ([set_num]) REFERENCES [sets]([set_num])
)
