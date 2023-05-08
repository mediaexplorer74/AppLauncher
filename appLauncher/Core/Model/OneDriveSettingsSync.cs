using Azure.Identity;

using Microsoft.Graph;

using System;
using System.Threading.Tasks;

namespace appLauncher.Core.Model
{
    public static class OneDriveSettingsSync
    {
        public static async Task SyncAppSettings()
        {
            //HttpClient http = new HttpClient();
            ////WebView view = new WebView(WebViewExecutionMode.SameThread);
            ////view.Navigate(new System.Uri("https://login.microsoftonline.com/common/oauth2/v2.0/authorize?client_id=2803478f-2b70-49d6-98c7-5673fde1122d&scope=files.readwrite&response_type=code&redirect_uri=https://127.0.0.1/"));
            ////view.Visibility = Windows.UI.Xaml.Visibility.Visible;
            //HttpResponseMessage response = await http.GetAsync("https://login.microsoftonline.com/common/oauth2/v2.0/authorize?client_id=2803478f-2b70-49d6-98c7-5673fde1122d&scope=files.readwrite&response_type=code&redirect_uri=https://login.microsoftonline.com/common/oauth2/nativeclient");
            ////var json = await response.Content.ReadAsStringAsync();
            ////var result = JsonConvert.DeserializeObject<JsonObject>(json);
            //if (response.StatusCode == System.Net.HttpStatusCode.OK)
            //{
            //    HttpResponseHeaders headers = response.Headers;
            //    if (headers != null && headers.Location != null)
            //    {
            //        var redirectedUrl = headers.Location.AbsoluteUri;
            //    }
            //}
            //var a = await response.Content.ReadAsStreamAsync();
            var scopes = new[] { "User.Read" };

            // Multi-tenant apps can use "common",
            // single-tenant apps must use the tenant ID from the Azure portal
            var tenantId = "common";

            // Value from app registration
            var clientId = "2803478f-2b70-49d6-98c7-5673fde1122d";

            // using Azure.Identity;
            var options = new InteractiveBrowserCredentialOptions
            {
                TenantId = tenantId,
                ClientId = clientId,
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
                // MUST be http://localhost or http://localhost:PORT
                // See https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/System-Browser-on-.Net-Core
                RedirectUri = new Uri("http://localhost"),
            };

            // https://learn.microsoft.com/dotnet/api/azure.identity.interactivebrowsercredential
            var interactiveCredential = new InteractiveBrowserCredential(options);

            var graphClient = new GraphServiceClient(interactiveCredential, scopes);
            await Task.Delay(1500);
        }
    }
}
