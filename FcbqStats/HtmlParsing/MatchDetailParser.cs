using AngleSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace FcbqStats.HtmlParsing;
internal class MatchDetailParser
{

    public record MatchDetails(string statsUid);

    private IConfiguration _config;

    public MatchDetailParser()
    {
        _config = Configuration.Default.WithDefaultLoader();
    }

    public async Task<MatchDetails?> ParseContent(int matchId)
    {
        var url = $"https://www.basquetcatala.cat/partits/llistatpartits/{matchId}";
        var context = BrowsingContext.New(_config);
        var document = await context.OpenAsync(new Url(url));
        var links = document.QuerySelectorAll<IHtmlAnchorElement>("a");
        var statsLink = links.FirstOrDefault(l => l.Href.ToLowerInvariant().Contains("/estadistiques"));
        if (statsLink == null)
        {
            return null;
        }
        return new MatchDetails(statsLink.Href.Split("/")[^1]);
    }

}
