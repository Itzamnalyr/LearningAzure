CREATE PROCEDURE [dbo].[GetOwners]
	@OwnerId INT = NULL
AS
BEGIN
	SELECT o.id, o.owner_name AS OwnerName
	FROM owners o
	WHERE (@OwnerId IS NULL OR o.id = @OwnerId)
	ORDER BY o.owner_name
END