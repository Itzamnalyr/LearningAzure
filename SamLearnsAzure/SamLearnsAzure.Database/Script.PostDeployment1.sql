/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

--Add initial data, if it doesn't already exist
GO
IF (NOT EXISTS (SELECT 1 FROM lego_owners o WHERE o.id = 1))
BEGIN
	INSERT INTO lego_owners
	SELECT 1, 'Sam'

	INSERT INTO lego_owners
	SELECT 2, 'Finn'

	INSERT INTO lego_owners
	SELECT 3, 'Stella'
END 

--IF (NOT EXISTS (SELECT 1 FROM lego_owner_sets os WHERE os.owner_id = 1) AND EXISTS (SELECT 1 FROM lego_sets))
--BEGIN
--	INSERT INTO lego_owner_sets 
--	SELECT '75159-1' AS set_num, 1 AS owner_id, 1 AS owned, 1 AS wanted
--	UNION
--	SELECT '41608-1' AS set_num, 1 AS owner_id, 1 AS owned, 1 AS wanted
--	UNION
--	SELECT '75955-1' AS set_num, 1 AS owner_id, 1 AS owned, 1 AS wanted
--	UNION
--	SELECT '75218-1' AS set_num, 1 AS owner_id, 1 AS owned, 1 AS wanted
--END
GO