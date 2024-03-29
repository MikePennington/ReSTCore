﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ReSTCore.Models;

namespace ReSTCore.Util
{
    public static class UriHelper
    {
        public static string Combine(this Uri uri, string uri2)
        {
            return CombineUri(uri.ToString(), uri2);
        }

        public static string CombineUri(this string uri1, string uri2)
        {
            if (uri1.Length == 0)
                return uri2;

            if (uri2.Length == 0)
                return uri1;

            uri1 = uri1.TrimEnd(new char[] {'/', '\\'});
            uri2 = uri2.TrimEnd(new char[] {'/', '\\'});

            return String.Format("{0}/{1}", uri1, uri2);
        }

        public static string ToBase64UrlString(string toEncode)
        {
            byte[] toEncodeAsBytes = Encoding.ASCII.GetBytes(toEncode);
            return HttpUtility.UrlEncode(Convert.ToBase64String(toEncodeAsBytes));
        }

        public static string FromBase64UrlString(string encodedData)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(HttpUtility.UrlDecode(encodedData));
            return Encoding.ASCII.GetString(encodedDataAsBytes);
        }
    }
}
