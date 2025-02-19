using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FcbqStats.Config;

public record CurrentConfig(int ClubId, int TeamId, int MatchId);
public class CurrentConfigManager
{
    private const string ConfigFileName = "fcbqstats.json";

    public async Task<CurrentConfig> Load()
    {
        if (!File.Exists(ConfigFileName))
        {
            return new CurrentConfig(-1, -1, -1);
        }
        await using (var file = System.IO.File.OpenRead(ConfigFileName))
        {
            return await System.Text.Json.JsonSerializer.DeserializeAsync<CurrentConfig>(file) ??
                   new CurrentConfig(-1, -1, -1);
        }
    }

    public async Task Save(CurrentConfig config)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(config);
        await File.WriteAllTextAsync(ConfigFileName, json);
    }
}
