﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace KoiFarmShop.Data.Models;

public partial class Voucher
{

    [Key]
    public int Id { get; set; }

    public string VoucherId { get; set; }

    public string VoucherCode { get; set; }

    public double DiscountAmount { get; set; }

    public double MinOrderAmount { get; set; }

    public int Status { get; set; }

    public DateTime? ValidityStartDate { get; set; }

    public DateTime? ValidityEndDate { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string ModifiedBy { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}