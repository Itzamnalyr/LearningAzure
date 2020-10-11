CREATE PROCEDURE [dbo].[GetBrowseSets]
	@ThemeId INT = NULL,
	@Year INT = NULL
AS
BEGIN
	exec GetSets
END