using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using TerrainGeneration_PluginBase;

namespace TerrainGeneration_Core
{
    [XmlInclude(typeof(TerrainInputValueSketch)), XmlInclude(typeof(TerrainInputValueForm))]
    public abstract class TerrainInputValue
    {
        public abstract void SetValue(TerrainInput terrainInput);
        public abstract void FillValue(TerrainInput terrainInput);
    }
}
