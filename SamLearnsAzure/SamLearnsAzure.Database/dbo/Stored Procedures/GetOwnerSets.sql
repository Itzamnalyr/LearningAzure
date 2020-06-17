CREATE PROCEDURE [dbo].[GetOwnerSets]
	@OwnerId int = NULL
AS
BEGIN
	SELECT os.set_num AS SetNum, os.owner_id AS OwnerId, os.owned, os.wanted, os.owner_set_id AS OwnerSetId,
        o.owner_name AS OwnerName 
	FROM owner_sets os
    JOIN owners o ON o.id = os.owner_id
    JOIN [sets] s ON os.set_num = s.set_num
	WHERE (@OwnerId IS NULL OR o.id = @OwnerId)
	ORDER BY s.[name]
END