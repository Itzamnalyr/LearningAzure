CREATE PROCEDURE [dbo].[GetBrowseThemes]
    @Year INT = NULL
AS
BEGIN
    SELECT t.id, 
        t.[name], 
        c.top_parent_id AS TopParentId, 
        COUNT(s.set_num) AS SetCount,
        t.[name] + ' (' + CONVERT(VARCHAR(50),COUNT(s.set_num)) + ' sets)' AS ThemeName
    FROM Themes t
    JOIN ThemeView c ON c.top_parent_id = t.id
    JOIN [sets] s ON c.id = s.theme_id
    WHERE t.parent_id IS NULL 
    AND (s.[year] = @Year OR @Year IS NULL)   
    GROUP BY t.id, t.[name], c.top_parent_id
	ORDER BY t.[name]
END