﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace KoiFarmShop.Data.Models;

public partial class Consignment
{
    public int Id { get; set; }

    public string ConsignmentId { get; set; }

    public string UserId { get; set; }

    public string KoiId { get; set; }

    public int Type { get; set; }

    public double? DealPrice { get; set; }

    public string Method { get; set; }

    public string PaymentId { get; set; }

    public string Note { get; set; }

    public string CustomerContact { get; set; }

    public string CustomerAddress { get; set; }

    public decimal? TotalWeight { get; set; }

    public int Status { get; set; }

    public DateOnly? ConsignmentDate { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string ModifiedBy { get; set; }

    public virtual KoiFish Koi { get; set; }

    public virtual Payment Payment { get; set; }

    public virtual User User { get; set; }
}