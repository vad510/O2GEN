USE [UFMDBLUK]
GO
/****** Object:  StoredProcedure [dbo].[PRC_UPDATE_DEFECT_COUNT]    Script Date: 04.02.2022 21:08:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[PRC_UPDATE_DEFECT_COUNT] 
	@SCId  BIGINT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DEFCOUNT INT;

	SET @DEFCOUNT = ISNULL((
	SELECT
    COUNT(*)
    FROM
    SchedulingContainers SC
    LEFT JOIN Tasks AS T ON T.SchedulingContainerId = SC.Id
    LEFT JOIN InspectionProtocols IP ON IP.TaskId = T.Id AND IP.IsDeleted <> 1
    LEFT JOIN InspectionProtocolItems IPI ON IPI.InspectionProtocolId = IP.Id
    LEFT JOIN Assets A ON A.Id = IP.AssetId
    LEFT JOIN Assets AC ON AC.Id = IPI.AssetId
    LEFT JOIN AssetParameterValues APV ON APV.Id = IPI.AssetParameterValueId
    LEFT JOIN AssetParameters AP ON AP.Id = APV.AssetParameterId
    LEFT JOIN Engineers E on E.Id = APV.ModifiedByUser
    WHERE SC.Id = @SCId
    AND APV.Value IS NOT NULL
    AND (
        --Цифровой
        (
            APV.AssetParameterTypeId <> 2
            AND
            (
                --Cильное отклонение
                (
                    TRY_CONVERT(float, REPLACE(APV.Value,',','.'))
                    between TRY_CONVERT(float, REPLACE(AP.BottomValue3,',','.'))
                    AND TRY_CONVERT(float, REPLACE(AP.TopValue3,',','.'))
                )
                --Отклонение
                OR
                (
                    TRY_CONVERT(float, REPLACE(APV.Value,',','.'))
                    between TRY_CONVERT(float, REPLACE(AP.BottomValue2,',','.'))
                    AND TRY_CONVERT(float, REPLACE(AP.TopValue2,',','.'))
                )
            )
            AND
            NOT (
                TRY_CONVERT(float, REPLACE(APV.Value,',','.'))
                between TRY_CONVERT(float, REPLACE(AP.BottomValue1,',','.'))
                AND TRY_CONVERT(float, REPLACE(AP.TopValue1,',','.'))
            )

        )
        OR
        (APV.AssetParameterTypeId = 2 AND APV.Value IN (N'1', N'2'))
    )),0);

	UPDATE SchedulingContainers SET DefectCount = @DEFCOUNT WHERE Id = @SCId;
END
