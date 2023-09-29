using System;
using System.Collections.Generic;

namespace QuanLySach.Models;

public partial class Author
{
    public int AuthorId { get; set; }

    public string? FullName { get; set; }

    public string? Title { get; set; }

    public DateTime? Date { get; set; }

    public bool Active { get; set; }

    public DateTime? CreateDate { get; set; }

    public int? CatId { get; set; }

    public virtual Category? Cat { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
