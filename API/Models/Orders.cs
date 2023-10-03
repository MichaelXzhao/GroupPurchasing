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

    public string? recommender { get; set; }

    public int? points { get; set; }

    public string? status { get; set; }

    public string? campaign { get; set; }

    public string? start { get; set; }

    public string? finish { get; set; }

    public int? afterdiscount { get; set; }

    public int? totaldiscount { get; set; }

    public int? totalprice { get; set; }
}
