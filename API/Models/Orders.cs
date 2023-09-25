using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Orders
{
    public int id { get; set; }

    public string? product { get; set; }

    public int? price { get; set; }

    public int? qty { get; set; }

    public string? picture { get; set; }

    public string? member { get; set; }

    public string? salepageid { get; set; }

    public string? shopid { get; set; }

    public string? skuid { get; set; }
}
