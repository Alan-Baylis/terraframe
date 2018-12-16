using System.Collections.Generic;

namespace TerrainGeneration_PluginBase
{
    public class TerrainOutputMesh : TerrainOutput
    {
        public enum RenderingMode
        {
            opaque = 0,
            fade = 2,
            transparent = 3
        }

        public List<string> MeshPath { get; set; } = new List<string>();


        public List<List<float[]>> VertexData { get; set; } = new List<List<float[]>>();
        public List<List<int[]>> FacesData { get; set; } = new List<List<int[]>>();
        public List<List<int[]>> FacesTextureCoordData { get; set; } = new List<List<int[]>>(); 
        public List<List<float[]>> TexureCoordData { get; set; } = new List<List<float[]>>();
        public List<string> MaterialName { get; set; } = new List<string>();
        public List<List<float[]>> NormalData { get; set; } = new List<List<float[]>>();
        public List<List<int[]>> FaceNormalData { get; set; } = new List<List<int[]>>();


        public List<string> MaterialColor { get; set; } = new List<string>();
        public List<string> MaterialTexture { get; set; } = new List<string>();
        public List<RenderingMode> MaterialMode { get; set; } = new List<RenderingMode>();


        public List<TerrainOutputMesh> SubMeshes { get; set; } = new List<TerrainOutputMesh>();
        public List<List<float[]>> Positions { get; set; } = new List<List<float[]>>();
        public List<List<float[]>> Rotations { get; set; } = new List<List<float[]>>();
        public List<List<float[]>> Scales { get; set; } = new List<List<float[]>>();
    

        public string MaterialFile { get; set; } = "";
        public float[] CameraPosition { get; set; }
        public float[] CameraRotation { get; set; }

        public TerrainOutputMesh()
        {
            Type = OutputType.Mesh;
        }

        public TerrainOutputMesh(string title)
        {
            Title = title;
            Type = OutputType.Mesh;
        }
    }
}
