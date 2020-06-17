CREATE PROCEDURE [dbo].[GetPartRelationships]
AS
BEGIN
	SELECT p.part_relationship_id AS PartRelationshipId, p.parent_part_num AS ParentPartNum, p.child_part_num AS ChildPartNum, p.rel_type AS RelType
	FROM part_relationships p
	ORDER BY p.part_relationship_id 
END