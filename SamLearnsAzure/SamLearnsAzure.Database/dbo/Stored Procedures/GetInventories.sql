CREATE PROCEDURE [dbo].[GetInventories]
AS
BEGIN
	SELECT i.id, i.set_num AS SetNum, i.[version]
	FROM inventories i
	ORDER BY i.id
END