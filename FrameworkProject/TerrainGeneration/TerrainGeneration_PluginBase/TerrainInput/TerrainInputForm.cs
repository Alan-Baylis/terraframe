using System.Collections.Generic;

namespace TerrainGeneration_PluginBase
{
    public class TerrainInputForm : TerrainInput
    {
        public List<FormField> FormFields { get; set; } = new List<FormField>();

        public TerrainInputForm()
        {
            Type = InputType.Form;
        }

        public TerrainInputForm(string title)
        {
            Type = InputType.Form;
            Title = title;
        }
    }
}
