using System;
using System.Collections.Generic;

namespace QuanLySach.Models;

public partial class Rate
{
    public int RateId { get; set; }

    public string? Description { get; set; }

    public string? Title { get; set; }

    public virtual ICollection<CharBst> CharBsts { get; set; } = new List<CharBst>();
}
