using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Amount should be greater than 0.")]
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        [Column(TypeName = "nvarchar(75)")]
        [StringLength(75, ErrorMessage = "Cannot exceed 75 chatacters")]
        public string? Note { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please select a category.")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [NotMapped]
        public string? CategoryTitleWithIcon
        {
            get
            {
                if (Category == null)
                {
                    return "";
                }
                else
                {
                    return Category.Icon + " " + Category.Title;
                }
            }
        }
        [NotMapped]
        public string? FormattedAmount
        {
            get
            {
                return ((Category == null || Category.Type == 0) ? "- " : "+ ") + Amount.ToString("C0");
            }
        }
    }
}
