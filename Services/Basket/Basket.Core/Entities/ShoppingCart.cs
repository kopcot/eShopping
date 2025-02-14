using Shared.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Basket.Core.Entities
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public class ShoppingCart : BaseEntity
    {
        public string UserName { get; set; }
        [ForeignKey("ItemIds")]
        public virtual List<ShoppingCartItem> Items { get; set; }
        [NotMapped]
        public decimal TotalPrice { get; set; }
        //[NotMapped]
        public int ItemCount { get; set; }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
