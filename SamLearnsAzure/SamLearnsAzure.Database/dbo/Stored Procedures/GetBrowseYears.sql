CREATE PROCEDURE [dbo].[GetBrowseYears]
	@ThemeId INT = NULL
AS
BEGIN
	SELECT [year], CONVERT(VARCHAR(50),[year]) + ' (' + CONVERT(VARCHAR(50),COUNT(*)) + ' sets)' AS YearName
	FROM [sets] s 
    WHERE (s.theme_id = @ThemeId OR @ThemeId IS NULL)
	GROUP BY [year]
	ORDER BY [year] DESC
END