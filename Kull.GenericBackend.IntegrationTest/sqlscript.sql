﻿CREATE TABLE dbo.Pets (PetId int PRIMARY KEY , PetName varchar(100), IsNice bit)
GO
CREATE TABLE dbo.TestDbVersion(VersionNr int)
GO
INSERT INTO dbo.TestDbVersion(VersionNr) VALUES(1)
GO
INSERT INTO dbo.Pets(PetId, PetName, IsNice)
SELECT 1, 'Dog', 0
UNION ALL 
SELECT 2, 'Dog 2', 1
GO
CREATE PROCEDURE spGetPets
	@OnlyNice bit=0
AS
BEGIN
	SELECT * FROM dbo.Pets
		WHERE IsNice=1 OR @OnlyNice=0
END
GO
CREATE PROCEDURE spGetPet
	@Petid int
AS
BEGIN
	SELECT * FROM dbo.Pets WHERE PetId=@PetId
END
GO
CREATE PROCEDURE spAddPet
	@PetName nvarchar(1000),
	@IsNice bit
AS
BEGIN
	-- Just pretending
	SELECT CONVERT(BIT,1) AS Success, 3 AS NewPetId
END
GO
CREATE PROCEDURE spDeletePet
	@Petid int
AS
BEGIN
	-- Just pretending
	SELECT CONVERT(BIT,1) AS Success
END
GO
CREATE PROCEDURE spSearchPets
	@SearchString nvarchar(MAX)
AS
BEGIN 	
	SELECT* FROM Pets WHERE PetName LIKE '%' + @SearchString + '%'
END