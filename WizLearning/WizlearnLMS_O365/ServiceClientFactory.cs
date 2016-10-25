using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Office365.Discovery;
using Microsoft.Office365.OutlookServices;
using Microsoft.Office365.SharePoint.CoreServices;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WizlearnLMS_O365
{
    public class ServiceClientFactory
    {
        private string _userUniqueId;

        private string _objectId;

        private string _tenantId;

        private AuthenticationContext _authContext;

        public string ServiceEndpointUri { get; set;}

        public ServiceClientFactory()
        {
            _userUniqueId = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;

            _objectId = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

            _tenantId = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;

            Assert.ThrowExceptionIfIsNullOrWhiteSpace(_userUniqueId, "User UniqueId should be be null, empty or write space");

            Assert.ThrowExceptionIfIsNullOrWhiteSpace(_objectId, "User UniqueId should be be null, empty or write space");

            Assert.ThrowExceptionIfIsNullOrWhiteSpace(_tenantId, "Tenant Id should be be null, empty or write space");

            var authority = string.Format("{0}/{1}", ServiceConstants.AzureADEndPoint, _tenantId);

            _authContext = new AuthenticationContext(authority, new SqlDBTokenCache(_userUniqueId));
        }

        public async Task<string> GetAccessToken()
        {

            string accessToken = string.Empty;


            var signInUserId = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userObjectId = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;


            DiscoveryClient discClient = new DiscoveryClient(new Uri(ServiceConstants.DiscoverySvcEndpointUri),
                async () =>
                {
                    var authResult = await _authContext.AcquireTokenSilentAsync(ServiceConstants.DiscoverySvcResourceId,
                                                                             new ClientCredential(OAuthSettings.ClientId,
                                                                                                  OAuthSettings.ClientSecret),
                                                                             new UserIdentifier(_objectId,
                                                                                                UserIdentifierType.UniqueId));
                    return authResult.AccessToken;


                });

            var dcr = await discClient.DiscoverCapabilityAsync("MyFiles");

            ServiceEndpointUri = dcr.ServiceEndpointUri.ToString();


            var authResult2 = await _authContext.AcquireTokenSilentAsync(dcr.ServiceResourceId,
                                                                                  new ClientCredential(OAuthSettings.ClientId,
                                                                                                       OAuthSettings.ClientSecret),
                                                                                  new UserIdentifier(_objectId,
                                                                                                     UserIdentifierType.UniqueId));

            accessToken = authResult2.AccessToken;

            return accessToken;
        }


        public async Task<OutlookServicesClient> CreateOutlookServicesClientWithAsync(string capabilityName)
        {
            try
            {
                var sqlDbTokenCache = _authContext.TokenCache as SqlDBTokenCache;

                sqlDbTokenCache.EnsureHasCacheMatched();

                DiscoveryClient discClient = new DiscoveryClient(new Uri(ServiceConstants.DiscoverySvcEndpointUri),

                async () =>
                {
                    var authResult = await _authContext.AcquireTokenSilentAsync(ServiceConstants.DiscoverySvcResourceId,
                                                                               new ClientCredential(OAuthSettings.ClientId,
                                                                                                    OAuthSettings.ClientSecret),
                                                                               new UserIdentifier(_objectId,
                                                                                                  UserIdentifierType.UniqueId));


                    return authResult.AccessToken;
                });


                var dcr = await discClient.DiscoverCapabilityAsync(capabilityName);

                return new OutlookServicesClient(dcr.ServiceEndpointUri,
                    async () =>
                    {
                        var authResult = await _authContext.AcquireTokenSilentAsync(dcr.ServiceResourceId,
                                                                                   new ClientCredential(OAuthSettings.ClientId,
                                                                                                        OAuthSettings.ClientSecret),
                                                                                   new UserIdentifier(_objectId,
                                                                                                      UserIdentifierType.UniqueId));

                        return authResult.AccessToken;
                    });
            }
            catch (AdalException exception)
            {
                if (exception.ErrorCode == AdalError.FailedToAcquireTokenSilently)
                {
                    _authContext.TokenCache.Clear();

                    throw new Office365AssertFailedException("Failed to acquire token silently");
                }

                return null;
            }
            catch (DiscoveryFailedException)
            {
                throw new Office365AssertFailedException("Failed to discovery the service, please ensure the app is registered under Office 365 tenant ad and grant the sufficient permissionss");
            }
        }

        public async Task<SharePointClient> CreateSharepointServicesClientWithAsync(string capabilityName)
        {
            try
            {
                var sqlDbTokenCache = _authContext.TokenCache as SqlDBTokenCache;

                sqlDbTokenCache.EnsureHasCacheMatched();

                DiscoveryClient discClient = new DiscoveryClient(new Uri(ServiceConstants.DiscoverySvcEndpointUri),

                async () =>
                {
                    var authResult = await _authContext.AcquireTokenSilentAsync(ServiceConstants.DiscoverySvcResourceId,
                                                                               new ClientCredential(OAuthSettings.ClientId,
                                                                                                    OAuthSettings.ClientSecret),
                                                                               new UserIdentifier(_objectId,
                                                                                                  UserIdentifierType.UniqueId));

                    return authResult.AccessToken;
                });

                var etst = await discClient.DiscoverCapabilitiesAsync();

                var dcr = await discClient.DiscoverCapabilityAsync(capabilityName);

                return new SharePointClient(dcr.ServiceEndpointUri,
                    async () =>
                    {
                        var authResult = await _authContext.AcquireTokenSilentAsync(dcr.ServiceResourceId,
                                                                                   new ClientCredential(OAuthSettings.ClientId,
                                                                                                        OAuthSettings.ClientSecret),
                                                                                   new UserIdentifier(_objectId,
                                                                                                      UserIdentifierType.UniqueId));

                        return authResult.AccessToken;
                    });
            }
            catch (AdalException exception)
            {
                if (exception.ErrorCode == AdalError.FailedToAcquireTokenSilently)
                {
                    _authContext.TokenCache.Clear();

                    throw new Office365AssertFailedException("Failed to acquire token silently");
                }

                return null;
            }
            catch(DiscoveryFailedException)
            {
                throw new Office365AssertFailedException("Failed to discovery the service, please ensure the app is registered under Office 365 tenant ad and grant the sufficient permissionss");
            }
        }
    }
}