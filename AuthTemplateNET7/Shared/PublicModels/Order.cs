using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.PublicModels;

//added

public class Order
{
    public Guid Id { get; set; }

    public Guid MemberId { get; set; }

    [Column(TypeName = "smalldatetime")]
    public DateTime Date { get; set; } = DateTime.UtcNow;

    public List<OrderItem> OrderItems { get; set; } = new();

    public OrderStatus OrderStatus { get; set; }

    [MaxLength(512)]
    public string Memo { get; set; }

    public decimal Total { get; set; }

    #region UI

    public string StatusCss
    {
        get
        {
            string text = "text-";
            switch (OrderStatus)
            {
                case OrderStatus.Creating:
                    return null;
                case OrderStatus.Checkout:
                    return text + "secondary";
                case OrderStatus.PaymentPending:
                    return text + "info";
                case OrderStatus.Paid:
                    return text + "success";
                case OrderStatus.Shippped:
                    return text + "success";
                case OrderStatus.Cancelled:
                    return text + "danger";
                default:
                    return null;
            }
        }
    }

    #endregion //UI
}

public enum OrderStatus : byte
{
    Creating,
    Checkout,
    PaymentPending,
    Paid,
    Shippped,
    Cancelled
}
