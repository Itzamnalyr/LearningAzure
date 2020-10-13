CREATE PROCEDURE [dbo].[GetBrowseYears]
	@ThemeId INT = NULL
AS
BEGIN
	CREATE TABLE #TmpTheme(theme_id int)
	INSERT INTO #TmpTheme
	SELECT c.id
	FROM ThemeView c
	WHERE (c.top_parent_id = @ThemeId OR @ThemeId IS NULL)

	SELECT s.[year], CONVERT(VARCHAR(50),[year]) + ' (' + CONVERT(VARCHAR(50), COUNT(*)) + ' sets)' AS YearName
	FROM [sets] s 
    JOIN Themes t ON t.id = s.theme_id
    JOIN #TmpTheme c ON c.theme_id = t.id      
	GROUP BY s.[year]
	ORDER BY s.[year] DESC

	DROP TABLE #TmpTheme
END
GO