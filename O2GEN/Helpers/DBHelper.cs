﻿using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using O2GEN.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace O2GEN.Helpers
{
    public static class DBHelper
    {
        //private static string DBConnection = $"Data Source={(Environment.UserName == "Michael" ? "DESKTOP-JFNR95O\\SQLEXPRESS" : "DESKTOP-A17N4G7\\SQLEXPRESS01")};Initial Catalog=UFMDBLUK;Integrated Security=SSPI;";
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
        private static string GetZRP(DateTime From, DateTime To, int status, int? DeptId)
        {
            return "" +
                $"declare @__start_2 datetime2(7)='{From.ToString("yyyy-MM-dd HH:mm:ss")}', @__finish_1 datetime2(7)='{To.ToString("yyyy-MM-dd HH:mm:ss")}'; " +
                "SELECT[e].[Id], [e].[CloseTime], [e].[StartTime], [t0].[Id] as [ObjId], [t0].[DisplayName] as [ObjName], [t0].[ObjectUID],  [t5].[Id] as [TypeId], [t5].[DisplayName] as [TypeName], [t6].[Id] as [StatusId], [t6].[DisplayName] as [StatusName], [t7].[AppointmentFinish], [t7].[AppointmentStart], [t8].[Name] as [RouteName], [t10].[DisplayName] as [ResName] " +
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
                $"WHERE {(status > 0 ? $"[e].[SCStatusId] = {status} AND " : "")}  (([e].[IsDeleted] <> 1) AND([e].[TenantId] = CAST(1 AS bigint))) AND(([t7].[DepartmentId] IS NOT NULL {(DeptId != null ? $"AND[t7].[DepartmentId] = {DeptId}" : "")} ) AND(CASE " +
                "WHEN EXISTS( " +
                "   SELECT 1 " +
                "   FROM[Assignments] AS[e9] " +
                "   WHERE(([e9].[IsDeleted] <> 1) AND([e9].[TenantId] = CAST(1 AS bigint))) AND([e].[Id] = [e9].[SchedulingContainerId])) " +
                "    THEN CASE " +
                "        WHEN EXISTS( " +
                "            SELECT 1 " +
                "            FROM[Assignments] AS[e10] " +
                "            WHERE((([e10].[IsDeleted] <> 1) AND([e10].[TenantId] = CAST(1 AS bigint))) AND(([e10].[Start] < @__finish_1) AND([e10].[Finish] > @__start_2))) AND([e].[Id] = [e10].[SchedulingContainerId])) " +
                "        THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) " +
                "    END ELSE CASE " +
                "        WHEN([t7].[AppointmentStart] < @__finish_1) AND([t7].[AppointmentFinish] > @__start_2) " +
                "        THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) " +
                "    END " +
                "END = 1)) " +
                "ORDER BY[e].[Id]";
        }

        private static string GetZRP(int ID)
        {
            return "" +
                "SELECT [e].[Id], [e].[ObjectUID], [e].[StartTime], [e].[CloseTime], [e].[DepartmentId], [e].[SCTypeId], [e].[SCStatusId], [SR].[RequirementResourceId], [IDoc].[Name] " +
                "FROM [SchedulingContainers] AS[e] " +
                "LEFT JOIN [SchedulingRequirements] AS[SR] ON [SR].[IsDeleted] <> 1 AND [e].[RequirementId] = [SR].[Id] " +
                "LEFT JOIN [Tasks] AS [T] ON [T].[IsDeleted] <> 1 AND [e].[Id] = [T].[SchedulingContainerId] " +
                "LEFT JOIN [InspectionDocuments] AS [IDoc] ON [IDoc].[IsDeleted] <> 1 AND [T].[Id] = [IDoc].[TaskId] " +
                $"WHERE [e].[Id] = {ID} AND [e].[IsDeleted] <> 1 AND [e].[TenantId] = CAST(1 AS bigint) ";
        }

        /// <summary>
        /// Вставка Контроля
        /// </summary>
        /// <returns></returns>
        private static string CreateZRP(ZRP obj, string UserName)
        {
            //SchedulingRequirements                    +
            //Tasks                                     +
            //  InspectionDocuments                     +
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
            $"(isnull((SELECT top 1 id FROM PPUsers  where name = { (string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
            "getdate(), " +
            $"'{Guid.NewGuid().ToString("D")}', " +
            $"{ obj.DepartmentID}, " +
            $"{ obj.ResourceId}, " +
            $"'{ obj.StartTime.ToString("yyyy-MM-dd HH:mm:ss")}', " +
            $"'{ obj.EndTime.ToString("yyyy-MM-dd HH:mm:ss")}', " +
            $"'{ obj.StartTime.ToString("yyyy-MM-dd HH:mm:ss")}', " +
            $"'{ obj.EndTime.ToString("yyyy-MM-dd HH:mm:ss")}'); " +

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
            $"(isnull((SELECT top 1 id FROM PPUsers  where name = { (string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
            "getdate(), " +
            $"'{Guid.NewGuid().ToString("D")}', " +
            $"1, " +
            $"{obj.SCTypeId}, " +
            "(SELECT TOP 1 id FROM @SRId), " +
            "1, " +
            $"'{ obj.StartTime.ToString("yyyy-MM-dd HH:mm:ss")}', " +
            $"'{ obj.EndTime.ToString("yyyy-MM-dd HH:mm:ss")}', " +
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
            $"(isnull((SELECT top 1 id FROM PPUsers  where name = { (string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
            "getdate(), " +
            $"'{Guid.NewGuid().ToString("D")}', " +
            "(SELECT TOP 1 id FROM @SCId), " +
            "1, " +
            "1, " +
            $"{ obj.EndTime.Subtract(obj.StartTime).TotalMilliseconds}, " +
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
            $"(isnull((SELECT top 1 id FROM PPUsers  where name = { (string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
            "getdate(), " +
            $"'{Guid.NewGuid().ToString("D")}', " +
            $"(SELECT TOP 1 id FROM @SCId), " +
            $"(SELECT TOP 1 id FROM @TId), " +
            $"{obj.ResourceId}, " +
            $"'{ obj.StartTime.ToString("yyyy-MM-dd HH:mm:ss")}', " +
            $"'{ obj.EndTime.ToString("yyyy-MM-dd HH:mm:ss")}', " +
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
            $"(isnull((SELECT top 1 id FROM PPUsers  where name = { (string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
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
        private static string CreateInspectionProtocols(ZRP obj, string UserName, long TaskId, long IDocId)
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
                    $"(isnull((SELECT top 1 id FROM PPUsers  where name = { (string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
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
        private static string CreateAssetParameters(ZRP obj, string UserName, Dictionary<long, long> IPData)
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
                            $"(isnull((SELECT top 1 id FROM PPUsers  where name = { (string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
                            $"getdate(), " +
                            $"'{Guid.NewGuid().ToString("D")}', " +
                            $"{obj.NewTechPoz[i].Childs[j].Id}, " +
                            $"{obj.NewTechPoz[i].Childs[j].Childs[k].Id}, " +
                            $"N'{obj.NewTechPoz[i].DisplayName} {obj.NewTechPoz[i].Childs[j].DisplayName}', " +
                            $"0, " +
                            $"0, " +
                            $"(SELECT TOP 1 ValueType FROM AssetParameters WHERE Id = {obj.NewTechPoz[i].Childs[j].Childs[k].Id}), " +
                            $"0, " +
                            $"getdate(), " +
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
        private static string CreateInspectionProtocolItems(ZRP obj, string UserName, Dictionary<long, long> IPData, List<AssetParametersRowData> APVData)
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
                            $"(isnull((SELECT top 1 id FROM PPUsers  where name = { (string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
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
        private static string UpdateZRP(ZRP obj, string UserName)
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
            $"ModifiedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"{(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}")}),-1)), " +
            "ModificationTime = getdate(), " +
            "Revision = @SRRev, " +
            $"EarlyStart = '{obj.StartTime.ToString("yyyy-MM-dd HH:mm:ss")}', " +
            $"DueDate = '{obj.EndTime.ToString("yyyy-MM-dd HH:mm:ss")}', " +
            $"AppointmentStart = '{obj.StartTime.ToString("yyyy-MM-dd HH:mm:ss")}', " +
            $"AppointmentFinish = '{obj.EndTime.ToString("yyyy-MM-dd HH:mm:ss")}' " +
            $"WHERE ID in (SELECT RequirementId FROM SchedulingContainers WHERE ID = {obj.Id}); " +

            "UPDATE SchedulingContainers SET " +
            $"ModifiedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"{(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}")}),-1)), " +
            "ModificationTime = getdate(), " +
            "Revision = @SCRev, " +
            $"SCStatusId = {obj.SCStatusId}, " +
            $"SCTypeId = {obj.SCTypeId}, " +
            $"StartTime = '{obj.StartTime.ToString("yyyy-MM-dd HH:mm:ss")}', " +
            $"CloseTime = '{obj.EndTime.ToString("yyyy-MM-dd HH:mm:ss")}', " +
            $"DepartmentId = {obj.DepartmentID} " +
            $"WHERE ID = {obj.Id}; " +

            "UPDATE Tasks SET " +
            $"ModifiedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"{(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}")}),-1)), " +
            "ModificationTime = getdate(), " +
            "Revision = @TRev, " +
            $"SR_Duration = {obj.EndTime.Subtract(obj.StartTime).TotalMilliseconds} " +
            $"WHERE SchedulingContainerId = {obj.Id}; " +

            "UPDATE Assignments SET " +
            $"ModifiedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"{(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}")}),-1)), " +
            "ModificationTime = getdate(), " +
            "Revision = @ARev, " +
            $"ResourceId = {obj.ResourceId}, " +
            $"Start = '{ obj.StartTime.ToString("yyyy-MM-dd HH:mm:ss")}', " +
            $"Finish = '{ obj.EndTime.ToString("yyyy-MM-dd HH:mm:ss")}' " +
            $"WHERE SchedulingContainerId = {obj.Id}; " +

            "UPDATE InspectionDocuments SET " +
            $"ModifiedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"{(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}")}),-1)), " +
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
        private static string DeleteInspectionProtocol(int ID, string UserName)
        {
            return "DECLARE @IPRev bigint; " +
                "set @IPRev = (isnull((SELECT max(revision) id FROM InspectionProtocols ),0)+1); " +

                "UPDATE InspectionProtocols SET " +
                $"DeletedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
                "DeletionTime = getdate(), " +
                "Revision = @IPRev, " +
                "IsDeleted = 1 " +
                $"WHERE ID = {ID}; " +

                $"UPDATE PPEntityCollections SET Revision = @IPRev WHERE ID = {(int)RevEntry.InspectionProtocol};";
        }
        private static string DeleteZRP(int ID, string UserName)
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
            $"DeletedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"{(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}")}),-1)), " +
            "DeletionTime = getdate(), " +
            "IsDeleted = 1," +
            "Revision = @SRRev " +
            $"WHERE ID in (SELECT RequirementId FROM SchedulingContainers WHERE ID = {ID}); " +

            "UPDATE SchedulingContainers SET " +
            $"DeletedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"{(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}")}),-1)), " +
            "DeletionTime = getdate(), " +
            "IsDeleted = 1," +
            "Revision = @SCRev " +
            $"WHERE ID = {ID}; " +

            "UPDATE Tasks SET " +
            $"DeletedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"{(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}")}),-1)), " +
            "DeletionTime = getdate(), " +
            "IsDeleted = 1," +
            "Revision = @TRev " +
            $"WHERE SchedulingContainerId = {ID}; " +

            "UPDATE Assignments SET " +
            $"DeletedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"{(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}")}),-1)), " +
            "DeletionTime = getdate(), " +
            "IsDeleted = 1," +
            "Revision = @ARev " +
            $"WHERE SchedulingContainerId = {ID}; " +

            "UPDATE InspectionDocuments SET " +
            $"DeletedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"{(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}")}),-1)), " +
            "DeletionTime = getdate(), " +
            "IsDeleted = 1," +
            "Revision = @IDRev " +
            $"WHERE TaskId in (SELECT Id FROM Tasks WHERE SchedulingContainerId = {ID}); " +


            $"UPDATE PPEntityCollections SET Revision = @SRRev WHERE ID = {(int)RevEntry.SchedulingRequirement}; " +
            $"UPDATE PPEntityCollections SET Revision = @SCRev WHERE ID = {(int)RevEntry.SchedulingContainer}; " +
            $"UPDATE PPEntityCollections SET Revision = @TRev WHERE ID = {(int)RevEntry.Task}; " +
            $"UPDATE PPEntityCollections SET Revision = @IDRev WHERE ID = {(int)RevEntry.InspectionDocument}; " +
            $"UPDATE PPEntityCollections SET Revision = @ARev WHERE ID = {(int)RevEntry.Assignment}; ";
        }

        private static string GetInspectionProtocols(int SchedContID)
        {
            return "select IPs.Id, " +
            "IPs.ObjectUID,  " +
            "IPs.Name,  " +
            "CONCAT(IPI.Name, ' - ', AP.DisplayName) AS ItemName " +
            "from InspectionProtocolItems AS IPI " +
            "inner join InspectionProtocols AS IPs on IPI.InspectionProtocolId = IPs.Id " +
            "inner join AssetParameterValues AS APV on IPI.AssetParameterValueId = APV.Id " +
            "inner join AssetParameters AS AP on AP.Id = APV.AssetParameterId " +
            $"WHERE InspectionDocumentId in (select id from InspectionDocuments where TaskId in (select id from Tasks where SchedulingContainerId = {SchedContID})) AND IPs.IsDeleted <> 1 ";
        }
        #endregion

        #region Статусы
        /// <summary>
        /// Статусы
        /// </summary>
        /// <returns></returns>
        private static string SelectSCStatuses()
        {
            return "SELECT id, DisplayName, ObjectUID FROM SCStatuses where IsDeleted <> 1 AND TenantId = CAST(1 AS bigint)";
        }
        #endregion

        #region Статусы
        /// <summary>
        /// Календари
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
        private static string SelectControls(int ID = -1)
        {
            return "SELECT AP.Id, AP.DisplayName, AP.ObjectUID, AP.BottomValue1, AP.TopValue1, AP.BottomValue2, AP.TopValue2, AP.BottomValue3, AP.TopValue3, APT.Id AS AssetParameterTypeId, APT.Name AS ValueTypeName " +
                "FROM AssetParameters AS AP " +
                "INNER JOIN AssetParameterTypes AS APT ON AP.AssetParameterTypeId = APT.Id " +
                "WHERE(AP.IsDeleted <> 1) AND(AP.TenantId = CAST(1 AS bigint)) " +
                $"{(ID == -1 ? "" : "AND AP.Id = " + ID)} " +
                "ORDER BY AP.Id desc";
        }
        /// <summary>
        /// Вставка Контроля
        /// </summary>
        /// <returns></returns>
        private static string СreateControl(Control obj, string UserName)
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
                "AssetParameterTypeId) " +

                $"VALUES " +

                $"(N'{obj.ValueBottom1.Replace(',', '.')}', " +
                $"N'{obj.ValueBottom2.Replace(',', '.')}', " +
                $"N'{obj.ValueBottom3.Replace(',', '.')}', " +
                $"(isnull((SELECT top 1 id FROM PPUsers  where name =  {(string.IsNullOrEmpty(UserName) ? "NULL" : $"{(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}")}),-1)) , " +
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
                $"{obj.ValueTop1.Replace(',', '.')}, " +
                $"{obj.ValueTop2.Replace(',', '.')}, " +
                $"{obj.ValueTop3.Replace(',', '.')}, " +
                $"'{GetControlType(obj.AssetParameterTypeId)}', " +
                $"0, " +
                $"0, " +
                $"0, " +
                $"0, " +
                $"0, " +
                $"0, " +
                $"0, " +
                $"0," +
                $"{obj.AssetParameterTypeId}); " +
                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = 5;";
        }
        private static string UpdateControl(Control obj, string UserName)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM AssetParameters ),0)+1); " +
                "UPDATE AssetParameters SET " +
                $"BottomValue1 = N'{obj.ValueBottom1.Replace(',', '.')}', " +
                $"BottomValue2 = N'{obj.ValueBottom2.Replace(',', '.')}', " +
                $"BottomValue3 = N'{obj.ValueBottom3.Replace(',', '.')}', " +
                $"ModifiedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"{(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}")}),-1)), " +
                "ModificationTime = getdate(), " +
                $"DisplayName = N'{obj.DisplayName}', " +
                $"Name = N'{obj.DisplayName}', " +
                "Revision = @revision, " +
                $"TopValue1 = {obj.ValueTop1.Replace(',', '.')}, " +
                $"TopValue2 = {obj.ValueTop2.Replace(',', '.')}, " +
                $"TopValue3 = {obj.ValueTop3.Replace(',', '.')}, " +
                $"ValueType = '{GetControlType(obj.AssetParameterTypeId)}', " +
                $"AssetParameterTypeId = {obj.AssetParameterTypeId} " +
                $"WHERE ID = {obj.Id}; " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.AssetParameter};";
        }
        private static string DeleteControl(int ID, string UserName)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM AssetParameters ),0)+1); " +
                "UPDATE AssetParameters SET " +
                $"DeletedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
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
        #endregion

        #region Маршруты
        /// <summary>
        /// Маршруты
        /// </summary>
        /// <returns></returns>
        private static string SelectAssetParameterSets(int ID = -1, int? DeptId = null)
        {
            return "SELECT e.Id, e.DisplayName, e.ObjectUID, t.id as DepartmentID, t.DisplayName as DepartmentName  " +
                "FROM AssetParameterSets AS e " +
                "LEFT JOIN( " +
                "    SELECT * " +
                "    FROM Departments AS e0 " +
                "    WHERE (e0.IsDeleted<> 1) AND(e0.TenantId = CAST(1 AS bigint)) " +
                ") AS t ON e.DepartmentId = t.Id " +
                "WHERE e.IsDeleted <> 1 " +
                $"{(ID == -1 ? "" : "AND e.Id = " + ID)} " +
                $"{(DeptId == null ? "" : $"AND e.DepartmentId = {DeptId} ")}" +
                "ORDER BY e.DisplayName";
        }
        private static string SelectAssetParameterSetsByDepartment(int DeptId)
        {
            return "SELECT e.Id, e.DisplayName, e.ObjectUID, e.DepartmentId, t.DisplayName as DepartmentName  " +
                "FROM AssetParameterSets AS e " +
                "LEFT JOIN Departments t ON e.DepartmentId = t.Id AND (t.IsDeleted<> 1) AND(t.TenantId = CAST(1 AS bigint)) " +
                "WHERE e.IsDeleted <> 1 " +
                $"AND e.DepartmentId = {DeptId} " +
                "ORDER BY e.DisplayName";
        }

        /// <summary>
        /// Создание маршрута
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string CreateAssetParameterSet(AssetParameterSet obj, string UserName)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM AssetParameterSets ),0)+1); " +
                "DECLARE @InsertedId TABLE(id int); " +
                "INSERT into AssetParameterSets " +
                "(DisplayName, " +
                "DepartmentId, " +
                "ObjectUID, " +
                "CreatedByUser, " +
                "CreationTime," +
                "Revision)" +
                "output inserted.Id into @InsertedId " +
                "values " +
                $"(N'{obj.DisplayName}', " +
                $"{obj.DepartmentID}, " +
                $"'{obj.ObjectUID.ToString("D")}', " +
                $"(isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
                "getdate(), " +
                "Revision);" +
                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.AssetParameterSet};" +
                $"SELECT TOP 1 id FROM @InsertedPId;";
        }
        /// <summary>
        /// Обновление маршрута
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string UpdateAssetParameterSet(AssetParameterSet obj, string UserName)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM AssetParameterSets ),0)+1); " +
                "UPDATE AssetParameterSets SET " +
                $"DisplayName = N'{obj.DisplayName}', " +
                $"DepartmentId = {obj.DepartmentID}, " +
                $"ModifiedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
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
        private static string DeleteAssetParameterSet(int ID, string UserName)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM AssetParameterSets ),0)+1); " +
                "UPDATE AssetParameterSets SET " +
                $"DeletedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
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
        private static string CreateAssetParameterSetDetails(long APSId, long AId, long ACId, long APId, string UserName)
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
                $"(isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
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
        private static string DeleteAssetParameterSetDetails(long ID, string UserName)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM AssetParameterSetRecords ),0)+1); " +
                "UPDATE AssetParameterSetRecords SET " +
                $"DeletedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
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
        private static string SelectAssetClass(int ID = -1)
        {
            return "SELECT Id, DisplayName, ObjectUID, ParentId " +
                "FROM AssetClass AS e " +
                "WHERE (IsDeleted <> 1) " +
                $"{(ID == -1 ? "" : "AND Id = " + ID)} " +
                "AND (TenantId = CAST(1 AS bigint)) " +
                "order by DisplayName";
        }
        /// <summary>
        /// Создание класса объектов
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string CreateAssetClass(AssetClass obj, string UserName)
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
                $"(isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
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
        private static string UpdateAssetClass(AssetClass obj, string UserName)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM AssetClass ),0)+1); " +
                "UPDATE AssetClass SET " +
                "Revision = @revision, " +
                $"Name = N'{obj.DisplayName}', " +
                $"DisplayName = N'{obj.DisplayName}', " +
                $"ModifiedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
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
        private static string DeleteAssetClass(int ID, string UserName)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM AssetClass ),0)+1); " +
                "UPDATE AssetClass SET " +
                $"DeletedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
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
        private static string CreateAssetClassParameter(AssetClassParameter obj, string UserName)
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
        private static string DeleteAssetClassParameter(int ID, string UserName)
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
        private static string CreatePersonPosition(PersonPosition obj, string UserName)
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
        private static string UpdatePersonPosition(PersonPosition obj, string UserName)
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
        private static string DeletePersonPosition(int ID, string UserName)
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
        private static string CreatePersonCategory(PersonCategory obj, string UserName)
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
        private static string UpdatePersonCategory(PersonCategory obj, string UserName)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM PersonCategories ),0)+1); " +
                "UPDATE PersonCategories SET " +
                $"Name = N'{obj.Name}', " +
                "ModificationTime = getdate(), " +
                $"ParentId = {(obj.ParentId == null ? "NULL" : obj.ParentId)}, " +
                $"WHERE ID = {obj.Id}; " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.PersonCategory};";
        }
        private static string DeletePersonCategory(int ID, string UserName)
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
        private static string SelectAssets(int? DeptID = -1, bool NotesOnly = false)
        {
            return "SELECT e.Id, e.DisplayName, e.Description, e.ExternalId, e.ObjectUID, e.ParentId,  t1.DisplayName as StateName, t1.ObjectUID, AT.Id as TypeID, AT.DisplayName as TypeName, e.AssetTypeId " +
                "FROM Assets AS e " +
                "LEFT JOIN ( " +
                "    SELECT e2.Id, e2.DisplayName, e2.ExternalId, e2.IsDeleted, e2.Name, e2.ObjectUID, e2.Revision, e2.TenantId " +
                "    FROM AssetStates AS e2 " +
                "    WHERE (e2.IsDeleted <> 1) AND (e2.TenantId = CAST(1 AS bigint)) " +
                ") AS t1 ON e.AssetStateId = t1.Id " +
                "LEFT JOIN AssetTypes AT on AT.Id = e.AssetTypeId " +
                "WHERE ((e.IsDeleted <> 1) AND (e.TenantId = CAST(1 AS bigint))) " +
                (DeptID != null && DeptID != -1 ? $"AND e.DepartmentId =  {DeptID} " : "") +
                (NotesOnly ? $"AND e.AssetSortId = 1 " : "") +
                "ORDER BY e.DisplayName";
        }
        private static string SelectAsset(int ID)
        {
            return "SELECT Id, DisplayName, ParentId, ObjectUID, DepartmentId, AssetSortId, AssetClassId, ExternalId " +
                "FROM Assets " +
                $"WHERE Id = {ID}";
        }
        private static string GetSimpleAsset(int ID)
        {
            return "SELECT p.id ParentId, p.DisplayName ParentName,  ch.Id ChildId, ch.DisplayName ChildName, prm.Id ParamId, prm.DisplayName ParamName " +
                "FROM Assets p " +
                "LEFT JOIN Assets ch on p.Id = ch.ParentId  AND ch.IsDeleted <> 1 " +
                "LEFT JOIN AssetParameterPair pair on pair.AssetId = ch.Id AND pair.IsDeleted <> 1 " +
                "LEFT JOIN AssetParameters prm on prm.Id = pair.AssetParameterId AND prm.IsDeleted <> 1 " +
                $"WHERE p.Id = {ID} AND p.IsDeleted <> 1; ";
        }
        private static string SelectAssetParameterForAsset(int AssetID)
        {
            return "SELECT AssetParameterId as Id FROM AssetParameterPair where IsDeleted <> 1 AND AssetId = " + AssetID;
        }
        private static string CreateAssetParameterPair(long AssetId, long ParameterId)
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
                $"('{Guid.NewGuid().ToString("D")}', " +
                $"@Rev, " +
                $"{AssetId}, " +
                $"{ParameterId}," +
                "0);" +

                $"UPDATE PPEntityCollections SET Revision = @Rev WHERE ID = {(int)RevEntry.AssetParameterPair};";
        }
        private static string DeleteAssetParameterPair(long AssetId, long AssetParameterId)
        {
            return "DECLARE @Rev bigint; " +
                "set @Rev = (isnull((SELECT max(revision) id FROM AssetParameterPair ),0)+1); " +

                "UPDATE AssetParameterPair SET " +
                "Revision = @Rev, " +
                "IsDeleted = 1 " +
                $"WHERE AssetId = {AssetId} AND AssetParameterId = {AssetParameterId}; " +

                $"UPDATE PPEntityCollections SET Revision = @Rev WHERE ID = {(int)RevEntry.AssetParameterPair};";
        }
        /// <summary>
        /// Создание объекта
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string CreateAsset(Asset obj, string UserName)
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
                "AssetSubtype)" +
                "OUTPUT inserted.Id into @AId " +
                "values " +
                $"({(obj.ParentId == null ? "NULL" : obj.ParentId)}, " +
                $"N'{obj.DisplayName}', " +
                $"N'{obj.DisplayName}', " +
                $"{obj.DepartmentId}, " +
                $"{obj.AssetSortId}, " +
                $"{(obj.AssetClassId == null ? "NULL" : obj.AssetClassId)}, " +
                $"{(string.IsNullOrEmpty(obj.Maximo) ? "NULL" : $"'{obj.Maximo}'")}, " +
                $"'{obj.ObjectUID.ToString("D")}', " +
                $"(isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
                "getdate(), " +
                "@Revision, " +
                "0, " +
                "1, " +
                "0, " +
                "getdate(), " +
                "getdate(), " +
                "-1);" +
                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.Asset};" +
                $"SELECT id FROM @AId;";
        }
        /// <summary>
        /// Обновление объекта
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string UpdateAsset(Asset obj, string UserName)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM Assets ),0)+1); " +
                "UPDATE Assets SET " +
                $"ParentId = {(obj.ParentId == null ? "NULL" : obj.ParentId)}, " +
                $"DisplayName = N'{obj.DisplayName}', " +
                $"Name = N'{obj.DisplayName}', " +
                $"DepartmentId = {obj.DepartmentId}, " +
                $"ExternalId = {(string.IsNullOrEmpty(obj.Maximo) ? "NULL" : $"'{obj.Maximo}'")}, " +
                $"AssetSortId = {obj.AssetSortId}, " +
                $"AssetClassId = {(obj.AssetClassId == null ? "NULL" : obj.AssetClassId)}, " +
                $"ModifiedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
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
        private static string DeleteAsset(int ID, string UserName)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM Assets ),0)+1); " +
                "UPDATE Assets SET " +
                $"DeletedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
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
        private static string SelectDepartments(int ID = -1, bool IsChildOnly = false)
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
        /// Создание подразделения
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string CreateDepartment(Department obj, string UserName)
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
        private static string UpdateDepartment(Department obj, string UserName)
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
        private static string DeleteDepartment(int ID, string UserName)
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
        private static string SelectResourceAllocations(int ID)
        {
            return $"SELECT Id, ObjectUID, EngineerId, ResourceId FROM ResourceAllocations where ResourceId = {ID} AND IsDeleted <> 1 ";
        }
        /// <summary>
        /// Привязка работника к  бригаде
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string CreateResourceAllocation(ResourceAllocations obj, string UserName)
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
                $"(isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
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
        private static string DeleteResourceAllocation(int ID, string UserName)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM ResourceAllocations ),0)+1); " +
                "UPDATE ResourceAllocations SET " +
                "IsDeleted = 1, " +
                $"DeletedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
                "DeletionTime = getdate(), " +
                "ResourceId = NULL " +
                $"WHERE ID = {ID}; " +
                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.ResourceAllocation};";
        }
        /// <summary>
        /// Бригады
        /// </summary>
        /// <returns></returns>
        private static string SelectResources(long? ID = null, long? DepID = null)
        {
            return "SELECT Id, DisplayName, ObjectUID, ResourceTypeId, ResourceStateId, DepartmentId, Latitude, Longitude, Address " +
                "FROM Resources WHERE " +
                $"{(ID != null ? $"Id = {ID} AND" : "")} " +
                $"{(DepID != null ? $"DepartmentId = {DepID} AND" : "")} " +
                "(IsDeleted <> 1) AND (TenantId = CAST(1 AS bigint)) order by DisplayName";
        }
        private static string SelectResourcesByDepartment(int DepartmentID = -1)
        {
            return "SELECT Id, DisplayName, ObjectUID, ResourceTypeId, ResourceStateId, DepartmentId, Latitude, Longitude, Address " +
                "FROM Resources WHERE " +
                $"DepartmentId = {DepartmentID} " +
                "(IsDeleted <> 1) AND (TenantId = CAST(1 AS bigint)) order by DisplayName";
        }
        /// <summary>
        /// Создание бригады
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string CreateResource(Resource obj, string UserName)
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
                $"{obj.Latitude}, " +
                $"{obj.Longitude}, " +
                $"N'{obj.Address}', " +
                $"{(obj.ResourceStateId == null ? "NULL" : obj.ResourceStateId)}, " +
                $"(isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
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
        private static string UpdateResource(Resource obj, string UserName)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM Resources ),0)+1); " +
                "UPDATE Resources SET " +
                "Revision = @revision, " +
                $"DisplayName = N'{obj.DisplayName}', " +
                $"DepartmentId = {obj.DepartmentId}, " +
                $"Latitude = {obj.Latitude}, " +
                $"Longitude = {obj.Longitude}, " +
                $"Address = N'{obj.Address}', " +
                $"ResourceStateId = {(obj.ResourceStateId == null ? "NULL" : obj.ResourceStateId)}, " +
                $"ModifiedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
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
        private static string DeleteResource(int ID, string UserName)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM Resources ),0)+1); " +
                "UPDATE Resources SET " +
                $"DeletedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
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
        private static string SelectEngineers(long? ID = null, long? DeptId = null)
        {
            return "SELECT e.Id, t1.PersonName, e.ObjectUID, e.PersonId, t0.DisplayName as DepartmentName, isnull(t2.IsUser,0) as IsUser , e.DepartmentId, e.PersonId, t1.Surname, t1.GivenName, t1.MiddleName, e.CalendarId, t1.PersonPositionId " +
                "FROM Engineers AS e " +
                "LEFT JOIN( " +
                "    SELECT e1.Id, e1.DisplayName " +
                "    FROM Departments AS e1 " +
                "    WHERE (e1.IsDeleted <> 1) AND (e1.TenantId = CAST(1 AS bigint)) " +
                ") AS t0 ON e.DepartmentId = t0.Id " +
                "LEFT JOIN( " +
                "    SELECT Id, UserId, CONCAT(Surname,' ',GivenName,' ', MiddleName) as PersonName, PersonPositionId, Surname, GivenName, MiddleName " +
                "   FROM Persons " +
                "    WHERE(IsDeleted <> 1) AND TenantId = CAST(1 AS bigint) " +
                ") AS t1 ON e.PersonId = t1.Id " +
                "LEFT JOIN( " +
                "    SELECT id, count(*) as IsUser " +
                "    FROM PPUsers " +
                "    WHERE(IsDeleted <> 1) AND TenantId = CAST(1 AS bigint) " +
                "    GROUP BY Id " +
                ") AS t2 ON t1.UserId = t2.Id " +
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
        private static string CreateEngineer(Engineer obj, string UserName)
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
                "values" +
                "(0, " +
                "@PRevision, " +
                $"(isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
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
                "values" +
                $"(N'{obj.Surname} {obj.GivenName.Substring(0, 1) + "."} {(!string.IsNullOrEmpty(obj.MiddleName) ? obj.MiddleName.Substring(0, 1) + "." : "")}'," +
                $"(select top 1 id from @InsertedPId), " +
                $"{(obj.CalendarId == null ? "NULL" : obj.CalendarId)}, " +
                $"{(obj.DepartmentId == null ? "NULL" : obj.DepartmentId)}, " +
                $"(isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
                $"getdate(), " +
                $"'{obj.ObjectUID.ToString("D")}', " +
                $"@ERevision, " +
                $"1, " +
                $"0, " +
                $"0); " +

                $"UPDATE PPEntityCollections SET Revision = @PRevision WHERE ID = {(int)RevEntry.Person};" +
                $"UPDATE PPEntityCollections SET Revision = @ERevision WHERE ID = {(int)RevEntry.Engineer};";
        }
        /// <summary>
        /// Обновление работника
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string UpdateEngineer(Engineer obj, string UserName)
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
                $"ModifiedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
                "ModificationTime = getdate(), " +
                "Revision = @PRevision " +
                $"WHERE ID = {obj.PersonId}; " +

                "UPDATE Engineers SET " +
                $"DisplayName = N'{obj.Surname} {obj.GivenName.Substring(0, 1) + "."} {(!string.IsNullOrEmpty(obj.MiddleName) ? obj.MiddleName.Substring(0, 1) + "." : "")}', " +
                $"CalendarId = {(obj.CalendarId == null ? "NULL" : obj.CalendarId)}, " +
                $"DepartmentId = {(obj.DepartmentId == null ? "NULL" : obj.DepartmentId)}, " +
                $"ModifiedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
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
        private static string DeleteEngineer(int ID, string UserName)
        {
            return "DECLARE @PRevision bigint, @ERevision bigint; " +
                "set @PRevision = (isnull((SELECT max(revision) id FROM Persons ),0)+1); " +
                "set @ERevision = (isnull((SELECT max(revision) id FROM Engineers ),0)+1); " +

                "UPDATE Persons SET " +
                $"DeletedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
                "DeletionTime = getdate(), " +
                "Revision = @PRevision, " +
                "IsDeleted = 1 " +
                $"WHERE ID in (select PersonId from Engineers where Id = {ID}); " +

                "UPDATE Engineers SET " +
                $"DeletedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
                "DeletionTime = getdate(), " +
                "Revision = @ERevision, " +
                "IsDeleted = 1 " +
                $"WHERE ID = {ID}; " +

                $"UPDATE PPEntityCollections SET Revision = @PRevision WHERE ID = {(int)RevEntry.Person}; " +
                $"UPDATE PPEntityCollections SET Revision = @ERevision WHERE ID = {(int)RevEntry.Engineer};";
        }

        /// <summary>
        /// Список людей для вставки в бригады
        /// </summary>
        /// <returns></returns>
        private static string SelectEngineersList()
        {
            return "SELECT e.Id, t1.Surname, t1.GivenName, t1.MiddleName, t1.DisplayName, PP.DisplayName as AppointName " +
                "FROM Engineers AS e  " +
                "LEFT JOIN Persons AS t1 ON e.PersonId = t1.Id  " +
                "LEFT JOIN PersonPositions as PP on PP.Id = t1.PersonPositionId " +
                "WHERE ((e.IsDeleted <> 1) " +
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
        private static string CreatePerson(Person obj, string UserName)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM Persons ),0)+1); " +
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
                "values" +
                $"(N'{obj.DisplayName}'," +
                $"N'{obj.Surname}', " +
                $"N'{obj.GivenName}', " +
                $"N'{obj.MiddleName}', " +
                $"{obj.UserId}, " +
                $"N'{obj.PersonPositionId}', " +
                $"(isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
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
        private static string UpdatePerson(Person obj, string UserName)
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
                $"ModifiedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
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
        private static string DeletePerson(int ID, string UserName)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM Persons ),0)+1); " +
                "UPDATE Persons SET " +
                $"DeletedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
                "DeletionTime = getdate(), " +
                "Revision = @revision, " +
                "IsDeleted = 1 " +
                $"WHERE ID = {ID}; " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.Person}; ";
        }



        #endregion

        #region  Отчеты
        /// <summary>
        /// Отчёт по значениям контролей
        /// </summary>
        /// <returns></returns>
        private static string SelectCVR(DateTime From, DateTime To, long DepartmentId, long AssetId)
        {
            return "SELECT A.Id AS AId, A.DisplayName AS AName, AC.Id AS ACId, AC.DisplayName AS ACName, AP.Id AS APId, AP.DisplayName AS APName, SC.Id AS SCId, SC.StartTime, APV.Value " +
                "FROM SchedulingContainers AS SC " +
                "INNER JOIN Departments AS D ON SC.DepartmentId = D.Id " +
                "INNER JOIN Tasks AS T ON T.SchedulingContainerId = SC.Id " +
                "INNER JOIN InspectionProtocols AS IProt ON IProt.TaskId = T.Id AND IProt.IsDeleted <> 1 " +
                "INNER JOIN InspectionProtocolItems AS IProtItems ON IProtItems.InspectionProtocolId = IProt.Id " +
                "INNER JOIN AssetParameterValues AS APV ON APV.Id = IProtItems.AssetParameterValueId " +
                "INNER JOIN Assets AS A ON A.Id = IProt.AssetId " +
                "INNER JOIN Assets AS AC ON AC.Id = APV.AssetId " +
                "INNER JOIN AssetParameters AS AP ON AP.Id = APV.AssetParameterId " +
                $"WHERE SC.StartTime <= '{To.ToString("yyyy-MM-dd HH:mm:ss")}' AND SC.CloseTime >= '{From.ToString("yyyy-MM-dd HH:mm:ss")}' AND D.Id = {DepartmentId} AND A.Id = {AssetId} AND SC.IsDeleted <> 1 " +
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
                $"AND SC.StartTime <= '{To.ToString("yyyy-MM-dd HH:mm:ss")}' AND SC.CloseTime >= '{From.ToString("yyyy-MM-dd HH:mm:ss")}' AND D.IsDeleted <> 1 " +
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

        #endregion

        #region Процедуры получения данных
        #region ЗРП
        public static List<ZRP> GetZRP(DateTime From, DateTime To, ILogger logger, int status = -1, int? DeptId = null)
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
                    using (var command = new SqlCommand(GetZRP(From, To, status, DeptId), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
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
                                    ResName = row["ResName"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных БР");
            }
            return output;
        }

        public static ZRP GetZRP(int ID, string UserName, ILogger logger)
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
                    using (var command = new SqlCommand(GetZRP(ID), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
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
                output.InsProt = GetInspectionProtocols(output.Id, UserName, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных БР");
            }
            return output;
        }

        public static void CreateZRP(ZRP obj, string UserName, ILogger logger)
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
                    using (var command = new SqlCommand(CreateZRP(obj, UserName), connection))
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
                Dictionary<long, long> IPData = CreateInspectionProtocols(obj, UserName, logger, TID, IDocID);
                List<AssetParametersRowData> APVData = CreateAssetParameters(obj, UserName, logger, IPData);
                CreateInspectionProtocolItems(obj, UserName, logger, IPData, APVData);
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
        private static Dictionary<long, long> CreateInspectionProtocols(ZRP obj, string UserName, ILogger logger, long TID, long IDocID)
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
                    using (var command = new SqlCommand(CreateInspectionProtocols(obj, UserName, TID, IDocID), connection))
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
        private static List<AssetParametersRowData> CreateAssetParameters(ZRP obj, string UserName, ILogger logger, Dictionary<long, long> IPData)
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
                    using (var command = new SqlCommand(CreateAssetParameters(obj, UserName, IPData), connection))
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
        private static void CreateInspectionProtocolItems(ZRP obj, string UserName, ILogger logger, Dictionary<long, long> IPData, List<AssetParametersRowData> APData)
        {
            ExecuteNonQuery(CreateInspectionProtocolItems(obj, UserName, IPData, APData), logger);
        }
        #endregion

        public static void UpdateZRP(ZRP obj, string UserName, ILogger logger)
        {
            ExecuteNonQuery(UpdateZRP(obj, UserName), logger);

            var Details = GetInspectionProtocols(obj.Id, UserName, logger);

            for (int i = 0; i < Details.Count; i++)
            {
                if (obj.InsProt.Find(x => x.Id == Details[i].Id) == null)
                {
                    DeleteInspectionProtocol(Details[i].Id, UserName, logger);
                }
            }

        }
        public static void DeleteZRP(int ID, string UserName, ILogger logger)
        {
            ExecuteNonQuery(DeleteZRP(ID, UserName), logger);
        }
        public static List<InspectionProtocol> GetInspectionProtocols(int ShedContID, string UserName, ILogger logger)
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
                    using (var command = new SqlCommand(GetInspectionProtocols(ShedContID), connection))
                    {
                        InspectionProtocol Tmp = null;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
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
                                        Name = row["Name"].ToString()
                                    };
                                    output.Add(Tmp);
                                }
                                Tmp.Items.Add(new InspectionProtocolItem() {
                                    Name = row["ItemName"].ToString()
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
        public static void DeleteInspectionProtocol(int InspectionProtocolID, string UserName, ILogger logger)
        {
            ExecuteNonQuery(DeleteInspectionProtocol(InspectionProtocolID, UserName), logger);
        }
        #endregion

        #region Календари
        /// <summary>
        /// Календари
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
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
        public static List<Control> GetControls(ILogger logger = null)
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
                                    AssetParameterTypeId = long.Parse(row["AssetParameterTypeId"].ToString())
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

        public static Control GetControl(int ID, ILogger logger)
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
                    using (var command = new SqlCommand(SelectControls(ID), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
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
                                    AssetParameterTypeId = long.Parse(row["AssetParameterTypeId"].ToString())
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
        public static void CreateControl(Control obj, string UserName, ILogger logger)
        {
            ExecuteNonQuery(СreateControl(obj, UserName), logger);
        }
        public static void UpdateControl(Control obj, string UserName, ILogger logger)
        {
            ExecuteNonQuery(UpdateControl(obj, UserName), logger);
        }
        public static void DeleteControl(int ID, string UserName, ILogger logger)
        {
            ExecuteNonQuery(DeleteControl(ID, UserName), logger);
        }
        #endregion

        #region Маршруты
        /// <summary>
        /// Маршруты
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<AssetParameterSet> GetAssetParameterSets(ILogger logger = null, int? DeptID = -1)
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
                    using (var command = new SqlCommand(SelectAssetParameterSets(DeptId: DeptID), connection))
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
                                    DepartmentID = string.IsNullOrEmpty(row["DepartmentID"].ToString()) ? (int?)null : int.Parse(row["DepartmentID"].ToString())
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
        public static AssetParameterSet GetAssetParameterSet(int ID, ILogger logger)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
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
                                    DepartmentID = string.IsNullOrEmpty(row["DepartmentID"].ToString()) ? (int?)null : int.Parse(row["DepartmentID"].ToString())
                                };
                            }
                        }
                    }
                }
                output.Childs = GetAssetParameterSetDetails(output.Id, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static void CreateAssetParameterSet(AssetParameterSet obj, string UserName, ILogger logger)
        {
            string APID = ExecuteScalar(CreateAssetParameterSet(obj, UserName), logger);
            for (int i = 0; i < obj.Childs.Count; i++)
            {
                for (int j = 0; j < obj.Childs[i].Childs.Count; j++)
                {
                    for (int k = 0; k < obj.Childs[i].Childs[j].Childs.Count; k++)
                    {
                        CreateAssetParameterSetRecord(long.Parse(APID), obj.Childs[i].Id, obj.Childs[i].Childs[j].Id, obj.Childs[i].Childs[j].Childs[k].Id, UserName, logger);
                    }
                }
            }
        }
        public static void UpdateAssetParameterSet(AssetParameterSet obj, string UserName, ILogger logger)
        {
            ExecuteNonQuery(UpdateAssetParameterSet(obj, UserName), logger);
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
                            CreateAssetParameterSetRecord(obj.Id, obj.Childs[i].Id, obj.Childs[i].Childs[j].Id, obj.Childs[i].Childs[j].Childs[k].Id, UserName, logger);
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
                    DeleteAssetParameterSetRecord(currentData[i].APSRId, UserName, logger);
                    continue;
                }
                AC = AP.Childs.Find(x => x.Id == currentData[i].AChildId);
                if (AC == null)
                {
                    DeleteAssetParameterSetRecord(currentData[i].APSRId, UserName, logger);
                    continue;
                }
                P = AC.Childs.Find(x => x.Id == currentData[i].ParamId);
                if (P == null)
                {
                    DeleteAssetParameterSetRecord(currentData[i].APSRId, UserName, logger);
                    continue;
                }

            }
        }
        public static void DeleteAssetParameterSet(int ID, string UserName, ILogger logger)
        {
            ExecuteNonQuery(DeleteAssetParameterSet(ID, UserName), logger);
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
        public static void CreateAssetParameterSetRecord(long APSId, long AParId, long AChildId, long ParId, string UserName, ILogger logger)
        {
            ExecuteNonQuery(CreateAssetParameterSetDetails(APSId, AParId, AChildId, ParId, UserName), logger);
        }
        /// <summary>
        /// Удаление деталей маршрута.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="UserName"></param>
        /// <param name="logger"></param>
        public static void DeleteAssetParameterSetRecord(long ID, string UserName, ILogger logger)
        {
            ExecuteNonQuery(DeleteAssetParameterSetDetails(ID, UserName), logger);
        }

        /// <summary>
        /// Данные по техпозициям для нового обхода
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<Hierarchy> GetAssetParameterSetDetails(int ID, ILogger logger)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
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
                logger.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
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
        public static List<AssetClass> GetAssetClasses(ILogger logger = null)
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
                    using (var command = new SqlCommand(SelectAssetClass(), connection))
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

        public static void CreateAssetClass(AssetClass obj, string UserName, ILogger logger)
        {
            var idnew = ExecuteScalar(CreateAssetClass(obj, UserName), logger);

            foreach (var item in obj.Parameters)
            {
                CreateAssetClassParameter(new AssetClassParameter() { AssetClassId = int.Parse(idnew), AssetParameterId = item }, UserName, logger);
            }
        }
        public static void UpdateAssetClass(AssetClass obj, string UserName, ILogger logger)
        {
            ExecuteNonQuery(UpdateAssetClass(obj, UserName), logger);
            var currPar = GetAssetClassParameters(obj.Id);
            foreach (var item in obj.Parameters)
            {
                if (currPar.Find(x => x.AssetParameterId == item) == null)
                    CreateAssetClassParameter(new AssetClassParameter() { AssetClassId = obj.Id, AssetParameterId = item }, UserName, logger);
            }
            foreach (var item in currPar)
            {
                if (!obj.Parameters.Contains(item.AssetParameterId))
                    DeleteAssetClassParameter(item.Id, UserName, logger);
            }
        }
        public static void DeleteAssetClass(int ID, string UserName, ILogger logger)
        {
            ExecuteNonQuery(DeleteAssetClass(ID, UserName), logger);
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
        public static void CreateAssetClassParameter(AssetClassParameter obj, string UserName, ILogger logger)
        {
            ExecuteNonQuery(CreateAssetClassParameter(obj, UserName), logger);
        }
        public static void DeleteAssetClassParameter(int ID, string UserName, ILogger logger)
        {
            ExecuteNonQuery(DeleteAssetClassParameter(ID, UserName), logger);
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

        public static void CreatePersonPosition(PersonPosition obj, string UserName, ILogger logger)
        {
            ExecuteNonQuery(CreatePersonPosition(obj, UserName), logger);
        }
        public static void UpdatePersonPosition(PersonPosition obj, string UserName, ILogger logger)
        {
            ExecuteNonQuery(UpdatePersonPosition(obj, UserName), logger);
        }
        public static void DeletePersonPosition(int ID, string UserName, ILogger logger)
        {
            ExecuteNonQuery(DeletePersonPosition(ID, UserName), logger);
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
        public static void CreatePersonCategory(PersonCategory obj, string UserName, ILogger logger)
        {
            ExecuteNonQuery(CreatePersonCategory(obj, UserName), logger);
        }
        public static void UpdatePersonCategory(PersonCategory obj, string UserName, ILogger logger)
        {
            ExecuteNonQuery(UpdatePersonCategory(obj, UserName), logger);
        }
        public static void DeletePersonCategory(int ID, string UserName, ILogger logger)
        {
            ExecuteNonQuery(DeletePersonCategory(ID, UserName), logger);
        }
        #endregion

        #region Объекты
        /// <summary>
        /// Объекты
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<Asset> GetAssets(ILogger logger = null, int? DeptID = -1, bool NotesOnly = false)
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
                    using (var command = new SqlCommand(SelectAssets(DeptID, NotesOnly), connection))
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
                                    ParentId = string.IsNullOrEmpty(row["ParentId"].ToString()) ? (int?)null : int.Parse(row["ParentId"].ToString())
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
            foreach (var item in all.FindAll(x => x.ParentId == null))
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
                                    AssetSortId = string.IsNullOrEmpty(row["AssetSortId"].ToString()) ? (int?)null : int.Parse(row["AssetSortId"].ToString())
                                };
                            }
                        }
                    }
                }
                foreach (var item in GetAssetParameterForAsset(output.Id, logger))
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
        public static Hierarchy GetSimpleAsset(int ID, ILogger logger)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            Hierarchy output = null;
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(GetSimpleAsset(ID), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            Hierarchy AC = null;
                            Hierarchy AP = null;
                            foreach (var row in dataReader.Select(row => row))
                            {
                                if (output == null)
                                {
                                    output = new Hierarchy() { Id = int.Parse(row["ParentId"].ToString()), DisplayName = row["ParentName"].ToString() };
                                }
                                AC = output.Childs.Find(x => x.Id == int.Parse(row["ChildId"].ToString()));
                                if (AC == null)
                                {
                                    AC = new Hierarchy() { Id = int.Parse(row["ChildId"].ToString()), DisplayName = row["ChildName"].ToString() };
                                    output.Childs.Add(AC);
                                }
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
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static void CreateAsset(Asset obj, string UserName, ILogger logger)
        {
            //Родитель и класс объектов только у узлов.
            if (obj.AssetSortId == 1) { obj.ParentId = null; obj.AssetClassId = null; obj.Parameters = null; }
            long id = long.Parse(ExecuteScalar(CreateAsset(obj, UserName), logger));
            if (obj.Parameters != null)
            {
                foreach (var item in obj.Parameters)
                {
                    CreateAssetParameterPair(id, item, logger);
                }
            }
        }
        public static void UpdateAsset(Asset obj, string UserName, ILogger logger)
        {
            ExecuteNonQuery(UpdateAsset(obj, UserName), logger);

            var pairs = GetAssetParameterForAsset(obj.Id, logger);
            foreach (var item in pairs)
            {
                if (!obj.Parameters.Contains(item))
                    DeleteAssetParameterPair(obj.Id, item, logger);
            }
            foreach (var item in obj.Parameters)
            {
                if (!pairs.Contains(item))
                    CreateAssetParameterPair(obj.Id, item, logger);
            }
        }
        public static void DeleteAsset(int ID, string UserName, ILogger logger)
        {
            ExecuteNonQuery(DeleteAsset(ID, UserName), logger);
        }

        public static List<int> GetAssetParameterForAsset(int AssetId, ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger?.LogDebug("connection string is null or empty");
                return null;
            }
            List<int> output = new List<int>();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectAssetParameterForAsset(AssetId), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(int.Parse(row["Id"].ToString()));
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

        public static void CreateAssetParameterPair(long AssetId, long AssetParameterId, ILogger logger)
        {
            ExecuteNonQuery(CreateAssetParameterPair(AssetId, AssetParameterId), logger);
        }
        public static void DeleteAssetParameterPair(long AssetId, long AssetParameterId, ILogger logger)
        {
            ExecuteNonQuery(DeleteAssetParameterPair(AssetId, AssetParameterId), logger);
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
        /// Участки
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<Department> GetDepartments(bool ClearList = false, int ForDept = -1, ILogger logger = null)
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
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                all.Add(new Department()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString(),
                                    ObjectUID = new Guid(row["ObjectUID"].ToString()),
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
                        if (temp.Childs is null) temp.Childs = new List<Department>();
                        temp.Childs.Add(all[i]);
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

        public static List<Department> GetChildDepartments(ILogger logger = null)
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
                    using (var command = new SqlCommand(SelectDepartments(IsChildOnly: true), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new Department()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    DisplayName = row["DisplayName"].ToString(),
                                    ObjectUID = new Guid(row["ObjectUID"].ToString()),
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

        public static Department GetDepartment(int id, ILogger logger)
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
                    using (var command = new SqlCommand(SelectDepartments(id), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
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
                                    Latitude = (string.IsNullOrEmpty(row["Latitude"].ToString()) ? 0 : double.Parse(row["Latitude"].ToString())),
                                    Longitude = (string.IsNullOrEmpty(row["Longitude"].ToString()) ? 0 : double.Parse(row["Longitude"].ToString())),
                                    Name = row["Name"].ToString(),
                                    Organization = row["Organization"].ToString(),
                                    ShortCode = row["ShortCode"].ToString(),
                                    TimeZone = row["TimeZone"].ToString(),
                                    ParentName = row["ParentDisplayName"].ToString()
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

        public static void CreateDepartment(Department obj, string UserName, ILogger logger)
        {
            ExecuteNonQuery(CreateDepartment(obj, UserName), logger);
        }
        public static void UpdateDepartment(Department obj, string UserName, ILogger logger)
        {
            ExecuteNonQuery(UpdateDepartment(obj, UserName), logger);
        }
        public static void DeleteDepartment(int ID, string UserName, ILogger logger)
        {
            ExecuteNonQuery(DeleteDepartment(ID, UserName), logger);
        }
        #endregion

        #region Бригады
        /// <summary>
        /// Бригады
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<Resource> GetResources(long? Id = null, long? DeptId = null, ILogger logger = null)
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
                    using (var command = new SqlCommand(SelectResources(Id, DeptId), connection))
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
                                    Address = row["Address"].ToString(),
                                    DepartmentId = string.IsNullOrEmpty(row["DepartmentId"].ToString()) ? (int?)null : int.Parse(row["DepartmentId"].ToString()),
                                    ResourceStateId = string.IsNullOrEmpty(row["ResourceStateId"].ToString()) ? (int?)null : int.Parse(row["ResourceStateId"].ToString()),
                                    ResourceTypeId = string.IsNullOrEmpty(row["ResourceTypeId"].ToString()) ? (int?)null : int.Parse(row["ParResourceTypeIdentId"].ToString()),
                                    Latitude = (string.IsNullOrEmpty(row["Latitude"].ToString()) ? 0 : double.Parse(row["Latitude"].ToString())),
                                    Longitude = (string.IsNullOrEmpty(row["Longitude"].ToString()) ? 0 : double.Parse(row["Longitude"].ToString()))
                                };
                            }
                        }
                    }
                }
                foreach (var item in GetResourceAllocations(output.Id, logger))
                {
                    output.Parameters.Add(item.EngineerID);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static void CreateResource(Resource obj, string UserName, ILogger logger)
        {
            var idnew = ExecuteScalar(CreateResource(obj, UserName), logger);
            foreach (var item in obj.Parameters)
            {
                CreateResourceAllocation(new ResourceAllocations() { ResourceID = int.Parse(idnew), EngineerID = item }, UserName, logger);
            }
        }
        public static void UpdateResource(Resource obj, string UserName, ILogger logger)
        {
            ExecuteNonQuery(UpdateResource(obj, UserName), logger);
            var currPar = GetResourceAllocations(obj.Id);
            foreach (var item in obj.Parameters)
            {
                if (currPar.Find(x => x.EngineerID == item) == null)
                    CreateResourceAllocation(new ResourceAllocations() { ResourceID = obj.Id, EngineerID = item }, UserName, logger);
            }
            foreach (var item in currPar)
            {
                if (!obj.Parameters.Contains(item.EngineerID))
                    DeleteResourceAllocation(item.Id, UserName, logger);
            }
        }
        public static void DeleteResource(int ID, string UserName, ILogger logger)
        {
            ExecuteNonQuery(DeleteResource(ID, UserName), logger);
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
        public static List<ResourceAllocations> GetResourceAllocations(int ResourceId, ILogger logger = null)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<ResourceAllocations> output = new List<ResourceAllocations>();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectResourceAllocations(ResourceId), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new ResourceAllocations()
                                {
                                    //Id, ObjectUID, EngineerId, ResourceId
                                    Id = int.Parse(row["Id"].ToString()),
                                    EngineerID = int.Parse(row["EngineerId"].ToString()),
                                    ResourceID = int.Parse(row["ResourceId"].ToString()),
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
        public static void CreateResourceAllocation(ResourceAllocations obj, string UserName, ILogger logger)
        {
            ExecuteNonQuery(CreateResourceAllocation(obj, UserName), logger);
        }
        public static void DeleteResourceAllocation(int ID, string UserName, ILogger logger)
        {
            ExecuteNonQuery(DeleteResourceAllocation(ID, UserName), logger);
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
        public static List<Engineer> GetEngineers(long? DeptId = null, ILogger logger = null)
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
                    using (var command = new SqlCommand(SelectEngineers(DeptId: DeptId), connection))
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
        /// <summary>
        /// Список работников для заполнения.
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<Engineer> GetEngineersList(ILogger logger = null)
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
                    using (var command = new SqlCommand(SelectEngineersList(), connection))
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
                                    AppointName = row["AppointName"].ToString()
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
                                    Surname = row["Surname"].ToString()
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
        public static void CreateEngineer(Engineer obj, string UserName, ILogger logger)
        {
            ExecuteNonQuery(CreateEngineer(obj, UserName), logger);
        }
        public static void UpdateEngineer(Engineer obj, string UserName, ILogger logger)
        {
            ExecuteNonQuery(UpdateEngineer(obj, UserName), logger);
        }
        public static void DeleteEngineer(int ID, string UserName, ILogger logger)
        {
            ExecuteNonQuery(DeleteEngineer(ID, UserName), logger);
        }
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
        public static void CreatePerson(Person obj, string UserName, ILogger logger)
        {
            ExecuteScalar(CreatePerson(obj, UserName), logger);
        }
        public static void UpdatePerson(Person obj, string UserName, ILogger logger)
        {
            ExecuteScalar(UpdatePerson(obj, UserName), logger);
        }
        public static void DeletePerson(int ID, string UserName, ILogger logger)
        {
            ExecuteScalar(DeletePerson(ID, UserName), logger);
        }


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
        #endregion

        #endregion

        #region Служебные
        private static string ExecuteScalar(string commandString, ILogger logger)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
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
                        output = command.ExecuteScalar().ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе скалярного значения {commandString}");
                output = null;
            }
            return output;
        }
        private static void ExecuteNonQuery(string commandString, ILogger logger)
        {
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
                    using (var command = new SqlCommand(commandString, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе скалярного значения {commandString}");
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
        AssetParameterTypes = 219 
    }
}
