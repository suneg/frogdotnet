using System.Collections.Generic;

namespace Frog.Orm
{
    public enum OrderDirection { Ascending, Descending }

    public class Order
    {
        public List<OrderPart> Parts { get; set; }

        public Order()
        {
            Parts = new List<OrderPart>();
        }

        public class OrderPart
        {
            public string Column { get; set; }
            public OrderDirection Direction { get; set; }
        }

        public static Order By(string columnName)
        {
            return By(columnName, OrderDirection.Ascending);
        }

        public static Order By(string columnName, OrderDirection direction)
        {
            var order = new Order();

            var part = new OrderPart();
            part.Column = columnName;
            part.Direction = direction;

            order.Parts.Add(part);
            return order;
        }
    }
}