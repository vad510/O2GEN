SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		MBezruchenko
-- Create date: 20220307
-- Description:	Удаление обхода
-- =============================================
CREATE PROCEDURE [dbo].[PRC_DELETE_SC]
	 @ID BIGINT,
	 @EngId BIGINT
AS
BEGIN
	SET NOCOUNT ON;

    DECLARE @SRRev bigint, @SCRev bigint, @TRev bigint, @IDRev bigint, @IPRev bigint, @IPIRev bigint, @APVRev bigint, @ARev bigint; 

    SET @SRRev = (isnull((SELECT max(revision) id FROM SchedulingRequirements), 0) + 1); 
    SET @SCRev = (isnull((SELECT max(revision) id FROM SchedulingContainers), 0) + 1); 
    SET @TRev = (isnull((SELECT max(revision) id FROM Tasks), 0) + 1); 
    SET @IDRev = (isnull((SELECT max(revision) id FROM InspectionDocuments), 0) + 1); 
    SET @IPRev = (isnull((SELECT max(revision) id FROM InspectionProtocols), 0) + 1); 
    SET @IPIRev = (isnull((SELECT max(revision) id FROM InspectionProtocolItems), 0) + 1); 
    SET @APVRev = (isnull((SELECT max(revision) id FROM AssetParameterValues), 0) + 1); 
    SET @ARev = (isnull((SELECT max(revision) id FROM Assignments), 0) + 1); 

    UPDATE SchedulingRequirements SET 
    DeletedByUser = @EngId, 
    DeletionTime = getdate(), 
    IsDeleted = 1,
    Revision = @SRRev 
    WHERE ID in (SELECT RequirementId FROM SchedulingContainers WHERE ID = @ID); 

    UPDATE SchedulingContainers SET
    DeletedByUser = @EngId,
    DeletionTime = getdate(),
    IsDeleted = 1,
    Revision = @SCRev
    WHERE ID = @ID;

    UPDATE Tasks SET
    DeletedByUser = @EngId,
    DeletionTime = getdate(),
    IsDeleted = 1,
    Revision = @TRev
    WHERE SchedulingContainerId = @ID;

    UPDATE Assignments SET
    DeletedByUser = @EngId,
    DeletionTime = getdate(),
    IsDeleted = 1,
    Revision = @ARev
    WHERE SchedulingContainerId = @ID;

    UPDATE InspectionDocuments SET
    DeletedByUser = @EngId,
    DeletionTime = getdate(),
    IsDeleted = 1,
    Revision = @IDRev
    WHERE TaskId in (SELECT Id FROM Tasks WHERE SchedulingContainerId = @ID);


    UPDATE PPEntityCollections SET Revision = @SRRev WHERE ID = 105;
    UPDATE PPEntityCollections SET Revision = @SCRev WHERE ID = 104;
    UPDATE PPEntityCollections SET Revision = @TRev WHERE ID = 116;
    UPDATE PPEntityCollections SET Revision = @IDRev WHERE ID = 42;
    UPDATE PPEntityCollections SET Revision = @ARev WHERE ID = 14;
END
GO
