namespace TerrainGeneration_PluginBase
{
    public class FormFieldText : FormField
    {
        public string Value { get; set; } = "";

        public FormFieldText()
        {
            Type = FormFieldType.Text;
        }
    }
}
