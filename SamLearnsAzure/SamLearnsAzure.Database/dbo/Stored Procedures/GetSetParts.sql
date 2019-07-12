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
		SUM(ips.quantity) AS Quantity,
		CASE WHEN (p2.part_num IS NULL) THEN 0 ELSE 1 END AS IsCustomImage
	FROM parts p 
	JOIN part_categories pc ON p.part_cat_id = pc.id
	JOIN inventory_parts ips ON ips.part_num = p.part_num
	JOIN inventories i ON i.id = ips.inventory_id 
	JOIN colors c ON ips.color_id = c.id
	LEFT JOIN part_images p2 ON p.part_num = p2.part_num
	WHERE i.set_num = @SetNum --'75218-1'
	AND ips.is_spare = 0
	GROUP BY c.[id], c.[name], p.part_num, p.[name], pc.id, pc.[name], p2.part_num
	ORDER BY c.[id], p.[name]
END
GO