CREATE PROCEDURE [dbo].[GetSets]
	@SetNum VARCHAR(100) = NULL
AS
BEGIN
	SELECT s.set_num AS SetNum, s.[name], s.num_parts AS NumParts, s.theme_id AS ThemeId, s.[year]
	FROM [sets] s
	WHERE (@SetNum IS NULL OR s.set_num = @SetNum)
	ORDER BY s.[name]
END