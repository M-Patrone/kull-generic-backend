
using System.Net.Http;

namespace Kull.GenericBackend.Utils;

public static class HttpMethodHelper
{

   //TODO: write tests 
    public static bool TryParseHttpMethod(string? s, out HttpMethod method)
    {// default placeholder
        method = HttpMethod.Get; 

        if (string.IsNullOrWhiteSpace(s))
            return false;
        
        //this would also allow for custom headers => TODO
        method = new HttpMethod(s.Trim().ToUpperInvariant());
        return true;
    }
}
