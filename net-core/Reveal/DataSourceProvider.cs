using DocumentFormat.OpenXml.Drawing.Diagrams;
using Reveal.Sdk;
using Reveal.Sdk.Data;
using Reveal.Sdk.Data.Microsoft.SqlServer;

namespace RevealSdk.Server.Reveal
{
    internal class DataSourceProvider : IRVDataSourceProvider
    {
        public Task<RVDataSourceItem> ChangeDataSourceItemAsync
                (IRVUserContext userContext, string dashboardId, RVDataSourceItem dataSourceItem)
        {
            if (dataSourceItem is RVSqlServerDataSourceItem sqlDsi)
            {
                ChangeDataSourceAsync(userContext, sqlDsi.DataSource);


                sqlDsi.CustomQuery = (sqlDsi.Table == "All Orders" || sqlDsi.Table == "All Invoices") ?
                    (userContext.Properties["Role"] == "Admin" ? "Select * from [" + sqlDsi.Table + "]" :
                    "Select * from [" + sqlDsi.Table + "] where customerId = '" + userContext.UserId + "'") : null;

            }
            return Task.FromResult(dataSourceItem);
        }

        public Task<RVDashboardDataSource> ChangeDataSourceAsync(IRVUserContext userContext, RVDashboardDataSource dataSource)
        {
            if (dataSource is RVAzureSqlDataSource sqlDs)
            {
                sqlDs.Host = "database.windows.net";
                sqlDs.Database = "Northwind";
            }
            return Task.FromResult(dataSource);
        }
    }
}