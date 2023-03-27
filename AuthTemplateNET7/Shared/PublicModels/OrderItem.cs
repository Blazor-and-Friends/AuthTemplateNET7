using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthTemplateNET7.Shared.PublicModels;

//added

public class OrderItem
{
    public int Id { get; set; }
    public Guid OrderId { get; set; }

    public int ProductId { get; set; }

    [Required, MaxLength(256)]
    public string Description { get; set; }

    [MaxLength(128)]
    public string ImgUrl { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(6,2)")]
    public decimal UnitPrice { get; set; }

    #region getters

    public decimal LineTotal => UnitPrice * Quantity;

    #endregion

    #region ctors

    public OrderItem() { }

    public OrderItem(Product product)
    {
        ProductId = product.Id;
        Description = product.Name;
        ImgUrl = product.ImgUrl;
        Quantity = 1;
        UnitPrice = product.Price;
    }

    #endregion //ctors
}
