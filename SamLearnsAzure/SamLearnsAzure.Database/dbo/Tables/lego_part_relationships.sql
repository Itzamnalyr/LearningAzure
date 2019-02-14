CREATE TABLE [dbo].[lego_part_relationships]
(
	[rel_type] VARCHAR(10) NOT NULL,
	[child_part_num] VARCHAR(100) NULL,
	[parent_part_num] VARCHAR(100) NULL, 
    [part_relationship_id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY--, 
    --CONSTRAINT [FK_lego_part_relationships_child_lego_parts] FOREIGN KEY ([child_part_num]) REFERENCES [lego_parts]([part_num]), 
    --CONSTRAINT [FK_lego_part_relationships_parent_lego_parts] FOREIGN KEY ([parent_part_num]) REFERENCES [lego_parts]([part_num])
)
