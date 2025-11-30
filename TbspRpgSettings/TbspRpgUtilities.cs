using System.Text.RegularExpressions;
using TbspRpgSettings.Settings;

namespace TbspRpgSettings;

public class TbspRpgUtilities
{
    public Regex EmbeddedSourceScriptRegex = new Regex(@"{\s*" + SourceTagTypes.Script + @"\s*\:([^}]*)}");
    public Regex EmbeddedObjectRegex = new Regex(@"{\s*" + SourceTagTypes.Object + @"\s*\:([^}]*)}");
    public static readonly int DB_EMPTY_ID = 0;
}