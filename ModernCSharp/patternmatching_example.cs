//using System;

//OrderProcessor processor = new OrderProcessor();

//var order1 = new Order(1, 50m, OrderStatus.Pending, false);
//Console.WriteLine(processor.CalculateShippingCost(order1)); // Should print 10

//var order2 = new Order(2, 150m, OrderStatus.Shipped, false);
//Console.WriteLine(processor.CalculateShippingCost(order2)); // Should print 5

//var order3 = new Order(3, 50m, OrderStatus.Pending, true);
//Console.WriteLine(processor.CalculateShippingCost(order3)); // Should print 15
//public enum OrderStatus { Pending, Shipped, Delivered, Cancelled }

//public record Order(int Id, decimal Amount, OrderStatus Status, bool IsPriority);



//public class OrderProcessor
//{
//    public decimal CalculateShippingCost(Order order)
//    {
//        // Use a switch expression with property patterns
//        return order switch
//        {
//            // Check multiple properties at once
//            { Status: OrderStatus.Cancelled } => 0m,
//            { IsPriority: true, Status: OrderStatus.Delivered } => 25m,
//            { IsPriority: true } => 15m,
//            { Amount: > 100 } => 5m,
//            _ => 10m  // default case
//        };
//    }

//}