USE [UFMDBLUK]
GO
/****** Object:  UserDefinedFunction [dbo].[TBFN_GET_STATISTICS]    Script Date: 25.02.2022 21:08:32 ******/
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
	(12),
	(29),
	(30),
	(31),
	(18)

	DECLARE @Temp TABLE(DeptId INT, DeptName NVARCHAR(MAX), [Value] NVARCHAR(MAX))
	
	INSERT INTO @Result
	SELECT 
	D.DisplayName AS Name, 
	(SELECT COUNT(*) 
		FROM SchedulingContainers SC 
		WHERE SC.DepartmentId IN (SELECT Id FROM TBFN_GET_DEPARTMENTS(D.Id)) 
		AND SC.StartTime BETWEEN @FromD AND @ToD 
		AND SC.SCStatusId = 3 
		AND SC.IsDeleted <> 1) AS Value 
	FROM Departments D 
	WHERE D.IsDeleted <> 1
	AND D.Id NOT IN (SELECT ID FROM @IgnoreDepts)
	AND D.Id IN (SELECT Id FROM TBFN_GET_DEPARTMENTS(@DeptId));

	RETURN 
END
