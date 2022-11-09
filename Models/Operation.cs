using System;
using System.ComponentModel.DataAnnotations.Schema;
using BudgetMVVM.Data;

namespace BudgetMvvm.Models
{
    public class Operation
    {
        public int Id { get; set; }
        public OperationType Type { get; set; }
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }

        [NotMapped]
        public Category? Category
        {
            get => DbRepository.GetCategory(CategoryId);
        }
    }
}