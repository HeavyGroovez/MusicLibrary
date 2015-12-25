using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicLibrary.Helpers
{
    public static class StringHelper
    {
        // String truncater
        public static string Truncate(string data, int maxLen) 
        { 
            if (data.Length > 0)
            {
                if (data.Length >= maxLen)
                {
                    return data.Substring(0, maxLen);
                }
                else
                {
                    return data;
                }
            }
            else
            {
                return "";
            }
        }
    }
}