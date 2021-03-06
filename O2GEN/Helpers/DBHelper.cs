using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using O2GEN.Enums;
using O2GEN.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace O2GEN.Helpers
{
    public static class DBHelper
    {

        private static string ENCR = "02GD89htgGbWEp98ubrg9oies:DGH:ORGojhg89_@jog;SOgh";
        private static string DBConnection = "Data Source=10.201.192.241;Initial Catalog=UFMDBLUK;User ID=sa;Password=Zz123456789;";

        private static string GetConnectionString()
        {
            switch (Environment.UserName.ToLower())
            {
                case "michael":
                    return "Server=DESKTOP-JFNR95O\\SQLEXPRESS;Initial Catalog=UFMDBLUK;Integrated Security=SSPI";
                case "е":
                    return "Server=DESKTOP-IO142CD\\SQLEXPRESS;Initial Catalog=UFMDBLUK;Integrated Security=SSPI";
                case "ev510":
                    return "Server=DESKTOP-A17N4G7\\SQLEXPRESS01;Initial Catalog=UFMDBLUK;Integrated Security=SSPI";
                default:
                    return DBConnection;
            }
        }

        #region Тексты запросов

        #region Обходы
        private static string GetZRP(int[] statuses, long? DeptId, long UserDept)
        {
            string statusSQL = "";
            if(statuses!= null)
            {

                for (int i = 0; i < statuses.Length; i++)
                {
                    if (!string.IsNullOrEmpty(statusSQL))
                        statusSQL += ", ";
                    statusSQL += (int)(statuses[i]);
                }
            }

            return "" +
                "SELECT " +
                "[e].[Id], " +
                "[e].[CloseTime], " +
                "[e].[StartTime], " +
                "[t0].[Id] as [ObjId], " +
                "[t0].[DisplayName] as " +
                "[ObjName], [t0].[ObjectUID],  " +
                "[t5].[Id] as [TypeId], " +
                "[t5].[DisplayName] as [TypeName], " +
                "[t6].[Id] as [StatusId], " +
                "[t6].[DisplayName] as [StatusName], " +
                "[t7].[AppointmentFinish], " +
                "[t7].[AppointmentStart], " +
                "[t8].[Name] as [RouteName], " +
                "[t10].[DisplayName] as [ResName], " +
                "Eng.DisplayName AS PerformerName, " +
                "CASE WHEN [e].DefectCount = 0 THEN N'Нет' ELSE CAST([e].DefectCount AS nvarchar(5))  END DefectCount " +
                "FROM [SchedulingContainers] AS [e] " +
                "LEFT JOIN( " +
                "    SELECT[e0].* " +
                "   FROM[SCReasons] AS[e0] " +
                "    WHERE([e0].[IsDeleted] <> 1) AND([e0].[TenantId] = CAST(1 AS bigint)) " +
                ") AS[t] ON[e].[SCReasonId] = [t].[Id] " +
                "LEFT JOIN( " +
                "    SELECT[e1].* " +
                "   FROM[Departments] AS[e1] " +
                "    WHERE([e1].[IsDeleted] <> 1) AND([e1].[TenantId] = CAST(1 AS bigint)) " +
                ") AS[t0] ON[e].[DepartmentId] = [t0].[Id] " +
                "LEFT JOIN( " +
                "    SELECT[e6].* " +
                "   FROM[SCTypes] AS[e6] " +
                "    WHERE([e6].[IsDeleted] <> 1) AND([e6].[TenantId] = CAST(1 AS bigint)) " +
                ") AS[t5] ON[e].[SCTypeId] = [t5].[Id] " +
                "LEFT JOIN( " +
                "    SELECT[e7].* " +
                "   FROM[SCStatuses] AS[e7] " +
                "    WHERE([e7].[IsDeleted] <> 1) AND([e7].[TenantId] = CAST(1 AS bigint)) " +
                ") AS[t6] ON[e].[SCStatusId] = [t6].[Id] " +
                "LEFT JOIN( " +
                "    SELECT[e8].* " +
                "   FROM[SchedulingRequirements] AS[e8] " +
                "    WHERE[e8].[IsDeleted] <> 1 " +
                ") AS[t7] ON[e].[RequirementId] = [t7].[Id] " +
                "LEFT JOIN( " +
                "    SELECT[e8].* " +
                "   FROM[InspectionDocuments] AS[e8] " +
                "    WHERE([e8].[IsDeleted] <> 1)  " +
                ") AS[t8] on[e].[Id] = [t8].[TaskId] " +
                "LEFT JOIN Engineers AS Eng ON Eng.Id = [t8].EngineerId " +
                "LEFT JOIN( " +
                "    SELECT[e9].* " +
                "   FROM[Assignments] AS[e9] " +
                "    WHERE([e9].[IsDeleted] <> 1)  " +
                ") AS[t9] on[t9].[SchedulingContainerId] = [e].[Id] " +
                "LEFT JOIN( " +
                "    SELECT[e10].* " +
                "   FROM[Resources] AS[e10] " +
                "    WHERE([e10].[IsDeleted] <> 1) AND([e10].[TenantId] = CAST(1 AS bigint)) " +
                ") AS[t10] ON[t9].[ResourceId] = [t10].[Id] " +
                $"INNER JOIN TBFN_GET_DEPARTMENTS({UserDept}) dps ON dps.Id = e.DepartmentId " +
                $"WHERE (@DisplayName IS NULL OR [e].[Id] LIKE N'%' + @DisplayName + N'%' OR Eng.DisplayName LIKE N'%' + @DisplayName + N'%' OR [t8].[Name] LIKE N'%' + @DisplayName + N'%') AND {(statuses!= null && statuses.Length > 0 ? $"[e].[SCStatusId] in ({statusSQL}) AND " : "")}  (([e].[IsDeleted] <> 1) AND([e].[TenantId] = CAST(1 AS bigint))) AND(([t7].[DepartmentId] IS NOT NULL {(DeptId != null ? $"AND[t7].[DepartmentId] = {DeptId}" : "")} ) AND(CASE " +
                "WHEN EXISTS( " +
                "   SELECT 1 " +
                "   FROM[Assignments] AS[e9] " +
                "   WHERE(([e9].[IsDeleted] <> 1) AND([e9].[TenantId] = CAST(1 AS bigint))) AND([e].[Id] = [e9].[SchedulingContainerId])) " +
                "    THEN CASE " +
                "        WHEN EXISTS( " +
                "            SELECT 1 " +
                "            FROM[Assignments] AS[e10] " +
                "            WHERE((([e10].[IsDeleted] <> 1) AND([e10].[TenantId] = CAST(1 AS bigint))) AND(([e10].[Start] < @DTo) AND([e10].[Finish] > @DFrom))) AND([e].[Id] = [e10].[SchedulingContainerId])) " +
                "        THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) " +
                "    END ELSE CASE " +
                "        WHEN([t7].[AppointmentStart] < @DTo) AND([t7].[AppointmentFinish] > @DFrom) " +
                "        THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) " +
                "    END " +
                "END = 1)) " +
                "ORDER BY[t7].[AppointmentStart] DESC";
        }

        private static string GetZRP()
        {
            return "" +
                "SELECT [e].[Id], [e].[ObjectUID], [e].[StartTime], [e].[CloseTime], [e].[DepartmentId], [e].[SCTypeId], [e].[SCStatusId], [SR].[RequirementResourceId], [IDoc].[Name] " +
                "FROM [SchedulingContainers] AS[e] " +
                "LEFT JOIN [SchedulingRequirements] AS[SR] ON [SR].[IsDeleted] <> 1 AND [e].[RequirementId] = [SR].[Id] " +
                "LEFT JOIN [Tasks] AS [T] ON [T].[IsDeleted] <> 1 AND [e].[Id] = [T].[SchedulingContainerId] " +
                "LEFT JOIN [InspectionDocuments] AS [IDoc] ON [IDoc].[IsDeleted] <> 1 AND [T].[Id] = [IDoc].[TaskId] " +
                $"WHERE [e].[Id] = @Id AND [e].[IsDeleted] <> 1 AND [e].[TenantId] = CAST(1 AS bigint) ";
        }

        /// <summary>
        /// Вставка Контроля
        /// </summary>
        /// <returns></returns>
        private static string CreateZRP(ZRP obj, long EngId)
        {
            //SchedulingRequirements                    +
            //Tasks                                     +
            //  InspectionDocuments                     + EngineerId пользователь занятый обходом.
            //      InspectionProtocols                 +
            //      AssetParameterValues                +
            //          InspectionProtocolItems         
            //  SchedulingContainers                    +

            return "DECLARE @SRRev bigint, @SCRev bigint, @TRev bigint, @IDRev bigint, @IPRev bigint, @IPIRev bigint, @APVRev bigint, @ARev bigint; " +

            "SET @SRRev = (isnull((SELECT max(revision) id FROM SchedulingRequirements), 0) + 1); " +
            "SET @SCRev = (isnull((SELECT max(revision) id FROM SchedulingContainers), 0) + 1); " +
            "SET @TRev = (isnull((SELECT max(revision) id FROM Tasks), 0) + 1); " +
            "SET @IDRev = (isnull((SELECT max(revision) id FROM InspectionDocuments), 0) + 1); " +
            "SET @IPRev = (isnull((SELECT max(revision) id FROM InspectionProtocols), 0) + 1); " +
            "SET @IPIRev = (isnull((SELECT max(revision) id FROM InspectionProtocolItems), 0) + 1); " +
            "SET @APVRev = (isnull((SELECT max(revision) id FROM AssetParameterValues), 0) + 1); " +
            "SET @ARev = (isnull((SELECT max(revision) id FROM Assignments), 0) + 1); " +

            "DECLARE @SRId TABLE(id bigint); " +
            "DECLARE @SCId TABLE(id bigint); " +
            "DECLARE @TId TABLE(id bigint); " +
            "DECLARE @IDId TABLE(id bigint); " +
            "DECLARE @IPIdAIdPair TABLE(id bigint, assetid bigint); " +
            "DECLARE @IPIId TABLE(id bigint); " +
            "DECLARE @APVId TABLE(id bigint, assetid bigint, assetparameterid bigint); " +

            "INSERT INTO SchedulingRequirements " +
            "(IsDeleted, " +
            "Revision, " +
            "CreatedByUser, " +
            "CreationTime, " +
            "ObjectUID, " +
            "DepartmentId, " +
            "RequirementResourceId, " +
            "EarlyStart, " +
            "DueDate, " +
            "AppointmentStart, " +
            "AppointmentFinish) " +
            "OUTPUT inserted.Id into @SRId " +
            "VALUES " +
            "(0, " +
            "@SRRev, " +
            $"{EngId}, " +
            "getdate(), " +
            $"'{Guid.NewGuid().ToString("D")}', " +
            $"{ obj.DepartmentID}, " +
            $"{ obj.ResourceId}, " +
            $"'{ obj.StartTime.ToString("yyyyMMdd HH:mm:ss")}', " +
            $"'{ obj.StartTime.AddDays(1).ToString("yyyyMMdd HH:mm:ss")}', " +
            $"'{ obj.StartTime.ToString("yyyyMMdd HH:mm:ss")}', " +
            $"'{ obj.StartTime.AddDays(1).ToString("yyyyMMdd HH:mm:ss")}'); " +

            "INSERT INTO SchedulingContainers " +
            "(IsDeleted, " +
            "Revision, " +
            "CreatedByUser, " +
            "CreationTime, " +
            "ObjectUID, " +
            "SCStatusId, " +
            "SCTypeId, " +
            "RequirementId, " +
            "TenantId, " +
            "StartTime, " +
            "CloseTime, " +
            "DepartmentId) " +
            "OUTPUT inserted.Id into @SCId " +
            "VALUES " +
            "(0, " +
            "@SCRev, " +
            $"{EngId}, " +
            "getdate(), " +
            $"'{Guid.NewGuid().ToString("D")}', " +
            $"1, " +
            $"{obj.SCTypeId}, " +
            "(SELECT TOP 1 id FROM @SRId), " +
            "1, " +
            $"'{ obj.StartTime.ToString("yyyyMMdd HH:mm:ss")}', " +
            $"'{ obj.StartTime.AddDays(1).ToString("yyyyMMdd HH:mm:ss")}', " +
            $"{ obj.DepartmentID}); " +

            "INSERT INTO Tasks " +
            "(IsDeleted, " +
            "Revision, " +
            "CreatedByUser, " +
            "CreationTime, " +
            "ObjectUID, " +
            "SchedulingContainerId, " +
            "TaskStatusId, " +
            "TenantId, " +
            "SR_Duration, " +
            "UseContractor, " +
            "UseContractorVehicle) " +
            "OUTPUT inserted.Id into @TId " +
            "VALUES " +
            "(0, " +
            "@TRev, " +
            $"{EngId}, " +
            "getdate(), " +
            $"'{Guid.NewGuid().ToString("D")}', " +
            "(SELECT TOP 1 id FROM @SCId), " +
            "1, " +
            "1, " +
            $"{ obj.StartTime.AddDays(1).Subtract(obj.StartTime).TotalMilliseconds}, " +
            "0, " +
            "0); " +

            "INSERT INTO Assignments " +
            "(IsDeleted, " +
            "Revision, " +
            "CreatedByUser, " +
            "CreationTime, " +
            "ObjectUID, " +
            "SchedulingContainerId, " +
            "TaskId, " +
            "ResourceId, " +
            "Start, " +
            "Finish, " +
            "TenantId) " +
            "VALUES " +
            "(0, " +
            "@ARev, " +
            $"{EngId}, " +
            "getdate(), " +
            $"'{Guid.NewGuid().ToString("D")}', " +
            $"(SELECT TOP 1 id FROM @SCId), " +
            $"(SELECT TOP 1 id FROM @TId), " +
            $"{obj.ResourceId}, " +
            $"'{ obj.StartTime.ToString("yyyyMMdd HH:mm:ss")}', " +
            $"'{ obj.StartTime.AddDays(1).ToString("yyyyMMdd HH:mm:ss")}', " +
            "1); " +

            "INSERT INTO InspectionDocuments " +
            "(IsDeleted, " +
            "Revision, " +
            "CreatedByUser, " +
            "CreationTime, " +
            "CreationDateTime, " +
            "InspectionDateTime, " +
            "ObjectUID, " +
            "Name, " +
            "DepartmentId, " +
            "TaskId) " +
            "OUTPUT inserted.Id into @IDId " +
            "VALUES " +
            "(0, " +
            "@IDRev, " +
            $"{EngId}, " +
            "getdate(), " +
            "getdate(), " +
            "getdate(), " +
            $"'{Guid.NewGuid().ToString("D")}', " +
            $"N'{obj.RouteName}', " +
            $"{obj.DepartmentID}, " +
            "(SELECT TOP 1 id FROM @TId)); " +


            $"UPDATE PPEntityCollections SET Revision = @SRRev WHERE ID = {(int)RevEntry.SchedulingRequirement}; " +
            $"UPDATE PPEntityCollections SET Revision = @SCRev WHERE ID = {(int)RevEntry.SchedulingContainer}; " +
            $"UPDATE PPEntityCollections SET Revision = @TRev WHERE ID = {(int)RevEntry.Task}; " +
            $"UPDATE PPEntityCollections SET Revision = @IDRev WHERE ID = {(int)RevEntry.InspectionDocument}; " +
            $"UPDATE PPEntityCollections SET Revision = @ARev WHERE ID = {(int)RevEntry.Assignment}; " +
            $"SELECT (SELECT TOP 1 id FROM @SRId) AS SRID, (SELECT TOP 1 id FROM @TId) AS TID, (SELECT TOP 1 id FROM @IDId) AS IDocID, (SELECT TOP 1 id FROM @SCId) AS SCID;";
        }
        private static string CreateInspectionProtocols(ZRP obj, long EngId, long TaskId, long IDocId)
        {

            string InspectionProtocols = "DECLARE @IPRev bigint; " +
                "SET @IPRev = (isnull((SELECT max(revision) id FROM InspectionProtocols), 0) + 1); " +

                "DECLARE @IPIdAIdPair TABLE(id bigint, assetid bigint); " +

                "INSERT INTO InspectionProtocols " +
                "(IsDeleted, " +
                "Revision, " +
                "CreatedByUser, " +
                "CreationTime, " +
                "ObjectUID, " +
                "Name, " +
                "InspectionDocumentId, " +
                "InspectionProtocolStatusId, " +
                "DepartmentId, " +
                "AssetId, " +
                "TaskId, " +
                "InspectionDateTime) " +
                "OUTPUT inserted.Id, inserted.AssetId into @IPIdAIdPair " +
                "VALUES ";
            for (int i = 0; i < obj.NewTechPoz.Count; i++)
            {
                if (i > 0) InspectionProtocols += ",";
                InspectionProtocols +=
                    "(" +
                    "0, " +
                    "@IPRev, " +
                    $"{EngId}, " +
                    $"getdate(), " +
                    $"'{Guid.NewGuid().ToString("D")}', " +
                    $"N'{obj.NewTechPoz[i].DisplayName}', " +
                    $"{IDocId}, " +
                    $"NULL, " +
                    $"{obj.DepartmentID}, " +
                    $"{obj.NewTechPoz[i].Id}, " +
                    $"{TaskId}, " +
                    $"getdate())";
            }
            InspectionProtocols += $"; UPDATE PPEntityCollections SET Revision = @IPRev WHERE ID = {(int)RevEntry.InspectionProtocol}; " +
                "SELECT id AS IPItemId, AssetId FROM @IPIdAIdPair;";
            return InspectionProtocols;
        }
        private static string CreateAssetParameters(ZRP obj, long EngId, Dictionary<long, long> IPData)
        {
            string SQLReq = "";
            for (int i = 0; i < obj.NewTechPoz.Count; i++)
            {
                for (int j = 0; j < obj.NewTechPoz[i].Childs.Count; j++)
                {
                    for (int k = 0; k < obj.NewTechPoz[i].Childs[j].Childs.Count; k++)
                    {
#warning Есть  вероятность краша на вставке 1000+ записей, переделать на insert select union select
                        if (!string.IsNullOrEmpty(SQLReq))
                        {
                            SQLReq += ",";
                        }
                        SQLReq +=
                            "(" +
                            "0, " +
                            "@APVRev, " +
                            $"{EngId}, " +
                            $"getdate(), " +
                            $"(SELECT ObjectUID FROM AssetParameterPair WHERE AssetId = {obj.NewTechPoz[i].Childs[j].Id} AND AssetParameterId = {obj.NewTechPoz[i].Childs[j].Childs[k].Id} AND IsDeleted <> 1), " +
                            $"{obj.NewTechPoz[i].Childs[j].Id}, " +
                            $"{obj.NewTechPoz[i].Childs[j].Childs[k].Id}, " +
                            $"N'{obj.NewTechPoz[i].DisplayName} {obj.NewTechPoz[i].Childs[j].DisplayName}', " +
                            $"0, " +
                            $"0, " +
                            $"(SELECT TOP 1 ValueType FROM AssetParameters WHERE Id = {obj.NewTechPoz[i].Childs[j].Childs[k].Id}), " +
                            $"0, " +
                            $"'{new DateTime(0).ToString("yyyyMMdd HH:mm:ss.fff")}', " +
                            $"{IPData[obj.NewTechPoz[i].Id]}, " +
                            $"(SELECT TOP 1 AssetParameterTypeId FROM AssetParameters WHERE Id = {obj.NewTechPoz[i].Childs[j].Childs[k].Id}))";
                    }
                }
            }
            SQLReq = "DECLARE @APVRev bigint; " +
                "SET @APVRev = (isnull((SELECT max(revision) id FROM AssetParameterValues), 0) + 1); " +

                "DECLARE @APVData TABLE(ID bigint, AssetId bigint, AssetParameterId bigint, IPId bigint); " +

                "INSERT INTO AssetParameterValues " +
                "(IsDeleted, " +
                "Revision, " +
                "CreatedByUser, " +
                "CreationTime, " +
                "ObjectUID, " +
                "AssetId, " +
                "AssetParameterId, " +
                "Name, " +
                "Level, " +
                "Critical, " +
                "ValueType, " +
                "BoolValue, " +
                "DateTime, " +
                "InspectionProtocolId, " +
                "AssetParameterTypeId) " +

                "OUTPUT inserted.Id, inserted.AssetId, inserted.AssetParameterId, inserted.InspectionProtocolId into @APVData " +
                "VALUES " +
                SQLReq +
                $"; UPDATE PPEntityCollections SET Revision = @APVRev WHERE ID = {(int)RevEntry.AssetParameterValue}; " +
                "SELECT ID, AssetId, AssetParameterId, IPId FROM @APVData;";
            return SQLReq;
        }
        private static string CreateInspectionProtocolItems(ZRP obj, long EngId, Dictionary<long, long> IPData, List<AssetParametersRowData> APVData)
        {
            string SQLReq = "";
            for (int i = 0; i < obj.NewTechPoz.Count; i++)
            {
                for (int j = 0; j < obj.NewTechPoz[i].Childs.Count; j++)
                {
                    for (int k = 0; k < obj.NewTechPoz[i].Childs[j].Childs.Count; k++)
                    {
#warning Есть  вероятность краша на вставке 1000+ записей, переделать на insert select union select
                        if (!string.IsNullOrEmpty(SQLReq))
                        {
                            SQLReq += ",";
                        }
                        SQLReq +=
                            "(" +
                            "0, " +
                            "@IPIRev, " +
                            $"{EngId}, " +
                            $"getdate(), " +
                            $"'{Guid.NewGuid().ToString("D")}', " +
                            $"N'{obj.NewTechPoz[i].DisplayName} {obj.NewTechPoz[i].Childs[j].DisplayName}', " +
                            $"getdate(), " +
                            $"{obj.NewTechPoz[i].Childs[j].Id}, " +
                            $"{IPData[obj.NewTechPoz[i].Id]}, " +
                            $"{APVData.Find(x => x.IPId == IPData[obj.NewTechPoz[i].Id] && x.AssetId == obj.NewTechPoz[i].Childs[j].Id && x.AssetParameterId == obj.NewTechPoz[i].Childs[j].Childs[k].Id).Id}, " +
                            "0, " +
                            "0, " +
                            "0, " +
                            "0, " +
                            "0, " +
                            "0, " +
                            "0, " +
                            "0, " +
                            "0, " +
                            "0, " +
                            "0, " +
                            "0, " +
                            "0," +
                            $"N'{obj.NewTechPoz[i].DisplayName}')";
                    }
                }
            }
            SQLReq = "DECLARE @IPIRev bigint; " +
                "SET @IPIRev = (isnull((SELECT max(revision) id FROM InspectionProtocolItems), 0) + 1); " +

                "INSERT INTO InspectionProtocolItems " +
                "(IsDeleted, " +
                "Revision, " +
                "CreatedByUser, " +
                "CreationTime, " +
                "ObjectUID, " +
                "Name, " +
                "InspectionDateTime, " +
                "AssetId, " +
                "InspectionProtocolId, " +
                "AssetParameterValueId, " +
                "ValueLong1, " +
                "ValueLong2, " +
                "ValueLong3, " +
                "ValueLong4, " +
                "ValueLong5, " +
                "ValueLong6, " +
                "ValueDouble1, " +
                "ValueDouble2, " +
                "ValueDouble3, " +
                "ValueDouble4, " +
                "ValueDouble5, " +
                "ValueDouble6, " +
                "ValueBool3, " +
                "GroupString) VALUES " +
                SQLReq +
                $"; UPDATE PPEntityCollections SET Revision = @IPIRev WHERE ID = {(int)RevEntry.InspectionProtocolItem}; ";
            return SQLReq;
        }
        private static string UpdateZRP(ZRP obj, long EngId)
        {
            return "DECLARE @SRRev bigint, @SCRev bigint, @TRev bigint, @IDRev bigint, @IPRev bigint, @IPIRev bigint, @APVRev bigint, @ARev bigint; " +

            "SET @SRRev = (isnull((SELECT max(revision) id FROM SchedulingRequirements), 0) + 1); " +
            "SET @SCRev = (isnull((SELECT max(revision) id FROM SchedulingContainers), 0) + 1); " +
            "SET @TRev = (isnull((SELECT max(revision) id FROM Tasks), 0) + 1); " +
            "SET @IDRev = (isnull((SELECT max(revision) id FROM InspectionDocuments), 0) + 1); " +
            "SET @IPRev = (isnull((SELECT max(revision) id FROM InspectionProtocols), 0) + 1); " +
            "SET @IPIRev = (isnull((SELECT max(revision) id FROM InspectionProtocolItems), 0) + 1); " +
            "SET @APVRev = (isnull((SELECT max(revision) id FROM AssetParameterValues), 0) + 1); " +
            "SET @ARev = (isnull((SELECT max(revision) id FROM Assignments), 0) + 1); " +

            "UPDATE SchedulingRequirements SET " +
            $"ModifiedByUser = {EngId}, " +
            "ModificationTime = getdate(), " +
            "Revision = @SRRev, " +
            $"EarlyStart = '{obj.StartTime.ToString("yyyyMMdd HH:mm:ss")}', " +
            $"DueDate = '{obj.StartTime.AddDays(1).ToString("yyyyMMdd HH:mm:ss")}', " +
            $"AppointmentStart = '{obj.StartTime.ToString("yyyyMMdd HH:mm:ss")}', " +
            $"AppointmentFinish = '{obj.StartTime.AddDays(1).ToString("yyyyMMdd HH:mm:ss")}' " +
            $"WHERE ID in (SELECT RequirementId FROM SchedulingContainers WHERE ID = {obj.Id}); " +

            "UPDATE SchedulingContainers SET " +
            $"ModifiedByUser = {EngId}, " +
            "ModificationTime = getdate(), " +
            "Revision = @SCRev, " +
            $"SCStatusId = {obj.SCStatusId}, " +
            $"SCTypeId = {obj.SCTypeId}, " +
            $"StartTime = '{obj.StartTime.ToString("yyyyMMdd HH:mm:ss")}', " +
            $"CloseTime = '{obj.StartTime.AddDays(1).ToString("yyyyMMdd HH:mm:ss")}', " +
            $"DepartmentId = {obj.DepartmentID} " +
            $"WHERE ID = {obj.Id}; " +

            "UPDATE Tasks SET " +
            $"ModifiedByUser = {EngId}, " +
            "ModificationTime = getdate(), " +
            "Revision = @TRev, " +
            $"SR_Duration = {obj.StartTime.AddDays(1).Subtract(obj.StartTime).TotalMilliseconds} " +
            $"WHERE SchedulingContainerId = {obj.Id}; " +

            "UPDATE Assignments SET " +
            $"ModifiedByUser = {EngId}, " +
            "ModificationTime = getdate(), " +
            "Revision = @ARev, " +
            $"ResourceId = {obj.ResourceId}, " +
            $"Start = '{ obj.StartTime.ToString("yyyyMMdd HH:mm:ss")}', " +
            $"Finish = '{ obj.StartTime.AddDays(1).ToString("yyyyMMdd HH:mm:ss")}' " +
            $"WHERE SchedulingContainerId = {obj.Id}; " +

            "UPDATE InspectionDocuments SET " +
            $"ModifiedByUser = {EngId}, " +
            "ModificationTime = getdate(), " +
            "Revision = @IDRev, " +
            $"Name = N'{obj.RouteName}', " +
            $"DepartmentId = {obj.DepartmentID} " +
            $"WHERE TaskId in (SELECT Id FROM Tasks WHERE SchedulingContainerId = {obj.Id}); " +


            $"UPDATE PPEntityCollections SET Revision = @SRRev WHERE ID = {(int)RevEntry.SchedulingRequirement}; " +
            $"UPDATE PPEntityCollections SET Revision = @SCRev WHERE ID = {(int)RevEntry.SchedulingContainer}; " +
            $"UPDATE PPEntityCollections SET Revision = @TRev WHERE ID = {(int)RevEntry.Task}; " +
            $"UPDATE PPEntityCollections SET Revision = @IDRev WHERE ID = {(int)RevEntry.InspectionDocument}; " +
            $"UPDATE PPEntityCollections SET Revision = @ARev WHERE ID = {(int)RevEntry.Assignment}; ";
        }
        /// <summary>
        /// Удаление деталей обхода. 
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string DeleteInspectionProtocol()
        {
            return "DECLARE @IPRev bigint; " +
                "set @IPRev = (isnull((SELECT max(revision) id FROM InspectionProtocols ),0)+1); " +

                "UPDATE InspectionProtocols SET " +
                $"DeletedByUser = @EngId, " +
                "DeletionTime = getdate(), " +
                "Revision = @IPRev, " +
                "IsDeleted = 1 " +
                $"WHERE ID = @ID; " +

                $"UPDATE PPEntityCollections SET Revision = @IPRev WHERE ID = {(int)RevEntry.InspectionProtocol};";
        }
        private static string DeleteZRP()
        {


            return "DECLARE @SRRev bigint, @SCRev bigint, @TRev bigint, @IDRev bigint, @IPRev bigint, @IPIRev bigint, @APVRev bigint, @ARev bigint; " +

            "SET @SRRev = (isnull((SELECT max(revision) id FROM SchedulingRequirements), 0) + 1); " +
            "SET @SCRev = (isnull((SELECT max(revision) id FROM SchedulingContainers), 0) + 1); " +
            "SET @TRev = (isnull((SELECT max(revision) id FROM Tasks), 0) + 1); " +
            "SET @IDRev = (isnull((SELECT max(revision) id FROM InspectionDocuments), 0) + 1); " +
            "SET @IPRev = (isnull((SELECT max(revision) id FROM InspectionProtocols), 0) + 1); " +
            "SET @IPIRev = (isnull((SELECT max(revision) id FROM InspectionProtocolItems), 0) + 1); " +
            "SET @APVRev = (isnull((SELECT max(revision) id FROM AssetParameterValues), 0) + 1); " +
            "SET @ARev = (isnull((SELECT max(revision) id FROM Assignments), 0) + 1); " +

            "UPDATE SchedulingRequirements SET " +
            "DeletedByUser = @EngId, " +
            "DeletionTime = getdate(), " +
            "IsDeleted = 1," +
            "Revision = @SRRev " +
            "WHERE ID in (SELECT RequirementId FROM SchedulingContainers WHERE ID = @ID); " +

            "UPDATE SchedulingContainers SET " +
            $"DeletedByUser = @EngId, " +
            "DeletionTime = getdate(), " +
            "IsDeleted = 1," +
            "Revision = @SCRev " +
            "WHERE ID = @ID; " +

            "UPDATE Tasks SET " +
            "DeletedByUser = @EngId, " +
            "DeletionTime = getdate(), " +
            "IsDeleted = 1," +
            "Revision = @TRev " +
            "WHERE SchedulingContainerId = {ID}; " +

            "UPDATE Assignments SET " +
            "DeletedByUser = @EngId, " +
            "DeletionTime = getdate(), " +
            "IsDeleted = 1," +
            "Revision = @ARev " +
            "WHERE SchedulingContainerId = {ID}; " +

            "UPDATE InspectionDocuments SET " +
            "DeletedByUser = @EngId, " +
            "DeletionTime = getdate(), " +
            "IsDeleted = 1," +
            "Revision = @IDRev " +
            "WHERE TaskId in (SELECT Id FROM Tasks WHERE SchedulingContainerId = @ID); " +


            $"UPDATE PPEntityCollections SET Revision = @SRRev WHERE ID = {(int)RevEntry.SchedulingRequirement}; " +
            $"UPDATE PPEntityCollections SET Revision = @SCRev WHERE ID = {(int)RevEntry.SchedulingContainer}; " +
            $"UPDATE PPEntityCollections SET Revision = @TRev WHERE ID = {(int)RevEntry.Task}; " +
            $"UPDATE PPEntityCollections SET Revision = @IDRev WHERE ID = {(int)RevEntry.InspectionDocument}; " +
            $"UPDATE PPEntityCollections SET Revision = @ARev WHERE ID = {(int)RevEntry.Assignment}; ";
        }
        private static string GetInspectionProtocols()
        {
            return "SELECT IPs.Id, " +
            "IPs.ObjectUID,  " +
            "IPs.Name,  " +
            "CONCAT(IPI.Name, ' - ', AP.DisplayName) AS ItemName, " +
            "CASE WHEN APV.ModificationTime IS NULL THEN NULL ELSE APV.DateTime END AS ItemData, " +
            "CASE APV.AssetParameterTypeId " + //Визуальный
            "WHEN 2 THEN (CASE APV.Value WHEN N'0' THEN N'Норма' WHEN N'1' THEN N'Отклонение' WHEN N'2' THEN N'Сильное отклонение' ELSE N'' END) " +
            "ELSE APV.Value END AS ItemValue, " +
            "APV.Comment, " +
            "IPs.NFCReceived, " +
            "APV.IsPen, " +
            //Покраска текста
            "CASE APV.AssetParameterTypeId " + //Визуальный
            "WHEN " +
            "2 " +
            "THEN ( " +
                $"CASE APV.Value WHEN N'0' THEN {(int)TextColor.Green} " +
                $"WHEN N'1' THEN {(int)TextColor.Yellow} " +
                $"WHEN N'2' THEN {(int)TextColor.Red} " +
                $"ELSE {(int)TextColor.Default} END" +
            ") ELSE (" +
                "CASE WHEN " +
                "(" +
                    "TRY_CONVERT(float, REPLACE(APV.Value,',','.')) " +
                    "between TRY_CONVERT(float, REPLACE(AP.BottomValue1,',','.')) " +
                    "AND TRY_CONVERT(float, REPLACE(AP.TopValue1,',','.')) " +
                $") THEN {(int)TextColor.Green} " +
                $"WHEN " +
                //Cильное отклонение
                "(" +
                    "TRY_CONVERT(float, REPLACE(APV.Value,',','.')) " +
                    "between TRY_CONVERT(float, REPLACE(AP.BottomValue3,',','.')) " +
                    "AND TRY_CONVERT(float, REPLACE(AP.TopValue3,',','.')) " +
                $") THEN {(int)TextColor.Red} " +
                $"WHEN " +
                //Отклонение
                "(" +
                    "TRY_CONVERT(float, REPLACE(APV.Value,',','.')) " +
                    "between TRY_CONVERT(float, REPLACE(AP.BottomValue2,',','.')) " +
                    "AND TRY_CONVERT(float, REPLACE(AP.TopValue2,',','.')) " +
                $") THEN {(int)TextColor.Yellow} " +
                $"ELSE {(int)TextColor.Default} END) END " +
            "AS TextColor, " +
            "IPState.Id AS StateId, " +
            "ISNULL(IPState.DisplayName, N'Не указан') AS StateName " +
            "FROM InspectionProtocolItems AS IPI " +
            "INNER JOIN InspectionProtocols AS IPs on IPI.InspectionProtocolId = IPs.Id " +
            "INNER JOIN AssetParameterValues AS APV on IPI.AssetParameterValueId = APV.Id " +
            "INNER JOIN AssetParameters AS AP on AP.Id = APV.AssetParameterId " +
            "LEFT JOIN Assets AS A on A.Id = IPs.AssetId " +
            "LEFT JOIN InspectionProtocolStatuses AS IPState on IPState.Id = IPs.InspectionProtocolStatusId " +
            "WHERE InspectionDocumentId in (select id from InspectionDocuments where TaskId in (select id from Tasks where SchedulingContainerId = @SchedContID)) AND IPs.IsDeleted <> 1 " +
            "ORDER BY IPState.Id, IPs.Name ";
        }
        #endregion

        #region Статусы SC
        /// <summary>
        /// Статусы
        /// </summary>
        /// <returns></returns>
        private static string SelectSCStatuses()
        {
            return "SELECT id, DisplayName, ObjectUID FROM SCStatuses where IsDeleted <> 1 AND TenantId = CAST(1 AS bigint)";
        }
        #endregion

        #region Типы SC
        /// <summary>
        /// Типы
        /// </summary>
        /// <returns></returns>
        private static string SelectSCTypes()
        {
            return "SELECT id, DisplayName, ObjectUID FROM SCTypes where IsDeleted <> 1 AND TenantId = CAST(1 AS bigint)";
        }
        #endregion

        #region Календари
        /// <summary>
        /// Календари
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        private static string SelectCalendars()
        {
            return "SELECT id, Title, ObjectUID FROM PPCalendars where IsDeleted <> 1 AND TenantId = CAST(1 AS bigint)";
        }
        #endregion

        #region Типы ТО
        /// <summary>
        /// Типы ТО
        /// </summary>
        /// <returns></returns>
        private static string SelectTOTypes(int ID = -1)
        {
            return "SELECT Id, Name, DisplayName, ObjectUID " +
                "FROM AssetParameterSpecies " +
                "WHERE(IsDeleted <> 1) AND(TenantId = CAST(1 AS bigint)) " +
                $"{(ID == -1 ? "" : "AND Id = " + ID)} " +
                "ORDER BY Id";
        }
        /// <summary>
        /// Вставка типа ТО
        /// </summary>
        /// <param name="DisplayName"></param>
        /// <param name="ExternalId"></param>
        /// <param name="Name"></param>
        /// <param name="ObjectUID"></param>
        /// <returns></returns>
        private static string InsertToType(string DisplayName, string ExternalId, string Name, Guid ObjectUID)
        {
            return "INSERT INTO AssetParameterSpecie (DisplayName, ExternalId, IsDeleted, Name, ObjectUID, Revision, TenantId) " +
                $"VALUES (N'{DisplayName}', N'{ObjectUID.ToString("D")}', 0, N'{Name}', '{ObjectUID.ToString("D")}', (isnull((SELECT max(revision) id FROM AssetParameterSpecies ),0)+1), 1);";
        }
        #endregion

        #region Контроли
        /// <summary>
        /// Контроли
        /// </summary>
        /// <returns></returns>
        private static string SelectControls()
        {
            return "SELECT AP.Id, AP.DisplayName, AP.ObjectUID, AP.BottomValue1, AP.TopValue1, AP.BottomValue2, AP.TopValue2, AP.BottomValue3, AP.TopValue3, APT.Id AS AssetParameterTypeId, APT.Name AS ValueTypeName, AP.Description, AP.DepartmentId, AP.CreationTime, ISNULL(AP.ModificationTime, AP.CreationTime) AS ModificationTime, ISNULL(D.DisplayName, N'Не указано') AS DepartmentName " +
                "FROM AssetParameters AS AP " +
                "INNER JOIN AssetParameterTypes AS APT ON AP.AssetParameterTypeId = APT.Id " +
                "LEFT JOIN Departments D ON D.Id = AP.DepartmentId " + 
                "WHERE(AP.IsDeleted <> 1) AND(AP.TenantId = CAST(1 AS bigint)) AND " +
                "(@ID IS NULL OR AP.Id = @ID) AND " +
                "(@AssetParameterTypeId IS NULL OR AP.AssetParameterTypeId = @AssetParameterTypeId) AND " +
                "(@DisplayName IS NULL OR AP.DisplayName like N'%' + @DisplayName + N'%') AND " +
                "(@DeptId IS NULL OR AP.DepartmentId = @DeptId) " +
                "ORDER BY AP.DisplayName";
        }
        /// <summary>
        /// Вставка Контроля
        /// </summary>
        /// <returns></returns>
        private static string СreateControl(Control obj, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM AssetParameters ),0)+1);" +
                "INSERT INTO AssetParameters " +
                "(BottomValue1, " +
                "BottomValue2, " +
                "BottomValue3, " +
                "CreatedByUser, " +
                "CreationTime, " +
                "DisplayName, " +
                "ExternalId, " +
                "IsDeleted, " +
                "IsDynamic, " +
                "Name, " +
                "ObjectUID, " +
                "Revision, " +
                "SpanCount, " +
                "TenantId, " +
                "TopValue1, " +
                "TopValue2, " +
                "TopValue3, " +
                "ValueType, " +
                "Level1, " +
                "Critical1, " +
                "Level2, " +
                "Critical2, " +
                "Level3, " +
                "Critical3, " +
                "Level4, " +
                "Critical4, " +
                "AssetParameterTypeId, " +
                "DepartmentId," +
                "Description) " +

                $"VALUES " +

                $"({(string.IsNullOrEmpty(obj.ValueBottom1) ? "NULL" : $"N'{obj.ValueBottom1.Replace(',', '.')}'")}, " +
                $"{(string.IsNullOrEmpty(obj.ValueBottom2) ? "NULL" : $"N'{obj.ValueBottom2.Replace(',', '.')}'")}, " +
                $"{(string.IsNullOrEmpty(obj.ValueBottom3) ? "NULL" : $"N'{obj.ValueBottom3.Replace(',', '.')}'")}, " +
                $"{EngId} , " +
                $"getdate(), " +
                $"N'{obj.DisplayName}', " +
                $"N'{obj.ObjectUID.ToString("D")}', " +
                $"0, " +
                $"0, " +
                $"N'{obj.DisplayName}', " +
                $"'{obj.ObjectUID.ToString("D")}', " +
                $"@revision, " +
                $"0, " +
                $"1, " +
                $"{(string.IsNullOrEmpty(obj.ValueTop1) ? "NULL" : $"N'{obj.ValueTop1.Replace(',', '.')}'")}, " +
                $"{(string.IsNullOrEmpty(obj.ValueTop2) ? "NULL" : $"N'{obj.ValueTop2.Replace(',', '.')}'")}, " +
                $"{(string.IsNullOrEmpty(obj.ValueTop3) ? "NULL" : $"N'{obj.ValueTop3.Replace(',', '.')}'")}, " +
                $"'{GetControlType((long)obj.AssetParameterTypeId)}', " +
                $"0, " +
                $"0, " +
                $"0, " +
                $"0, " +
                $"0, " +
                $"0, " +
                $"0, " +
                $"0, " +
                $"{obj.AssetParameterTypeId}, " +
                $"{(obj.DepartmentId==null?"NULL":obj.DepartmentId)}, " +
                $"N'{obj.Description}'); " +
                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = 5;";
        }
        private static string UpdateControl(Control obj, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM AssetParameters ),0)+1); " +
                "UPDATE AssetParameters SET " +
                $"BottomValue1 = {(string.IsNullOrEmpty(obj.ValueBottom1) ? "NULL" : $"N'{obj.ValueBottom1.Replace(',', '.')}'")}, " +
                $"BottomValue2 = {(string.IsNullOrEmpty(obj.ValueBottom2) ? "NULL" : $"N'{obj.ValueBottom2.Replace(',', '.')}'")}, " +
                $"BottomValue3 = {(string.IsNullOrEmpty(obj.ValueBottom3) ? "NULL" : $"N'{obj.ValueBottom3.Replace(',', '.')}'")}, " +
                $"ModifiedByUser = {EngId}, " +
                "ModificationTime = getdate(), " +
                $"DisplayName = N'{obj.DisplayName}', " +
                $"Name = N'{obj.DisplayName}', " +
                "Revision = @revision, " +
                $"TopValue1 = {(string.IsNullOrEmpty(obj.ValueTop1) ? "NULL" : $"N'{obj.ValueTop1.Replace(',', '.')}'")}, " +
                $"TopValue2 = {(string.IsNullOrEmpty(obj.ValueTop2) ? "NULL" : $"N'{obj.ValueTop2.Replace(',', '.')}'")}, " +
                $"TopValue3 = {(string.IsNullOrEmpty(obj.ValueTop3) ? "NULL" : $"N'{obj.ValueTop3.Replace(',', '.')}'")}, " +
                $"ValueType = '{GetControlType((long)obj.AssetParameterTypeId)}', " +
                $"AssetParameterTypeId = {obj.AssetParameterTypeId}, " +
                $"DepartmentId = {(obj.DepartmentId==null?"NULL": obj.DepartmentId)}, " +
                $"Description = N'{obj.Description}' " +
                $"WHERE ID = {obj.Id}; " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.AssetParameter};";
        }
        private static string DeleteControl(int ID, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM AssetParameters ),0)+1); " +
                "UPDATE AssetParameters SET " +
                $"DeletedByUser = {EngId}, " +
                "DeletionTime = getdate(), " +
                "Revision = @revision, " +
                "IsDeleted = 1 " +
                $"WHERE ID = {ID}; " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.AssetParameter}; ";
        }

        /// <summary>
        /// Типы контролей
        /// </summary>
        /// <returns></returns>
        private static string SelectAssetParameterTypes()
        {
            return $"SELECT Id, Name " +
                $"FROM AssetParameterTypes " +
                $"where IsDeleted <> 1; ";
        }
        /// <summary>
        /// Данные для графика контроля
        /// </summary>
        /// <returns></returns>
        private static string SelectControlStatistic(long? DepartmentId, long? AssetParameterSetId, long? AssetId, long? AssetChildId, long? AssetParameterId, DateTime From, DateTime To)
        {
            return $"SELECT  APV.Value, isnull(APV.DateTime, APV.ModificationTime) Stamp " +
                "FROM SchedulingContainers SC " +
                "LEFT JOIN Tasks T ON T.SchedulingContainerId = SC.Id " +
                "LEFT JOIN InspectionProtocols IP ON IP.TaskId = T.Id AND IP.IsDeleted <> 1 " +
                "LEFT JOIN InspectionProtocolItems IPI ON IPI.InspectionProtocolId = IP.Id " +
                "LEFT JOIN AssetParameterValues APV ON APV.Id = IPI.AssetParameterValueId " +
                $"INNER JOIN AssetParameterSetRecords APSR ON APSR.AssetParameterSetId = {AssetParameterSetId} AND IP.AssetId = APSR.AssetId AND APSR.AssetComponentId = APV.AssetId AND APSR.AssetParameterId = APV.AssetParameterId " +
                "WHERE SC.SCStatusId = 3 " +
                "AND SC.IsDeleted <> 1 " +
                $"AND SC.StartTime <= '{To.ToString("yyyyMMdd HH:mm:ss")}' " +
                $"AND SC.CloseTime >= '{From.ToString("yyyyMMdd HH:mm:ss")}' " +
                "AND APV.Value IS NOT NULL " +
                $"AND SC.DepartmentId = {DepartmentId} " +
                $"AND IP.AssetId = {AssetId} " +
                $"AND APV.AssetId = {AssetChildId} " +
                $"AND APV.AssetParameterId = {AssetParameterId} " +
                $"ORDER BY Stamp; ";
        }
        #endregion

        #region Маршруты
        /// <summary>
        /// Маршруты
        /// </summary>
        /// <returns></returns>
        private static string SelectAssetParameterSets(int ID = -1, int? DeptId = null, long UserDept = -1)
        {
            return "SELECT e.Id, e.DisplayName, e.ObjectUID, t.id as DepartmentID, t.DisplayName as DepartmentName, e.CreationTime, ISNULL(e.ModificationTime, e.CreationTime) AS ModificationTime " +
                "FROM AssetParameterSets AS e " +
                "LEFT JOIN( " +
                "    SELECT * " +
                "    FROM Departments AS e0 " +
                "    WHERE (e0.IsDeleted<> 1) AND(e0.TenantId = CAST(1 AS bigint)) " +
                ") AS t ON e.DepartmentId = t.Id " +
                $"INNER JOIN TBFN_GET_DEPARTMENTS({UserDept}) dps ON dps.Id = e.DepartmentId " +
                "WHERE e.IsDeleted <> 1 " +
                $"{(ID == -1 ? "" : "AND e.Id = " + ID)} " +
                $"{(DeptId == null ? "" : $"AND e.DepartmentId = {DeptId} ")}" +
                "ORDER BY e.DisplayName";
        }
        //private static string SelectAssetParameterSetsByDepartment(int DeptId)
        //{
        //    return "SELECT e.Id, e.DisplayName, e.ObjectUID, e.DepartmentId, t.DisplayName as DepartmentName  " +
        //        "FROM AssetParameterSets AS e " +
        //        "LEFT JOIN Departments t ON e.DepartmentId = t.Id AND (t.IsDeleted<> 1) AND(t.TenantId = CAST(1 AS bigint)) " +
        //        "WHERE e.IsDeleted <> 1 " +
        //        $"AND e.DepartmentId = {DeptId} " +
        //        "ORDER BY e.DisplayName";
        //}

        /// <summary>
        /// Создание маршрута
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string CreateAssetParameterSet(AssetParameterSet obj, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM AssetParameterSets ),0)+1); " +
                "DECLARE @InsertedId TABLE(id bigint); " +
                "INSERT into AssetParameterSets " +
                "(DisplayName, " +
                "DepartmentId, " +
                "ObjectUID, " +
                "CreatedByUser, " +
                "CreationTime," +
                "Revision," +
                "IsDeleted) " +
                "output inserted.Id into @InsertedId " +
                "values " +
                $"(N'{obj.DisplayName}', " +
                $"{obj.DepartmentID}, " +
                $"'{obj.ObjectUID.ToString("D")}', " +
                $"{EngId}, " +
                "getdate(), " +
                "@revision," +
                "0); " +
                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.AssetParameterSet}; " +
                $"SELECT TOP 1 id FROM @InsertedId;";
        }
        /// <summary>
        /// Обновление маршрута
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string UpdateAssetParameterSet(AssetParameterSet obj, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM AssetParameterSets ),0)+1); " +
                "UPDATE AssetParameterSets SET " +
                $"DisplayName = N'{obj.DisplayName}', " +
                $"DepartmentId = {obj.DepartmentID}, " +
                $"ModifiedByUser = {EngId}, " +
                "ModificationTime = getdate(), " +
                "Revision = @revision " +
                $"WHERE ID = {obj.Id}; " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.AssetParameterSet};";
        }
        /// <summary>
        /// Удаление маршрута
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string DeleteAssetParameterSet(int ID, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM AssetParameterSets ),0)+1); " +
                "UPDATE AssetParameterSets SET " +
                $"DeletedByUser = {EngId}, " +
                "DeletionTime = getdate(), " +
                "Revision = @revision, " +
                "IsDeleted = 1 " +
                $"WHERE ID = {ID}; " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.AssetParameterSet}; ";
        }
        /// <summary>
        /// Получаем детали маршрута
        /// </summary>
        /// <param name="ID">Маршрут</param>
        /// <returns></returns>
        private static string SelectAssetParameterSetDetails(int ID)
        {
            return "  SELECT APSR.AssetId, A.DisplayName AS AssetName, APSR.AssetComponentId, AC.DisplayName AS AssetComponentName, APSR.AssetParameterId, AP.DisplayName AS AssetParameterName " +
                "FROM AssetParameterSetRecords APSR " +
                "LEFT JOIN Assets A ON APSR.AssetId = A.Id " +
                "LEFT JOIN Assets AC ON APSR.AssetComponentId = AC.Id " +
                "LEFT JOIN AssetParameters AP ON APSR.AssetParameterId = AP.Id " +
                $"WHERE APSR.AssetParameterSetId = {ID} " +
                "AND APSR.IsDeleted <> 1; ";
        }
        private static string CreateAssetParameterSetDetails(long APSId, long AId, long ACId, long APId, long EngId)
        {

            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM AssetParameterSetRecords ),0)+1); " +
                "DECLARE @InsertedId TABLE(id int); " +
                "INSERT into AssetParameterSetRecords " +
                "(IsDeleted, " +
                "Revision, " +
                "CreatedByUser, " +
                "CreationTime," +
                "ObjectUID," +
                "AssetParameterSetId," +
                "AssetId," +
                "AssetComponentId," +
                "AssetParameterId)" +
                "values " +
                $"(0, " +
                $"@revision, " +
                $"{EngId}, " +
                "getdate(), " +
                $"'{Guid.NewGuid().ToString("D")}', " +
                $"{APSId}, " +
                $"{AId}, " +
                $"{ACId}, " +
                $"{APId});" +
                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.AssetParameterSetRecord};";
        }

        /// <summary>
        /// Удаление деталей маршрута
        /// </summary>
        /// <returns></returns>
        private static string DeleteAssetParameterSetDetails(long ID, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM AssetParameterSetRecords ),0)+1); " +
                "UPDATE AssetParameterSetRecords SET " +
                $"DeletedByUser = {EngId}, " +
                "DeletionTime = getdate(), " +
                "Revision = @revision, " +
                "IsDeleted = 1 " +
                $"WHERE ID = {ID}; " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.AssetParameterSetRecord}; ";
        }
        /// <summary>
        /// Для обновления/удаления деталей маршрута.
        /// </summary>
        /// <returns></returns>
        private static string GetAssetParameterSetRecords(long APSID)
        {
            return $"SELECT Id, AssetId, AssetComponentId, AssetParameterId FROM AssetParameterSetRecords where IsDeleted<>1 AND  AssetParameterSetId = {APSID}";
        }
        #endregion

        #region Классы объектов
        /// <summary>
        /// Классы объектов
        /// </summary>
        /// <returns></returns>
        private static string SelectAssetClass(int ID = -1, string DisplayName = "")
        {
            return "SELECT Id, DisplayName, ObjectUID, ParentId " +
                "FROM AssetClass AS e " +
                "WHERE (IsDeleted <> 1) " +
                $"{(ID == -1 ? "" : "AND Id = " + ID)} " +
                $"{(string.IsNullOrEmpty(DisplayName)? "" : $"AND DisplayName like N'%{DisplayName}%' ")} " +
                "AND (TenantId = CAST(1 AS bigint)) " +
                "order by DisplayName";
        }
        /// <summary>
        /// Создание класса объектов
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string CreateAssetClass(AssetClass obj, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM AssetClass ),0)+1); " +
                "DECLARE @InsertedPId TABLE(id int); " +
                "INSERT INTO AssetClass " +
                $"(Name, " +
                $"DisplayName, " +
                $"CreatedByUser, " +
                "CreationTime, " +
                "ObjectUID, " +
                "Revision, " +
                "TenantId, " +
                "IsDeleted)" +
                "output inserted.Id into @InsertedPId " +
                "values" +
                $"(N'{obj.DisplayName}', " +
                $"N'{obj.DisplayName}', " +
                $"{EngId}, " +
                $"getdate(), " +
                $"'{obj.ObjectUID.ToString("D")}', " +
                $"@revision, " +
                $"1, " +
                $"0); " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.AssetClass};" +
                $"SELECT TOP 1 id FROM @InsertedPId;";
        }
        /// <summary>
        /// Обновление класса объектов
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string UpdateAssetClass(AssetClass obj, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM AssetClass ),0)+1); " +
                "UPDATE AssetClass SET " +
                "Revision = @revision, " +
                $"Name = N'{obj.DisplayName}', " +
                $"DisplayName = N'{obj.DisplayName}', " +
                $"ModifiedByUser = {EngId}, " +
                "ModificationTime = getdate() " +
                $"WHERE ID = {obj.Id}; " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.AssetClass};";
        }
        /// <summary>
        /// Удаление класса объектов
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string DeleteAssetClass(int ID, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM AssetClass ),0)+1); " +
                "UPDATE AssetClass SET " +
                $"DeletedByUser = {EngId}, " +
                "DeletionTime = getdate(), " +
                "Revision = @revision, " +
                "IsDeleted = 1 " +
                $"WHERE ID = {ID}; " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.AssetClass}; ";
        }
        /// <summary>
        /// Связанный контроль
        /// </summary>
        /// <param name="AssetClassId"></param>
        /// <returns></returns>
        private static string SelectAssetClassParameters(int AssetClassId)
        {
            return "SELECT ACP.Id, ACP.ObjectUID, ACP.AssetClassId, ACP.AssetParameterId, AP.DisplayName as AssetParameterName " +
                "FROM AssetClassParameter ACP " +
                "INNER JOIN AssetParameters AP ON AP.id = ACP.AssetParameterId " +
                "WHERE (ACP.IsDeleted <> 1) " +
                $"AND ACP.AssetClassId = {AssetClassId}";
        }
        /// <summary>
        /// Создание класса объектов
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string CreateAssetClassParameter(AssetClassParameter obj, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM AssetClassParameter ),0)+1); " +
                "INSERT INTO AssetClassParameter " +
                $"(AssetClassId, " +
                $"AssetParameterId, " +
                "ObjectUID, " +
                "Revision, " +
                "IsDeleted)" +
                "values" +
                $"({obj.AssetClassId}, " +
                $"{obj.AssetParameterId}, " +
                $"'{obj.ObjectUID.ToString("D")}', " +
                $"@revision, " +
                $"0); " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.AssetClassParameter};";
        }
        /// <summary>
        /// Удаление класса объектов
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string DeleteAssetClassParameter(int ID, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM AssetClassParameter ),0)+1); " +
                "UPDATE AssetClassParameter SET " +
                "Revision = @revision, " +
                "IsDeleted = 1 " +
                $"WHERE ID = {ID}; " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.AssetClassParameter}; ";
        }

        #endregion

        #region Должность
        /// <summary>
        /// Должность
        /// </summary>
        /// <returns></returns>
        private static string SelectPersonPositions(int ID = -1)
        {
            return "SELECT Id, Name, DisplayName, ObjectUID " +
                "FROM PersonPositions " +
                "WHERE (IsDeleted <> 1) " +
                $"{(ID == -1 ? "" : "AND Id = " + ID)} " +
                "AND (TenantId = CAST(1 AS bigint)) order by id desc";
        }
        /// <summary>
        /// Создание должности
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string CreatePersonPosition(PersonPosition obj, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM PersonPositions ),0)+1); " +
                "INSERT into PersonPositions " +
                "(Name, " +
                "DisplayName, " +
                "ObjectUID, " +
                "Revision," +
                "TenantId," +
                "IsDeleted)" +
                "values " +
                $"(N'{obj.Name}', " +
                $"N'{obj.DisplayName}', " +
                $"'{obj.ObjectUID.ToString("D")}', " +
                "@Revision," +
                "1," +
                "0);" +
                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.PersonPosition};";
        }
        private static string UpdatePersonPosition(PersonPosition obj, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM PersonPositions ),0)+1); " +
                "UPDATE PersonPositions SET " +
                $"Name = N'{obj.Name}', " +
                $"DisplayName = N'{obj.DisplayName}', " +
                "Revision = @revision " +
                $"WHERE ID = {obj.Id}; " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.PersonPosition};";
        }
        private static string DeletePersonPosition(int ID, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM PersonPositions ),0)+1); " +
                "UPDATE PersonPositions SET " +
                "Revision = @revision, " +
                "IsDeleted = 1 " +
                $"WHERE ID = {ID}; " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.PersonPosition}; ";
        }
        #endregion

        #region Категория персонала
        /// <summary>
        /// Категория персонала
        /// </summary>
        /// <returns></returns>
        private static string SelectPersonCategories(int ID = -1)
        {
            return "SELECT Id, Name, DisplayName, ObjectUID, ParentId " +
                "FROM PersonCategories " +
                "WHERE (IsDeleted <> 1) " +
                $"{(ID == -1 ? "" : "AND Id = " + ID)} " +
                "AND (TenantId = CAST(1 AS bigint)) order by DisplayName";
        }
        /// <summary>
        /// Создание категории персонала
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string CreatePersonCategory(PersonCategory obj, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM PersonCategories ),0)+1); " +
                "INSERT into PersonCategories " +
                "(Name, " +
                "DisplayName, " +
                "ObjectUID, " +
                "ParentId, " +
                "Revision, " +
                "IsDeleted, " +
                "TenantId)" +
                "values " +
                $"(N'{obj.Name}', " +
                $"N'{obj.DisplayName}', " +
                $"'{obj.ObjectUID.ToString("D")}', " +
                $"{(obj.ParentId == null ? "NULL" : obj.ParentId)}, " +
                "@Revision, " +
                "0, " +
                "1);" +
                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.PersonCategory};";
        }
        private static string UpdatePersonCategory(PersonCategory obj, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM PersonCategories ),0)+1); " +
                "UPDATE PersonCategories SET " +
                $"Name = N'{obj.Name}', " +
                $"DisplayName = N'{obj.DisplayName}', " +
                $"ParentId = {(obj.ParentId == null ? "NULL" : obj.ParentId)} " +
                $"WHERE ID = {obj.Id}; " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.PersonCategory};";
        }
        private static string DeletePersonCategory(int ID, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM PersonCategories ),0)+1); " +
                "UPDATE PersonCategories SET " +
                "Revision = @revision, " +
                "IsDeleted = 1 " +
                $"WHERE ID = {ID}; " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.PersonCategory}; ";
        }
        #endregion

        #region Объекты
        /// <summary>
        /// Объекты
        /// </summary>
        /// <returns></returns>
        private static string SelectAssets(int? DeptID = -1, bool NotesOnly = false, long UserDept = -1 )
        {
            return "SELECT e.Id, e.DisplayName, e.Description, e.ExternalId, e.ObjectUID, e.ParentId,  t1.DisplayName as StateName, t1.ObjectUID, AT.Id as TypeID, AT.DisplayName as TypeName, e.AssetTypeId, e.AssetStateId, e.CreationTime, ISNULL(e.ModificationTime, e.CreationTime) AS ModificationTime " +
                "FROM Assets AS e " +
                "LEFT JOIN ( " +
                "    SELECT e2.Id, e2.DisplayName, e2.ExternalId, e2.IsDeleted, e2.Name, e2.ObjectUID, e2.Revision, e2.TenantId " +
                "    FROM AssetStates AS e2 " +
                "    WHERE (e2.IsDeleted <> 1) AND (e2.TenantId = CAST(1 AS bigint)) " +
                ") AS t1 ON e.AssetStateId = t1.Id " +
                "LEFT JOIN AssetTypes AT on AT.Id = e.AssetTypeId " +
                $"INNER JOIN TBFN_GET_DEPARTMENTS({UserDept}) dps ON dps.Id = e.DepartmentId " +
                "WHERE ((e.IsDeleted <> 1) AND (e.TenantId = CAST(1 AS bigint))) " +
                (DeptID != null && DeptID != -1 ? $"AND (e.DepartmentId =  {DeptID} OR e.DepartmentId IS NULL)" : "") +
                (NotesOnly ? $"AND e.AssetSortId = 1 " : "") +
                "ORDER BY e.DisplayName ";
        }
        private static string SelectAsset(int ID)
        {
            return "SELECT Id, DisplayName, ParentId, ObjectUID, DepartmentId, AssetSortId, AssetClassId, ExternalId, AssetStateId " +
                "FROM Assets " +
                $"WHERE Id = {ID}";
        }
        private static string GetSimpleAsset(int? ID, int? DeptId)
        {
            return "SELECT p.id ParentId, p.DisplayName ParentName,  ch.Id ChildId, ch.DisplayName ChildName, prm.Id ParamId, prm.DisplayName ParamName " +
                "FROM Assets p " +
                "LEFT JOIN Assets ch on p.Id = ch.ParentId  AND ch.IsDeleted <> 1 " +
                "LEFT JOIN AssetParameterPair pair on pair.AssetId = ch.Id AND pair.IsDeleted <> 1 " +
                "LEFT JOIN AssetParameters prm on prm.Id = pair.AssetParameterId AND prm.IsDeleted <> 1 " +
                $"WHERE " +
                $"{(ID == null? "": $"p.Id = {ID} AND")} " +
                $"{(DeptId == null? "": $"p.DepartmentId = {DeptId} AND")} " +
                $"p.IsDeleted <> 1 " +
                $"AND p.ParentId IS NULL " +
                $"ORDER BY ParentName, ChildName, ParamName; ";
        }
        private static string SelectAssetParameterForAsset(long DeptId, long AssetId)
        {
            return
                //Берем все для подразделения, исключая уже работающие в узле.
                "SELECT A.Id, " +                 //AssetParameterId
                "NULL AS PairId, " +            //AssetParameterPairId
                "CASE WHEN A.Description <> '' THEN CONCAT(A.DisplayName, ' (', A.Description, ')') ELSE A.DisplayName END DisplayName, " +
                "0 AS IsPair, " +               //Выбран
                "NULL AS ObjectUID, " + 
                "ISNULL(D.DisplayName, N'Нет') DeptName " +
                "FROM AssetParameters A " +
                "LEFT JOIN Departments D ON D.Id = A.DepartmentId " +
                "WHERE A.Id IN( " +
                "SELECT ID  " +
                "FROM AssetParameters " +
                "WHERE  " +
                $"DepartmentId = {DeptId} " +
                $"AND IsDeleted <> 1 " +
                $"AND ID NOT IN (SELECT AssetParameterId FROM AssetParameterPair WHERE IsDeleted <> 1 AND AssetId = {AssetId})) " +
                "UNION " +
                //Берем все что касается текущего узла.
                "SELECT " +
                "AP.Id, " +
                "APP.Id AS PairId , " +
                "CASE WHEN AP.Description <> '' THEN CONCAT(AP.DisplayName, ' (', AP.Description, ')') ELSE AP.DisplayName END DisplayName, " +
                "1 AS IsPair, " +               //Выбран
                "APP.ObjectUID, " +
                "ISNULL(D.DisplayName, N'Нет') DeptName " +
                "FROM AssetParameterPair APP " +
                "LEFT JOIN AssetParameters AP ON AP.Id = APP.AssetParameterId " +
                "LEFT JOIN Departments D ON D.Id = AP.DepartmentId " +
                "WHERE APP.IsDeleted <> 1 " +
                $"AND APP.AssetId = {AssetId} ORDER BY DisplayName";
        }
        private static string CreateAssetParameterPair(long AssetId, AssetControl Control)
        {
            return "DECLARE @Rev bigint; " +
                "set @Rev = (isnull((SELECT max(revision) id FROM AssetParameterPair ),0)+1); " +

                "INSERT into AssetParameterPair " +
                "(ObjectUID, " +
                "Revision," +
                "AssetId," +
                "AssetParameterId," +
                "IsDeleted)" +
                "values " +
                $"({(Control.ObjectUID==null?"NULL":$"'{((Guid)Control.ObjectUID).ToString("D")}'")}, " +
                $"@Rev, " +
                $"{AssetId}, " +
                $"{Control.AssetParameterId}," +
                "0);" +

                $"UPDATE PPEntityCollections SET Revision = @Rev WHERE ID = {(int)RevEntry.AssetParameterPair};";
        }
        private static string UpdateAssetParameterPair(long AssetId, AssetControl Control)
        {
            return "DECLARE @Rev bigint; " +
                "set @Rev = (isnull((SELECT max(revision) id FROM AssetParameterPair ),0)+1); " +

                "UPDATE AssetParameterPair SET " +
                $"ObjectUID = {(Control.ObjectUID==null?"NULL":$"'{((Guid)Control.ObjectUID).ToString("D")}'")}, " +
                "Revision = @Rev " +
                $"WHERE Id = {Control.PairId}; " +

                $"UPDATE PPEntityCollections SET Revision = @Rev WHERE ID = {(int)RevEntry.AssetParameterPair};";
        }
        private static string DeleteAssetParameterPair(long AssetParameterPairId)
        {
            return "DECLARE @Rev bigint; " +
                "set @Rev = (isnull((SELECT max(revision) id FROM AssetParameterPair ),0)+1); " +

                "UPDATE AssetParameterPair SET " +
                "Revision = @Rev, " +
                "IsDeleted = 1 " +
                $"WHERE Id = {AssetParameterPairId}; " +

                $"UPDATE PPEntityCollections SET Revision = @Rev WHERE ID = {(int)RevEntry.AssetParameterPair};";
        }
        /// <summary>
        /// Создание объекта
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string CreateAsset(Asset obj, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM Assets ),0)+1); " +
                "DECLARE @AId TABLE(id bigint); " +

                "INSERT into Assets " +
                "(ParentId, " +
                "DisplayName, " +
                "Name, " +
                "DepartmentId, " +
                "AssetSortId, " +
                "AssetClassId, " +
                "ExternalId, " +
                "ObjectUID, " +
                "CreatedByUser, " +
                "CreationTime," +
                "Revision, " +
                "IsDeleted, " +
                "TenantId, " +
                "IsDangerous, " +
                "OperationStart, " +
                "MadeDate, " +
                "AssetSubtype, " +
                "AssetStateId)" +
                "OUTPUT inserted.Id into @AId " +
                "values " +
                $"({(obj.ParentId == null ? "NULL" : obj.ParentId)}, " +
                $"N'{obj.DisplayName}', " +
                $"N'{obj.DisplayName}', " +
                $"{obj.DepartmentId}, " +
                $"{obj.AssetSortId}, " +
                $"{(obj.AssetClassId == null ? "NULL" : obj.AssetClassId)}, " +
                $"{(string.IsNullOrEmpty(obj.Maximo) ? "NULL" : $"N'{obj.Maximo}'")}, " +
                $"'{obj.ObjectUID.ToString("D")}', " +
                $"{EngId}, " +
                "getdate(), " +
                "@Revision, " +
                "0, " +
                "1, " +
                "0, " +
                "getdate(), " +
                "getdate(), " +
                "-1," +
                $"{(obj.AssetStateId == null ? "NULL" : obj.AssetStateId)});" +
                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.Asset};" +
                $"SELECT id FROM @AId;";
        }
        /// <summary>
        /// Обновление объекта
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string UpdateAsset(Asset obj, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM Assets ),0)+1); " +
                "UPDATE Assets SET " +
                $"ParentId = {(obj.ParentId == null ? "NULL" : obj.ParentId)}, " +
                $"DisplayName = N'{obj.DisplayName}', " +
                $"Name = N'{obj.DisplayName}', " +
                $"DepartmentId = {obj.DepartmentId}, " +
                $"ExternalId = {(string.IsNullOrEmpty(obj.Maximo) ? "NULL" : $"N'{obj.Maximo}'")}, " +
                $"AssetSortId = {(obj.AssetSortId== null? "NULL": $"{obj.AssetSortId}")}, " +
                $"AssetClassId = {(obj.AssetClassId == null ? "NULL" : obj.AssetClassId)}, " +
                $"AssetStateId = {(obj.AssetStateId == null ? "NULL" : obj.AssetStateId)}, " +
                $"ModifiedByUser = {EngId}, " +
                "ModificationTime = getdate(), " +
                "Revision = @revision " +
                $"WHERE ID = {obj.Id}; " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.Asset};";
        }
        /// <summary>
        /// Удаление объекта
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string DeleteAsset(int ID, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM Assets ),0)+1); " +
                "UPDATE Assets SET " +
                $"DeletedByUser = {EngId}, " +
                "DeletionTime = getdate(), " +
                "Revision = @revision, " +
                "IsDeleted = 1 " +
                $"WHERE ID = {ID}; " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.Asset}; ";
        }

        private static string SelectAssetTypes()
        {
            return "SELECT id, DisplayName, ObjectUID " +
                "FROM AssetTypes " +
                "where IsDeleted <> 1 " +
                "AND TenantId = CAST(1 AS bigint)";
        }
        private static string SelectAssetStates()
        {
            return "SELECT id, DisplayName " +
                "FROM AssetStates " +
                "where IsDeleted <> 1 " +
                "AND TenantId = CAST(1 AS bigint)";
        }
        private static string SelectAssetSorts()
        {
            return "SELECT id, DisplayName, ObjectUID " +
                "FROM AssetSorts " +
                "where IsDeleted <> 1 " +
                "AND TenantId = CAST(1 AS bigint)";
        }
        #endregion

        #region Участки
        /// <summary>
        /// Участки
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        private static string _OLDSelectDepartments(int ID = -1, bool IsChildOnly = false)
        {
            return "SELECT D.*, P.DisplayName as ParentDisplayName " +
                "FROM Departments as D " +
                "LEFT JOIN Departments as P on D.ParentId = P.id " +
                "WHERE " +
                $"{(ID > 0 ? $"D.id = {ID} AND" : "")} " +
                $"{(IsChildOnly ? $"D.ParentId IS NOT NULL AND" : "")} " +
                "(D.IsDeleted <> 1) AND(D.TenantId = CAST(1 AS bigint)) order by D.DisplayName";
        }
        /// <summary>
        /// Получаем список 
        /// </summary>
        /// <returns></returns>
        private static string SelectDepartments()
        {
            return "SELECT * FROM TBFN_GET_DEPARTMENTS(@RootDeptId) order by DisplayName";
        }
        private static string SelectDepartment()
        {
            return "SELECT * FROM Departments WHERE Id = @DeptId";
        }
        /// <summary>
        /// Создание подразделения
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string CreateDepartment(Department obj, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM Departments ),0)+1); " +
                "INSERT into Departments " +
                "(ParentId, " +
                "Name, " +
                "DisplayName, " +
                "Latitude, " +
                "Longitude, " +
                "TimeZone, " +
                "Organization, " +
                "ShortCode, " +
                "ObjectUID, " +
                "Revision," +
                "IsDeleted, " +
                "TenantId)" +
                "values " +
                $"({(obj.ParentId == null ? "NULL" : obj.ParentId)}, " +
                $"N'{obj.Name}', " +
                $"N'{obj.DisplayName}', " +
                $"{obj.Latitude}, " +
                $"{obj.Longitude}, " +
                $"N'{obj.TimeZone}', " +
                $"N'{obj.Organization}', " +
                $"N'{obj.ShortCode}', " +
                $"'{obj.ObjectUID.ToString("D")}', " +
                "@Revision, " +
                "0, " +
                "1);" +
                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.Department};";
        }
        /// <summary>
        /// Обновление подразделения
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string UpdateDepartment(Department obj, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM Departments ),0)+1); " +
                "UPDATE Departments SET " +
                $"ParentId = {(obj.ParentId == null ? "NULL" : obj.ParentId)}, " +
                $"Name = N'{obj.Name}', " +
                $"DisplayName = N'{obj.DisplayName}', " +
                $"Latitude = {obj.Latitude}, " +
                $"Longitude = {obj.Longitude}, " +
                $"TimeZone = N'{obj.TimeZone}', " +
                $"Organization = N'{obj.Organization}', " +
                $"ShortCode = N'{obj.ShortCode}', " +
                "Revision = @revision " +
                $"WHERE ID = {obj.Id}; " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.Department};";
        }
        /// <summary>
        /// Удаление подразделения
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string DeleteDepartment(int ID, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM Departments ),0)+1); " +
                "UPDATE Departments SET " +
                "Revision = @revision, " +
                "IsDeleted = 1 " +
                $"WHERE ID = {ID}; " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.Department}; ";
        }
        #endregion

        #region Бригады
        /// <summary>
        /// Типы бригады
        /// </summary>
        /// <returns></returns>
        private static string SelectResourceTypes()
        {
            return "SELECT id, DisplayName, ObjectUID FROM ResourceTypes where IsDeleted <> 1 AND TenantId = CAST(1 AS bigint)";
        }
        /// <summary>
        /// Состояние бригады
        /// </summary>
        /// <returns></returns>
        private static string SelectResourceStates()
        {
            return "SELECT id, DisplayName, ObjectUID FROM ResourceStates where IsDeleted <> 1 AND TenantId = CAST(1 AS bigint)";
        }
        /// <summary>
        /// Список работников внутри бригады
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        private static string _SelectResourceEngineers(long DepartmentId, long ResourceId)
        {
            return $"SELECT " +
                $"E.Id, " +
                $"E.DisplayName, " +
                $"RA.Id AS ResEngId, " +
                $"CASE WHEN RA.Id IS NULL THEN 0 ELSE 1 END AS Selected " +
                $"FROM Engineers E " +
                $"LEFT JOIN ResourceAllocations RA ON RA.EngineerId = E.Id AND ({(ResourceId > 0 ? $" RA.ResourceId = {ResourceId} OR " : "")} RA.ResourceId IS NULL)" +
                $"WHERE E.IsDeleted = 0 " +
                $"AND (RA.IsDeleted = 0 OR RA.IsDeleted IS NULL) " +
                $"AND E.DepartmentId = {DepartmentId} " +
                $"ORDER BY E.DisplayName";
            
        }
        /// <summary>
        /// Привязка работника к  бригаде
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string CreateResourceAllocation(ResourceAllocations obj, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM ResourceAllocations ),0)+1); " +
                "INSERT INTO ResourceAllocations " +
                "(IsDeleted, " +
                "Revision," +
                "CreatedByUser, " +
                "CreationTime, " +
                "ObjectUID, " +
                "EngineerId, " +
                "ResourceId) " +
                "values " +
                "(0," +
                "@revision, " +
                $"{EngId}, " +
                $"getdate(), " +
                $"'{obj.ObjectUID.ToString("D")}', " +
                $"{obj.EngineerID}, " +
                $"{obj.ResourceID})" +
                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.ResourceAllocation};";
        }
        /// <summary>
        /// Удаление работника из бригады
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string DeleteResourceAllocation(long ID, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM ResourceAllocations ),0)+1); " +
                "UPDATE ResourceAllocations SET " +
                "IsDeleted = 1, " +
                $"DeletedByUser = {EngId}, " +
                "DeletionTime = getdate(), " +
                "ResourceId = NULL " +
                $"WHERE ID = {ID}; " +
                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.ResourceAllocation};";
        }
        /// <summary>
        /// Бригады
        /// </summary>
        /// <returns></returns>
        private static string SelectResources(long? ID = null, long? DepID = null, long UserDept = -1)
        {
            return "SELECT r.Id, r.DisplayName, r.ObjectUID, r.ResourceTypeId, r.ResourceStateId, r.DepartmentId, r.Latitude, r.Longitude, r.Address, r.CreationTime, ISNULL(r.ModificationTime, r.CreationTime) AS ModificationTime " +
                "FROM Resources r " +
                $"INNER JOIN TBFN_GET_DEPARTMENTS({UserDept}) dps ON dps.Id = r.DepartmentId " +
                $"WHERE " +
                $"{(ID != null ? $"r.Id = {ID} AND" : "")} " +
                $"{(DepID != null ? $"r.DepartmentId = {DepID} AND" : "")} " +
                "(r.IsDeleted <> 1) AND (r.TenantId = CAST(1 AS bigint)) order by r.DisplayName";
        }
        /// <summary>
        /// Создание бригады
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string CreateResource(Resource obj, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM Resources ),0)+1); " +
                "DECLARE @InsertedPId TABLE(id int); " +
                "INSERT INTO Resources " +
                $"(DisplayName, " +
                $"Latitude, " +
                $"Longitude, " +
                $"Address, " +
                $"ResourceStateId, " +
                $"CreatedByUser, " +
                "CreationTime, " +
                "ObjectUID, " +
                "Revision, " +
                "TenantId, " +
                "IsDeleted, " +
                "DepartmentId)" +
                "output inserted.Id into @InsertedPId " +
                "values" +
                $"(N'{obj.DisplayName}'," +
                $"NULL, " +
                $"NULL, " +
                $"NULL, " +
                $"NULL, " +
                $"{EngId}, " +
                $"getdate(), " +
                $"'{obj.ObjectUID.ToString("D")}', " +
                $"@revision, " +
                $"1, " +
                $"0, " +
                $"{obj.DepartmentId}); " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.Resource};" +
                $"SELECT TOP 1 id FROM @InsertedPId;";
        }
        /// <summary>
        /// Обновление бригады
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string UpdateResource(Resource obj, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM Resources ),0)+1); " +
                "UPDATE Resources SET " +
                "Revision = @revision, " +
                $"DisplayName = N'{obj.DisplayName}', " +
                $"DepartmentId = {obj.DepartmentId}, " +
                $"ModifiedByUser = {EngId}, " +
                "ModificationTime = getdate() " +
                $"WHERE ID = {obj.Id}; " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.Resource};";
        }
        /// <summary>
        /// Удаление бригады
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string DeleteResource(int ID, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM Resources ),0)+1); " +
                "UPDATE Resources SET " +
                $"DeletedByUser = {EngId}, " +
                "DeletionTime = getdate(), " +
                "Revision = @revision, " +
                "IsDeleted = 1 " +
                $"WHERE ID = {ID}; " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.Resource}; ";
        }
        #endregion

        #region Рабочие календари
        #endregion

        #region Роли
        /// <summary>
        /// Роли
        /// </summary>
        /// <returns></returns>
        private static string SelectPPRoles()
        {
            return "SELECT Id, DisplayName, Name, ObjectUID FROM PPRoles AS e WHERE (IsDeleted <> 1) AND (TenantId = CAST(1 AS bigint)) ORDER BY Id";
        }
        #endregion

        #region Работники
        /// <summary>
        /// Работники
        /// </summary>
        /// <returns></returns>
        private static string SelectEngineers(long? ID = null, long? DeptId = null, long UserDept = -1)
        {
            return "SELECT e.Id, CONCAT(P.Surname,' ',P.GivenName,' ', P.MiddleName) as PersonName, e.ObjectUID, e.PersonId, D.DisplayName as DepartmentName, isnull(U.Name,0) as IsUser , e.DepartmentId, e.PersonId, P.Surname, P.GivenName, P.MiddleName, e.CalendarId, P.PersonPositionId, U.Id AS UserId, U.Name AS Login, e.CreationTime, ISNULL(e.ModificationTime, e.CreationTime) AS ModificationTime  " +
                "FROM Engineers AS e " +
                "LEFT JOIN Departments AS D ON e.DepartmentId = D.Id AND D.IsDeleted <> 1 " +
                "LEFT JOIN Persons AS P ON e.PersonId = P.Id AND P.IsDeleted <> 1 " +
                "LEFT JOIN PPUsers AS U ON P.UserId = U.Id AND U.IsDeleted <> 1 " +
                $"INNER JOIN TBFN_GET_DEPARTMENTS({UserDept}) dps ON dps.Id = e.DepartmentId " +
                "WHERE ((e.IsDeleted <> 1) " +
                $"{(ID == null ? "" : "AND e.Id = " + ID)} " +
                $"{(DeptId == null ? "" : "AND e.DepartmentId = " + DeptId)} " +
                "AND(e.TenantId = CAST(1 AS bigint))) " +
                "AND(e.PersonId IS NOT NULL ) ORDER BY PersonName";
        }
        /// <summary>
        /// Создание работника
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string CreateEngineer(Engineer obj, long EngId)
        {
            return "DECLARE @PRevision bigint, @ERevision bigint; " +
                "set @PRevision = (isnull((SELECT max(revision) id FROM Persons ),0)+1); " +
                "set @ERevision = (isnull((SELECT max(revision) id FROM Engineers ),0)+1); " +
                "DECLARE @InsertedPId TABLE(id int); " +
                "INSERT INTO Persons " +
                "(IsDeleted, " +
                "Revision, " +
                "CreatedByUser, " +
                "CreationTime, " +
                "ObjectUID, " +
                "DisplayName, " +
                "Surname, " +
                "GivenName, " +
                "MiddleName, " +
                "TenantId, " +
                "PersonPositionId) " +
                "output inserted.Id into @InsertedPId " +
                "VALUES" +
                "(0, " +
                "@PRevision, " +
                $"{EngId}, " +
                $"getdate(), " +
                $"'{obj.ObjectUID.ToString("D")}', " +
                $"N'{obj.Surname} {obj.GivenName.Substring(0, 1) + "."} {(!string.IsNullOrEmpty(obj.MiddleName) ? obj.MiddleName.Substring(0, 1) + "." : "")}', " +
                $"N'{obj.Surname}', " +
                $"N'{obj.GivenName}', " +
                $"{(!string.IsNullOrEmpty(obj.MiddleName) ? "N'" + obj.MiddleName + "'" : "NULL")}, " +
                $"1, " +
                $"{(obj.PersonPositionId == null ? "NULL" : obj.PersonPositionId)}); " +

                "INSERT INTO Engineers " +
                $"(DisplayName, " +
                $"PersonId, " +
                $"CalendarId, " +
                $"DepartmentId, " +
                $"CreatedByUser, " +
                "CreationTime, " +
                "ObjectUID, " +
                "Revision, " +
                "TenantId, " +
                "IsDeleted, " +
                "IsActive)" +
                "VALUES " +
                $"(N'{obj.Surname} {obj.GivenName.Substring(0, 1) + "."} {(!string.IsNullOrEmpty(obj.MiddleName) ? obj.MiddleName.Substring(0, 1) + "." : "")}'," +
                $"(select top 1 id from @InsertedPId), " +
                $"{(obj.CalendarId == null ? "NULL" : obj.CalendarId)}, " +
                $"{(obj.DepartmentId == null ? "NULL" : obj.DepartmentId)}, " +
                $"{EngId}, " +
                $"getdate(), " +
                $"'{obj.ObjectUID.ToString("D")}', " +
                $"@ERevision, " +
                $"1, " +
                $"0, " +
                $"0); " +

                $"UPDATE PPEntityCollections SET Revision = @PRevision WHERE ID = {(int)RevEntry.Person}; " +
                $"UPDATE PPEntityCollections SET Revision = @ERevision WHERE ID = {(int)RevEntry.Engineer}; " +
                $"SELECT TOP 1 Id FROM @InsertedPId; ";
        }
        /// <summary>
        /// Обновление работника
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string UpdateEngineer(Engineer obj, long EngId)
        {
            return "DECLARE @PRevision bigint, @ERevision bigint; " +
                "set @PRevision = (isnull((SELECT max(revision) id FROM Persons ),0)+1); " +
                "set @ERevision = (isnull((SELECT max(revision) id FROM Engineers ),0)+1); " +

                "UPDATE Persons SET " +
                $"DisplayName = N'{obj.Surname} {obj.GivenName.Substring(0, 1) + "."} {(!string.IsNullOrEmpty(obj.MiddleName) ? obj.MiddleName.Substring(0, 1) + "." : "")}', " +
                $"Surname = N'{obj.Surname}', " +
                $"GivenName = N'{obj.GivenName}', " +
                $"MiddleName = {(!string.IsNullOrEmpty(obj.MiddleName) ? "N'" + obj.MiddleName + "'" : "NULL")}, " +
                $"PersonPositionId = {(obj.PersonPositionId == null ? "NULL" : obj.PersonPositionId)}, " +
                $"ModifiedByUser = {EngId}, " +
                "ModificationTime = getdate(), " +
                "Revision = @PRevision " +
                $"WHERE ID = {obj.PersonId}; " +

                "UPDATE Engineers SET " +
                $"DisplayName = N'{obj.Surname} {obj.GivenName.Substring(0, 1) + "."} {(!string.IsNullOrEmpty(obj.MiddleName) ? obj.MiddleName.Substring(0, 1) + "." : "")}', " +
                $"CalendarId = {(obj.CalendarId == null ? "NULL" : obj.CalendarId)}, " +
                $"DepartmentId = {(obj.DepartmentId == null ? "NULL" : obj.DepartmentId)}, " +
                $"ModifiedByUser = {EngId}, " +
                "ModificationTime = getdate(), " +
                "Revision = @ERevision " +
                $"WHERE ID = {obj.Id}; " +

                $"UPDATE PPEntityCollections SET Revision = @PRevision WHERE ID = {(int)RevEntry.Person}; " +
                $"UPDATE PPEntityCollections SET Revision = @ERevision WHERE ID = {(int)RevEntry.Engineer};";
        }
        /// <summary>
        /// Удаление работника
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string DeleteEngineer(int ID, long EngId)
        {
            return "DECLARE @PRevision bigint, @ERevision bigint, @PPURevision bigint; " +
                "set @PPURevision = (isnull((SELECT max(revision) id FROM PPUsers ),0)+1); " +
                "set @PRevision = (isnull((SELECT max(revision) id FROM Persons ),0)+1); " +
                "set @ERevision = (isnull((SELECT max(revision) id FROM Engineers ),0)+1); " +

                "UPDATE Persons SET " +
                $"DeletedByUser = {EngId}, " +
                "DeletionTime = getdate(), " +
                "Revision = @PRevision, " +
                "IsDeleted = 1 " +
                $"WHERE ID in (select PersonId from Engineers where Id = {ID}); " +

                "UPDATE Engineers SET " +
                $"DeletedByUser = {EngId}, " +
                "DeletionTime = getdate(), " +
                "Revision = @ERevision, " +
                "IsDeleted = 1 " +
                $"WHERE ID = {ID}; " +

                "UPDATE PPUsers SET " +
                $"DeletedByUser = {EngId}, " +
                "DeletionTime = getdate(), " +
                "Revision = @PPURevision, " +
                "IsDeleted = 1 " +
                $"WHERE ID = (SELECT UserId FROM Persons WHERE Id = (select PersonId from Engineers where Id = {ID})); " +

                $"UPDATE PPEntityCollections SET Revision = @PPURevision WHERE ID = {(int)RevEntry.PPUser}; " +
                $"UPDATE PPEntityCollections SET Revision = @PRevision WHERE ID = {(int)RevEntry.Person}; " +
                $"UPDATE PPEntityCollections SET Revision = @ERevision WHERE ID = {(int)RevEntry.Engineer};";
        }

        /// <summary>
        /// Список людей для вставки в бригады
        /// </summary>
        /// <returns></returns>
        private static string SelectEngineersList(long? DeptId)
        {
            return "SELECT e.Id, t1.Surname, t1.GivenName, t1.MiddleName, t1.DisplayName, PP.DisplayName as AppointName, e.CreationTime, ISNULL(e.ModificationTime, e.CreationTime) AS ModificationTime  " +
                "FROM Engineers AS e  " +
                "LEFT JOIN Persons AS t1 ON e.PersonId = t1.Id  " +
                "LEFT JOIN PersonPositions as PP on PP.Id = t1.PersonPositionId " +
                "WHERE ((e.IsDeleted <> 1) " +
                $"{(DeptId == null? "":$"AND e.DepartmentId = {DeptId} ")}" +
                "AND(e.TenantId = CAST(1 AS bigint))) " +
                "AND(e.PersonId IS NOT NULL )";
        }
        /// <summary>
        /// Личная информация
        /// </summary>
        /// <returns></returns>
        private static string SelectPerson(int ID)
        {
            return $"SELECT Id, Surname, GivenName, MiddleName, UserId, PersonPositionId, ObjectUID FROM Persons WHERE  Id = {ID}";
        }
        /// <summary>
        /// Создание личной информации
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string CreatePerson(Person obj, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM Persons ),0)+1); " +
                "DECLARE @InsertedPId TABLE(id int); " +
                "INSERT INTO Persons " +
                $"(DisplayName, " +
                $"Surname, " +
                $"GivenName, " +
                $"MiddleName, " +
                $"UserId, " +
                $"PersonPositionId, " +
                $"CreatedByUser, " +
                "CreationTime, " +
                "ObjectUID, " +
                "Revision)" +
                "output inserted.Id into @InsertedPId " +
                "values" +
                $"(N'{obj.DisplayName}'," +
                $"N'{obj.Surname}', " +
                $"N'{obj.GivenName}', " +
                $"N'{obj.MiddleName}', " +
                $"{obj.UserId}, " +
                $"N'{obj.PersonPositionId}', " +
                $"{EngId}, " +
                $"getdate(), " +
                $"'{obj.ObjectUID.ToString("D")}', " +
                $"@revision); " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.Person};";
        }
        /// <summary>
        /// Обновление личной информации
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string UpdatePerson(Person obj, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM Persons ),0)+1); " +
                "UPDATE Persons SET " +
                "Revision = @revision, " +
                $"DisplayName = N'{obj.DisplayName}', " +
                $"Surname = N'{obj.Surname}', " +
                $"GivenName = N'{obj.GivenName}', " +
                $"MiddleName = N'{obj.MiddleName}', " +
                $"PersonPositionId = {obj.PersonPositionId}, " +
                $"ModifiedByUser = {EngId}, " +
                "ModificationTime = getdate(), " +
                $"WHERE ID = {obj.Id}; " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.Person};";
        }
        /// <summary>
        /// Удаление личной информации
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string DeletePerson(int ID, long EngId)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM Persons ),0)+1); " +
                "UPDATE Persons SET " +
                $"DeletedByUser = {EngId}, " +
                "DeletionTime = getdate(), " +
                "Revision = @revision, " +
                "IsDeleted = 1 " +
                $"WHERE ID = {ID}; " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.Person}; ";
        }

        /// <summary>
        ///  Данные для входа
        /// </summary>
        /// <returns></returns>
        private static string SelectPPUser(int ID)
        {
            return $"SELECT Id, Surname, GivenName, MiddleName, UserId, PersonPositionId, ObjectUID FROM Persons WHERE  Id = {ID}";
        }
        /// <summary>
        /// Создание данных для входа
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string CreatePPUser(Engineer obj, long EngId)
        {
            return "DECLARE @PPURevision bigint, @PRevision bigint; " +
                "set @PPURevision = (isnull((SELECT max(revision) id FROM PPUsers ),0)+1); " +
                "set @PRevision = (isnull((SELECT max(revision) id FROM Persons ),0)+1); " +
                "DECLARE @InsertedPPUId TABLE(id int); " +

                "INSERT INTO PPUsers " +
                "(IsDeleted," +
                "Revision," +
                "CreatedByUser," +
                "CreationTime," +
                "Name," +
                "DisplayName," +
                "ObjectUID," +
                "TenantId," +
                "PWD)" +
                "output inserted.Id into @InsertedPPUId " +
                "VALUES " +
                "(0, " +
                "@PPURevision, " +
                $"{EngId}, " +
                $"getdate(), " +
                $"N'{obj.Login}', " +
                $"N'{obj.Login}', " +
                $"'{Guid.NewGuid().ToString("D")}'," +
                $"1," +
                $"ENCRYPTBYPASSPHRASE(N'{ENCR}', N'{obj.Password}'));" +

                $"UPDATE Persons SET " +
                $"UserId = (SELECT TOP 1 Id FROM @InsertedPPUId), " +
                $"ModifiedByUser = {EngId}, " +
                "ModificationTime = getdate()" +
                $"WHERE Id = {obj.PersonId}; " +

                $"UPDATE PPEntityCollections SET Revision = @PPURevision WHERE ID = {(int)RevEntry.PPUser};"+
                $"UPDATE PPEntityCollections SET Revision = @PRevision WHERE ID = {(int)RevEntry.Person};";
        }
        /// <summary>
        /// Обновление данных для входа
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string UpdatePPUser(Engineer obj, long EngId)
        {
            return "DECLARE @rev bigint; " +
                "set @rev = (isnull((SELECT max(revision) id FROM PPUsers ),0)+1); " +
                "UPDATE PPUsers SET " +
                "Revision = @rev, " +
                $"PWD = ENCRYPTBYPASSPHRASE(N'{ENCR}', N'{obj.Password}'), " +
                $"ModifiedByUser = {EngId}, " +
                "ModificationTime = getdate() " +
                $"WHERE ID = {obj.UserId}; " +

                $"UPDATE PPEntityCollections SET Revision = @rev WHERE ID = {(int)RevEntry.PPUser};";
        }
        /// <summary>
        /// Удаление данных для входа
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string DeletePPUser(int ID, long EngId)
        {
            return "DECLARE @rev bigint; " +
                "set @rev = (isnull((SELECT max(revision) id FROM PPUsers ),0)+1); " +
                "UPDATE PPUsers SET " +
                $"DeletedByUser = {EngId}, " +
                "DeletionTime = getdate(), " +
                "Revision = @rev, " +
                "IsDeleted = 1 " +
                $"WHERE ID = {ID}; " +

                $"UPDATE PPEntityCollections SET Revision = @rev WHERE ID = {(int)RevEntry.PPUser}; ";
        }

        #endregion

        #region  Отчеты
        /// <summary>
        /// Отчёт по значениям контролей
        /// </summary>
        /// <returns></returns>
        private static string SelectCVR(DateTime From, DateTime To, long DepartmentId, long AssetId)
        {
            return "SELECT A.Id AS AId, " +
                "A.DisplayName AS AName, " +
                "AC.Id AS ACId, " +
                "AC.DisplayName AS ACName, " +
                "AP.Id AS APId, " +
                "AP.DisplayName AS APName, " +
                "SC.Id AS SCId, " +
                "SC.StartTime, " +
                "CASE APV.AssetParameterTypeId " + 
                "WHEN 2 THEN (CASE APV.Value WHEN N'0' THEN N'Норма' WHEN N'1' THEN N'Отклонение' WHEN N'2' THEN N'Сильное отклонение' ELSE N'' END) " +
                "ELSE APV.Value END AS Value " +
                "FROM SchedulingContainers AS SC " +
                "INNER JOIN Departments AS D ON SC.DepartmentId = D.Id " +
                "INNER JOIN Tasks AS T ON T.SchedulingContainerId = SC.Id " +
                "INNER JOIN InspectionProtocols AS IProt ON IProt.TaskId = T.Id AND IProt.IsDeleted <> 1 " +
                "INNER JOIN InspectionProtocolItems AS IProtItems ON IProtItems.InspectionProtocolId = IProt.Id " +
                "INNER JOIN AssetParameterValues AS APV ON APV.Id = IProtItems.AssetParameterValueId " +
                "INNER JOIN Assets AS A ON A.Id = IProt.AssetId " +
                "INNER JOIN Assets AS AC ON AC.Id = APV.AssetId " +
                "INNER JOIN AssetParameters AS AP ON AP.Id = APV.AssetParameterId " +
                $"WHERE SC.StartTime <= '{To.ToString("yyyy-MM-dd HH:mm:ss")}' AND SC.CloseTime >= '{From.ToString("yyyyMMdd HH:mm:ss")}' AND D.Id = {DepartmentId} AND A.Id = {AssetId} AND SC.IsDeleted <> 1 " +
                "ORDER BY SC.StartTime";
        }
        private static string SelectAssetsFromAPS(long APSId)
        {
            return "SELECT DISTINCT A.Id, A.DisplayName " +
                "FROM AssetParameterSetRecords AS APSR " +
                "INNER JOIN Assets AS A ON A.Id = APSR.AssetId AND A.IsDeleted <> 1 " +
                "WHERE APSR.IsDeleted <> 1 " +
                $"AND APSR.AssetParameterSetId = {APSId}";
        }
        private static string SelectAssetChildsFromAsset(long AssetId)
        {
            return "SELECT A.Id, A.DisplayName " +
                "FROM Assets AS A " +
                "WHERE A.IsDeleted <> 1 " +
                $"AND A.ParentId = {AssetId}";
        }
        private static string SelectControlsFromAssetChilds(long AssetChildId)
        {
            return "SELECT AP.Id, AP.DisplayName " +
                "FROM AssetParameterPair AS APP " +
                "INNER JOIN AssetParameters AS AP ON AP.Id = APP.AssetParameterId AND AP.IsDeleted <> 1 " +
                "WHERE APP.IsDeleted <> 1 " +
                $"AND APP.AssetId = {AssetChildId}";
        }
        /// <summary>
        /// Отчет по осмотренным Тех. позициям
        /// </summary>
        /// <returns></returns>
        private static string SelectVA(DateTime From, DateTime To)
        {
            return "SELECT D.Id, D.DisplayName, count(*) AS Rows " +
                "FROM Departments AS D " +
                "LEFT JOIN SchedulingContainers AS SC ON D.Id = SC.DepartmentId AND SC.IsDeleted <> 1 " +
                "LEFT JOIN Tasks AS T ON T.SchedulingContainerId = SC.Id " +
                "LEFT JOIN InspectionProtocols AS IProt ON IProt.TaskId = T.Id AND IProt.IsDeleted <> 1 " +
                "WHERE IProt.ModificationTime IS NOT NULL AND IProt.InspectionProtocolStatusId IS NOT NULL " +
                $"AND SC.StartTime <= '{To.ToString("yyyy-MM-dd HH:mm:ss")}' AND SC.CloseTime >= '{From.ToString("yyyyMMdd HH:mm:ss")}' AND D.IsDeleted <> 1 " +
                "group by D.Id, D.DisplayName ";
        }

        /// <summary>
        /// Отчет по всем Тех. позициям
        /// </summary>
        /// <returns></returns>
        private static string SelectNA(DateTime From, DateTime To)
        {
            return "SELECT D.Id, D.DisplayName, count(*) AS Rows " +
                "FROM Departments AS D " +
                "LEFT JOIN SchedulingContainers AS SC ON D.Id = SC.DepartmentId AND SC.IsDeleted <> 1 " +
                "LEFT JOIN Tasks AS T ON T.SchedulingContainerId = SC.Id " +
                "LEFT JOIN InspectionProtocols AS IProt ON IProt.TaskId = T.Id AND IProt.IsDeleted <> 1 " +
                $"WHERE SC.StartTime <= '{To.ToString("yyyy-MM-dd HH:mm:ss")}' AND SC.CloseTime >= '{From.ToString("yyyy-MM-dd HH:mm:ss")}' AND D.IsDeleted <> 1 " +
                "group by D.Id, D.DisplayName ";
        }
        #endregion


        private static string LoginIsExist(string Login)
        {
            return $"SELECT ISNULL((SELECT TOP 1 ID FROM PPUsers WHERE Name = N'{Login}' AND IsDeleted <> 1),-1);";
        }


        public static string SelectUserData(Credentials data)
        {
            return "SELECT e.Id, e.DepartmentId, PPos.DisplayName AppointName, D.DisplayName DepartmentName, P.GivenName, P.MiddleName, P.Surname, Crend.Name, Crend.RoleCode " +
                "FROM PPUsers Crend " +
                "LEFT JOIN Persons P ON P.UserId = Crend.Id " +
                "LEFT JOIN Engineers E ON E.PersonId = P.Id " +
                "LEFT JOIN Departments D ON D.Id = E.DepartmentId " +
                "LEFT JOIN PersonPositions PPos ON PPos.Id = P.PersonPositionId " +
                $"where Crend.Name = N'{data.UserName}' AND HASHBYTES('SHA2_256',DECRYPTBYPASSPHRASE(N'{ENCR}', Crend.PWD))  = HASHBYTES('SHA2_256',N'{data.Password}') AND Crend.IsDeleted <> 1; ";
        }

        public static string SignInLog()
        {
            return @"INSERT INTO EngineerLoginLog
           ([EngineerId]
           ,[Stamp]
           ,[Platform])
     VALUES
           (@EngineerId,
           @Stamp,
           @Platform)";
        }

        #endregion


        #region Приложения
        private static string GetAttachments(long SCId)
        {
            return "" +
                "SELECT " +
                "AD.Id, " +
                $"{SCId} AS SCId, " +
                "AD.FileName, " +
                "AD.CreationTime, " +
                "AD.ObjectUID AS FileRef, " +
                "IP.Name AS IPName, " +
                "AC.DisplayName AS IPIName, " +
                "AP.DisplayName AS APName " +
                "FROM AttachedData AD " +
                "LEFT JOIN AssetParameterValues APV ON APV.Id = AD.AssetParameterValueId " +
                "LEFT JOIN AssetParameters AP ON AP.Id = APV.AssetParameterId " +
                "LEFT JOIN InspectionProtocolItems IPI ON IPI.AssetParameterValueId = APV.Id " +
                "LEFT JOIN InspectionProtocols IP ON IPI.InspectionProtocolId = IP.Id " +
                "LEFT JOIN Assets AC ON AC.Id = IPI.AssetId " +
                $"WHERE AD.SchedulingContainerId = {SCId};";
        }
        private static string GetAttachment(Guid guid)
        {
            return "" +
                "SELECT " +
                "DC.Data, " +
                "AD.FileName " +
                "FROM AttachedData AD " +
                "INNER JOIN DataContent DC ON AD.DataContentId = DC.Id " +
                $"WHERE ObjectUID = '{guid.ToString("D")}';";
        }
        #endregion

        #region Деффекты
        private static string GetDeffects(long SCId)
        {
            return "" +
                "SELECT " +
                "A.DisplayName AS AName, " +
                "AC.DisplayName AS ACName, " +
                "AP.DisplayName AS APVName, " +
                "E.DisplayName AS UserName, " +
                "CASE APV.AssetParameterTypeId " + //Визуальный
                "WHEN 2 THEN (CASE APV.Value WHEN N'0' THEN N'Норма' WHEN N'1' THEN N'Отклонение' WHEN N'2' THEN N'Сильное отклонение' ELSE N'' END) " +
                "ELSE APV.Value END AS Value, " +
                "APV.DateTime AS Timestamp, " +
                "APV.Comment " +
                "FROM " +
                "SchedulingContainers SC " +
                "LEFT JOIN Tasks AS T ON T.SchedulingContainerId = SC.Id " +
                "LEFT JOIN InspectionProtocols IP ON IP.TaskId = T.Id AND IP.IsDeleted <> 1 " +
                "LEFT JOIN InspectionProtocolItems IPI ON IPI.InspectionProtocolId = IP.Id " +
                "LEFT JOIN Assets A ON A.Id = IP.AssetId " +
                "LEFT JOIN Assets AC ON AC.Id = IPI.AssetId " +
                "LEFT JOIN AssetParameterValues APV ON APV.Id = IPI.AssetParameterValueId " +
                "LEFT JOIN AssetParameters AP ON AP.Id = APV.AssetParameterId " +
                "LEFT JOIN Engineers E on E.Id = APV.ModifiedByUser " +
                $"WHERE SC.Id = { SCId} " +
                "AND APV.Value IS NOT NULL " +
                "AND (" +
                    //Цифровой
                    "(" +
                        "APV.AssetParameterTypeId <> 2 " +
                        "AND " +
                        "(" +
                            //Cильное отклонение
                            "(" +
                                "TRY_CONVERT(float, REPLACE(APV.Value,',','.')) " +
                                "between TRY_CONVERT(float, REPLACE(AP.BottomValue3,',','.')) " +
                                "AND TRY_CONVERT(float, REPLACE(AP.TopValue3,',','.'))" +
                            ")" +
                            //Отклонение
                            "OR " +
                            "(" +
                                "TRY_CONVERT(float, REPLACE(APV.Value,',','.')) " +
                                "between TRY_CONVERT(float, REPLACE(AP.BottomValue2,',','.')) " +
                                "AND TRY_CONVERT(float, REPLACE(AP.TopValue2,',','.'))" +
                            ")" +
                        ") " +
                        "AND " +
                        "NOT (" +
                            "TRY_CONVERT(float, REPLACE(APV.Value,',','.')) " +
                            "between TRY_CONVERT(float, REPLACE(AP.BottomValue1,',','.')) " +
                            "AND TRY_CONVERT(float, REPLACE(AP.TopValue1,',','.'))" +
                        ")" +

                    ") " +
                    "OR " +
                    //Визуальный
                    "(APV.AssetParameterTypeId = 2 AND APV.Value IN (N'1', N'2'))" +
                ") " +
                "order by IP.Id, IPI.AssetId ";
        }
        #endregion

        #region Статистика
        private static string GetStatistics()
        {
            return "SELECT Name, Value FROM TBFN_GET_STATISTICS(@DeptId, @FromD, @ToD) ORDER BY Name";
        }
        /// <summary>
        /// Берет для отчета только завершенные
        /// </summary>
        /// <returns></returns>
        private static string GetStatisticsRTEC2()
        {
            #warning в ручную убраны несколько  должностей, начальники  и еще ктото. переделать на флаг
            return @"
                DECLARE @t TABLE 
                (
                DepId int,
                DepName NVARCHAR(MAX),
                ResName NVARCHAR(MAX),

                ResCountDay int,
                ResCountNight int,
                ResCount int,

                DepCountDay int,
                DepCountNight int,
                DepCount int, 

                ResPlanDay int,
                ResPlanNight int,
                ResPlan int,

                DepPlanDay int,
                DepPlanNight int,
                DepPlan int
                );
                INSERT INTO @t
                SELECT 
                D.Id, 
                D.DisplayName DepName, 
                R.DisplayName ResName,
                (SELECT COUNT(SC.Id) FROM SchedulingRequirements SR 
                LEFT JOIN SchedulingContainers SC ON SC.RequirementId = SR.Id
                WHERE R.Id = SR.RequirementResourceId
                AND SR.AppointmentStart >= @FromD 
                AND SR.AppointmentFinish <= @ToD 
				AND DATEPART(HOUR, SR.AppointmentStart) >= 8
				AND DATEPART(HOUR, SR.AppointmentStart) < 20
                AND SC.SCStatusId = 3) AS ResCountDay,

                (SELECT COUNT(SC.Id) FROM SchedulingRequirements SR 
                LEFT JOIN SchedulingContainers SC ON SC.RequirementId = SR.Id
                WHERE R.Id = SR.RequirementResourceId
                AND SR.AppointmentStart >= @FromD 
                AND SR.AppointmentFinish <= @ToD 
				AND (DATEPART(HOUR, SR.AppointmentStart) < 8
				OR DATEPART(HOUR, SR.AppointmentStart) >= 20)
                AND SC.SCStatusId = 3) AS ResCountNight,

                (SELECT COUNT(SC.Id) FROM SchedulingRequirements SR 
                LEFT JOIN SchedulingContainers SC ON SC.RequirementId = SR.Id
                WHERE R.Id = SR.RequirementResourceId
                AND SR.AppointmentStart >= @FromD 
                AND SR.AppointmentFinish <= @ToD 
                AND SC.SCStatusId = 3) AS ResCount,

                0 AS DepCountDay,
                0 AS DepCountNight,
                0 AS DepCount,

                R.PlanDay AS ResPlanDay,
                R.PlanNight AS ResPlanNight,
                R.PlanDay + R.PlanNight AS ResPlan,

                0 AS DepPlanDay,
                0 AS DepPlanNight,
                0 AS DepPlan 
                
                FROM
                Departments D
                LEFT JOIN Resources R ON R.DepartmentId = D.Id AND R.Id NOT IN (86,90,91)
                WHERE 
                D.Id IN (SELECT Id FROM TBFN_GET_DEPARTMENTS(@DeptId))  
                AND D.IsDeleted <> 1
                AND (R.IsDeleted <> 1 OR  R.IsDeleted IS NULL)

                GROUP BY D.Id, D.DisplayName, R.Id, R.DisplayName, R.PlanDay, R.PlanNight 
                ORDER BY DepName, ResName


                UPDATE t SET 
				t.DepCountDay = (SELECT SUM(ResCountDay) FROM @t WHERE DepId IN (SELECT Id FROM TBFN_GET_DEPARTMENTS(t.DepId))),
				t.DepCountNight = (SELECT SUM(ResCountNight) FROM @t WHERE DepId IN (SELECT Id FROM TBFN_GET_DEPARTMENTS(t.DepId))),
				t.DepCount = (SELECT SUM(ResCount) FROM @t WHERE DepId IN (SELECT Id FROM TBFN_GET_DEPARTMENTS(t.DepId))),
				t.DepPlanDay = (SELECT SUM(ResPlanDay) FROM @t WHERE DepId IN (SELECT Id FROM TBFN_GET_DEPARTMENTS(t.DepId))),
				t.DepPlanNight = (SELECT SUM(ResPlanNight) FROM @t WHERE DepId IN (SELECT Id FROM TBFN_GET_DEPARTMENTS(t.DepId))),
				t.DepPlan = (SELECT SUM(ResPlan) FROM @t WHERE DepId IN (SELECT Id FROM TBFN_GET_DEPARTMENTS(t.DepId)))
                FROM @T t
                
                

                SELECT *, 
                DepCountDay - DepPlanDay AS DepDiffDay, 
                DepCountNight - DepPlanNight AS DepDiffNight, 
                DepCount - DepPlan AS DepDiff, 
                ResCountDay - ResPlanDay AS ResDiffDay, 
                ResCountNight - ResPlanNight AS ResDiffNight, 
                ResCount - ResPlan AS ResDiff 
                FROM @t ORDER BY DepName, ResName
                ";
        }
        #endregion

        #region Дефекты максимо
        private static string SelectMaximoStatuses()
        {
            return "SELECT " +
                "Id, " +
                "DisplayName " +
                "FROM MaximoStatuses " +
                "WHERE IsDeleted <> 1 " +
                "ORDER BY Id";
        }
        private static string SelectMaximoStatistics(long? Id, DateTime? From, DateTime? To, long? DeptId, long? StatusId, long UserDept = -1)
        {
            return "SELECT " +
                "MR.Id, " +
                "D.DisplayName DepartmentName, " +
                "ID.Name TaskName, " +
                "MR.CreationTime, " +
                "MR.CustomAssetCode, " +
                "MR.CustomAssetChildCode, " +
                "MR.CustomDescription, " +
                "MR.MaximoStatusId MaximoStatusId, " +
                "MS.DisplayName MaximoStatus, " +
                "MR.TICKETID, " +
                "MR.TICKETUID, " +
                "MR.LastSend, " +
                "MR.MaximoError " +
                "FROM MaximoReporterSendLog MR " +
                "LEFT JOIN MaximoStatuses MS ON MS.Id = MR.MaximoStatusId " +
                "LEFT JOIN SchedulingContainers SC ON SC.Id = MR.SCId " +
                "LEFT JOIN Tasks T ON T.SchedulingContainerId = MR.SCId " +
                "LEFT JOIN Departments D ON D.Id = SC.DepartmentId " +
                "LEFT JOIN InspectionDocuments ID ON ID.TaskId = T.Id " +
                $"INNER JOIN TBFN_GET_DEPARTMENTS({UserDept}) dps ON dps.Id = SC.DepartmentId " +
                "WHERE 1 = 1 " +
                (Id == null ? "" : $"AND MR.Id = {Id} ") +
                (From == null ? "" : $"AND MR.CreationTime >= '{((DateTime)From).ToString("yyyyMMdd HH:mm:ss")}' ") +
                (To == null ? "" : $"AND MR.CreationTime <= '{((DateTime)To).ToString("yyyyMMdd HH:mm:ss")}' ") +
                (DeptId == null ? "" : $"AND D.Id = {DeptId} ") +
                (StatusId == null ? "" : $"AND MS.Id = {StatusId} ") +
                "ORDER BY MR.Id";
        }
        private static string MaximoStatisticsUpdate()
        {
            return "Update MaximoReporterSendLog " +
                "SET CustomAssetCode = @CustomAssetCode," +
                "CustomAssetChildCode = @CustomAssetChildCode, " +
                "CustomDescription = @CustomDescription, " +
                "ModifiedByUser = @User, " +
                "ModificationTime = getdate() " +
                "WHERE Id = @Id ";
        }
        private static string MaximoStatisticsSend()
        {
            return "Update MaximoReporterSendLog " +
                "SET MaximoStatusId = 2," +
                $"ModifiedByUser = @User, " +
                $"ModificationTime = getdate() " +
                $"WHERE Id = @Id ";
        }
        #endregion

        #region Процедуры получения данных
        #region ЗРП
        public static List<ZRP> GetZRP(DateTime From, DateTime To, ILogger logger, int[] statuses = null, string DisplayName = "", long? DeptId = null, long UserDept = -1)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<ZRP> output = new List<ZRP>();

            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(GetZRP(statuses, DeptId, UserDept), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        command.Parameters.Add(new SqlParameter() { ParameterName = "@DFrom", SqlDbType = SqlDbType.DateTime, Value = From });
                        command.Parameters.Add(new SqlParameter() { ParameterName = "@DTo", SqlDbType = SqlDbType.DateTime, Value = To });
                        command.Parameters.Add(new SqlParameter() { ParameterName = "@DisplayName", SqlDbType = SqlDbType.NVarChar, Value = String.IsNullOrEmpty(DisplayName) ? DBNull.Value : DisplayName });

                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new ZRP()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    StartTime = Convert.ToDateTime(row["AppointmentStart"]),
                                    EndTime = Convert.ToDateTime(row["AppointmentFinish"]),
                                    ObjId = int.Parse(row["ObjId"].ToString()),
                                    ObjName = row["ObjName"].ToString(),
                                    SCTypeId = int.Parse(row["TypeId"].ToString()),
                                    SCTypeName = row["TypeName"].ToString(),
                                    SCStatusId = int.Parse(row["StatusId"].ToString()),
                                    SCStatusName = row["StatusName"].ToString(),
                                    RouteName = row["RouteName"].ToString(),
                                    ResName = row["ResName"].ToString(),
                                    PerformerName = row["PerformerName"].ToString(),
                                    DeffectNum = row["DefectCount"].ToString()
                                });
                            }
                        }
                    }
                }
                //List<Deffect> deffects = null;
                //foreach (var zrp in output)
                //{
                //    deffects = GetDeffects(zrp.Id, logger);
                //    zrp.DeffectNum = (deffects.Count == 0 ? "Нет" : deffects.Count.ToString());
                //}
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных БР");
            }
            return output;
        }

        public static ZRP GetZRP(int ID, long EngId, ILogger logger)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            ZRP output = new ZRP();

            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(GetZRP(), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        command.Parameters.Add(new SqlParameter() { ParameterName = "@Id", SqlDbType = SqlDbType.BigInt, Value = ID });

                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output = new ZRP()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    StartTime = Convert.ToDateTime(row["StartTime"]),
                                    EndTime = Convert.ToDateTime(row["CloseTime"]),
                                    SCTypeId = int.Parse(row["SCTypeId"].ToString()),
                                    SCStatusId = int.Parse(row["SCStatusId"].ToString()),
                                    RouteName = row["Name"].ToString(),
                                    DepartmentID = int.Parse(row["DepartmentId"].ToString()),
                                    ResourceId = int.Parse(row["RequirementResourceId"].ToString()),
                                    ObjectUID = string.IsNullOrEmpty(row["ObjectUID"].ToString()) ? null : new Guid(row["ObjectUID"].ToString())

                                };
                            }
                        }
                    }
                }
                output.InsProt = GetInspectionProtocols(output.Id, EngId, logger);
                output.Attachments = GetAttachments(output.Id, logger);
                output.Deffects = GetDeffects(output.Id, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных БР");
            }
            return output;
        }

        public static void CreateZRP(ZRP obj, long EngId, ILogger logger)
        {
            long SRID = 0, TID = 0, IDocID = 0, SCID = 0;
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return;
            }
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(CreateZRP(obj, EngId), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                SRID = long.Parse(row["SRID"].ToString());
                                TID = long.Parse(row["TID"].ToString());
                                IDocID = long.Parse(row["IDocID"].ToString());
                                SCID = long.Parse(row["SCID"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных БР");
            }

            if (obj.NewTechPoz.Count > 0)
            {
                //AssetId IPItemId
                Dictionary<long, long> IPData = CreateInspectionProtocols(obj, EngId, logger, TID, IDocID);
                List<AssetParametersRowData> APVData = CreateAssetParameters(obj, EngId, logger, IPData);
                CreateInspectionProtocolItems(obj, EngId, logger, IPData, APVData);
            }
        }
        #region Вставка деталей
        private struct AssetParametersRowData
        {
            public long Id;
            public long AssetId;
            public long AssetParameterId;
            public long IPId;
        }
        private static Dictionary<long, long> CreateInspectionProtocols(ZRP obj, long EngId, ILogger logger, long TID, long IDocID)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            Dictionary<long, long> output = new Dictionary<long, long>();

            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(CreateInspectionProtocols(obj, EngId, TID, IDocID), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(long.Parse(row["AssetId"].ToString()), long.Parse(row["IPItemId"].ToString()));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на создании деталей протокола {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        private static List<AssetParametersRowData> CreateAssetParameters(ZRP obj, long EngId, ILogger logger, Dictionary<long, long> IPData)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<AssetParametersRowData> output = new List<AssetParametersRowData>();

            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(CreateAssetParameters(obj, EngId, IPData), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new AssetParametersRowData()
                                {
                                    Id = long.Parse(row["ID"].ToString()),
                                    AssetId = long.Parse(row["AssetId"].ToString()),
                                    AssetParameterId = long.Parse(row["AssetParameterId"].ToString()),
                                    IPId = long.Parse(row["IPId"].ToString()),
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на создании деталей протокола {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        private static void CreateInspectionProtocolItems(ZRP obj, long EngId, ILogger logger, Dictionary<long, long> IPData, List<AssetParametersRowData> APData)
        {
            ExecuteNonQuery(CreateInspectionProtocolItems(obj, EngId, IPData, APData), logger);
        }
        #endregion

        public static void UpdateZRP(ZRP obj, long EngId, ILogger logger)
        {
            ExecuteNonQuery(UpdateZRP(obj, EngId), logger);

            var Details = GetInspectionProtocols(obj.Id, EngId, logger);

            for (int i = 0; i < Details.Count; i++)
            {
                if (obj.InsProt.Find(x => x.Id == Details[i].Id) == null)
                {
                    DeleteInspectionProtocol(Details[i].Id, EngId, logger);
                }
            }

        }
        public static void DeleteZRP(long ID, long EngId, ILogger logger)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter() { ParameterName = "@ID", SqlDbType = SqlDbType.BigInt, Value = ID });
            param.Add(new SqlParameter() { ParameterName = "@EngId", SqlDbType = SqlDbType.BigInt, Value = EngId });
            ExecuteNonQuery(DeleteZRP(), logger,param);
        }
        public static List<InspectionProtocol> GetInspectionProtocols(long ShedContID, long EngId, ILogger logger)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<InspectionProtocol> output = new List<InspectionProtocol>();

            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(GetInspectionProtocols(), connection))
                    {
                        InspectionProtocol Tmp = null;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        command.Parameters.Add(new SqlParameter() { ParameterName = "@SchedContID", SqlDbType = SqlDbType.BigInt, Value = ShedContID });

                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                Tmp = output.Find(x => x.Id == int.Parse(row["Id"].ToString()));
                                if (Tmp == null)
                                {
                                    Tmp = new InspectionProtocol()
                                    {
                                        Id = int.Parse(row["Id"].ToString()),
                                        ObjectUID = string.IsNullOrEmpty(row["ObjectUID"].ToString()) ? null : new Guid(row["ObjectUID"].ToString()),
                                        Name = row["Name"].ToString(),
                                        IsNFC = bool.Parse(row["NFCReceived"].ToString()),
                                        StateId = string.IsNullOrEmpty(row["StateId"].ToString()) ? null : long.Parse(row["StateId"].ToString()),
                                        StateName = row["StateName"].ToString()
                                    };
                                    output.Add(Tmp);
                                }
                                Tmp.Items.Add(new InspectionProtocolItem()
                                {
                                    Name = row["ItemName"].ToString(),
                                    Date = string.IsNullOrEmpty(row["ItemData"].ToString()) ? null : Convert.ToDateTime(row["ItemData"]),
                                    Value = row["ItemValue"].ToString(),
                                    Comment = row["Comment"].ToString(),
                                    IsPen = bool.Parse(row["IsPen"].ToString()),
                                    TextColor = (TextColor) int.Parse(row["TextColor"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных БР {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static void DeleteInspectionProtocol(long InspectionProtocolID, long EngId, ILogger logger)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter() { ParameterName = "@ID", SqlDbType = SqlDbType.BigInt, Value = InspectionProtocolID });
            param.Add(new SqlParameter() { ParameterName = "@EngId", SqlDbType = SqlDbType.BigInt, Value = EngId });
            ExecuteNonQuery(DeleteInspectionProtocol(), logger, param);
        }
        #endregion

        #region Календари
        /// <summary>
        /// Календари
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        [Obsolete]
        public static List<Calendar> GetCalendars(ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<Calendar> output = new List<Calendar>();

            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectCalendars(), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new Models.Calendar()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    Title = row["Title"].ToString(),
                                    ObjectUID = new Guid(row["ObjectUID"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        #endregion

        #region Статусы
        /// <summary>
        /// Календари
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<SCStatus> GetSCStatuses(ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<SCStatus> output = new List<SCStatus>();

            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectSCStatuses(), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new Models.SCStatus()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString(),
                                    ObjectUID = new Guid(row["ObjectUID"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        #endregion

        #region Типы
        /// <summary>
        /// Типы
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<SCTypes> GetSCTypes(ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<SCTypes> output = new List<SCTypes>();

            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectSCTypes(), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new SCTypes()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString(),
                                    ObjectUID = new Guid(row["ObjectUID"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        #endregion

        #region Типы ТО
        /// <summary>
        /// Типы ТО
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<TOType> GetTOTypes(ILogger logger)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<TOType> output = new List<TOType>();

            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectTOTypes(), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new TOType()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    Name = row["Name"].ToString(),
                                    DisplayName = row["DisplayName"].ToString(),
                                    ObjectUID = new Guid(row["ObjectUID"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static TOType GetTOType(int ID, ILogger logger)
        {
            TOType output = new TOType();
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectTOTypes(), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output = new TOType()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    Name = row["Name"].ToString(),
                                    DisplayName = row["DisplayName"].ToString(),
                                    ObjectUID = new Guid(row["ObjectUID"].ToString())
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        #endregion

        #region Контроли
        /// <summary>
        /// Контроли
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<Control> GetControls(long? AssetParameterTypeId = null, string DisplayName = "", long? DeptId = null, ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<Control> output = new List<Control>();

            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectControls(), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        command.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.BigInt, ParameterName = "@ID", Value = DBNull.Value });
                        command.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.BigInt, ParameterName = "@AssetParameterTypeId", Value = (object)AssetParameterTypeId?? DBNull.Value });
                        command.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.NVarChar, ParameterName = "@DisplayName", Value = String.IsNullOrEmpty(DisplayName)? DBNull.Value : DisplayName });
                        command.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.BigInt, ParameterName = "@DeptId", Value = (object)DeptId ?? DBNull.Value });
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new Control()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString(),
                                    ObjectUID = new Guid(row["ObjectUID"].ToString()),
                                    DisplayValueType = row["ValueTypeName"].ToString(),
                                    AssetParameterTypeId = long.Parse(row["AssetParameterTypeId"].ToString()),
                                    Description = row["Description"].ToString(),
                                    CreateStamp = Convert.ToDateTime(row["CreationTime"].ToString()),
                                    ModifyStamp = Convert.ToDateTime(row["ModificationTime"].ToString()),
                                    DepartmentName = row["DepartmentName"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }

        public static Control GetControl(long ID, ILogger logger)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            Control output = new Control();
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectControls(), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        command.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.BigInt, ParameterName = "@ID", Value = ID });
                        command.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.BigInt, ParameterName = "@AssetParameterTypeId", Value = DBNull.Value });
                        command.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.NVarChar, ParameterName = "@DisplayName", Value = DBNull.Value });
                        command.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.BigInt, ParameterName = "@DeptId", Value = DBNull.Value });
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output = new Control()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString(),
                                    ObjectUID = new Guid(row["ObjectUID"].ToString()),
                                    DisplayValueType = row["ValueTypeName"].ToString(),
                                    ValueBottom1 = row["BottomValue1"].ToString(),
                                    ValueTop1 = row["TopValue1"].ToString(),
                                    ValueBottom2 = row["BottomValue2"].ToString(),
                                    ValueTop2 = row["TopValue2"].ToString(),
                                    ValueBottom3 = row["BottomValue3"].ToString(),
                                    ValueTop3 = row["TopValue3"].ToString(),
                                    AssetParameterTypeId = long.Parse(row["AssetParameterTypeId"].ToString()),
                                    DepartmentId = (string.IsNullOrEmpty(row["DepartmentId"].ToString()) ? null : long.Parse(row["DepartmentId"].ToString())),
                                    Description = row["Description"].ToString(),
                                    DepartmentName = row["DepartmentName"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }

        /// <summary>
        /// Типы контролей
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<AssetParameterType> GetAssetParameterTypes(ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger?.LogDebug("connection string is null or empty");
                return null;
            }

            List<AssetParameterType> output = new List<AssetParameterType>();

            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectAssetParameterTypes(), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new AssetParameterType()
                                {
                                    Id = long.Parse(row["Id"].ToString()),
                                    Name = row["Name"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static void CreateControl(Control obj, long EngId, ILogger logger)
        {
            ExecuteNonQuery(СreateControl(obj, EngId), logger);
        }
        public static void UpdateControl(Control obj, long EngId, ILogger logger)
        {
            ExecuteNonQuery(UpdateControl(obj, EngId), logger);
        }
        public static void DeleteControl(int ID, long EngId, ILogger logger)
        {
            ExecuteNonQuery(DeleteControl(ID, EngId), logger);
        }
        public static List<ControlStatistic> GetControlStatistic(long? DepartmentId, long? AssetParameterSetId, long? AssetId, long? AssetChildId, long? AssetParameterId, DateTime From, DateTime To, ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger?.LogDebug("connection string is null or empty");
                return null;
            }

            List<ControlStatistic> output = new List<ControlStatistic>();
            if (AssetParameterId == null) return output;
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectControlStatistic(DepartmentId, AssetParameterSetId, AssetId, AssetChildId, AssetParameterId, From, To), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new ControlStatistic()
                                {
                                    y = double.Parse(row["Value"].ToString(), System.Globalization.CultureInfo.InvariantCulture),
                                    x = DateTimeHelper.TicksFromNETToJS(Convert.ToDateTime(row["Stamp"]).Ticks)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        #endregion

        #region Маршруты
        /// <summary>
        /// Маршруты
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<AssetParameterSet> GetAssetParameterSets(long UserDept = -1, ILogger logger = null, int? DeptID = -1)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger?.LogDebug("connection string is null or empty");
                return null;
            }

            List<AssetParameterSet> output = new List<AssetParameterSet>();

            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectAssetParameterSets(DeptId: DeptID, UserDept: UserDept), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new AssetParameterSet()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString(),
                                    ObjectUID = new Guid(row["ObjectUID"].ToString()),
                                    DepartmentName = row["DepartmentName"].ToString(),
                                    DepartmentID = string.IsNullOrEmpty(row["DepartmentID"].ToString()) ? 0 : int.Parse(row["DepartmentID"].ToString()),
                                    CreateStamp = Convert.ToDateTime(row["CreationTime"].ToString()),
                                    ModifyStamp = Convert.ToDateTime(row["ModificationTime"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static AssetParameterSet GetAssetParameterSet(int ID, ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger?.LogDebug("connection string is null or empty");
                return null;
            }

            AssetParameterSet output = new AssetParameterSet();
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectAssetParameterSets(ID: ID), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output = new AssetParameterSet()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString(),
                                    ObjectUID = new Guid(row["ObjectUID"].ToString()),
                                    DepartmentName = row["DepartmentName"].ToString(),
                                    DepartmentID = string.IsNullOrEmpty(row["DepartmentID"].ToString()) ? 0 : int.Parse(row["DepartmentID"].ToString())
                                };
                            }
                        }
                    }
                }
                output.Childs = GetAssetParameterSetDetails(output.Id, logger);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static void CreateAssetParameterSet(AssetParameterSet obj, long EngId, ILogger logger)
        {
            string APID = ExecuteScalar(CreateAssetParameterSet(obj, EngId), logger);
            for (int i = 0; i < obj.Childs.Count; i++)
            {
                for (int j = 0; j < obj.Childs[i].Childs.Count; j++)
                {
                    for (int k = 0; k < obj.Childs[i].Childs[j].Childs.Count; k++)
                    {
                        CreateAssetParameterSetRecord(long.Parse(APID), obj.Childs[i].Id, obj.Childs[i].Childs[j].Id, obj.Childs[i].Childs[j].Childs[k].Id, EngId, logger);
                    }
                }
            }
        }
        public static void UpdateAssetParameterSet(AssetParameterSet obj, long EngId, ILogger logger)
        {
            ExecuteNonQuery(UpdateAssetParameterSet(obj, EngId), logger);
            var currentData = GetAssetParameterSetRecords(obj.Id, logger);

            //Добавляем  новые
            for (int i = 0; i < obj.Childs.Count; i++)
            {
                for (int j = 0; j < obj.Childs[i].Childs.Count; j++)
                {
                    for (int k = 0; k < obj.Childs[i].Childs[j].Childs.Count; k++)
                    {
                        if (currentData.Find(x => x.AParentId == obj.Childs[i].Id && x.AChildId == obj.Childs[i].Childs[j].Id && x.ParamId == obj.Childs[i].Childs[j].Childs[k].Id) == null)
                        {
                            CreateAssetParameterSetRecord(obj.Id, obj.Childs[i].Id, obj.Childs[i].Childs[j].Id, obj.Childs[i].Childs[j].Childs[k].Id, EngId, logger);
                        }
                    }
                }
            }
            //Удаляем убранные пользователем.
            for (int i = 0; i < currentData.Count; i++)
            {
                Hierarchy AP = null;
                Hierarchy AC = null;
                Hierarchy P = null;
                AP = obj.Childs.Find(x => x.Id == currentData[i].AParentId);
                if (AP == null)
                {
                    DeleteAssetParameterSetRecord(currentData[i].APSRId, EngId, logger);
                    continue;
                }
                AC = AP.Childs.Find(x => x.Id == currentData[i].AChildId);
                if (AC == null)
                {
                    DeleteAssetParameterSetRecord(currentData[i].APSRId, EngId, logger);
                    continue;
                }
                P = AC.Childs.Find(x => x.Id == currentData[i].ParamId);
                if (P == null)
                {
                    DeleteAssetParameterSetRecord(currentData[i].APSRId, EngId, logger);
                    continue;
                }

            }
        }
        public static void DeleteAssetParameterSet(int ID, long EngId, ILogger logger)
        {
            ExecuteNonQuery(DeleteAssetParameterSet(ID, EngId), logger);
        }

        /// <summary>
        /// Создание деталей маршрута
        /// </summary>
        /// <param name="APSId"></param>
        /// <param name="AParId"></param>
        /// <param name="AChildId"></param>
        /// <param name="ParId"></param>
        /// <param name="UserName"></param>
        /// <param name="logger"></param>
        public static void CreateAssetParameterSetRecord(long APSId, long AParId, long AChildId, long ParId, long EngId, ILogger logger)
        {
            ExecuteNonQuery(CreateAssetParameterSetDetails(APSId, AParId, AChildId, ParId, EngId), logger);
        }
        /// <summary>
        /// Удаление деталей маршрута.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="UserName"></param>
        /// <param name="logger"></param>
        public static void DeleteAssetParameterSetRecord(long ID, long EngId, ILogger logger)
        {
            ExecuteNonQuery(DeleteAssetParameterSetDetails(ID, EngId), logger);
        }

        /// <summary>
        /// Данные по техпозициям для нового обхода
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<Hierarchy> GetAssetParameterSetDetails(int ID, ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger?.LogDebug("connection string is null or empty");
                return null;
            }

            List<Hierarchy> output = new List<Hierarchy>();
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectAssetParameterSetDetails(ID), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            Hierarchy A = null;
                            Hierarchy AC = null;
                            Hierarchy AP = null;
                            foreach (var row in dataReader.Select(row => row))
                            {
                                A = output.Find(x => x.Id == int.Parse(row["AssetId"].ToString()));
                                if (A == null)
                                {
                                    A = new Hierarchy() { Id = int.Parse(row["AssetId"].ToString()), DisplayName = row["AssetName"].ToString() };
                                    output.Add(A);
                                }
                                AC = A.Childs.Find(x => x.Id == int.Parse(row["AssetComponentId"].ToString()));
                                if (AC == null)
                                {
                                    AC = new Hierarchy() { Id = int.Parse(row["AssetComponentId"].ToString()), DisplayName = row["AssetComponentName"].ToString() };
                                    A.Childs.Add(AC);
                                }
                                AP = AC.Childs.Find(x => x.Id == int.Parse(row["AssetParameterId"].ToString()));
                                if (AP == null)
                                {
                                    AP = new Hierarchy() { Id = int.Parse(row["AssetParameterId"].ToString()), DisplayName = row["AssetParameterName"].ToString() };
                                    AC.Childs.Add(AP);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        /// <summary>
        /// Данные по техпозициям проверки
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private static List<AssetParameterSetDataSet> GetAssetParameterSetRecords(long APSId, ILogger logger)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<AssetParameterSetDataSet> output = new List<AssetParameterSetDataSet>();
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    using (var command = new SqlCommand(GetAssetParameterSetRecords(APSId), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new AssetParameterSetDataSet()
                                {
                                    APSRId = long.Parse(row["Id"].ToString()),
                                    AParentId = long.Parse(row["AssetId"].ToString()),
                                    AChildId = long.Parse(row["AssetComponentId"].ToString()),
                                    ParamId = long.Parse(row["AssetParameterId"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        private class AssetParameterSetDataSet
        {
            public long APSRId;
            public long AParentId;
            public long AChildId;
            public long ParamId;
        }
        #endregion

        #region Классы объектов
        /// <summary>
        /// Классы объектов
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<AssetClass> GetAssetClasses(string DisplayName = "", ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<AssetClass> output = new List<AssetClass>();

            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectAssetClass(DisplayName: DisplayName), connection)) 
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new AssetClass()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString(),
                                    ObjectUID = new Guid(row["ObjectUID"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static AssetClass GetAssetClass(int ID, ILogger logger)
        {
            AssetClass output = new AssetClass();
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectAssetClass(ID), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output = new AssetClass()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString(),
                                    ObjectUID = new Guid(row["ObjectUID"].ToString())
                                };
                            }
                        }
                    }
                }
                foreach (var item in GetAssetClassParameters(output.Id, logger))
                {
                    output.Parameters.Add(item.AssetParameterId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }

        public static void CreateAssetClass(AssetClass obj, long EngId, ILogger logger)
        {
            var idnew = ExecuteScalar(CreateAssetClass(obj, EngId), logger);

            foreach (var item in obj.Parameters)
            {
                CreateAssetClassParameter(new AssetClassParameter() { AssetClassId = int.Parse(idnew), AssetParameterId = item }, EngId, logger);
            }
        }
        public static void UpdateAssetClass(AssetClass obj, long EngId, ILogger logger)
        {
            ExecuteNonQuery(UpdateAssetClass(obj, EngId), logger);
            var currPar = GetAssetClassParameters(obj.Id);
            foreach (var item in obj.Parameters)
            {
                if (currPar.Find(x => x.AssetParameterId == item) == null)
                    CreateAssetClassParameter(new AssetClassParameter() { AssetClassId = obj.Id, AssetParameterId = item }, EngId, logger);
            }
            foreach (var item in currPar)
            {
                if (!obj.Parameters.Contains(item.AssetParameterId))
                    DeleteAssetClassParameter(item.Id, EngId, logger);
            }
        }
        public static void DeleteAssetClass(int ID, long EngId, ILogger logger)
        {
            ExecuteNonQuery(DeleteAssetClass(ID, EngId), logger);
        }
        /// <summary>
        /// Контроли в классах объектов
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<AssetClassParameter> GetAssetClassParameters(int AssetClassId, ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<AssetClassParameter> output = new List<AssetClassParameter>();

            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectAssetClassParameters(AssetClassId), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new AssetClassParameter()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    AssetClassId = int.Parse(row["AssetClassId"].ToString()),
                                    AssetParameterId = int.Parse(row["AssetParameterId"].ToString()),
                                    AssetParameterName = row["AssetParameterName"].ToString(),
                                    ObjectUID = new Guid(row["ObjectUID"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static void CreateAssetClassParameter(AssetClassParameter obj, long EngId, ILogger logger)
        {
            ExecuteNonQuery(CreateAssetClassParameter(obj, EngId), logger);
        }
        public static void DeleteAssetClassParameter(int ID, long EngId, ILogger logger)
        {
            ExecuteNonQuery(DeleteAssetClassParameter(ID, EngId), logger);
        }


        #endregion

        #region Должность
        /// <summary>
        /// Должность
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<PersonPosition> GetPersonPositions(ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<PersonPosition> output = new List<PersonPosition>();

            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectPersonPositions(), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new PersonPosition()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString(),
                                    ObjectUID = new Guid(row["ObjectUID"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static PersonPosition GetPersonPosition(int ID, ILogger logger)
        {
            PersonPosition output = new PersonPosition();
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectPersonPositions(ID), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output = new PersonPosition()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    Name = row["Name"].ToString(),
                                    DisplayName = row["DisplayName"].ToString(),
                                    ObjectUID = new Guid(row["ObjectUID"].ToString())
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }

        public static void CreatePersonPosition(PersonPosition obj, long EngId, ILogger logger)
        {
            ExecuteNonQuery(CreatePersonPosition(obj, EngId), logger);
        }
        public static void UpdatePersonPosition(PersonPosition obj, long EngId, ILogger logger)
        {
            ExecuteNonQuery(UpdatePersonPosition(obj, EngId), logger);
        }
        public static void DeletePersonPosition(int ID, long EngId, ILogger logger)
        {
            ExecuteNonQuery(DeletePersonPosition(ID, EngId), logger);
        }
        #endregion

        #region Категория персонала
        /// <summary>
        /// Категория персонала
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<PersonCategory> GetPersonCategories(ILogger logger)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<PersonCategory> output = new List<PersonCategory>();

            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectPersonCategories(), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new PersonCategory()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString(),
                                    ObjectUID = new Guid(row["ObjectUID"].ToString()),
                                    ParentId = string.IsNullOrEmpty(row["ParentId"].ToString()) ? (int?)null : int.Parse(row["ParentId"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static PersonCategory GetPersonCategory(int ID, ILogger logger)
        {
            PersonCategory output = new PersonCategory();
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectPersonCategories(ID), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output = new PersonCategory()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    Name = row["Name"].ToString(),
                                    DisplayName = row["DisplayName"].ToString(),
                                    ObjectUID = new Guid(row["ObjectUID"].ToString()),
                                    ParentId = string.IsNullOrEmpty(row["ParentId"].ToString()) ? (int?)null : int.Parse(row["ParentId"].ToString())
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static void CreatePersonCategory(PersonCategory obj, long EngId, ILogger logger)
        {
            ExecuteNonQuery(CreatePersonCategory(obj, EngId), logger);
        }
        public static void UpdatePersonCategory(PersonCategory obj, long EngId, ILogger logger)
        {
            ExecuteNonQuery(UpdatePersonCategory(obj, EngId), logger);
        }
        public static void DeletePersonCategory(int ID, long EngId, ILogger logger)
        {
            ExecuteNonQuery(DeletePersonCategory(ID, EngId), logger);
        }
        #endregion

        #region Объекты
        /// <summary>
        /// Объекты
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<Asset> GetAssets(ILogger logger = null, int? DeptID = -1, string DisplayName = null, bool NotesOnly = false, long UserDept = -1)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger?.LogDebug("connection string is null or empty");
                return null;
            }

            List<Asset> output = new List<Asset>();
            List<Asset> all = new List<Asset>();

            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectAssets(DeptID, NotesOnly, UserDept), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                all.Add(new Asset()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString(),
                                    ObjectUID = new Guid(row["ObjectUID"].ToString()),
                                    Description = row["Description"].ToString(),
                                    Maximo = row["ExternalId"].ToString(),
                                    Status = row["StateName"].ToString(),
                                    ParentId = string.IsNullOrEmpty(row["ParentId"].ToString()) ? (int?)null : int.Parse(row["ParentId"].ToString()),
                                    CreateStamp = Convert.ToDateTime(row["CreationTime"].ToString()),
                                    ModifyStamp = Convert.ToDateTime(row["ModificationTime"].ToString()),
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }

            Asset temp = null;
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].ParentId != null)
                {
                    temp = all.Find(x => x.Id == all[i].ParentId);
                    if (temp != null)
                    {
                        if (temp.Childs is null) temp.Childs = new List<Asset>();
                        temp.Childs.Add(all[i]);
                    }
                }
            }
            foreach (var item in all.FindAll(x => 
            x.ParentId == null && 
            (string.IsNullOrEmpty(DisplayName) 
            || x.DisplayName.ToLower().Contains(DisplayName.ToLower()))))
            {
                output.Add(item);
            }
            return output;
        }

        public static Asset GetAssets(int ID, ILogger logger)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            Asset output = new Asset();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectAsset(ID), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output = new Asset()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString(),
                                    ObjectUID = new Guid(row["ObjectUID"].ToString()),
                                    Maximo = row["ExternalId"].ToString(),
                                    ParentId = string.IsNullOrEmpty(row["ParentId"].ToString()) ? (int?)null : int.Parse(row["ParentId"].ToString()),
                                    DepartmentId = string.IsNullOrEmpty(row["DepartmentId"].ToString()) ? (int?)null : int.Parse(row["DepartmentId"].ToString()),
                                    AssetClassId = string.IsNullOrEmpty(row["AssetClassId"].ToString()) ? (int?)null : int.Parse(row["AssetClassId"].ToString()),
                                    AssetSortId = string.IsNullOrEmpty(row["AssetSortId"].ToString()) ? (int?)null : int.Parse(row["AssetSortId"].ToString()),
                                    AssetStateId = string.IsNullOrEmpty(row["AssetStateId"].ToString()) ? (int?)null : int.Parse(row["AssetStateId"].ToString())
                                };
                            }
                        }
                    }
                }
                foreach (var item in GetAssetParameterForAsset((long)output.DepartmentId, output.Id, logger))
                {
                    output.Parameters.Add(item);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static List<Hierarchy> GetSimpleAssets(int? Id, int? DeptId, ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger?.LogDebug("connection string is null or empty");
                return null;
            }

            List<Hierarchy> output = new List<Hierarchy>();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(GetSimpleAsset(Id, DeptId), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            Hierarchy A = null;
                            Hierarchy AC = null;
                            Hierarchy AP = null;
                            foreach (var row in dataReader.Select(row => row))
                            {
                                if (!string.IsNullOrEmpty(row["ParentId"].ToString()))
                                {
                                    A = output.Find(x => x.Id == int.Parse(row["ParentId"].ToString()));
                                    if (A == null)
                                    {
                                        A = new Hierarchy() { Id = int.Parse(row["ParentId"].ToString()), DisplayName = row["ParentName"].ToString() };
                                        output.Add(A);
                                    }
                                }
                                if (!string.IsNullOrEmpty(row["ChildId"].ToString()))
                                {
                                    AC = A.Childs.Find(x => x.Id == int.Parse(row["ChildId"].ToString()));
                                    if (AC == null)
                                    {
                                        AC = new Hierarchy() { Id = int.Parse(row["ChildId"].ToString()), DisplayName = row["ChildName"].ToString() };
                                        A.Childs.Add(AC);
                                    }
                                }
                                if (!string.IsNullOrEmpty(row["ParamId"].ToString()))
                                {
                                    AP = AC.Childs.Find(x => x.Id == int.Parse(row["ParamId"].ToString()));
                                    if (AP == null)
                                    {
                                        AP = new Hierarchy() { Id = int.Parse(row["ParamId"].ToString()), DisplayName = row["ParamName"].ToString() };
                                        AC.Childs.Add(AP);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static void CreateAsset(Asset obj, long EngId, ILogger logger)
        {
            //Родитель и класс объектов только у узлов.
            if (obj.AssetSortId == 1) { obj.ParentId = null; obj.AssetClassId = null; obj.Parameters = null; }
            //Статус только  для тех позиции
            else { obj.AssetStateId = null; }
            long id = long.Parse(ExecuteScalar(CreateAsset(obj, EngId), logger));
            if (obj.Parameters != null)
            {
                foreach (var item in obj.Parameters.FindAll(x => x.IsPair))
                {
                    if(item.GroupNum!= "-" && item.ObjectUID== null)
                    {
                        Guid gd = Guid.NewGuid();
                        foreach (var g in obj.Parameters.FindAll(x => x.GroupNum == item.GroupNum))
                        {
                            g.ObjectUID = gd;
                        }
                    }
                }
                foreach (var item in obj.Parameters.FindAll(x=>x.IsPair))
                {
                    CreateAssetParameterPair(id, item, logger);
                }
            }
        }
        public static void UpdateAsset(Asset obj, long EngId, ILogger logger)
        {
            ExecuteNonQuery(UpdateAsset(obj, EngId), logger);

            foreach (var item in obj.Parameters.FindAll(x => x.IsPair))
            {
                if (item.GroupNum != "-" && item.ObjectUID == null)
                {
                    Guid gd = Guid.NewGuid();
                    foreach (var g in obj.Parameters.FindAll(x => x.GroupNum == item.GroupNum))
                    {
                        g.ObjectUID = gd;
                    }
                }
            }

            var pairs = GetAssetParameterForAsset((long)obj.DepartmentId, obj.Id, logger);
            foreach (var item in pairs.FindAll(x=>x.IsPair))
            {
                if (obj.Parameters.Find(x=>x.IsPair && x.AssetParameterId == item.AssetParameterId)== null)
                    DeleteAssetParameterPair((long)item.PairId, logger);
            }
            foreach (var item in obj.Parameters.FindAll(x=>x.IsPair && x.PairId==null))
            {
                CreateAssetParameterPair(obj.Id, item, logger);
            }
            foreach (var item in obj.Parameters.FindAll(x => x.IsPair && x.PairId != null))
            {
                UpdateAssetParameterPair(obj.Id, item, logger);
            }
        }
        public static void DeleteAsset(int ID, long EngId, ILogger logger)
        {
            ExecuteNonQuery(DeleteAsset(ID, EngId), logger);
        }

        public static List<AssetControl> GetAssetParameterForAsset(long DeptId, long AssetId = -1, ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger?.LogDebug("connection string is null or empty");
                return null;
            }
            List<AssetControl> output = new List<AssetControl>();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectAssetParameterForAsset(DeptId, AssetId), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(
                                    new AssetControl()
                                    {
                                        AssetParameterId = long.Parse(row["Id"].ToString()),
                                        DisplayName = $"{row["DisplayName"].ToString()} {row["DeptName"].ToString()}",
                                        IsPair = row["IsPair"].ToString() == "1",
                                        PairId = string.IsNullOrEmpty(row["PairId"].ToString())? null : long.Parse(row["PairId"].ToString()),
                                        ObjectUID = string.IsNullOrEmpty(row["ObjectUID"].ToString())? null : new Guid(row["ObjectUID"].ToString())
                                    });
                            }
                        }
                    }
                }
                //Расставляем группы
                int counter = 1;
                foreach (var item in output)
                {
                    if(item.ObjectUID!= null && item.GroupNum == "-")
                    {
                        if(output.FindAll(x=>x.ObjectUID == item.ObjectUID).Count>1)
                        {
                            foreach (var groupitem in output.FindAll(x => x.ObjectUID == item.ObjectUID))
                            {
                                groupitem.GroupNum = counter.ToString();
                            }
                            counter++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }

        public static void CreateAssetParameterPair(long AssetId, AssetControl Control, ILogger logger)
        {
            ExecuteNonQuery(CreateAssetParameterPair(AssetId, Control), logger);
        }
        public static void UpdateAssetParameterPair(long AssetId, AssetControl Control, ILogger logger)
        {
            ExecuteNonQuery(UpdateAssetParameterPair(AssetId, Control), logger);
        }
        public static void DeleteAssetParameterPair(long AssetParameterPairId, ILogger logger)
        {
            ExecuteNonQuery(DeleteAssetParameterPair(AssetParameterPairId), logger);
        }
        public static List<AssetType> GetAssetTypes(ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<AssetType> output = new List<AssetType>();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectAssetTypes(), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new AssetType()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString(),
                                    ObjectUID = new Guid(row["ObjectUID"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static List<AssetState> GetAssetStates(ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<AssetState> output = new List<AssetState>();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectAssetStates(), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new AssetState()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static List<AssetSort> GetAssetSorts(ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<AssetSort> output = new List<AssetSort>();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectAssetSorts(), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new AssetSort()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString(),
                                    ObjectUID = new Guid(row["ObjectUID"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        #endregion

        #region Участки
        /// <summary>
        /// Подразделения
        /// </summary>
        /// <param name="RootDepartId">Базовое подразделение</param>
        /// <param name="ClearList">Без вложенности</param>
        /// <param name="ForDept">Удаляет дочерние подразделения указанного</param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<Department> GetDepartments(long? RootDepartId = null, bool ClearList = false, int ForDept = -1, ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<Department> output = new List<Department>();
            List<Department> all = new List<Department>();

            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectDepartments(), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        command.Parameters.Add(new SqlParameter() { ParameterName = "@RootDeptId", SqlDbType = SqlDbType.BigInt, Value = RootDepartId });
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                all.Add(new Department()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString(),
                                    ParentId = (string.IsNullOrEmpty(row["ParentId"].ToString()) ? null : int.Parse(row["ParentId"].ToString()))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            if (ClearList)
            {
                if (ForDept > -1)
                    return removeChilds(all, ForDept);
                else
                    return all;
            }
            else
            {
                Department temp = null;
                for (int i = 0; i < all.Count; i++)
                {
                    if (all[i].ParentId != null)
                    {
                        temp = all.Find(x => x.Id == all[i].ParentId);
                        if (temp != null)
                        {
                            if (temp.Childs is null) temp.Childs = new List<Department>();
                            temp.Childs.Add(all[i]);
                        }
                    }
                }
                foreach (var item in all.FindAll(x => x.ParentId == null))
                {
                    output.Add(item);
                }
            }
            return output;
        }
        private static List<Department> removeChilds(List<Department> list, int id)
        {
            foreach (var item in list.FindAll(x => x.ParentId == id).ToArray())
            {
                removeChilds(list, item.Id);
                list.Remove(item);
            }
            return list;
        }

        public static List<Department> GetChildDepartments(long? RootDepartId = null, ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<Department> output = new List<Department>();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectDepartments(), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        command.Parameters.Add(new SqlParameter() { ParameterName = "@RootDeptId", SqlDbType = SqlDbType.BigInt, Value = RootDepartId });

                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new Department()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString(),
                                    ParentId = (string.IsNullOrEmpty(row["ParentId"].ToString()) ? null : int.Parse(row["ParentId"].ToString()))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }

        public static Department GetDepartment(int DeptId, ILogger logger)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;

            }
            Department output = new Department();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectDepartment(), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        command.Parameters.Add(new SqlParameter() { ParameterName = "@DeptId", SqlDbType = SqlDbType.BigInt, Value = DeptId });
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output = new Department()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString(),
                                    ObjectUID = new Guid(row["ObjectUID"].ToString()),
                                    ParentId = (string.IsNullOrEmpty(row["ParentId"].ToString()) ? null : int.Parse(row["ParentId"].ToString())),
                                    Name = row["Name"].ToString(),
                                    ShortCode = row["ShortCode"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }

        public static void CreateDepartment(Department obj, long EngId, ILogger logger)
        {
            ExecuteNonQuery(CreateDepartment(obj, EngId), logger);
        }
        public static void UpdateDepartment(Department obj, long EngId, ILogger logger)
        {
            ExecuteNonQuery(UpdateDepartment(obj, EngId), logger);
        }
        public static void DeleteDepartment(int ID, long EngId, ILogger logger)
        {
            ExecuteNonQuery(DeleteDepartment(ID, EngId), logger);
        }
        #endregion

        #region Бригады
        /// <summary>
        /// Бригады
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<Resource> GetResources(long? Id = null, long? DeptId = null, ILogger logger = null, long UserDept = -1)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<Resource> output = new List<Resource>();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectResources(Id, DeptId, UserDept), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new Resource()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString(),
                                    ObjectUID = new Guid(row["ObjectUID"].ToString()),
                                    CreateStamp = Convert.ToDateTime(row["CreationTime"].ToString()),
                                    ModifyStamp = Convert.ToDateTime(row["ModificationTime"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static Resource GetResource(int ID, ILogger logger)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            Resource output = new Resource();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectResources(ID: ID), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output = new Resource()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString(),
                                    ObjectUID = new Guid(row["ObjectUID"].ToString()),
                                    DepartmentId = string.IsNullOrEmpty(row["DepartmentId"].ToString()) ? (int?)null : int.Parse(row["DepartmentId"].ToString()),
                                    CreateStamp = Convert.ToDateTime(row["CreationTime"].ToString()),
                                    ModifyStamp = Convert.ToDateTime(row["ModificationTime"].ToString())
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static void CreateResource(Resource obj, long EngId, ILogger logger)
        {
            var idnew = ExecuteScalar(CreateResource(obj, EngId), logger);
            foreach (var SelectedItem in obj.Engineers.FindAll(x=>x.Selected))
            {
                CreateResourceAllocation(new ResourceAllocations() { ResourceID = int.Parse(idnew), EngineerID = SelectedItem.TargetObject.Id }, EngId, logger);
            }
        }
        public static void UpdateResource(Resource obj, long UserId, ILogger logger)
        {
            ExecuteNonQuery(UpdateResource(obj, UserId), logger);

            foreach (var SelectedItem in obj.Engineers.FindAll(x=>!x.BaseSelected && x.Selected))
            {
                CreateResourceAllocation(new ResourceAllocations() { ResourceID = obj.Id, EngineerID = SelectedItem.TargetObject.Id }, UserId, logger);
            }
            foreach (var UnselectedItem in obj.Engineers.FindAll(x => x.BaseSelected && !x.Selected))
            {
                DeleteResourceAllocation((long)UnselectedItem.MatchId, UserId, logger);
            }
        }
        public static void DeleteResource(int ID, long EngId, ILogger logger)
        {
            ExecuteNonQuery(DeleteResource(ID, EngId), logger);
        }
        public static List<ResourceState> GetResourceStates(ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<ResourceState> output = new List<ResourceState>();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectResourceStates(), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new ResourceState()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString(),
                                    ObjectUID = new Guid(row["ObjectUID"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static List<Selectable<Engineer>> GetResourceEngineers(long DepartmentId, long ResourceId, ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<Selectable<Engineer>> output = new List<Selectable<Engineer>>();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(_SelectResourceEngineers(DepartmentId, ResourceId), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new Selectable<Engineer>()
                                {
                                    TargetObject = new Engineer() 
                                    {
                                        Id = long.Parse(row["Id"].ToString()),
                                        DisplayName = row["DisplayName"].ToString()
                                    },
                                    MatchId = string.IsNullOrEmpty(row["ResEngId"].ToString())? null: long.Parse(row["ResEngId"].ToString()),
                                    BaseSelected = row["Selected"].ToString() == "1",
                                    Selected = row["Selected"].ToString() == "1"
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static void CreateResourceAllocation(ResourceAllocations obj, long EngId, ILogger logger)
        {
            ExecuteNonQuery(CreateResourceAllocation(obj, EngId), logger);
        }
        public static void DeleteResourceAllocation(long ID, long EngId, ILogger logger)
        {
            ExecuteNonQuery(DeleteResourceAllocation(ID, EngId), logger);
        }
        public static List<ResourceType> GetResourceTypes(ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<ResourceType> output = new List<ResourceType>();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectResourceTypes(), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new ResourceType()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString(),
                                    ObjectUID = new Guid(row["ObjectUID"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        #endregion

        #region Роли
        /// <summary>
        /// Роли
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<PPRole> GetPPRoles(ILogger logger)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<PPRole> output = new List<PPRole>();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectPPRoles(), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new PPRole()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString(),
                                    Name = row["Name"].ToString(),
                                    ObjectUID = new Guid(row["ObjectUID"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        #endregion

        #region Работники
        /// <summary>
        /// Работники
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<Engineer> GetEngineers(long? DeptId = null, ILogger logger = null, long UserDept = -1)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger?.LogDebug("connection string is null or empty");
                return null;
            }

            List<Engineer> output = new List<Engineer>();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectEngineers(DeptId: DeptId, UserDept: UserDept), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new Engineer()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    PersonName = row["PersonName"].ToString(),
                                    DepartmentName = row["DepartmentName"].ToString(),
                                    IsUser = row["IsUser"].ToString() != "0",
                                    ObjectUID = new Guid(row["ObjectUID"].ToString()),
                                    CreateStamp = Convert.ToDateTime(row["CreationTime"].ToString()),
                                    ModifyStamp = Convert.ToDateTime(row["ModificationTime"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        /// <summary>
        /// Список работников для заполнения.
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<Engineer> GetEngineersList(long? DeptId = null, ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger?.LogDebug("connection string is null or empty");
                return null;
            }

            List<Engineer> output = new List<Engineer>();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectEngineersList(DeptId), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new Engineer()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    Surname = row["Surname"].ToString(),
                                    GivenName = row["GivenName"].ToString(),
                                    MiddleName = row["MiddleName"].ToString(),
                                    DisplayName = row["DisplayName"].ToString(),
                                    AppointName = row["AppointName"].ToString(),
                                    CreateStamp = Convert.ToDateTime(row["CreationTime"].ToString()),
                                    ModifyStamp = Convert.ToDateTime(row["ModificationTime"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }

        public static Engineer GetEngineer(int ID, ILogger logger)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            Engineer output = new Engineer();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectEngineers(ID: ID), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output = new Engineer()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    PersonName = row["PersonName"].ToString(),
                                    DepartmentName = row["DepartmentName"].ToString(),
                                    IsUser = row["IsUser"].ToString() != "0",
                                    ObjectUID = new Guid(row["ObjectUID"].ToString()),
                                    DepartmentId = string.IsNullOrEmpty(row["DepartmentId"].ToString()) ? (int?)null : int.Parse(row["DepartmentId"].ToString()),
                                    CalendarId = string.IsNullOrEmpty(row["CalendarId"].ToString()) ? (int?)null : int.Parse(row["CalendarId"].ToString()),
                                    PersonId = string.IsNullOrEmpty(row["PersonId"].ToString()) ? (int?)null : int.Parse(row["PersonId"].ToString()),
                                    PersonPositionId = string.IsNullOrEmpty(row["PersonPositionId"].ToString()) ? (int?)null : int.Parse(row["PersonPositionId"].ToString()),
                                    GivenName = row["GivenName"].ToString(),
                                    MiddleName = row["MiddleName"].ToString(),
                                    Surname = row["Surname"].ToString(),
                                    UserId = string.IsNullOrEmpty(row["UserId"].ToString()) ? (int?)null : int.Parse(row["UserId"].ToString()),
                                    Login = row["Login"].ToString(),
                                    CreateStamp = Convert.ToDateTime(row["CreationTime"].ToString()),
                                    ModifyStamp = Convert.ToDateTime(row["ModificationTime"].ToString())
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static void CreateEngineer(Engineer obj, long EngId, ILogger logger)
        {
            obj.PersonId = int.Parse(ExecuteScalar(CreateEngineer(obj, EngId), logger));
            if(obj.IsUser)
            {
                CreatePPUser(obj, EngId, logger);
            }
        }
        public static void UpdateEngineer(Engineer obj, long EngId, string CurrentLogin, string Role, ILogger logger)
        {
            ExecuteNonQuery(UpdateEngineer(obj, EngId), logger);
            if(CurrentLogin == obj.Login || Role.ToLower() == "sa" || Role.ToLower() == "a")
            {
                if (obj.IsUser && obj.UserId == null)
                {
                    CreatePPUser(obj, EngId, logger);
                }
                else if (obj.IsUser && obj.UserId != null)
                {
                    if (!string.IsNullOrEmpty(obj.Password))
                    {
                        UpdatePPUser(obj, EngId, logger);
                    }
                }
                else if (!obj.IsUser && obj.UserId != null)
                {
                    DeletePPUser((int)obj.UserId, EngId, logger);
                }
            }

        }
        public static void DeleteEngineer(int ID, long EngId, ILogger logger)
        {
            ExecuteNonQuery(DeleteEngineer(ID, EngId), logger);
        }

        public static void CreatePPUser(Engineer obj, long EngId, ILogger logger)
        {
            ExecuteNonQuery(CreatePPUser(obj, EngId), logger);
        }
        public static void UpdatePPUser(Engineer obj, long EngId, ILogger logger)
        {
            ExecuteNonQuery(UpdatePPUser(obj, EngId), logger);
        }
        public static void DeletePPUser(int ID, long EngId, ILogger logger)
        {
            ExecuteNonQuery(DeletePPUser(ID, EngId), logger);
        }

        #region Возможно нет нужды
        public static Person GetPerson(int ID, ILogger logger)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            Person output = new Person();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectPerson(ID), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output = new Person()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    ObjectUID = new Guid(row["ObjectUID"].ToString()),
                                    PersonPositionId = string.IsNullOrEmpty(row["PersonPositionId"].ToString()) ? (int?)null : int.Parse(row["PersonPositionId"].ToString()),
                                    GivenName = row["GivenName"].ToString(),
                                    MiddleName = row["MiddleName"].ToString(),
                                    Surname = row["Surname"].ToString(),
                                    UserId = int.Parse(row["UserId"].ToString())
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static void CreatePerson(Person obj, long EngId, ILogger logger)
        {
            ExecuteScalar(CreatePerson(obj, EngId), logger);
        }
        public static void UpdatePerson(Person obj, long EngId, ILogger logger)
        {
            ExecuteScalar(UpdatePerson(obj, EngId), logger);
        }
        public static void DeletePerson(int ID, long EngId, ILogger logger)
        {
            ExecuteScalar(DeletePerson(ID, EngId), logger);
        }
        #endregion

        #endregion

        #region Отчеты

        public static List<Asset> GetAssetsFromAPS(long APSId, ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger?.LogDebug("connection string is null or empty");
                return null;
            }

            List<Asset> output = new List<Asset>();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectAssetsFromAPS(APSId), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add( new Asset()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static List<Hierarchy> GetAssetChildsFromAsset(long AssetId, ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger?.LogDebug("connection string is null or empty");
                return null;
            }

            List<Hierarchy> output = new List<Hierarchy>();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectAssetChildsFromAsset(AssetId), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new Hierarchy()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static List<Hierarchy> GetControlsFromAssetChilds(long AssetChildId, ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger?.LogDebug("connection string is null or empty");
                return null;
            }

            List<Hierarchy> output = new List<Hierarchy>();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectControlsFromAssetChilds(AssetChildId), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new Hierarchy()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static ControlValueReportData GetCVR(DateTime From, DateTime To, long DepartmentId, long AssetId, ILogger logger)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            ControlValueReportData output = new ControlValueReportData();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectCVR(From, To, DepartmentId,AssetId), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                if(!output.SCs.ContainsKey(long.Parse(row["SCId"].ToString())))
                                {
                                    output.SCs.Add(long.Parse(row["SCId"].ToString()), Convert.ToDateTime(row["StartTime"]));
                                }
                                ControlValueReportRow RowData = output.Rows.Find(x => x.AId == long.Parse(row["AId"].ToString()) && x.ACId == long.Parse(row["ACId"].ToString()) && x.APId == long.Parse(row["APId"].ToString()));
                                if(RowData == null)
                                {
                                    RowData = new ControlValueReportRow()
                                    {
                                        AId = long.Parse(row["AId"].ToString()),
                                        AName = row["AName"].ToString(),
                                        ACId = long.Parse(row["ACId"].ToString()),
                                        ACName = row["ACName"].ToString(),
                                        APId = long.Parse(row["APId"].ToString()),
                                        APName = row["APName"].ToString()
                                    };
                                    output.Rows.Add(RowData);
                                }
                                RowData.Data.Add(long.Parse(row["SCId"].ToString()), row["Value"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        /// <summary>
        /// Отчет по осмотренным Тех. позициям
        /// </summary>
        public static List<AssetsReportData> GetVA(DateTime From, DateTime To, ILogger logger)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<AssetsReportData> output = new List<AssetsReportData>();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectVA(From, To), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new AssetsReportData()
                                {
                                    Id = long.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString(),
                                    Rows = long.Parse(row["Rows"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        /// <summary>
        /// Отчет по осмотренным Тех. позициям
        /// </summary>
        public static List<AssetsReportData> GetNA(DateTime From, DateTime To, ILogger logger)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<AssetsReportData> output = new List<AssetsReportData>();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectNA(From, To), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new AssetsReportData()
                                {
                                    Id = long.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString(),
                                    Rows = long.Parse(row["Rows"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }

        /// <summary>
        /// Возвращает Id указанного логина из таблицы PPUsers. В случае отсутствия -1
        /// </summary>
        /// <param name="Login"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static string LoginIsExist(string Login, ILogger logger)
        {
            return ExecuteScalar(LoginIsExist(Login), logger);
        }
        #endregion


        #region Авторизация
        public static UserData GetUserData(Credentials data, ILogger logger)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            UserData output = null;

            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectUserData(data), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output = new UserData()
                                {
                                    Id = long.Parse(row["Id"].ToString()),
                                    DeptId = string.IsNullOrEmpty(row["DepartmentId"].ToString()) ? 0 : long.Parse(row["DepartmentId"].ToString()),
                                    AppointName = row["AppointName"].ToString(),
                                    DepartmentName = row["DepartmentName"].ToString(),
                                    GivenName = row["GivenName"].ToString(),
                                    MiddleName = row["MiddleName"].ToString(),
                                    Surname = row["Surname"].ToString(),
                                    RoleCode = row["RoleCode"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }

        public static void SignInLog(Credentials data, ILogger logger)
        {
            List<SqlParameter> parameters = new()
            {
                new SqlParameter("@EngineerId", SqlDbType.BigInt) { Value = data.Id },
                new SqlParameter("@Stamp", SqlDbType.DateTime) { Value = DateTime.Now },
                new SqlParameter("@Platform", SqlDbType.TinyInt) { Value = 0 }
            };
            ExecuteNonQuery(SignInLog(), logger, parameters);
        }
        #endregion

        #region Приложения
        public static List<Attachment> GetAttachments(long SCId, ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger?.LogDebug("connection string is null or empty");
                return null;
            }

            List<Attachment> output = new List<Attachment>();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(GetAttachments(SCId), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new Attachment()
                                {
                                    Id = long.Parse(row["Id"].ToString()),
                                    SCId = long.Parse(row["SCId"].ToString()),
                                    IPName = row["IPName"].ToString(),
                                    IPIName = row["IPIName"].ToString(),
                                    APName = row["APName"].ToString(),
                                    FileName = row["FileName"].ToString(),
                                    CreationTime = DateTime.Parse(row["CreationTime"].ToString()),
                                    FileRef = Guid.Parse(row["FileRef"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static AttachmentData GetAttachment(Guid guid, ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger?.LogDebug("connection string is null or empty");
                return null;
            }

            AttachmentData output = null;
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(GetAttachment(guid), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output = new AttachmentData()
                                {
                                    FileName = row["FileName"].ToString(),
                                    Data = (byte[])row["Data"]
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        #endregion

        #region Деффекты
        public static List<Deffect> GetDeffects(long SCId, ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger?.LogDebug("connection string is null or empty");
                return null;
            }

            List<Deffect> output = new List<Deffect>();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(GetDeffects(SCId), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new Deffect()
                                {
                                    AName = row["AName"].ToString(),
                                    ACName = row["ACName"].ToString(),
                                    APVName = row["APVName"].ToString(),
                                    UserName = row["UserName"].ToString(),
                                    Value = row["Value"].ToString(),
                                    Timestamp = Convert.ToDateTime(row["Timestamp"].ToString()),
                                    Comment = row["Comment"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        #endregion

        #region Статистика
        public static List<Statistics> GetStatistics(DateTime Fromd, DateTime  Tod, long? DeptId, ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger?.LogDebug("connection string is null or empty");
                return null;
            }

            List<Statistics> output = new List<Statistics>();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    output.Add(new Statistics() { MetricName = "Количество пройденых обходов. " });
                    connection.Open();
                    using (var command = new SqlCommand(GetStatistics(), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        command.Parameters.Add(new SqlParameter() { ParameterName = "@DeptId", SqlDbType = SqlDbType.BigInt, Value = DeptId == null? 0: DeptId });
                        command.Parameters.Add(new SqlParameter() { ParameterName = "@FromD", SqlDbType = SqlDbType.DateTime, Value = Fromd });
                        command.Parameters.Add(new SqlParameter() { ParameterName = "@ToD", SqlDbType = SqlDbType.DateTime, Value = Tod });
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new Statistics()
                                {
                                    MetricName = row["Name"].ToString(),
                                    MetricValue = row["Value"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static List<string[]> GetStatisticsRTEC2(DateTime Fromd, DateTime  Tod, long? DeptId, ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger?.LogDebug("connection string is null or empty");
                return null;
            }

            List<string[]> output = new List<string[]>();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(GetStatisticsRTEC2(), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        command.Parameters.Add(new SqlParameter() { ParameterName = "@DeptId", SqlDbType = SqlDbType.BigInt, Value = DeptId == null? 0: DeptId });
                        command.Parameters.Add(new SqlParameter() { ParameterName = "@FromD", SqlDbType = SqlDbType.DateTime, Value = Fromd });
                        command.Parameters.Add(new SqlParameter() { ParameterName = "@ToD", SqlDbType = SqlDbType.DateTime, Value = Tod });
                        string dep = "";
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {

                                string[] sr = null;
                                if (dep != row["DepName"].ToString())
                                {
                                    output.Add(new string[10]);
                                    sr = new string[] 
                                    { 
                                        row["DepName"].ToString(), 
                                        row["DepPlanDay"].ToString(), 
                                        row["DepCountDay"].ToString(), 
                                        row["DepDiffDay"].ToString(),
                                        row["DepPlanNight"].ToString(),
                                        row["DepCountNight"].ToString(),
                                        row["DepDiffNight"].ToString(),
                                        row["DepPlan"].ToString(),
                                        row["DepCount"].ToString(),
                                        row["DepDiff"].ToString()
                                    };
                                    output.Add(sr);
                                    dep = row["DepName"].ToString();
                                }
                                if (!string.IsNullOrEmpty(row["ResName"].ToString()))
                                {
                                    sr = new string[] 
                                    {
                                        row["ResName"].ToString(),
                                        row["ResPlanDay"].ToString(),
                                        row["ResCountDay"].ToString(),
                                        row["ResDiffDay"].ToString(),
                                        row["ResPlanNight"].ToString(),
                                        row["ResCountNight"].ToString(),
                                        row["ResDiffNight"].ToString(),
                                        row["ResPlan"].ToString(),
                                        row["ResCount"].ToString(),
                                        row["ResDiff"].ToString()
                                    };
                                    output.Add(sr);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        #endregion
        #region Дефекты максимо

        public static List<MaximoStatus> GetMaximoStatuses(ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<MaximoStatus> output = new List<MaximoStatus>();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectMaximoStatuses(), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new MaximoStatus()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static List<MaximoDefect> GetMaximoStatistics(long? Id = null, DateTime? From = null, DateTime? To = null, long? DeptId = null, long? StatusId = null, long UserDept = -1, ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<MaximoDefect> output = new List<MaximoDefect>();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectMaximoStatistics(Id, From, To, DeptId, StatusId, UserDept), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new MaximoDefect()
                                {
                                    Id = long.Parse(row["Id"].ToString()),
                                    DepartmentName = row["DepartmentName"].ToString(),
                                    TaskName = row["TaskName"].ToString(),
                                    CreationTime = Convert.ToDateTime(row["CreationTime"].ToString()),
                                    CustomAssetCode = row["CustomAssetCode"].ToString(),
                                    CustomAssetChildCode = row["CustomAssetChildCode"].ToString(),
                                    CustomDescription = row["CustomDescription"].ToString(),
                                    MaximoStatusId = long.Parse(row["MaximoStatusId"].ToString()),
                                    MaximoStatus = row["MaximoStatus"].ToString(),
                                    TICKETID = row["TICKETID"].ToString(),
                                    TICKETUID = row["TICKETUID"].ToString(),
                                    LastSend = string.IsNullOrEmpty(row["LastSend"].ToString()) ? null : Convert.ToDateTime(row["LastSend"]),
                                    MaximoError = row["MaximoError"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }

        public static void MaximoStatisticsUpdate(MaximoDefect Data, long User, ILogger logger = null)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(new SqlParameter() { ParameterName = "@Id", SqlDbType = SqlDbType.BigInt, Value = Data.Id });
            sqlParameters.Add(new SqlParameter() { ParameterName = "@User", SqlDbType = SqlDbType.BigInt, Value = User });
            sqlParameters.Add(new SqlParameter() { ParameterName = "@CustomAssetCode", SqlDbType = SqlDbType.NVarChar, Value = Data.CustomAssetCode });
            sqlParameters.Add(new SqlParameter() { ParameterName = "@CustomAssetChildCode", SqlDbType = SqlDbType.NVarChar, Value = Data.CustomAssetChildCode });
            sqlParameters.Add(new SqlParameter() { ParameterName = "@CustomDescription", SqlDbType = SqlDbType.NVarChar, Value = Data.CustomDescription });
            ExecuteNonQuery(MaximoStatisticsUpdate(), logger, sqlParameters);
        }
        public static void MaximoStatisticsSend(long Id, long User, ILogger logger = null)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(new SqlParameter() { ParameterName = "@Id", SqlDbType = SqlDbType.BigInt, Value = Id });
            sqlParameters.Add(new SqlParameter() { ParameterName = "@User", SqlDbType = SqlDbType.BigInt, Value = User });
            ExecuteNonQuery(MaximoStatisticsSend(), logger, sqlParameters);
        }
        #endregion
        #endregion
        #region Служебные
        private static string ExecuteScalar(string commandString, ILogger logger = null, List<SqlParameter> sqlParameters = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger?.LogDebug("connection string is null or empty");
                return null;
            }

            string output = "";
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(commandString, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        if (sqlParameters != null)
                        {
                            foreach (var parameter in sqlParameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }
                        output = command.ExecuteScalar().ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе скалярного значения {commandString}");
                output = null;
            }
            return output;
        }
        private static void ExecuteNonQuery(string commandString, ILogger logger = null, List<SqlParameter> sqlParameters = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger?.LogDebug("connection string is null or empty");
                return;
            }

            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(commandString, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        if (sqlParameters != null)
                        {
                            foreach (var parameter in sqlParameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Ошибка на запросе скалярного значения {commandString}");
            }
        }

        public static IEnumerable<T> Select<T>(this IDataReader reader,
                                       Func<IDataReader, T> projection)
        {
            while (reader.Read())
            {
                yield return projection(reader);
            }
        }

        [Obsolete]
        private static string GetControlType(ControlType type)
        {
            switch (type)
            {
                case ControlType.Measurable:
                    return "TYPE_DOUBLE_N";
                case ControlType.Visual:
                    return "TYPE_BOOLEAN_N";
                case ControlType.Numerical:
                    return "TYPE_INT64_N";
                default:
                    return "";
            }
        }
        private static string GetControlType(long type)
        {
            switch (type)
            {
                case 4:
                case 5:
                    return "TYPE_DOUBLE_N";
                case 2:
                    return "TYPE_BOOLEAN_N";
                case 3:
                    return "TYPE_INT64_N";
                default:
                    return "";
            }
        }

        [Obsolete]
        private static int GetControlTypeIndex(string type)
        {
            switch (type)
            {
                case "TYPE_DOUBLE_N":
                    return 0;
                case "TYPE_BOOLEAN_N":
                    return 1;
                case "TYPE_INT64_N":
                    return 2;
                default: return -1;
            }
        }

        [Obsolete]
        private static string GetControlTypeDisplay(string type)
        {
            switch (type.Trim())
            {
                case "TYPE_DOUBLE_N":
                    return "Измеряемый";
                case "TYPE_BOOLEAN_N":
                    return "Визуальный";
                case "TYPE_INT64_N":
                    return "Числовой";
                default: return "";
            }
        }

        [Obsolete]
        public static string GetControlTypeDisplay(int type)
        {
            switch (type)
            {
                case 0:
                    return "Измеряемый";
                case 1:
                    return "Визуальный";
                case 2:
                    return "Числовой";
                default: return "";
            }
        }
        #endregion
    }


    [Obsolete]
    public enum ControlType
    {
        /// <summary>
        /// Измеряемый
        /// </summary>
        Measurable = 0,
        /// <summary>
        /// Визуальный
        /// </summary>
        Visual = 1,
        /// <summary>
        /// Числовой(целочисленный)
        /// </summary>
        Numerical = 2
    }
    public enum ZRPStatus
    {
        Created = 1,
        Started = 2,
        Ended = 3,
        Stoped = 4
    }
    /// <summary>
    /// ID в сводной таблице для фиксирования обновлений PPEntityCollections
    /// </summary>
    public enum RevEntry
    {
        Asset = 1,
        AssetClass = 2,
        AssetClassDefectTypeGroup = 3,
        AssetClassParameter = 4,
        AssetParameter = 5,
        AssetParameterMeasure = 6,
        AssetParameterPair = 7,
        AssetParameterValue = 8,
        AssetResponsible = 9,
        AssetSort = 10,
        AssetSpecies = 11,
        AssetState = 12,
        AssetType = 13,
        Assignment = 14,
        Attachment = 15,
        Bindings = 16,
        CallOrder = 17,
        CallOrderSchedulingContainer = 18,
        CallOrderStatus = 19,
        CallOrderType = 20,
        CheckList = 21,
        CheckListItem = 22,
        CheckListStatus = 23,
        CLItemDictionaryValue = 24,
        Defect = 25,
        DefectMeasure = 26,
        DefectMeasureGroup = 27,
        DefectMeasureType = 28,
        DefectStatus = 29,
        DefectType = 30,
        DefectTypeDTGroup = 31,
        DefectTypeGroup = 32,
        Department = 33,
        ElementUI = 34,
        Engineer = 35,
        EngineerPPE = 36,
        EngineerSkill = 37,
        EngineerWorkClothing = 38,
        EnvironmentState = 39,
        Equipment = 40,
        ESGroup = 41,
        InspectionDocument = 42,
        InspectionDocumentStatus = 43,
        InspectionDocumentStatusTransition = 44,
        InspectionDocumentType = 45,
        InspectionProtocol = 46,
        InspectionProtocolItem = 47,
        InspectionProtocolStatus = 48,
        InspectionProtocolStatusTransition = 49,
        InspectionProtocolType = 50,
        InspectionPurpose = 51,
        Laboratory = 52,
        LaboratoryMeteringType = 53,
        LaboratoryStatus = 54,
        LaboratoryType = 55,
        Layout = 56,
        License = 57,
        LicenseType = 58,
        MadeIn = 59,
        Material = 60,
        MaterialExpense = 61,
        MaterialType = 62,
        MeasureUnit = 63,
        MeteringType = 64,
        NavigationMenu = 65,
        OperationLog = 66,
        OperationLogEntry = 67,
        OperationLogEntryAbbreviation = 68,
        OperationLogEntryComment = 69,
        OperationLogEntryTemplate = 70,
        OperationLogEntryType = 71,
        PermissionUI = 72,
        Person = 73,
        PersonCategory = 74,
        PersonLicense = 75,
        PersonPhoneNumber = 76,
        PersonPosition = 77,
        PersonType = 78,
        PersonWorkRole = 79,
        PhoneNumberType = 80,
        PPAttachedData = 81,
        PPAttachedEntity = 82,
        PPCalendar = 83,
        PPCalendarRule = 84,
        PPE = 85,
        PPEntityCollection = 86,
        PPEType = 87,
        PPExpressionPermission = 88,
        PPGroup = 89,
        PPGroupRole = 90,
        PPObjectType = 91,
        PPPermission = 92,
        PPRole = 93,
        PPUser = 94,
        PPUserGroup = 95,
        PPUserRole = 96,
        Resource = 97,
        ResourceAllocation = 98,
        ResourceState = 99,
        ResourceType = 100,
        RoleUI = 101,
        RoleUIDepartment = 102,
        RoutineOperation = 103,
        SchedulingContainer = 104,
        SchedulingRequirement = 105,
        SchedulingRequirementSkill = 106,
        SCStatus = 107,
        SCStatusTransition = 108,
        SCType = 109,
        Signature = 110,
        Skill = 111,
        SkillGroup = 112,
        StatusReason = 113,
        StatusTransition = 114,
        Styles = 115,
        Task = 116,
        TaskAsset = 117,
        TaskCause = 118,
        TaskClass = 119,
        TaskOperation = 120,
        TaskPhoneNumber = 121,
        TaskReason = 122,
        TaskRepairType = 123,
        TaskSkill = 124,
        TaskSpecies = 125,
        TaskStatus = 126,
        TaskType = 127,
        TaskTypeSkill = 128,
        TaskTypeTaskCause = 129,
        TaskWorkSheet = 130,
        TechObjectType = 131,
        Tenant = 132,
        TimeBlock = 133,
        Tracker = 134,
        UserRoleUI = 135,
        UserSetting = 136,
        Vechical = 137,
        VechicalClass = 138,
        VechicalGroup = 139,
        VechicalState = 140,
        VechicalType = 141,
        Warehouse = 142,
        WarehousePosition = 143,
        WarehouseType = 144,
        WorkClothing = 145,
        WorkClothingType = 146,
        WorkCondition = 147,
        WorkMethod = 148,
        WorkOperation = 149,
        WorkOperationMeasure = 150,
        WorkPermissionEngineer = 151,
        WorkPermit = 152,
        WorkPermitCrewChange = 153,
        WorkPermitStatus = 154,
        WorkPermitStatusTransition = 155,
        WorkPermitType = 156,
        WorkplacePrepareStep = 157,
        WorkRole = 158,
        WorkSheet = 159,
        WorkSheetMaterial = 160,
        WorkSheetOperation = 161,
        WorkSheetTaskReason = 162,
        WPAdvancedBriefing = 163,
        WPAuthorBriefing = 164,
        WPDailyBriefing = 165,
        WPResponsibleBriefing = 166,
        WPSafetyAcceptorBriefing = 167,
        WPWorkplaceBriefing = 168,
        CallOrderCategory = 169,
        CallOrderMethod = 170,
        MaterialTransaction = 171,
        MTPosition = 172,
        MTRGroupPlanning = 173,
        MTStatus = 174,
        MTType = 175,
        DepartmentWarehouse = 176,
        SCReason = 177,
        AssetParameterSet = 178,
        AssetParameterSetRecord = 179,
        AssetTopology = 180,
        AuditInfo = 181,
        CompoundType = 182,
        Disconnection = 183,
        EmergencyPreparedness = 184,
        GenerateDocNumber = 185,
        MaterialGroup = 186,
        MaterialNomenclatureMaterial = 187,
        SCTypeTaskType = 188,
        TaskReasonsTaskRepairType = 189,
        UnscheduledRequestReason = 190,
        WorkPermissionEngineerLicense = 191,
        AssetParameterSpecies = 192,
        AssetParameterSpeciesPair = 193,
        CallOrderDiscType = 194,
        NonAvailableType = 195,
        Waybill = 196,
        WaybillCallOrder = 197,
        WaybillPerson = 198,
        WaybillPersonType = 199,
        WaybillStatus = 200,
        WaybillType = 201,
        AssetClassInspectionProtocolType = 202,
        AssetClassTaskReason = 203,
        AssetParameterInspectionProtocolType = 204,
        MeasurePosition = 205,
        ParameterPosition = 206,
        WorkSheetInspectionProtocolType = 207,
        BrigadeTemplate = 208,
        BrigadeTemplateRecord = 209,
        InspectionProtocolSort = 210,
        CallOrderChangeLog = 211,
        DepartmentComparison = 212,
        InspectionProtocolPurpose = 213,
        IPItemReason = 214,
        PersonPositionPersonPositionSNB = 215,
        PersonPositionSNB = 216,
        WorkPermitChangeLog = 217,
        WorkPermitChangeLogEngineer = 218,
        AssetParameterTypes = 219,
        AttachedData = 220
    }
}
