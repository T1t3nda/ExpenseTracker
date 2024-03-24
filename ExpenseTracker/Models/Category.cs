using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public enum CategoryType
    {
        Expense,
        Income
    }
    public class Category
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public CategoryType Type { get; set; }

        [NotMapped]
        public string? TitleWithIcon
        {
            get
            {
                return this.Icon + " " + this.Title;
            }
        }
        [NotMapped]
        public string TypeDisplay
        {
            get
            {
                return Type == 0 ? "Expense" : "Income";
            }
        }
    }
}
