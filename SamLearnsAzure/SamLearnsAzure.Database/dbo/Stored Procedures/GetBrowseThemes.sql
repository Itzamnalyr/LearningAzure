CREATE PROCEDURE [dbo].[GetBrowseThemes]
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
    SELECT c.id, 
        c.[name], 
        c.parent_id AS ParentId, 
        c.top_parent_id AS TopParentId, 
        COUNT(s.set_num) AS SetCount,
        c.[name] + ' (' + CONVERT(VARCHAR(50),COUNT(s.set_num)) + ' sets)' AS ThemeName
    FROM cte c
    JOIN [sets] s ON c.id = s.theme_id
    WHERE c.parent_id IS NULL
    AND (s.[year] = @Year OR @Year IS NULL)
    GROUP BY c.id, c.[name], c.parent_id, c.top_parent_id
	ORDER BY c.[name]
END