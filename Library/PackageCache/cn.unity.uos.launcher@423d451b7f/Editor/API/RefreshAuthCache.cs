using Unity.UOS.Networking;
using System;
using Unity.UOS.Common;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unity.UOS.Launcher
{
    public class RefreshAuthCache
    {
        private static HttpClient Client(string appID, string appServiceSecret, Uri baseUrl = null)
        {
            var authenticator = new HttpBasicAuthenticator(appID, appServiceSecret);
            var client = new HttpClient(new HttpClientOptions()
            {
                SdkName = "",
                SdkVersion = "",
                Authenticator = authenticator,
                BaseUrl = baseUrl == null ? Endpoints.UosEndpoint: baseUrl,
            });
            return client;
        }
        
        public static async void RefreshMultiverse()
        {
            var client = Client(Settings.AppID, Settings.AppServiceSecret, Endpoints.MultiverseEndpoint);
            var url = "/v1/game/refreshUosToken";
            var headers = new Dictionary<string, string>()
            {
                {
                    "Grpc-Metadata-X-Cache-Control", "no-cache"
                }
            };
            await client.Get<RefreshTokenResponse>(url, null, headers);
        }
        
        public static async void RefreshCdn()
        {
            var headers = new Dictionary<string, string>()
            {
                {
                    "Cache-Control", "no-cache"
                }
            };
            var client = Client(Settings.AppID, Settings.AppServiceSecret, Endpoints.CdnEndpoint);
            var url = "/api/v1/users/me/rcache/";
            await client.Get<RefreshTokenResponse>(url, null, headers);
        }
        
        public static async void RefreshSync()
        {
            var headers = new Dictionary<string, string>()
            {
                {
                    "Cache-Control", "no-cache"
                }
            };
            var client = Client(Settings.AppID, Settings.AppServiceSecret, Endpoints.SyncEndpoint);
            var url = $"/api/app/{Settings.AppID}/recache";
            await client.Post<RefreshTokenResponse>(url, "", null, headers);
        }
    }
}