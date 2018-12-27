using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kgtwebClient.Helpers
{
    public class LoginHelper
    {
        public static bool IsAuthenticated()
        {
            return (!String.IsNullOrWhiteSpace((string)System.Web.HttpContext.Current.Session["token"])
               && !String.IsNullOrWhiteSpace(((int)System.Web.HttpContext.Current.Session["CurrentUserId"]).ToString()));
        }

        public static bool IsCurrentUserAdmin()
        {
            return (bool)(System.Web.HttpContext.Current.Session["isAdmin"] ?? false);
        }

        public static bool IsCurrentUserMember()
        {
            return (bool)(System.Web.HttpContext.Current.Session["isMember"] ?? false);
        }

        public static int GetCurrentUserId()
        {
            return (int)(System.Web.HttpContext.Current.Session["CurrentUserId"] ?? 0);
        }

        public class TokenResponse
        {
            [JsonProperty(PropertyName = "access_token")]
            public string AccessToken { get; set; }

            [JsonProperty(PropertyName = "expires_in")]
            public int ExpiresIn { get; set; }

            public DateTime ExpiresAt { get; set; }

            public string Scope { get; set; }

            [JsonProperty(PropertyName = "token_type")]
            public string TokenType { get; set; }

            public bool IsValidAndNotExpiring
            {
                get
                {
                    return !String.IsNullOrEmpty(this.AccessToken) &&
              this.ExpiresAt > DateTime.UtcNow.AddSeconds(30);
                }
            }
        }
    }
}