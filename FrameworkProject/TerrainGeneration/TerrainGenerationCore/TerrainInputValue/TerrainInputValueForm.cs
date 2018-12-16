using System;
using System.Collections.Generic;
using TerrainGeneration_PluginBase;

namespace TerrainGeneration_Core
{
    public class TerrainInputValueForm : TerrainInputValue
    {
        public List<string> Values { get; set; } = new List<string>();

        override public void SetValue(TerrainInput terrainInput)
        {
            foreach(FormField formField in ((TerrainInputForm)terrainInput).FormFields)
            {
                switch (formField.Type)
                {
                    case FormFieldType.Check:
                        Values.Add(""+((FormFieldCheck)formField).Value);
                        break;
                    case FormFieldType.Integer:
                        Values.Add("" + ((FormFieldInteger)formField).Value);
                        break;
                    case FormFieldType.Number:
                        Values.Add("" + ((FormFieldNumber)formField).Value);
                        break;
                    case FormFieldType.Options:
                        Values.Add("" + ((FormFieldOptions)formField).Value);
                        break;
                    case FormFieldType.Text:
                        Values.Add("" + ((FormFieldText)formField).Value);
                        break;
                }
            }
        }

        override public void FillValue(TerrainInput terrainInput)
        {
            for(int i = 0; i  < Values.Count; i++)
            {
                FormField formField = ((TerrainInputForm)terrainInput).FormFields[i];
                string value = Values[i];
                switch (formField.Type)
                {
                    case FormFieldType.Check:
                        ((FormFieldCheck)formField).Value = Boolean.Parse(value);
                        break;
                    case FormFieldType.Integer:
                        ((FormFieldInteger)formField).Value = Int32.Parse(value);
                        break;
                    case FormFieldType.Number:
                        ((FormFieldNumber)formField).Value = Double.Parse(value);
                        break;
                    case FormFieldType.Options:
                        ((FormFieldOptions)formField).Value = Int32.Parse(value);
                        break;
                    case FormFieldType.Text:
                        ((FormFieldText)formField).Value = value;
                        break;
                }
            }
        }
    }
}
