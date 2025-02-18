using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace FcbqStats.HtmlParsing;
internal class ClubPageParser
{
    private IConfiguration _config;
    public ClubPageParser()
    {
        _config = Configuration.Default.WithDefaultLoader();
    }

    public async Task<IEnumerable<(string Text, int Code)>> ParseContent(int clubId)
    {
        var url = $"https://www.basquetcatala.cat/club/{clubId}";
        var context = BrowsingContext.New(_config);
        var document = await context.OpenAsync(new Url(url));
        var teams = document.QuerySelectorAll("a.c-0").OfType<IHtmlAnchorElement>().Select(l => (l.Text, l.Href))
            .Where(t => t.Href.Contains("/equip/"))
            .Select(t => (t.Text, t.Href.Split('/')[^1]))
            .Where(t => int.TryParse(t.Item2, out var id))
            .Select(t => (t.Text, int.Parse(t.Item2)));
        return teams;
    }
}

