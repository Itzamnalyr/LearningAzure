CREATE PROCEDURE [dbo].[GetOwners]
	@param1 int = 0,
	@param2 int
AS
	SELECT *
	FROM Owners o
	JOIN [sets] ss ON o.owner_name = ss.set_num
	order by o.owner_name
RETURN 0
