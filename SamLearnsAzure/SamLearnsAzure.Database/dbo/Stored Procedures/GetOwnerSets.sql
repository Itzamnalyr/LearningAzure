CREATE PROCEDURE [dbo].[GetOwnerSets]
	@OwnerId int = NULL
AS
BEGIN
	SELECT os.set_num AS SetNum, os.owner_id AS OwnerId, os.owned, os.wanted, os.owner_set_id AS OwnerSetId,
        o.owner_name AS OwnerName,
		s.[name] AS SetName,
        s.[year] AS SetYear, 
        t.[name] AS SetThemeName,
        s.num_parts AS SetNumParts
	FROM owner_sets os
    JOIN owners o ON o.id = os.owner_id
    JOIN [sets] s ON os.set_num = s.set_num
	JOIN themes t ON s.theme_id = t.id
	WHERE (@OwnerId IS NULL OR o.id = @OwnerId)
	ORDER BY s.[name]
END