namespace TerrainGeneration_PluginBase
{
    public class FormFieldNumber : FormField
    {
        public double Value { get; set; } = 0;

        public FormFieldNumber()
        {
            Type = FormFieldType.Number;
        }
    }
}
