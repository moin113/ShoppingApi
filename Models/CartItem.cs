﻿namespace MyntraClone.API.Models;

public class CartItem
{
    public int Id { get; set; }
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }

    public virtual Cart Cart { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}
