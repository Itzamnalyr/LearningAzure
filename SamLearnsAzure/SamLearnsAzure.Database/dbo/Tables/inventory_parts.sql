CREATE TABLE [dbo].[inventory_parts]
(
	[inventory_id] INT NOT NULL,
	[part_num] VARCHAR(100) NOT NULL,
	[color_id] INT NULL,
	[quantity] INT NOT NULL,
	[is_spare] BIT NULL, 
    [inventory_part_id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    CONSTRAINT [FK_inventory_parts_inventories] FOREIGN KEY ([inventory_id]) REFERENCES [inventories]([id]), 
    CONSTRAINT [FK_inventory_parts_parts] FOREIGN KEY ([part_num]) REFERENCES [parts]([part_num]), 
    CONSTRAINT [FK_inventory_parts_colors] FOREIGN KEY ([color_id]) REFERENCES [colors]([id]) 
)
GO

CREATE NONCLUSTERED INDEX [IX_inventory_parts_isspareandinventoryId] ON [dbo].[inventory_parts] ([is_spare], [inventory_id]) INCLUDE ([color_id], [part_num], [quantity]) WITH (ONLINE = ON)
GO