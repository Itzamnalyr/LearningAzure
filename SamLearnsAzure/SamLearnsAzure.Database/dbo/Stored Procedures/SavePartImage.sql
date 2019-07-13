CREATE PROCEDURE [dbo].[SavePartImage]
	@PartNum VARCHAR(100),
	@SourceImage VARCHAR(2000),
	@ColorId int
AS
BEGIN
	IF (EXISTS (SELECT 1 FROM set_images WHERE set_num = @PartNum))
	BEGIN
		DELETE FROM part_images 
		WHERE part_num = @PartNum
	END

	INSERT INTO part_images (part_num, source_image_url, color_id, last_updated)
	VALUES (@PartNum, @SourceImage, @ColorId, GETDATE())
END
GO