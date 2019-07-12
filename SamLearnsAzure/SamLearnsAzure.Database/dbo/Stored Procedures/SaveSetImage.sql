CREATE PROCEDURE [dbo].[SaveSetImage]
	@SetNum VARCHAR(100),
	@SetImage VARCHAR(500)
AS
BEGIN
	IF (EXISTS (SELECT 1 FROM set_images WHERE set_num = @SetNum))
	BEGIN
		DELETE FROM set_images 
		WHERE set_num = @SetNum
	END

	INSERT INTO set_images ([set_num], [set_image])
	VALUES (@SetNum, @SetImage)
END
GO