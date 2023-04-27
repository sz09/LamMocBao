using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
    [Serializable]
    [Table("Orders")]
    public class Order : Entity, IEntity
    {
        [StringLength(200)]
        public string Province { get; set; }
        [StringLength(200)]
        public string District { get; set; }
        [StringLength(200)]
        public string Ward { get; set; }
        [StringLength(200)]
        public string Address { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }

        [StringLength(20)]
        public string PromotionCode { get; set; }
        public string PromotionMessage { get; set; }
        public PaymentMethod? PaymentType { get; set; }
        public Guid? CustomerId { get; set; }
        public decimal CalculatedPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsDeliveryToAnotherAddress { get; set; }
        public OrderStatus Status { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }

    [Serializable]
    [Table("OrderDetails")]
    public class OrderDetail : Entity, IEntity
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public Guid SizeId { get; set; }
        public decimal Price { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
        public virtual ProductMaterial ProductMaterial{ get; set; }
        public virtual ProductSize ProductSize { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }

    [Serializable]
    [Table("DeliveryAddresses")]
    public class DeliveryAddress : Entity, IEntity
    {
        public Guid OrderId { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Receiver { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
    }

    public enum PaymentMethod
    {
        COD = 1,
        BankTranfer = 2
    }

    public enum OrderStatus
    {
        Ordered = 0,
        Paid = 1,
        Prepared = 2,
        Delivering = 3,
        Delivered = 4,
        Cancelled = 5,
        ReturnedBack = 6
    }
}
