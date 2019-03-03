CREATE PROCEDURE [dbo].[GetSetParts]
	--@PartNum VARCHAR(100) = NULL,
	@SetNum VARCHAR(100) = NULL
	--@ColorID INT = NULL
AS
BEGIN
	SELECT p.part_num AS PartNum, 
		p.[name] AS PartName,
		c.id AS ColorId,
		c.[name] AS ColorName,  
		pc.id AS PartCategoryId, 
		pc.[name] AS PartCategoryName,
		SUM(ips.quantity) AS Quantity
	FROM parts p 
	JOIN part_categories pc ON p.part_cat_id = pc.id
	JOIN inventory_parts ips ON ips.part_num = p.part_num
	JOIN inventories i ON i.id = ips.inventory_id 
	JOIN colors c ON ips.color_id = c.id
	WHERE i.set_num = @SetNum --'75218-1'
	AND ips.is_spare = 0
	GROUP BY c.[id], c.[name], p.part_num, p.[name], pc.id, pc.[name]
	ORDER BY c.[id], p.[name]
END
GO