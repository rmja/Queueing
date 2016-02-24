using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService
{
    public class RouteMatching
    {
        public static bool RouteMatchesPattern(string route, string pattern)
        {
            if (route == pattern)
            {
                return true;
            }

            var routeParts = route.Split('.');
            var patternParts = pattern.Split('.');

            for (var i = 0; i < patternParts.Length; i++)
            {
                var patternPart = patternParts[i];

                if (patternPart == "#")
                {
                    return true;
                }
                else if (i < routeParts.Length)
                {
                    if (routeParts[i] == patternPart || patternPart == "*")
                    {
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}
