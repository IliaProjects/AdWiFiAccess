using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace GWA.Classes
{
    public static class IdentityExtensions
    {
        /// <summary>
        ///     Return the user name using the UserNameClaimType
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static string GetUserName(this IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }
            if (identity is ClaimsIdentity claimsIdentity)
            {
                return claimsIdentity.FindFirstValue(ClaimTypes.Name);
            }
            return null;
        }

        /// <summary>
        ///     Return the user id using the UserIdClaimType
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static T GetUserId<T>(this IIdentity identity) where T : IConvertible
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }
            if (identity is ClaimsIdentity claimsIdentity)
            {
                string text = claimsIdentity.FindFirstValue(ClaimTypes.NameIdentifier);
                if (text != null)
                {
                    return (T)((object)Convert.ChangeType(text, typeof(T), CultureInfo.InvariantCulture));
                }
            }
            return default(T);
        }

        /// <summary>
        ///     Return the user id using the UserIdClaimType
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static string GetUserId(this IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }
            if (identity is ClaimsIdentity claimsIdentity)
            {
                return claimsIdentity.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            return null;
        }

        /// <summary>
        /// Return the user name using the GivenNameClaimType
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static string GetFirstName(this IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }
            if (identity is ClaimsIdentity claimsIdentity)
            {
                return claimsIdentity.FindFirstValue(ClaimTypes.GivenName);
            }
            return null;
        }

        /// <summary>
        /// Return the user name using the SurnameClaimType
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static string GetLastName(this IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }
            if (identity is ClaimsIdentity claimsIdentity)
            {
                return claimsIdentity.FindFirstValue(ClaimTypes.Surname);
            }
            return null;
        }

        /// <summary>
        /// Return the user name using the CustomerIdClaimType
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static string GetCustomerId(this IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }
            if (identity is ClaimsIdentity claimsIdentity)
            {
                return claimsIdentity.FindFirstValue("CustomerId");
            }
            return null;
        }

        /// <summary>
        /// Return the user name using the EmailClaimType
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static string GetEmail(this IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }
            if (identity is ClaimsIdentity claimsIdentity)
            {
                return claimsIdentity.FindFirstValue(ClaimTypes.Email);
            }
            return null;
        }

        public static bool GetIsRememberMe(this IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }
            if (identity is ClaimsIdentity claimsIdentity)
            {
                return Convert.ToBoolean(claimsIdentity.FindFirstValue("RememberMe"));
            }
            return false;
        }

        public static string GetSessionId(this IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }
            if (identity is ClaimsIdentity claimsIdentity)
            {
                return Convert.ToString(claimsIdentity.FindFirstValue("SessionId"));
            }
            return null;
        }

        /// <summary>
        ///     Return the claim value for the first claim with the specified type if it exists, null otherwise
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="claimType"></param>
        /// <returns></returns>
        public static string FindFirstValue(this ClaimsIdentity identity, string claimType)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }
            Claim claim = identity.FindFirst(claimType);
            if (claim == null)
            {
                return null;
            }
            return claim.Value;
        }
    }
}
