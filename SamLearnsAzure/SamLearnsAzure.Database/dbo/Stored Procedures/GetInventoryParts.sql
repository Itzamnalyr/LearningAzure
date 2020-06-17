CREATE PROCEDURE [dbo].[GetInventoryParts]
	@PartNum VARCHAR(100) 
AS
BEGIN
	SELECT i.inventory_id AS InventoryId, i.inventory_part_id AS InventoryPartId, i.color_id AS ColorId, 
		i.is_spare AS IsSpare, i.part_num AS PartNum, i.quantity
	FROM inventory_parts i
	WHERE i.part_num = @PartNum
	ORDER BY i.inventory_part_id
END