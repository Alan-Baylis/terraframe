using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TerrainGeneration_PluginBase;

namespace TerrainGeneration_Core
{
    public class TerrainPipelineLayer
    {
        public byte LayerCode { get; set; }
        public int PluginId { get; set; }
        public bool HasValidPlugin { get { return PluginId != 0; } }

        [XmlIgnore]
        public HashSet<LayerType> LayerTypes {
            get
            {
                return ToLayerTypesHash(LayerCode);
            }
        }

        internal static HashSet<LayerType> ToLayerTypesHash(byte layerCode)
        {
            HashSet<LayerType> layerTypes = new HashSet<LayerType>();
            foreach (LayerType type in Enum.GetValues(typeof(LayerType)))
            {
                if ((layerCode & (byte)type) > 0)
                    layerTypes.Add(type);
            }

            return layerTypes;
        }

        internal static byte ToPipelineLayerCode(HashSet<LayerType> _out, HashSet<LayerType> _in, HashSet<LayerType> _not)
        {
            byte code = 0;

            foreach (LayerType type in _out)
                code = (byte)(code | (byte)type);
            return code;
        }

        public TerrainPipelineLayer Clone()
        {
            TerrainPipelineLayer terrainPipelineLayer = new TerrainPipelineLayer();
            terrainPipelineLayer.LayerCode = LayerCode;
            terrainPipelineLayer.PluginId = PluginId;
            return terrainPipelineLayer;
        }
    }
}
