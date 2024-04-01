# Using ObjectFilter to Cusomtize the Data Sources Dialog in Reveal

This document provides a detailed explanation of the server and client-side code for the Reveal SDK. The code is designed to filter data based on user context and pass headers from the client to the server.

## Overview

The provided code is part of the Reveal SDK and is used to implement custom object filtering based on user context. The `ObjectFilterProvider` class implements the `IRVObjectFilter` interface, which provides methods for filtering both dashboard data sources and individual data source items.

## ObjectFilterProvider Class

### Description
The `ObjectFilterProvider` class provides object filtering functionality for the Reveal SDK. It implements the `IRVObjectFilter` interface.

### Methods

#### Filter(IRVUserContext userContext, RVDashboardDataSource dataSource)
This method filters the specified Reveal dashboard data source based on the user context. It takes two parameters: `userContext` and `dataSource`. The `userContext` parameter represents the user context, while the `dataSource` parameter represents the Reveal dashboard data source. The method returns a task that represents the asynchronous filtering operation. The task result contains a boolean value indicating whether the data source is filtered or not.

```csharp
public Task<bool> Filter(IRVUserContext userContext, RVDashboardDataSource dataSource)
{
    throw new NotImplementedException();
}
```

#### Filter(IRVUserContext userContext, RVDataSourceItem dataSourceItem)
This method filters the specified Reveal data source item based on the user context. It takes two parameters: `userContext` and `dataSourceItem`. The `userContext` parameter represents the user context, while the `dataSourceItem` parameter represents the Reveal data source item. The method returns a task that represents the asynchronous filtering operation. The task result contains a boolean value indicating whether the data source item is filtered or not.

```csharp
public Task<bool> Filter(IRVUserContext userContext, RVDataSourceItem dataSourceItem)
{
    if (userContext?.Properties != null && dataSourceItem is RVSqlServerDataSourceItem dataSQLItem)
    {
        if (userContext.Properties.TryGetValue("Role", out var roleObj) &&
            roleObj?.ToString()?.ToLower() == "user")
        {
            var allowedItems = new HashSet<string> { "All Invoices", "All Orders" };

            if ((dataSQLItem.Table != null && !allowedItems.Contains(dataSQLItem.Table)) ||
                (dataSQLItem.Procedure != null && !allowedItems.Contains(dataSQLItem.Procedure)))
            {
                return Task.FromResult(false);
            }
        }
    }
    return Task.FromResult(true);
}
```

## UserContextProvider Class

### Description
The `UserContextProvider` class implements the `IRVUserContextProvider` interface, which provides a method for getting the user context.

### Methods

#### GetUserContext(HttpContext aspnetContext)
This method retrieves the user context based on the provided HttpContext. It extracts the user ID, order ID, and employee ID from the request headers. It then determines the user's role and constructs a dictionary of properties. Finally, it returns a new `RVUserContext` object containing the user ID and the properties dictionary.

```csharp
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
```

## References
For more information, please refer to the official Reveal SDK documentation:

- [User Context](https://help.revealbi.io/web/user-context)
- [IRVObjectFilter Interface](https://help.revealbi.io/api/aspnet/latest/Reveal.Sdk.Data.IRVObjectFilter.html)
- [IRVUserContext Interface](https://help.revealbi.io/api/aspnet/latest/Reveal.Sdk.IRVUserContext.html)



## Server-Side Code

### UserContextProvider Class

The `UserContextProvider` class implements the `IRVUserContextProvider` interface and provides a method to get the user context from the HTTP context.

```csharp
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
```

This class retrieves the user ID, order ID, and employee ID from the HTTP headers. It then determines the user's role based on the user ID. If the user ID is "AROUT" or "BLONP", the user is an admin; otherwise, they are a regular user. The method then creates a dictionary of properties containing the order ID, employee ID, and role, and returns a new `RVUserContext` object with the user ID and properties.

### ObjectFilterProvider Class

The `ObjectFilterProvider` class implements the `IRVObjectFilter` interface and provides methods to filter data sources and data source items based on the user context.

```csharp
public class ObjectFilterProvider : IRVObjectFilter
{
    public Task<bool> Filter(IRVUserContext userContext, RVDashboardDataSource dataSource)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Filter(IRVUserContext userContext, RVDataSourceItem dataSourceItem)
    {
        if (userContext?.Properties != null && dataSourceItem is RVSqlServerDataSourceItem dataSQLItem)
        {
            if (userContext.Properties.TryGetValue("Role", out var roleObj) &&
                roleObj?.ToString()?.ToLower() == "user")
            {
                var allowedItems = new HashSet<string> { "All Invoices", "All Orders" };

                if ((dataSQLItem.Table != null && !allowedItems.Contains(dataSQLItem.Table)) ||
                    (dataSQLItem.Procedure != null && !allowedItems.Contains(dataSQLItem.Procedure)))
                {
                    return Task.FromResult(false);
                }
            }
        }
        return Task.FromResult(true);
    }
}
```

The `Filter` method for `RVDashboardDataSource` is not implemented in this example. The `Filter` method for `RVDataSourceItem` checks if the user context and properties are not null and if the data source item is an `RVSqlServerDataSourceItem`. If the user's role is "user", it checks if the data source item's table or procedure is not in the allowed items list. If it is not, the method returns a task with a result of `false`, indicating that the data source item is filtered. Otherwise, it returns a task with a result of `true`.

## Client-Side Code

The client-side code is written in JavaScript and is included in an HTML file. It uses the Reveal SDK to create a Reveal view, set additional headers, and request data sources.

```html
<script type="text/javascript">
    $.ig.RevealSdkSettings.setBaseUrl("https://localhost:7006/");

    $.ig.RevealSdkSettings.setAdditionalHeadersProvider(function (url) {
        return headers;
    });

    const headers = {};

    var revealView = new $.ig.RevealView("#revealView");
    revealView.interactiveFilteringEnabled = true;
    revealView.startInEditMode = true;  

    revealView.onDataSourcesRequested = (callback) => {    

        // Set values to pass to server
        var selectedCustomerId = $('#customerId').val();
        headers["x-header-customerId"] = selectedCustomerId;

        var selectedOrderId = $('#orderId').val();
        headers["x-header-orderId"] = selectedOrderId;

        var selectedEmployeeId = $('#employeeId').val();
        headers["x-header-employeeId"] = selectedEmployeeId;

        // What am I passing to the server
        console.log(headers);

        // create my data source
        var ds = new $.ig.RVAzureSqlDataSource();
        ds.id="sqlServer";
        ds.title = "SQL Server Data Source";
        ds.subtitle = "Full Northwind Database";

        callback(new $.ig.RevealDataSources([ds], [ ], false));        
    };     
</script>
```

The `setAdditionalHeadersProvider` method sets a function that returns the headers to be included in the request to the Reveal backend. The `onDataSourcesRequested` event handler sets the values to pass to the server based on the selected values in the HTML select elements. It then creates a new `RVAzureSqlDataSource` and calls the callback function with the new data source.

## How It All Works Together

When a user interacts with the Reveal view on the client side, the selected values from the HTML select elements are used to set headers in the HTTP request sent to the server for data retrieval. The `UserContextProvider` class on the server side retrieves these values from the headers and creates a user context. The `ObjectFilterProvider` class then uses this user context to filter the data source items. This ensures that the data displayed in the Reveal view is appropriate for the user's role and selections.
