CREATE PROCEDURE [dbo].[GetParts]
AS
BEGIN
	SELECT p.part_num AS PartNum, p.[name], p.part_cat_id AS PartCatId, p.part_material_id AS PartMaterialId
	FROM parts p
	ORDER BY p.[name]
END