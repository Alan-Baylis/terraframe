using UnityEngine;

using System.Collections.Generic;
using TerrainGeneration_PluginBase;
public class TerrainOutputMeshPanel : MonoBehaviour
{

    bool isAttached = false;
    List<Mesh> meshes;
    List<Material> materials;
    TerrainOutputMesh terrainOutput;

    List<List<Mesh>> subMeshes;
    List<List<Material>> subMaterials;

    internal void Init(TerrainOutput terrainOutput_)
    {
        terrainOutput = (TerrainOutputMesh)terrainOutput_;
        meshes = new List<Mesh>();
        materials = new List<Material>();

        subMeshes = new List<List<Mesh>>();
        subMaterials = new List<List<Material>>();

        processMesh(terrainOutput, meshes, materials);

        /*foreach (string meshPath in terrainOutput.MeshPath)
        {
            Mesh mesh = new Mesh();
            ObjImporter newMesh = new ObjImporter();
            mesh = newMesh.ImportFile(meshPath);
            meshes.Add(mesh);
            //mesh.RecalculateNormals();
        }*/
    }

    private void processMesh(TerrainOutputMesh terrainOutput, List<Mesh> meshes, List<Material> materials)
    {
        for (int chunk = 0; chunk < terrainOutput.VertexData.Count; chunk++)
        {
            Mesh mesh = new Mesh();
            Vector3[] vertexArray = new Vector3[terrainOutput.VertexData[chunk].Count];
            for (int i = 0; i < terrainOutput.VertexData[chunk].Count; i++)
            {
                vertexArray[i] = new Vector3(terrainOutput.VertexData[chunk][i][0], terrainOutput.VertexData[chunk][i][1], terrainOutput.VertexData[chunk][i][2]);
            }
            mesh.vertices = vertexArray;

            Vector3[] normalArray = new Vector3[terrainOutput.NormalData[chunk].Count];
            for (int i = 0; i < terrainOutput.NormalData[chunk].Count; i++)
            {
                normalArray[i] = new Vector3(terrainOutput.NormalData[chunk][i][0], terrainOutput.NormalData[chunk][i][1], terrainOutput.NormalData[chunk][i][2]);
            }
            mesh.normals = normalArray;

            if (terrainOutput.TexureCoordData.Count > 0 && terrainOutput.TexureCoordData[chunk].Count > 0)
            {
                Vector2[] uvArray = new Vector2[terrainOutput.TexureCoordData[chunk].Count];
                for (int i = 0; i < terrainOutput.TexureCoordData[chunk].Count; i++)
                {
                    uvArray[i] = new Vector2(terrainOutput.TexureCoordData[chunk][i][0], terrainOutput.TexureCoordData[chunk][i][1]);
                }
                mesh.uv = uvArray;
            }

            int[] triangles = new int[terrainOutput.FacesData[chunk].Count * 3];
            for (int i = 0; i < terrainOutput.FacesData[chunk].Count; i++)
            {
                int idx = i * 3;
                triangles[idx] = terrainOutput.FacesData[chunk][i][0] - 1;
                triangles[idx + 1] = terrainOutput.FacesData[chunk][i][1] - 1;
                triangles[idx + 2] = terrainOutput.FacesData[chunk][i][2] - 1;
            }
            mesh.triangles = triangles;

            mesh.RecalculateBounds();
            mesh.Optimize();

            meshes.Add(mesh);

            System.Console.WriteLine("Mesh created");
            if (terrainOutput.MaterialColor.Count > 0 || terrainOutput.MaterialMode.Count > 0 || terrainOutput.MaterialTexture.Count > 0)
            {
                System.Console.WriteLine("Creating material");
                Material material = new Material(Shader.Find("Standard"));
                material.SetFloat("_Glossiness", 0);

                if (terrainOutput.MaterialMode.Count > 0)
                {
                    Material alfaMaterial = Resources.Load<Material>("Materials/Alfa");

                    System.Console.WriteLine("Mode: " + (byte)terrainOutput.MaterialMode[chunk]);
                    material.SetFloat("_Mode", (byte)terrainOutput.MaterialMode[chunk]);

                    switch ((byte)terrainOutput.MaterialMode[chunk])
                    {
                        case 0:
                            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                            material.SetInt("_ZWrite", 1);
                            material.DisableKeyword("_ALPHATEST_ON");
                            material.DisableKeyword("_ALPHABLEND_ON");
                            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                            material.renderQueue = -1;
                            break;
                        case 1:
                            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                            material.SetInt("_ZWrite", 1);
                            material.EnableKeyword("_ALPHATEST_ON");
                            material.DisableKeyword("_ALPHABLEND_ON");
                            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                            material.renderQueue = 2450;
                            break;
                        case 2:
                            /*material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                            material.SetInt("_ZWrite", 0);
                            material.DisableKeyword("_ALPHATEST_ON");
                            material.EnableKeyword("_ALPHABLEND_ON");
                            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                            material.renderQueue = 3000;*/

                            material = alfaMaterial;
                            break;
                        case 3:
                            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                            material.SetInt("_ZWrite", 0);
                            material.DisableKeyword("_ALPHATEST_ON");
                            material.DisableKeyword("_ALPHABLEND_ON");
                            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                            material.renderQueue = 3000;
                            break;
                    }
                }

                if (terrainOutput.MaterialColor.Count > 0)
                {
                    Color color;
                    ColorUtility.TryParseHtmlString(terrainOutput.MaterialColor[chunk], out color);
                    material.SetColor("_Color", color);

                    System.Console.WriteLine(terrainOutput.MaterialColor[chunk]);
                    System.Console.WriteLine("" + color.r + " " + color.g + " " + color.b + " " + color.a);
                }

                if (terrainOutput.MaterialTexture.Count > 0 && terrainOutput.MaterialTexture[chunk].Length > 0)
                {
                    System.Console.WriteLine("file://" + terrainOutput.MaterialTexture[chunk]);
                    WWW www = new WWW("file://" + terrainOutput.MaterialTexture[chunk]);
                    Texture2D texture = new Texture2D(4, 4, TextureFormat.DXT1, false);

                    www.LoadImageIntoTexture(texture);

                    material.mainTexture = texture;
                }

                materials.Add(material);
            }
            else
            {
                System.Console.WriteLine("No material");
                materials.Add(null);
            }
        }

        if (terrainOutput.SubMeshes != null) {
            foreach (TerrainOutputMesh subMesh in terrainOutput.SubMeshes)
            {
                subMeshes.Add(new List<Mesh>());
                subMaterials.Add(new List<Material>());
                processMesh(subMesh, subMeshes[0], subMaterials[0]);
            }
        }
    }

    private void onAttach()
    {
        System.Console.WriteLine("Attach");

        GameObject meshContainer = GameObject.Find("MeshContainer");
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in meshContainer.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));

        UnityEngine.Object meshRendererPrefab = Resources.Load("Prefabs/MeshRenderer", typeof(GameObject));

        for(int i = 0; i < meshes.Count; i++)
        {
            Mesh mesh = meshes[i];
            Material material = materials[i];
            GameObject meshRenderer = (GameObject)Instantiate(meshRendererPrefab);
            meshRenderer.transform.parent = meshContainer.transform;
            //GameObject meshRenderer = GameObject.Find("MeshRenderer");
            MeshFilter filter = meshRenderer.GetComponent<MeshFilter>();

            filter.mesh = mesh;

            if(material != null)
            {
                meshRenderer.GetComponent<Renderer>().material = material;
            }
        }

        for (int j = 0; j < subMeshes.Count; j++)
        {
            for (int i = 0; i < subMeshes[j].Count; i++)
            {
                for (int k = 0; k < terrainOutput.Positions[j].Count; k++) {
                    Mesh mesh = subMeshes[j][i];
                    Material material = subMaterials[j][i];
                    GameObject meshRenderer = (GameObject)Instantiate(meshRendererPrefab);
                    meshRenderer.transform.parent = meshContainer.transform;
                    //GameObject meshRenderer = GameObject.Find("MeshRenderer");
                    MeshFilter filter = meshRenderer.GetComponent<MeshFilter>();

                    filter.mesh = mesh;

                    if (material != null)
                    {
                        meshRenderer.GetComponent<Renderer>().material = material;
                    }

                    System.Console.WriteLine("Sub pos: " + terrainOutput.Positions[j][k][0] +" "+ terrainOutput.Positions[j][k][1] + " " + terrainOutput.Positions[j][k][2]);
                    meshRenderer.transform.position = new Vector3(terrainOutput.Positions[j][k][0], terrainOutput.Positions[j][k][1], terrainOutput.Positions[j][k][2]);
                    meshRenderer.transform.localScale = new Vector3(terrainOutput.Scales[j][k][0], terrainOutput.Scales[j][k][1], terrainOutput.Scales[j][k][2]);
                }
            }
        }



        Vector3 cameraPos = new Vector3(0, 1, -10);
        Vector3 cameraRotation = new Vector3(0, 0, 0);

        if (terrainOutput.CameraPosition != null)
        {
            cameraPos = new Vector3(terrainOutput.CameraPosition[0], terrainOutput.CameraPosition[1], terrainOutput.CameraPosition[2]);
        }

        if (terrainOutput.CameraRotation != null)
        {
            cameraRotation = new Vector3(terrainOutput.CameraRotation[0], terrainOutput.CameraRotation[1], terrainOutput.CameraRotation[2]);
        }

        Camera.main.GetComponent<MainScript>().SetCameraMode(MainScript.CameraMode.DragFly, cameraPos, cameraRotation);
        Camera.main.GetComponent<MainScript>().SetCameraViewport(gameObject.GetComponent<RectTransform>());
    }

    private void onDetach()
    {
        System.Console.WriteLine("Detach");

        GameObject meshContainer = GameObject.Find("MeshContainer");
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in meshContainer.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));

        //GameObject meshRenderer = GameObject.Find("MeshRenderer");
        //MeshFilter filter = meshRenderer.GetComponent<MeshFilter>();


        //filter.mesh = null;

        Vector3 cameraPos = new Vector3(0, 1, -10);
        Vector3 cameraRotation = new Vector3(0, 0, 0);

        Camera.main.GetComponent<MainScript>().SetCameraMode(MainScript.CameraMode.Still, cameraPos, cameraRotation);
    }

    void Update()
    {
        if (gameObject.transform.parent != null)
        {
            if (!isAttached)
            {
                isAttached = true;
                onAttach();
            }
        }
        else
        {
            if (isAttached)
            {
                isAttached = false;
                onDetach();
            }
        }
    }
}
