CREATE PROCEDURE [dbo].[GetSetImages]
	@SetNum VARCHAR(100) 
AS
BEGIN
	SELECT s.set_num AS SetNum, s.set_image_id AS SetImageId, s.set_image AS SetImage
	FROM set_images s
	WHERE s.set_num = @SetNum
	ORDER BY s.set_image_id
END