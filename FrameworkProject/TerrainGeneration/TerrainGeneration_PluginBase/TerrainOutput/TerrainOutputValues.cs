using System.Collections.Generic;

namespace TerrainGeneration_PluginBase
{
    public class TerrainOutputValues : TerrainOutput
    {
        public List<string> ValueTitles { get; set; } = new List<string>();
        public List<string> ValueKeys { get; set; } = new List<string>();
        public List<string> Values { get; set; } = new List<string>();

        public TerrainOutputValues()
        {
            Type = OutputType.Values;
        }

        public TerrainOutputValues(string title)
        {
            Type = OutputType.Values;
            Title = title;
        }

        public void AddValue(string title, string value, string key)
        {
            ValueTitles.Add(title);
            Values.Add(value);
            ValueKeys.Add(key);
        }
    }
}
