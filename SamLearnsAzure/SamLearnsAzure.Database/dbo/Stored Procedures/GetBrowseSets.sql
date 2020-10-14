CREATE PROCEDURE [dbo].[GetBrowseSets]
	@ThemeId INT = NULL,
	@Year INT = NULL
AS
BEGIN
	CREATE TABLE #TmpTheme(theme_id int)
	INSERT INTO #TmpTheme
	SELECT c.id
	FROM ThemeView c
	WHERE (c.top_parent_id = @ThemeId OR @ThemeId IS NULL)

    SELECT s.set_num AS SetNum, s.[name], s.num_parts AS NumParts, s.theme_id AS ThemeId, t.[name] as ThemeName, s.[year]
    FROM [sets] s 
    JOIN themes t ON t.id = s.theme_id
    JOIN #TmpTheme c ON c.theme_id = t.id    
    WHERE (s.[year] = @Year OR @Year IS NULL)
	ORDER BY s.[year] DESC, s.[name]

	DROP TABLE #TmpTheme
END