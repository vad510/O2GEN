USE [UFMDBLUK]
GO
/****** Object:  UserDefinedFunction [dbo].[TBFN_GET_STATISTICS]    Script Date: 04.02.2022 21:08:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		MBezruchenko
-- Create date: 20220204
-- Description:	Получение статистики пройденых маршрутов по указаным параметрам
-- =============================================
ALTER FUNCTION [dbo].[TBFN_GET_STATISTICS]
(
	--Указатель на подразделение
	@DeptId bigint = 0,
	@FromD datetime,
	@ToD datetime
)
RETURNS 
@Result TABLE 
(
	[Name] NVARCHAR(MAX),
	[Value] NVARCHAR(MAX)
)
AS
BEGIN
	
	--Невыводимые подразделения
	DECLARE @IgnoreDepts TABLE (ID INT);
	INSERT INTO @IgnoreDepts (ID) VALUES
	(3 ),
	(4 ),
	(5 ),
	(6 ),
	(7 ),
	(8 ),
	(9 ),
	(10),
	(11),
	(12)

	DECLARE @Temp TABLE(DeptId INT, DeptName NVARCHAR(MAX), [Value] NVARCHAR(MAX))
	
	INSERT INTO @Temp (DeptId, DeptName,[Value])
	SELECT 
	D.Id,
	D.DisplayName AS Name, 
	(SELECT COUNT(*) 
		FROM SchedulingContainers SC 
		WHERE SC.DepartmentId IN (SELECT Id FROM TBFN_GET_DEPARTMENTS(D.Id)) 
		AND SC.StartTime BETWEEN @FromD AND @ToD 
		AND SC.SCStatusId = 3 
		AND SC.IsDeleted <> 1) AS Value 
	FROM Departments D 
	WHERE D.IsDeleted <> 1
	ORDER BY D.DisplayName

	INSERT INTO @Result 
	SELECT DeptName, [Value] FROM @Temp WHERE DeptId NOT IN (SELECT ID FROM @IgnoreDepts) AND (@DeptId = 0 OR @DeptId IS NULL OR DeptId IN (SELECT Id FROM TBFN_GET_DEPARTMENTS(@DeptId)))

	RETURN 
END
