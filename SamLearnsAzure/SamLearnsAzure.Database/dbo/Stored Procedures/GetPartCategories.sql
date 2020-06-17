CREATE PROCEDURE [dbo].[GetPartCategories]
AS
BEGIN
	SELECT p.id, p.[name]
	FROM part_categories p
	ORDER BY p.[name]
END