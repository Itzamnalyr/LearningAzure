CREATE PROCEDURE [dbo].[GetThemesWithChildren]
AS
BEGIN
    ; WITH cte AS (
        SELECT [name], id, parent_id, id AS head
        FROM themes
        WHERE parent_id is null
        --and id = 1
        UNION ALL

        SELECT child.[name], child.id, child.parent_id, parent.head
        FROM themes child
        JOIN cte parent ON parent.id = child.parent_id
    )
    SELECT  *
    FROM cte
    ORDER BY head
END
