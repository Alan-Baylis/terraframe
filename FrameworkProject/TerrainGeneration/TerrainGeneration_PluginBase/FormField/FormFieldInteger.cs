namespace TerrainGeneration_PluginBase
{
    public class FormFieldInteger : FormField
    {
        public int Value { get; set; } = 0;

        public FormFieldInteger()
        {
            Type = FormFieldType.Integer;
        }
    }
}
