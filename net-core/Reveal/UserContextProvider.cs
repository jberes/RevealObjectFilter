using Reveal.Sdk;

namespace RevealSdk.Server.Reveal
{
    public class UserContextProvider : IRVUserContextProvider
    {
        IRVUserContext IRVUserContextProvider.GetUserContext(HttpContext aspnetContext)
        {
            var userId = aspnetContext.Request.Headers["x-header-customerId"];
            var orderId = aspnetContext.Request.Headers["x-header-orderId"];
            var employeeId = aspnetContext.Request.Headers["x-header-employeeId"];


            string role = "User";
            if (userId == "AROUT" || userId == "BLONP")
            {
                role = "Admin";
            }

            var props = new Dictionary<string, object>() {
                    { "OrderId", orderId },
                    { "EmployeeId", employeeId },
                    { "Role", role } };

            Console.WriteLine("UserContextProvider: " + userId + " " + orderId + " " + employeeId);

            return new RVUserContext(userId, props);
        }
    }
}







// Define header names as constants
//public static class HeaderNames
//{
//    public const string CustomerId = "x-header-customerId";
//    public const string OrderId = "x-header-orderId";
//    public const string EmployeeId = "x-header-employeeId";
//}

//public IRVUserContext GetUserContext(HttpContext aspnetContext)
//{
//    // Use null-conditional operator to safely access headers
//    string userId = aspnetContext.Request.Headers[HeaderNames.CustomerId];
//    string orderId = aspnetContext.Request.Headers[HeaderNames.OrderId];
//    string employeeId = aspnetContext.Request.Headers[HeaderNames.EmployeeId];

//    string role = DetermineUserRole(userId);

//    // It's clearer to specify the Dictionary type here
//    Dictionary<string, object> props = new Dictionary<string, object>
//    {
//        { "OrderId", orderId },
//        { "EmployeeId", employeeId },
//        { "Role", role }
//    };

//    // Use a logging framework or library instead of Console.WriteLine
//    LogInformation($"UserContextProvider: {userId} {orderId} {employeeId}");

//    return new RVUserContext(userId, props);
//}

//private string DetermineUserRole(string userId)
//{
//    // Consider fetching these from a more scalable source
//    var adminUsers = new HashSet<string> { "AROUT", "BLONP" };
//    return adminUsers.Contains(userId) ? "Admin" : "User";
//}

//// Placeholder for actual logging implementation
//private void LogInformation(string message)
//{
//    // Implement logging according to your application's logging strategy
//    // For example, using ILogger<T> in ASP.NET Core
//    Console.WriteLine(message); // Replace with actual logging
//}
