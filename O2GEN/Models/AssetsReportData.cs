using System;
using System.Collections.Generic;

namespace O2GEN.Models
{
    /// <summary>
    /// Отчет по осмотренным Тех. позициям
    /// </summary>
    public class AssetsReportData
    {
        public long Id { get; set; }
        public string DisplayName { get; set; }
        public long Rows { get; set; }
    }
    public class AssetsReportMerge
    {
        public AssetsReportMerge(List<Department> Departments, List<AssetsReportData> First, List<AssetsReportData> Second)
        {
            foreach (Department Dept in Departments)
            {
                AssetsReportData F = First.Find(x => x.Id == Dept.Id);
                AssetsReportData S = Second.Find(x => x.Id == Dept.Id);

                Rows.Add(new AssetsReportMergeRow()
                {
                    DeptName = Dept.DisplayName,
                    FirstCount = F == null ? 0 : F.Rows,
                    SecondCount = S == null ? 0 : S.Rows,
                    Diff = $"{((S == null || S.Rows == 0 || F == null) ? 0 : (Math.Round((((decimal)S.Rows) / ((decimal)F.Rows)) * 100, 2)))} %"
                });
            }
        }
        public List<AssetsReportMergeRow> Rows { get; set; } = new List<AssetsReportMergeRow>();
    }
    public class AssetsReportMergeRow
    {
        public string DeptName { get; set; }
        public long FirstCount { get; set; }
        public long SecondCount { get; set; }
        public string Diff { get; set; }
    }
}
