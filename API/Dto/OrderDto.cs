public class AddOrderDto
{
    public string user_name { get; set; }
    public int product_qty { get; set; }
    public int price { get; set; }
    public string product { get; set; }
    public string picture { get; set; }
    public string salepageid { get; set; }
    public string shopid { get; set; }
    public string skuid { get; set; }
    public string? recommender { get;set;} = null;
}


public class AddLeaderDto
{
    public string user_name { get; set; }
    public string campaign { get; set; }
    public string start { get; set; }
    public string finish { get; set; }
    public string salepageid { get; set; }
}
