using System;
using System.Collections.Generic;

namespace QuanLySach.Models;

public partial class CharBst
{
    public int CharId { get; set; }

    public string? CharName { get; set; }

    public string? Contents { get; set; }

    public string? Thumb { get; set; }

    public bool Published { get; set; }

    public string? Title { get; set; }

    public string? Alias { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? Ordering { get; set; }

    public int? ProductId { get; set; }

    public int? Price { get; set; }

    public int? UnitsInStock { get; set; }

    public string? Description { get; set; }

    public bool Active { get; set; }

    public bool HomeFlag { get; set; }

    public bool BestSellers { get; set; }

    public int? RateId { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Rate? Rate { get; set; }
}
