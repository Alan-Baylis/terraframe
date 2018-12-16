using System.Collections.Generic;

namespace TerrainGeneration_PluginBase
{
    public class FormFieldOptions : FormField
    {
        public int Value { get; set; } = 0;
        public List<string> Options { get; set; } = new List<string>();

        public FormFieldOptions()
        {
            Type = FormFieldType.Options;
        }
    }
}
