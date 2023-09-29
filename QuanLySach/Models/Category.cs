using System;
using System.Collections.Generic;

namespace QuanLySach.Models;

public partial class Category
{
    public int CatId { get; set; }

    public string? CatName { get; set; }

    public string? Description { get; set; }

    public int? ParentId { get; set; }

    public int? Levels { get; set; }

    public int? Ordering { get; set; }

    public bool Published { get; set; }

    public string? Thumb { get; set; }

    public string? Title { get; set; }

    public string? Alias { get; set; }

    public string? MetaDesc { get; set; }

    public string? MetaKey { get; set; }

    public string? Cover { get; set; }

    public string? SchemaMarkup { get; set; }

    public virtual ICollection<Author> Authors { get; set; } = new List<Author>();

    public virtual ICollection<Page> Pages { get; set; } = new List<Page>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<TinDang> TinDangs { get; set; } = new List<TinDang>();
}
