using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GreenfieldLocalHubWebApp.Models
{
    public class orderProducts
    {
        public int orderProductsId { get; set; }
        public int ordersId { get; set; }
        public int productsId { get; set; }
        public int quantity { get; set; }
        public float unitPrice { get; set; }

        public orders orders { get; set; }
        public products products { get; set; }

    }
}
