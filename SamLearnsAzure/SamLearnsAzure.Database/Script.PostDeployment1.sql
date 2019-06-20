--Add initial data, if it doesn't already exist
GO
IF (NOT EXISTS (SELECT 1 FROM owners o WHERE o.id = 1))
BEGIN
	INSERT INTO owners
	SELECT 1, 'Sam'
END
IF (NOT EXISTS (SELECT 1 FROM owners o WHERE o.id = 2))
BEGIN
	INSERT INTO owners
	SELECT 2, 'Finn'
END
IF (NOT EXISTS (SELECT 1 FROM owners o WHERE o.id = 3))
BEGIN
	INSERT INTO owners
	SELECT 3, 'Stella'
END 

DELETE os FROM owner_sets os WHERE os.owner_id = 1
IF (NOT EXISTS (SELECT 1 FROM owner_sets os WHERE os.owner_id = 1) AND EXISTS (SELECT 1 FROM [sets]))
BEGIN
	INSERT INTO owner_sets 
	SELECT '75159-1' AS set_num, 1 AS owner_id, 1 AS owned, 1 AS wanted
	UNION
	SELECT '41608-1' AS set_num, 1 AS owner_id, 1 AS owned, 1 AS wanted
	UNION
	SELECT '75955-1' AS set_num, 1 AS owner_id, 1 AS owned, 1 AS wanted --Hogwarts Express
	UNION
	SELECT '75218-1' AS set_num, 1 AS owner_id, 1 AS owned, 1 AS wanted --X-Wing Starfighter
	UNION
	SELECT '30277-1' AS set_num, 1 AS owner_id, 1 AS owned, 1 AS wanted --First Order Star Destroyer (50 pieces)
	UNION
	SELECT '30056-1' AS set_num, 1 AS owner_id, 1 AS owned, 1 AS wanted --Star Destroyer (50 pieces)
	UNION
	SELECT '75055-1' AS set_num, 1 AS owner_id, 1 AS owned, 1 AS wanted --Imperial Star Destroyer (1400 pieces)
	UNION
	SELECT '75190-1' AS set_num, 1 AS owner_id, 1 AS owned, 1 AS wanted --First Order Star Destroyer (1500 pieces)
END
GO