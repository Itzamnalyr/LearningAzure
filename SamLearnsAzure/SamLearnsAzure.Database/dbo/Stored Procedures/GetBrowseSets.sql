CREATE PROCEDURE [dbo].[GetBrowseSets]
	@ThemeId INT = NULL,
	@Year INT = NULL
AS
BEGIN
    WITH cte AS(
          SELECT t.*, id AS top_parent_id 
          FROM themes t 
          WHERE parent_id IS NULL
      UNION ALL
          SELECT t.*, c.top_parent_id 
          FROM themes t 
          JOIN cte c ON c.id = t.parent_id
          WHERE t.id <> t.parent_id
    )
    SELECT s.set_num AS SetNum, s.[name], s.num_parts AS NumParts, s.theme_id AS ThemeId, s.[year]
    FROM cte c
    JOIN [sets] s ON c.id = s.theme_id
    WHERE c.parent_id IS NULL
    AND (s.[year] = @Year OR @Year IS NULL)
    AND (s.theme_id = @ThemeId OR c.top_parent_id = @ThemeId OR @ThemeId IS NULL)
    --GROUP BY c.id, c.[name], c.parent_id, c.top_parent_id
	ORDER BY s.[name]
END