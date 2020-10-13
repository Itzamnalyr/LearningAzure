CREATE VIEW [dbo].[ThemeView]
AS 
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
SELECT c.id, c.parent_id, c.top_parent_id
FROM cte c
--ORDER BY c.id, c.parent_id
