USE [UFMDBLUK]
GO
/****** Object:  UserDefinedFunction [dbo].[TBFN_GET_DEPARTMENTS]    Script Date: 04.02.2022 21:07:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER FUNCTION [dbo].[TBFN_GET_DEPARTMENTS]
(@RootDeptId BIGINT)
RETURNS 
@Deps TABLE 
(
	Id bigint, 
	DisplayName nvarchar(max),
	ParentId bigint
)
AS
BEGIN
	IF(@RootDeptId IS NULL OR @RootDeptId < 1 OR @RootDeptId = 27)
	BEGIN
		;WITH cte AS
		(
			SELECT     *
			FROM       Departments
			WHERE      ParentId IS NULL AND IsDeleted = 0
			UNION ALL
			SELECT     d.*
			FROM       Departments d
			INNER JOIN cte ON cte.Id = d.ParentId
			WHERE d.IsDeleted = 0
		)
		INSERT INTO @Deps (Id, DisplayName, ParentId)
		SELECT Id, DisplayName, ParentId FROM cte
	END
	ELSE
	BEGIN
		;WITH cte AS
		(
			SELECT     *
			FROM       Departments
			WHERE      Id = @RootDeptId AND IsDeleted = 0
			UNION ALL
			SELECT     d.*
			FROM       Departments d
			INNER JOIN cte ON cte.Id = d.ParentId
			WHERE d.IsDeleted = 0
		)
		INSERT INTO @Deps (Id, DisplayName, ParentId)
		SELECT Id, DisplayName, ParentId FROM cte
	END

	RETURN 
END
