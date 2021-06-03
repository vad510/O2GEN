using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using O2GEN.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Globalization;

namespace O2GEN.Helpers
{
    public static class DBHelper
    {
        private static string DBConnection = "";
        private static string GetConnectionString()
        {
            return DBConnection;
        }
        #region Запросы
        private static string TestTable (DateTime From, DateTime To)
        {
            return "" +
                $"declare @__start_2 datetime2(7)='{From.ToString("yyyy-MM-dd HH:mm:ss")}', @__finish_1 datetime2(7)='{To.ToString("yyyy-MM-dd HH:mm:ss")}'; " +
                "SELECT[e].[Id], [e].[CloseTime], [e].[StartTime], [t0].[Id] as [ObjId], [t0].[DisplayName] as [ObjName], [t0].[ObjectUID],  [t5].[Id] as [TypeId], [t5].[DisplayName] as [TypeName], [t6].[Id] as [StatusId], [t6].[DisplayName] as [StatusName], [t7].[AppointmentFinish], [t7].[AppointmentStart], [t8].[Name] as [RouteName], [t10].[DisplayName] as [ResName] " +
                "FROM[SchedulingContainers] AS[e] " +
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
                "WHERE(([e].[IsDeleted] <> 1) AND([e].[TenantId] = CAST(1 AS bigint))) AND(([t7].[DepartmentId] IS NOT NULL AND[t7].[DepartmentId] IN(CAST(25 AS bigint))) AND(CASE " +
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
        #endregion

        public static List<TestTableRow> GetTestTable(DateTime From, DateTime To, ILogger logger)
        {
            List<TestTableRow> output = new List<TestTableRow>();
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    using (var command = new SqlCommand(TestTable(From, To), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        using (var dataReader = command.ExecuteReader())
                        {
                            foreach (var row in dataReader.Select(row => row))
                            {
                                output.Add(new TestTableRow()
                                {
                                    Id = int.Parse(row["Id"].ToString()),
                                    StartTime = Convert.ToDateTime(row["AppointmentStart"]),
                                    EndTime = Convert.ToDateTime(row["AppointmentFinish"]),
                                    ObjId = int.Parse(row["ObjId"].ToString()),
                                    ObjName = row["ObjName"].ToString(),
                                    TypeId = int.Parse(row["TypeId"].ToString()),
                                    TypeName = row["TypeName"].ToString(),
                                    StatusId = int.Parse(row["StatusId"].ToString()),
                                    StatusName = row["StatusName"].ToString(),
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


        #region Служебные
        private static string ExecuteScalar(string commandString, ILogger logger)
        {
            string output = "";
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
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
        public static IEnumerable<T> Select<T>(this IDataReader reader,
                                       Func<IDataReader, T> projection)
        {
            while (reader.Read())
            {
                yield return projection(reader);
            }
        }
        #endregion
    }
}
