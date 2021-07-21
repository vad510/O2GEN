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
        private static string DBConnection = $"Data Source={(Environment.UserName == "Michael" ? "DESKTOP-JFNR95O\\SQLEXPRESS" : "DESKTOP-A17N4G7\\SQLEXPRESS01")};Initial Catalog=UFMDBLUK;Integrated Security=SSPI;";
        //private static string DBConnection = "Data Source=10.201.192.241;Initial Catalog=UFMDBLUK;User ID=sa;Password=Zz123456789;";

        private static string GetConnectionString()
        {
            //if (Environment.UserName == "Е")
            //    return string.Empty;

            return DBConnection;
        }

        #region Тексты запросов

        #region Обходы
        private static string GetZRP(DateTime From, DateTime To, int status)
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
                $"WHERE {(status > 0 ? $"[e].[SCStatusId] = {status} AND " : "")}  (([e].[IsDeleted] <> 1) AND([e].[TenantId] = CAST(1 AS bigint))) AND(([t7].[DepartmentId] IS NOT NULL AND[t7].[DepartmentId] IN(CAST(3 AS bigint))) AND(CASE " +
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
            //SchedulingRequirements
            //Tasks
            //  InspectionDocuments
            //      InspectionProtocols
            //          InspectionProtocolItems
            //              AssetParameterValues
            //  SchedulingContainers



            return "DECLARE @SRRev bigint, @SCRev bigint, @TRev bigint, @IDRev bigint, @IPRev bigint, @IPIRev bigint, @APVRev bigint; " +

            "SET @SRRev = (isnull((SELECT max(revision) id FROM SchedulingRequirements), 0) + 1); " +
            "SET @SCRev = (isnull((SELECT max(revision) id FROM SchedulingContainers), 0) + 1); " +
            "SET @TRev = (isnull((SELECT max(revision) id FROM Tasks), 0) + 1); " +
            "SET @IDRev = (isnull((SELECT max(revision) id FROM InspectionDocuments), 0) + 1); " +
            "SET @IPRev = (isnull((SELECT max(revision) id FROM InspectionProtocols), 0) + 1); " +
            "SET @IPIRev = (isnull((SELECT max(revision) id FROM InspectionProtocolItems), 0) + 1); " +
            "SET @APVRev = (isnull((SELECT max(revision) id FROM AssetParameterValues), 0) + 1); " +

            "DECLARE @SRId TABLE(id bigint); " +
            "DECLARE @SCId TABLE(id bigint); " +
            "DECLARE @TId TABLE(id bigint); " +
            "DECLARE @IDId TABLE(id bigint); " +
            "DECLARE @IPId TABLE(id bigint); " +
            "DECLARE @IPIId TABLE(id bigint); " +
            "DECLARE @APVId TABLE(id bigint); " +

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
            $"{ obj.SCStatusId}, " +
            $"{ obj.SCTypeId}, " +
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

            "INSERT INTO InspectionDocuments " +
            "(IsDeleted, " +
            "Revision, " +
            "CreatedByUser, " +
            "CreationTime, " +
            "CreationDateTime, " +
            "ObjectUID, " +
            "Name, " +
            "DepartmentId, " +
            "TaskId) " +
            "OUTPUT inserted.Id into @TId " +
            "VALUES " +
            "(0, " +
            "@IDRev, " +
            $"(isnull((SELECT top 1 id FROM PPUsers  where name = { (string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}),-1)), " +
            "getdate(), " +
            "getdate(), " +
            $"'{Guid.NewGuid().ToString("D")}', " +
            $"N'{obj.RouteName}', " +
            "(SELECT TOP 1 id FROM @TId)); " +
            $"UPDATE PPEntityCollections SET Revision = @SRRev WHERE ID = {(int)RevEntry.SchedulingRequirement}; " +
            $"UPDATE PPEntityCollections SET Revision = @SCRev WHERE ID = {(int)RevEntry.SchedulingContainer}; " +
            $"UPDATE PPEntityCollections SET Revision = @TRev WHERE ID = {(int)RevEntry.Task}; " +
            $"UPDATE PPEntityCollections SET Revision = @IDRev WHERE ID = {(int)RevEntry.InspectionDocument}; ";
        }
        private static string UpdateZRP(ZRP obj, string UserName)
        {
            return "DECLARE @SRRev bigint, @SCRev bigint, @TRev bigint, @IDRev bigint, @IPRev bigint, @IPIRev bigint, @APVRev bigint; " +

            "SET @SRRev = (isnull((SELECT max(revision) id FROM SchedulingRequirements), 0) + 1); " +
            "SET @SCRev = (isnull((SELECT max(revision) id FROM SchedulingContainers), 0) + 1); " +
            "SET @TRev = (isnull((SELECT max(revision) id FROM Tasks), 0) + 1); " +
            "SET @IDRev = (isnull((SELECT max(revision) id FROM InspectionDocuments), 0) + 1); " +
            "SET @IPRev = (isnull((SELECT max(revision) id FROM InspectionProtocols), 0) + 1); " +
            "SET @IPIRev = (isnull((SELECT max(revision) id FROM InspectionProtocolItems), 0) + 1); " +
            "SET @APVRev = (isnull((SELECT max(revision) id FROM AssetParameterValues), 0) + 1); " +

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
            $"UPDATE PPEntityCollections SET Revision = @IDRev WHERE ID = {(int)RevEntry.InspectionDocument}; ";
        }
        private static string DeleteZRP(int ID, string UserName)
        {


            return "DECLARE @SRRev bigint, @SCRev bigint, @TRev bigint, @IDRev bigint, @IPRev bigint, @IPIRev bigint, @APVRev bigint; " +

            "SET @SRRev = (isnull((SELECT max(revision) id FROM SchedulingRequirements), 0) + 1); " +
            "SET @SCRev = (isnull((SELECT max(revision) id FROM SchedulingContainers), 0) + 1); " +
            "SET @TRev = (isnull((SELECT max(revision) id FROM Tasks), 0) + 1); " +
            "SET @IDRev = (isnull((SELECT max(revision) id FROM InspectionDocuments), 0) + 1); " +
            "SET @IPRev = (isnull((SELECT max(revision) id FROM InspectionProtocols), 0) + 1); " +
            "SET @IPIRev = (isnull((SELECT max(revision) id FROM InspectionProtocolItems), 0) + 1); " +
            "SET @APVRev = (isnull((SELECT max(revision) id FROM AssetParameterValues), 0) + 1); " +

            "UPDATE SchedulingRequirements SET " +
            $"DeletedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"{(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}")}),-1)), " +
            "DeletionTime = getdate(), " +
            "Revision = @SRRev " +
            $"WHERE ID in (SELECT RequirementId FROM SchedulingContainers WHERE ID = {ID}); " +

            "UPDATE SchedulingContainers SET " +
            $"DeletedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"{(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}")}),-1)), " +
            "DeletionTime = getdate(), " +
            "Revision = @SCRev " +
            $"WHERE ID = {ID}; " +

            "UPDATE Tasks SET " +
            $"DeletedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"{(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}")}),-1)), " +
            "DeletionTime = getdate(), " +
            "Revision = @TRev " +
            $"WHERE SchedulingContainerId = {ID}; " +

            "UPDATE InspectionDocuments SET " +
            $"DeletedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName) ? "NULL" : $"{(string.IsNullOrEmpty(UserName) ? "NULL" : $"'{UserName}'")}")}),-1)), " +
            "DeletionTime = getdate(), " +
            "Revision = @IDRev " +
            $"WHERE TaskId in (SELECT Id FROM Tasks WHERE SchedulingContainerId = {ID}); " +


            $"UPDATE PPEntityCollections SET Revision = @SRRev WHERE ID = {(int)RevEntry.SchedulingRequirement}; " +
            $"UPDATE PPEntityCollections SET Revision = @SCRev WHERE ID = {(int)RevEntry.SchedulingContainer}; " +
            $"UPDATE PPEntityCollections SET Revision = @TRev WHERE ID = {(int)RevEntry.Task}; " +
            $"UPDATE PPEntityCollections SET Revision = @IDRev WHERE ID = {(int)RevEntry.InspectionDocument}; ";
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
            $"WHERE InspectionDocumentId in (select id from InspectionDocuments where TaskId in (select id from Tasks where SchedulingContainerId = {SchedContID})) AND IPI.IsDeleted <> 1 ";
        }
        #endregion

        #region Статусы
        /// <summary>
        /// Календари
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
            return "SELECT Id, DisplayName, ObjectUID, ValueType, BottomValue1, TopValue1, BottomValue2, TopValue2, BottomValue3, TopValue3 " +
                "FROM AssetParameters " +
                "WHERE(IsDeleted <> 1) AND(TenantId = CAST(1 AS bigint)) " +
                $"{(ID == -1 ? "" : "AND Id = " + ID)} " +
                "ORDER BY Id desc";
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
                "Critical4) " +

                $"VALUES " +

                $"(N'{obj.ValueBottom1.Replace(',', '.')}', " +
                $"N'{obj.ValueBottom2.Replace(',', '.')}', " +
                $"N'{obj.ValueBottom3.Replace(',', '.')}', " +
                $"(isnull((SELECT top 1 id FROM PPUsers  where name =  {(string.IsNullOrEmpty(UserName) ? "NULL" : $"{(string.IsNullOrEmpty(UserName)?"NULL":$"'{UserName}'")}")}),-1)) , " +
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
                $"'{GetControlType((ControlType)obj.ValueType)}', " +
                $"0, " +
                $"0, " +
                $"0, " +
                $"0, " +
                $"0, " +
                $"0, " +
                $"0, " +
                $"0); " +
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
                $"ModifiedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName)?"NULL":$"{(string.IsNullOrEmpty(UserName)?"NULL":$"'{UserName}'")}")}),-1)), " +
                "ModificationTime = getdate(), " +
                $"DisplayName = N'{obj.DisplayName}', " +
                $"Name = N'{obj.DisplayName}', " +
                "Revision = @revision, " +
                $"TopValue1 = {obj.ValueTop1.Replace(',', '.')}, " +
                $"TopValue2 = {obj.ValueTop2.Replace(',', '.')}, " +
                $"TopValue3 = {obj.ValueTop3.Replace(',', '.')}, " +
                $"ValueType = '{GetControlType((ControlType)obj.ValueType)}' " +
                $"WHERE ID = {obj.Id}; " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.AssetParameter};";
        }
        private static string DeleteControl(int ID, string UserName)
        {
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM AssetParameters ),0)+1); " +
                "UPDATE AssetParameters SET " +
                $"DeletedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName)?"NULL":$"'{UserName}'")}),-1)), " +
                "DeletionTime = getdate(), " +
                "Revision = @revision, " +
                "IsDeleted = 1 " +
                $"WHERE ID = {ID}; " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.AssetParameter}; ";
        }
        #endregion

        #region Маршруты
        /// <summary>
        /// Маршруты
        /// </summary>
        /// <returns></returns>
        private static string SelectAssetParameterSets(int ID = -1)
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
                "INSERT into AssetParameterSets " +
                "(DisplayName, " +
                "DepartmentId, " +
                "ObjectUID, " +
                "CreatedByUser, " +
                "CreationTime," +
                "Revision)" +
                "values " +
                $"N'{obj.DisplayName}', " +
                $"{obj.DepartmentID}, " +
                $"'{obj.ObjectUID.ToString("D")}', " +
                $"(isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName)?"NULL":$"'{UserName}'")}),-1)), " +
                "getdate(), " +
                "Revision);" +
                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.AssetParameterSet};";
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
                $"ModifiedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName)?"NULL":$"'{UserName}'")}),-1)), " +
                "ModificationTime = getdate(), " +
                "Revision = @revision, " +
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
                $"DeletedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName)?"NULL":$"'{UserName}'")}),-1)), " +
                "DeletionTime = getdate(), " +
                "Revision = @revision, " +
                "IsDeleted = 1 " +
                $"WHERE ID = {ID}; " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.AssetParameterSet}; ";
        }

        #warning TODO AssetParameterSetRecords
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
            return "SELECT Id, ObjectUID, AssetClassId, AssetParameterId " +
                "FROM AssetClassParameter " +
                "WHERE (IsDeleted <> 1) " +
                $"AND AssetClassId = {AssetClassId}";
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
        private static string SelectAssets(int DeptID = -1)
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
                (DeptID != -1 ? "AND e.DepartmentId =  {DeptID}" : "") +
                "ORDER BY e.DisplayName";
        }
        private static string SelectAsset(int ID)
        {
            return "SELECT Id, DisplayName, ParentId, ObjectUID, DepartmentId, AssetSortId, AssetClassId, ExternalId " +
                "FROM Assets " +
                $"WHERE Id = {ID}";
        }
        /// <summary>
        /// Создание объекта
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string CreateAsset(Asset obj, string UserName)
        {
            #warning Разобраться с тем что такое AssetSubtype
            return "DECLARE @revision bigint; " +
                "set @revision = (isnull((SELECT max(revision) id FROM Assets ),0)+1); " +
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
                "values " +
                $"({(obj.ParentId == null ? "NULL" : obj.ParentId)}, " +
                $"N'{obj.DisplayName}', " +
                $"N'{obj.DisplayName}', " +
                $"{obj.DepartmentId}, " +
                $"{obj.AssetSortId}, " +
                $"{(obj.AssetClassId == null ? "NULL" : obj.AssetClassId)}, " +
                $"N'{obj.Maximo}', " +
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
                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.Asset};";
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
                $"ExternalId = N'{obj.Maximo}', " +
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
                $"({(obj.ParentId == null ? "NULL" : obj.ParentId)}, "+
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
                $"ParentId = {(obj.ParentId == null? "NULL":obj.ParentId)}, " +
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
                $"(isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName)?"NULL":$"'{UserName}'")}),-1)), " +
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
                $"DeletedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName)?"NULL":$"'{UserName}'")}),-1)), " +
                "DeletionTime = getdate(), " +
                "ResourceId = NULL " +
                $"WHERE ID = {ID}; " +
                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.ResourceAllocation};";
        }
        /// <summary>
        /// Бригады
        /// </summary>
        /// <returns></returns>
        private static string SelectResources(int ID = -1)
        {
            return "SELECT Id, DisplayName, ObjectUID, ResourceTypeId, ResourceStateId, DepartmentId, Latitude, Longitude, Address " +
                "FROM Resources WHERE " +
                $"{(ID > 0 ? $"Id = {ID} AND" : "")} " +
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
                $"{(obj.ResourceStateId==null? "NULL": obj.ResourceStateId)}, " +
                $"(isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName)?"NULL":$"'{UserName}'")}),-1)), " +
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
                $"ModifiedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName)?"NULL":$"'{UserName}'")}),-1)), " +
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
                $"DeletedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName)?"NULL":$"'{UserName}'")}),-1)), " +
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
        private static string SelectEngineers(int ID = -1)
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
                $"{(ID == -1 ? "" : "AND e.Id = " + ID)} " +
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
                $"(isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName)?"NULL":$"'{UserName}'")}),-1)), " +
                $"getdate(), " +
                $"'{obj.ObjectUID.ToString("D")}', " +
                $"N'{obj.Surname} {obj.GivenName.Substring(0, 1) + "."} {(!string.IsNullOrEmpty(obj.MiddleName)? obj.MiddleName.Substring(0,1)+".":"" )}', " +
                $"N'{obj.Surname}', " +
                $"N'{obj.GivenName}', " +
                $"{(!string.IsNullOrEmpty(obj.MiddleName) ? "N'"+obj.MiddleName+"'": "NULL")}, " +
                $"1, " +
                $"{(obj.PersonPositionId == null? "NULL": obj.PersonPositionId)}); " +

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
                $"(isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName)?"NULL":$"'{UserName}'")}),-1)), " +
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
                $"ModifiedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName)?"NULL":$"'{UserName}'")}),-1)), " +
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
                $"(isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName)?"NULL":$"'{UserName}'")}),-1)), " +
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
                $"ModifiedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName)?"NULL":$"'{UserName}'")}),-1)), " +
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
                $"DeletedByUser = (isnull((SELECT top 1 id FROM PPUsers  where name = {(string.IsNullOrEmpty(UserName)?"NULL":$"'{UserName}'")}),-1)), " +
                "DeletionTime = getdate(), " +
                "Revision = @revision, " +
                "IsDeleted = 1 " +
                $"WHERE ID = {ID}; " +

                $"UPDATE PPEntityCollections SET Revision = @revision WHERE ID = {(int)RevEntry.Person}; ";
        }



        #endregion

        #endregion

        #region Процедуры получения данных
        #region ЗРП
        public static List<ZRP> GetZRP(DateTime From, DateTime To, ILogger logger, int status = -1)
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
                    using (var command = new SqlCommand(GetZRP(From, To, status), connection))
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

        public static ZRP GetZRP(int ID, ILogger logger)
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
                                    ObjectUID = string.IsNullOrEmpty(row["ObjectUID"].ToString())?null:new Guid(row["ObjectUID"].ToString())

                                };
                            }
                        }
                    }
                }
                output.InsProt = GetInspectionProtocols(output.Id, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных БР");
            }
            return output;
        }

        public static void CreateZRP(ZRP obj, string UserName, ILogger logger)
        {
            ExecuteNonQuery(CreateZRP(obj, UserName), logger);
        }
        public static void UpdateZRP(ZRP obj, string UserName, ILogger logger)
        {
            ExecuteNonQuery(UpdateZRP(obj, UserName), logger);
        }
        public static void DeleteZRP(int ID, string UserName, ILogger logger)
        {
            ExecuteNonQuery(DeleteZRP(ID, UserName), logger);
        }
        public static List<InspectionProtocol> GetInspectionProtocols(int ShedContID, ILogger logger)
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
                logger.LogError(ex, $"Ошибка на запросе данных БР");
            }
            return output;
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
                                    ValueType = GetControlTypeIndex(row["ValueType"].ToString()),
                                    DisplayValueType = GetControlTypeDisplay(row["ValueType"].ToString())
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
                                    ValueType = GetControlTypeIndex(row["ValueType"].ToString()),
                                    DisplayValueType = GetControlTypeDisplay(row["ValueType"].ToString()),
                                    ValueBottom1 = row["BottomValue1"].ToString(),
                                    ValueTop1 = row["TopValue1"].ToString(),
                                    ValueBottom2 = row["BottomValue2"].ToString(),
                                    ValueTop2 = row["TopValue2"].ToString(),
                                    ValueBottom3 = row["BottomValue3"].ToString(),
                                    ValueTop3 = row["TopValue3"].ToString(),
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
        public static List<AssetParameterSet> GetAssetParameterSets(ILogger logger)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<AssetParameterSet> output = new List<AssetParameterSet>();

            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectAssetParameterSets(), connection))
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
                logger.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
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
                    using (var command = new SqlCommand(SelectAssetParameterSets(ID), connection))
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
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static void CreateAssetParameterSet(AssetParameterSet obj, string UserName, ILogger logger)
        {
            ExecuteScalar(CreateAssetParameterSet(obj, UserName), logger);
        }
        public static void UpdateAssetParameterSet(AssetParameterSet obj, string UserName, ILogger logger)
        {
            ExecuteScalar(UpdateAssetParameterSet(obj, UserName), logger);
        }
        public static void DeleteAssetParameterSet(int ID, string UserName, ILogger logger)
        {
            ExecuteScalar(DeleteAssetParameterSet(ID, UserName), logger);
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
            var idnew= ExecuteScalar(CreateAssetClass(obj, UserName), logger);

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
                    CreateAssetClassParameter(new AssetClassParameter() { AssetClassId = obj.Id, AssetParameterId = item },UserName, logger);
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
                                    ObjectUID= new Guid(row["ObjectUID"].ToString())
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
        public static List<Asset> GetAssets(ILogger logger, int DeptID = -1)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<Asset> output = new List<Asset>();
            List<Asset> all = new List<Asset>();

            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectAssets(DeptID), connection))
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
                logger.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            while (all.Count > 0)
            {
                if (all[0].ParentId == (int?)null)
                {
                    output.Add(all[0]);
                }
                else
                {
                    Asset parrent = output.Find(x => x.Id == all[0].ParentId);
                    if (parrent == null)
                    {
                        parrent = all.Find(x => x.Id == all[0].ParentId);
                    }
                    if (parrent != null)
                    {
                        if (parrent.Childs == null)
                            parrent.Childs = new List<Asset>();
                        parrent.Childs.Add(all[0]);
                    }
                }
                all.RemoveAt(0);
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
                                Debug.WriteLine(row["Id"].ToString());
                                Debug.WriteLine(row["DisplayName"].ToString());
                                Debug.WriteLine(row["ObjectUID"].ToString());
                                Debug.WriteLine(row["ExternalId"].ToString());
                                Debug.WriteLine(row["ParentId"].ToString());
                                Debug.WriteLine(row["DepartmentId"].ToString());
                                Debug.WriteLine(row["AssetClassId"].ToString());
                                Debug.WriteLine(row["AssetSortId"].ToString());
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
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        public static void CreateAsset(Asset obj, string UserName, ILogger logger)
        {
            ExecuteNonQuery(CreateAsset(obj, UserName), logger);
        }
        public static void UpdateAsset(Asset obj, string UserName, ILogger logger)
        {
            ExecuteNonQuery(UpdateAsset(obj, UserName), logger);
        }
        public static void DeleteAsset(int ID, string UserName, ILogger logger)
        {
            ExecuteNonQuery(DeleteAsset(ID, UserName), logger);
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
        public static List<Department> GetDepartments(bool ClearList = false, ILogger logger = null)
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
            while (all.Count > 0)
            {
                #warning Не работает дальше второго слоя
                if (all[0].ParentId == (int?)null)
                {
                    output.Add(all[0]);
                }
                else
                {
                    Department parrent = output.Find(x => x.Id == all[0].ParentId);
                    if (parrent == null)
                    {
                        parrent = all.Find(x => x.Id == all[0].ParentId);
                    }
                    if (parrent != null)
                    {
                        if (parrent.Childs == null)
                            parrent.Childs = new List<Department>();
                        parrent.Childs.Add(all[0]);
                    }
                }
                all.RemoveAt(0);
            }
            return output;
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
        public static List<Resource> GetResources(ILogger logger = null)
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
                    using (var command = new SqlCommand(SelectResources(), connection))
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
                    using (var command = new SqlCommand(SelectResources(ID), connection))
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
        public static List<Engineer> GetEngineers(ILogger logger)
        {
            string con = GetConnectionString();

            if (string.IsNullOrEmpty(con))
            {
                logger.LogDebug("connection string is null or empty");
                return null;
            }

            List<Engineer> output = new List<Engineer>();
            try
            {
                using (var connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (var command = new SqlCommand(SelectEngineers(), connection))
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
                logger.LogError(ex, $"Ошибка на запросе данных {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
            return output;
        }
        /// <summary>
        /// Список работников для заполнения.
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<Engineer> GetEngineersList(ILogger logger=null)
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
                    using (var command = new SqlCommand(SelectEngineers(ID), connection))
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
    /// ID в сводной таблице для фиксирования обновлений.
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
        WorkPermitChangeLogEngineer = 218
    }
}
