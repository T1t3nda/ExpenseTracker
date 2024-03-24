using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Controller;
using System.Globalization;

namespace ExpenseTracker.Controllers
{
    public class DashboardController : Controller
    {

        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ActionResult> Index(string timeRange = "monthly")
        {

            ViewBag.SelectedOption = timeRange;

            DateTime StartDate;
            DateTime EndDate;

            switch (timeRange)
            {
                case "weekly":
                    StartDate = DateTime.Today.AddDays(-6); // Last  7 days
                    EndDate = DateTime.Today;
                    break;
                case "yearly":
                    StartDate = new DateTime(DateTime.Today.Year, 1, 1); // Start of the current year
                    EndDate = new DateTime(DateTime.Today.Year, 12, 31); // End of the current year
                    break;
                default: // monthly
                    StartDate = DateTime.Today.AddDays(-DateTime.Today.Day + 1); // Start of the current month
                    EndDate = DateTime.Today.AddMonths(1).AddDays(-1); // End of the current month
                    break;
            }

            List<Transaction> SelectedTransactions = await _context.Transactions
                .Include(x => x.Category)
                .Where(y => y.Date >= StartDate && y.Date <= EndDate)
                .ToListAsync();

            //Total Income
            int TotalIncome = (int)SelectedTransactions
                .Where(i => i.Category.Type == CategoryType.Income)
                .Sum(j => j.Amount);
            ViewBag.TotalIncome = TotalIncome.ToString();

            //Total Expense
            int TotalExpense = (int)SelectedTransactions
                .Where(i => i.Category.Type == 0)
                .Sum(j => j.Amount);
            ViewBag.TotalExpense = TotalExpense.ToString();

            //Balance
            int Balance = TotalIncome - TotalExpense;
            ViewBag.Balance = Balance.ToString();

            //Doughnut Chart - Expense By Category
            ViewBag.DoughnutChartData = SelectedTransactions
                .Where(i => i.Category.Type == 0)
                .GroupBy(j => j.Category.Id)
                .Select(k => new
                {
                    categoryTitleWithIcon = k.First().Category.Icon + " " + k.First().Category.Title,
                    amount = k.Sum(j => j.Amount),
                    formattedAmount = k.Sum(j => j.Amount).ToString("C0"),
                })
                .OrderByDescending(l => l.amount)
                .ToList();

            //Spline Chart - Income vs Expense
            //Income
            var incomeSummary = SelectedTransactions
                .Where(i => i.Category.Type == CategoryType.Income)
                .GroupBy(j => timeRange == "monthly" ? j.Date.ToString("dd-MMM") : timeRange == "yearly" ? j.Date.ToString("MMM-yyyy") : j.Date.ToString("dd-MMM"))
                .Select(k => new SplineChartData()
                {
                    period = k.Key,
                    income = (int)k.Sum(l => l.Amount)
                })
                .ToList();

            //Expense
            var expenseSummary = SelectedTransactions
                .Where(i => i.Category.Type == 0)
                .GroupBy(j => timeRange == "monthly" ? j.Date.ToString("dd-MMM") : timeRange == "yearly" ? j.Date.ToString("MMM-yyyy") : j.Date.ToString("dd-MMM"))
                .Select(k => new SplineChartData()
                {
                    period = k.Key,
                    expense = (int)k.Sum(l => l.Amount)
                })
                .ToList();

            // Combine Income & Expense
            ViewBag.SplineChartData = timeRange == "yearly" ?
                from month in Enumerable.Range(1, 12)
                select new
                {
                    period = new DateTime(DateTime.Now.Year, month, 1).ToString("MMM-yyyy"),
                    income = incomeSummary.Where(i => i.period.Contains(new DateTime(DateTime.Now.Year, month, 1).ToString("MMM"))).Sum(i => i.income),
                    expense = expenseSummary.Where(i => i.period.Contains(new DateTime(DateTime.Now.Year, month, 1).ToString("MMM"))).Sum(i => i.expense)
                }
                :
                from date in Enumerable.Range(0, (EndDate - StartDate).Days + 1)
                select new
                {
                    period = StartDate.AddDays(date).ToString("dd-MMM"),
                    income = incomeSummary.Where(i => i.period.Contains(StartDate.AddDays(date).ToString("dd-MMM"))).Sum(i => i.income),
                    expense = expenseSummary.Where(i => i.period.Contains(StartDate.AddDays(date).ToString("dd-MMM"))).Sum(i => i.expense)
                };




            return View();
        }
    }

    public class SplineChartData
    {
        public string period;
        public int income;
        public int expense;

    }

}
