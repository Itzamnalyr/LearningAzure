CREATE PROCEDURE [dbo].[GetColors]
AS
BEGIN
	SELECT c.id, c.[name], c.is_trans AS IsTrans, c.rgb
	FROM colors c
	ORDER BY c.[name]
END