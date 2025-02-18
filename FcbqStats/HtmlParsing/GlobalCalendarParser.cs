using AngleSharp;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;

namespace FcbqStats.HtmlParsing;

class GlobalCalendarItem()
{
    public string Date { get; set; }
    public string Hour { get; set; }
    public int LocalId { get; set; }
    public string LocalName { get; set; }
    public int AwayId { get; set; }
    public string AwayName { get; set; }
    public int MatchId { get; set; }
}

internal class GlobalCalendarParser
{
    private IConfiguration _config;

    public GlobalCalendarParser()
    {
        _config = Configuration.Default.WithDefaultLoader();
    }

    public async Task<IEnumerable<GlobalCalendarItem>> ParseContent(int clubId, int teamId)
    {
        var url = $"https://www.basquetcatala.cat/partits/calendari_equip_global/{clubId}/{teamId}";
        var context = BrowsingContext.New(_config);
        var document = await context.OpenAsync(new Url(url));
        var table = document.QuerySelector("#tbl-clubs-list") as IHtmlTableElement;
        if (table == null)
        {
            Console.WriteLine("Table tabke.tbl-clubs-list not found!!!!");
            return Array.Empty<GlobalCalendarItem>();
        }
        var matches = new List<GlobalCalendarItem>();
        foreach (var body in table.Bodies)
        {
            foreach (var row in  body.Rows)
            {
                var item = new GlobalCalendarItem();
                item.Date = row.Cells[0].QuerySelector("div.box")?.Text();
                item.Hour = row.Cells[1].QuerySelector("div.box")?.Text();
                var localHref = row.Cells[2].QuerySelector<IHtmlAnchorElement>("a").Href;
                item.LocalName = row.Cells[2].QuerySelector("a strong").Text();
                item.LocalId = int.Parse(localHref.Split('/')[^1]);
                var awayHref = row.Cells[3].QuerySelector<IHtmlAnchorElement>("a").Href;
                item.AwayName = row.Cells[3].QuerySelector("a strong").Text();
                item.AwayId = int.Parse(awayHref.Split('/')[^1]);
                var matchHref = row.Cells[6].QuerySelector<IHtmlAnchorElement>("a").Href;
                item.MatchId = int.Parse(matchHref.Split('/')[^1]);
                matches.Add(item);
            }
        }
        return matches;

    }
}
