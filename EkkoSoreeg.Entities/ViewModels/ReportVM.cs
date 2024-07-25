using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkkoSoreeg.Entities.ViewModels
{
    public class ReportVM
    {
        public int UsersCount { get; set; }
        public int OrdersNumber { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<int> MonthlySales { get; set; }
        public List<int> MonthlyRevenue { get; set; }
        public List<string> ProductSalesLabels { get; set; }
        public List<int> ProductSalesData { get; set; }
    }



}
