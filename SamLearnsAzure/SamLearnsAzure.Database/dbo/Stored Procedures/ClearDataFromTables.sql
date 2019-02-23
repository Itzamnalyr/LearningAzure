--This is used for migration 
CREATE PROCEDURE [dbo].[ClearDataFromTables]
AS
BEGIN
	DELETE FROM owner_sets
	DELETE FROM part_relationships
	DELETE FROM inventory_parts
	DELETE FROM inventory_sets
	DELETE FROM colors
	DELETE FROM parts
	DELETE FROM inventories
	DELETE FROM part_categories
	DELETE FROM [sets]
	DELETE FROM themes
END
GO