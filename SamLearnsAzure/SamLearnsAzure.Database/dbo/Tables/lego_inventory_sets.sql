CREATE TABLE [dbo].[lego_inventory_sets]
(
	[inventory_id] INT NOT NULL,
	[set_num] VARCHAR(100) NOT NULL,
	[quantity] INT NULL,
	[inventory_set_id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    CONSTRAINT [FK_lego_inventory_sets_lego_inventories] FOREIGN KEY ([inventory_id]) REFERENCES [lego_inventories]([id]), 
    CONSTRAINT [FK_lego_inventory_sets_lego_sets] FOREIGN KEY ([set_num]) REFERENCES [lego_sets]([set_num])
)
