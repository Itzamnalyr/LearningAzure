CREATE PROCEDURE [dbo].[GetThemes]
AS
BEGIN
	SELECT t.id, t.[name], t.parent_id AS ParentId
	FROM themes t
	ORDER BY [name]
END