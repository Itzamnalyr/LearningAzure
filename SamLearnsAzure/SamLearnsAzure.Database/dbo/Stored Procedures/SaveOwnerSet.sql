CREATE PROCEDURE [dbo].[SaveOwnerSet]
	@SetNum VARCHAR(100),
	@OwnerId INT,
    @Owned BIT, 
    @Wanted BIT
AS
BEGIN
	IF (EXISTS (SELECT 1 FROM owner_sets WHERE owner_id = @OwnerId AND set_num = @SetNum))
	BEGIN
		UPDATE o
		SET o.owned = @Owned,
			o.wanted = @Wanted 
		FROM owner_sets o 
		WHERE o.set_num = @SetNum
		AND o.owner_id = @OwnerId
	END
	ELSE
	BEGIN
		INSERT INTO owner_sets ([set_num], [owner_id], [owned], [wanted])
		VALUES (@SetNum, @OwnerId, @Owned, @Wanted)
	END
END
GO