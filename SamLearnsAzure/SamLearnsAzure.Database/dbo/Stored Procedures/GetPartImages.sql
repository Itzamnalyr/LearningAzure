CREATE PROCEDURE [dbo].[GetPartImages]
	@PartNum VARCHAR(100) = NULL
AS
BEGIN
	SELECT p.part_num AS PartNum, p.part_image_id AS PartImageId, p.source_image_url AS SourceImageUrl, p.color_id AS ColorId, p.last_updated AS LastUpdated
	FROM part_images p
	WHERE (@PartNum IS NULL OR p.part_num = @PartNum)
	ORDER BY p.part_num
END