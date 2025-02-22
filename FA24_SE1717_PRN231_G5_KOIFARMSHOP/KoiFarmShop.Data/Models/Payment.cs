﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace KoiFarmShop.Data.Models;

public partial class Payment
{
    public int Id { get; set; }

    public string PaymentId { get; set; }

    public string UserId { get; set; }

    public double Amount { get; set; }

    public string Currency { get; set; }

    public int Type { get; set; }

    public string PaymentMethod { get; set; }

    public string ConsignmentId { get; set; }

    public string OrderId { get; set; }

    public int? Refundable { get; set; }

    public string Note { get; set; }

    public int Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<Consignment> Consignments { get; set; } = new List<Consignment>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User User { get; set; }
}