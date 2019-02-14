CREATE TABLE [dbo].[lego_inventory_parts]
(
	[inventory_id] INT NOT NULL,
	[part_num] VARCHAR(100) NOT NULL,
	[color_id] INT NULL,
	[quantity] INT NULL,
	[is_spare] BIT NULL, 
    [inventory_part_id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    CONSTRAINT [FK_lego_inventory_parts_lego_inventories] FOREIGN KEY ([inventory_id]) REFERENCES [lego_inventories]([id]), 
    CONSTRAINT [FK_lego_inventory_parts_lego_parts] FOREIGN KEY ([part_num]) REFERENCES [lego_parts]([part_num]), 
    CONSTRAINT [FK_lego_inventory_parts_lego_colors] FOREIGN KEY ([color_id]) REFERENCES [lego_colors]([id]) 
)
