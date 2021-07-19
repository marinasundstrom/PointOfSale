namespace WebApi;

using Microsoft.AspNetCore.Http;

public class UrlBuilder
{
    private Uri? cached;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UrlBuilder(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Uri GetHostUrl()
    {
        if (cached != null) return cached;

        var request = _httpContextAccessor.HttpContext!.Request;

        bool hasNoPort = !request.Host.ToString().Contains(":");
        cached = new Uri($"{request.Scheme}://{request.Host}{(hasNoPort ? ":8080/api/" : string.Empty)}");

        return cached;
    }
}