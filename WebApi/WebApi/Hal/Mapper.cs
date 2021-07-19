namespace WebApi.Hal;

public partial class Mapper
{
    private readonly UrlBuilder urlBuilder;

    public Mapper(UrlBuilder urlBuilder)
    {
        this.urlBuilder = urlBuilder;
    }

    public string CreateEmbedQuery(string[] embed)
    {
        if (embed.Length == 0)
            return string.Empty;

        return $"&embed={string.Join("&embed=", embed)}";
    }

    public TModel Append<TModel>(string urlBase, TModel model, int skip, int limit, string[] embed, bool hasMore)
        where TModel : Resource
    {
        var host = urlBuilder
            .GetHostUrl()
            .ToString()
            .TrimEnd('/');

        model.Links.Add("self", new Link
        {
            Href = $"{host}{urlBase}?skip={skip}&limit={limit}{CreateEmbedQuery(embed)}"
        });

        /*

        model.Links.Add("find", new Link
        {
            Href = "{host}{urlBase}?page={?page}",
        });

        */

        if (skip != 0 && skip >= limit)
        {
            model.Links.Add("previous", new Link
            {
                Href = $"{host}{urlBase}?skip={skip - limit}&limit={limit}{CreateEmbedQuery(embed)}"
            });
        }

        if (hasMore)
        {
            model.Links.Add("next", new Link
            {
                Href = $"{host}{urlBase}?skip={skip + limit}&limit={limit}{CreateEmbedQuery(embed)}"
            });
        }

        return model;
    }
}