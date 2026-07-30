using System;
using System.Collections.Generic;

namespace ChinookExplorer.Data.Models;

public partial class InvoiceLine
{
    public int InvoiceLineId { get; set; }

    public int InvoiceId { get; set; }

    public int TrackId { get; set; }

    public double UnitPrice { get; set; }

    public int Quantity { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual Track Track { get; set; } = null!;
}
