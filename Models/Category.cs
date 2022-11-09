using System;

#nullable enable
namespace BudgetMvvm.Models
{
    public class Category : IEquatable<Category>
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        #region IEquatable
        public bool Equals(Category? category)
        {
            if (Id == category?.Id)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        #endregion
    }
}