﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace KoiFarmShop.Data.Models;

public partial class User
{
    public int Id { get; set; }

    public string UserId { get; set; }

    public string FullName { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public string Email { get; set; }

    public string Phone { get; set; }

    public int Role { get; set; }

    public int Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string ModifiedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string DeletedBy { get; set; }

    public virtual ICollection<Consignment> Consignments { get; set; } = new List<Consignment>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}