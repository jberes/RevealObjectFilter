using Reveal.Sdk;
using Reveal.Sdk.Data;
using Reveal.Sdk.Data.Microsoft.SqlServer;

namespace RevealSdk.Server.Reveal
{
    public class AuthenticationProvider : IRVAuthenticationProvider
    {
        public Task<IRVDataSourceCredential> ResolveCredentialsAsync(IRVUserContext userContext,
            RVDashboardDataSource dataSource)
        {
            IRVDataSourceCredential userCredential = new RVUsernamePasswordDataSourceCredential();
            
            if (dataSource is RVAzureSqlDataSource)
            {
                userCredential = new RVUsernamePasswordDataSourceCredential("uid", "pwd");
            }
            return Task.FromResult(userCredential);
        }
    }
}

