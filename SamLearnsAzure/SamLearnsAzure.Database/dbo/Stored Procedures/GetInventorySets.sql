CREATE PROCEDURE [dbo].[GetInventorySets]
AS
BEGIN
	SELECT i.inventory_id AS InventoryId, i.inventory_set_id AS InventorySetId, i.quantity, i.set_num AS SetNum
	FROM inventory_sets i
	ORDER BY i.inventory_id
END