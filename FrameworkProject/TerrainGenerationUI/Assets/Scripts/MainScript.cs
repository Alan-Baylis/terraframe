using System;
using TerrainGeneration_Core;
using UnityEngine;
using UnityEngine.UI;

public class MainScript : MonoBehaviour {

    public enum CameraMode
    {
        Still,
        DragFly
    }

    public Canvas canvas;

    public TerrainProjectManager TerrainProjectManager;


	// Use this for initialization
	void Start () {
        System.Console.SetOut(new CustomTextWriter());
        System.Console.WriteLine("Starting application");

        Helpers.Instance = new UnityHelpers();

        TerrainProjectManager.ShowAvailableProjects();
    }


    public CameraMode cameraMode = CameraMode.Still;

    public GameObject ProjectPanel;
    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 5F;
    public float sensitivityY = 5F;
    public float sensitivityScrollWheel = 5F;
    public float sensitivityDrag = 0.01f;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    private int screenshotSize = 300;

    float rotationY = 0F;

    bool takeScreenshot = false;
    bool takeScreenshot_ = false;
    bool dragging = false;
    Vector3 dragOrigin;
    private RectTransform rectTransform;

    void Update()
    {
        if (cameraMode.Equals(CameraMode.DragFly))
        {
            if(Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                transform.position = transform.position + transform.forward * Input.GetAxis("Mouse ScrollWheel") * sensitivityScrollWheel;
            }

            if (Input.GetMouseButton(1))
            {
                if (axes == RotationAxes.MouseXAndY)
                {
                    float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

                    rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                    rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                    transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
                }
                else if (axes == RotationAxes.MouseX)
                {
                    transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
                }
                else
                {
                    rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                    rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                    transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (!dragging)
                {
                    dragOrigin = Input.mousePosition;
                    dragging = true;
                }
                else
                {
                    Vector3 pos = Input.mousePosition - dragOrigin;
                    transform.position = transform.position + transform.right * pos.x * sensitivityDrag;
                    float prevX = transform.localEulerAngles.x;
                    transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
                    transform.position = transform.position + transform.forward * pos.y * sensitivityDrag;
                    transform.localEulerAngles = new Vector3(prevX, transform.localEulerAngles.y, transform.localEulerAngles.z);
                }
                return;
            }
            else
            {
                dragging = false;
            }

        }
    }

    public void TakeScreenShot()
    {
        takeScreenshot_ = true;
    }

    void LateUpdate()
    {
        if (takeScreenshot)
        {
            Camera camera = gameObject.GetComponent<Camera>();

            camera.pixelRect = new Rect(0, 0, Screen.width, Screen.height);

            RenderTexture rt = new RenderTexture(screenshotSize, screenshotSize, 24);
            camera.targetTexture = rt;
            Texture2D screenShot = new Texture2D(screenshotSize, screenshotSize, TextureFormat.RGB24, false);
            camera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, screenshotSize, screenshotSize), 0, 0);
            camera.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            Destroy(rt);
            int[][] values = new int[screenShot.width][];
            for (int x = 0; x < screenShot.width; x++)
            {
                values[x] = new int[screenShot.height];
                for (int y = 0; y < screenShot.height; y++)
                {
                    values[x][y] = DrawingPanel.ColorToInt(screenShot.GetPixel(x, screenShot.height-y-1));
                }
            }
            takeScreenshot = false;
            ProjectPanel.GetComponent<ProjectPanel>().SetTerrainThumbnail(values);

            if (rectTransform != null)
                SetCameraViewport(rectTransform);
        }
    }

    internal void SetCameraViewport(RectTransform rectTransform)
    {
        this.rectTransform = rectTransform;
        Rect rect = rectTransform.rect;
        gameObject.GetComponent<Camera>().pixelRect = new Rect(150, 50, rect.width,rect.height);
        if (takeScreenshot_)
        {
            takeScreenshot_ = false;
            takeScreenshot = true;
        }
    }

    public void SetCameraMode(CameraMode cameraMode, Vector3 position, Vector3 rotation)
    {
       this.cameraMode = cameraMode;
       gameObject.transform.position = position;
       gameObject.transform.rotation = Quaternion.Euler(rotation);
    }

    class UnityHelpers : Helpers
    {
        public override int[][] ResizePixels(int[][] pixels, int w1, int h1, int w2, int h2)
        {
            Console.WriteLine("Resizing in unity");
            Texture2D tex = new Texture2D(pixels.Length, pixels[0].Length);

            for (int i = 0; i < tex.width; i++)
            {
                for (int j = 0; j < tex.height; j++)
                {
                    tex.SetPixel(i, j, intToColor(pixels[i][j]));
                }
            }

            tex.Apply();
            TextureScale.Bilinear(tex, w2, h2);

            int[][] values = new int[tex.width][];
            for (int x = 0; x < tex.width; x++)
            {
                values[x] = new int[tex.height];
                for (int y = 0; y < tex.height; y++)
                {
                    values[x][y] = ColorToInt(tex.GetPixel(x, y));
                }
            }

            return values;
        }

        public override void SaveImageFile(string filename, int[][] imageData)
        {
            Texture2D tex = new Texture2D(imageData.Length, imageData[0].Length);

            for (int i = 0; i < tex.width; i++)
            {
                for (int j = 0; j < tex.height; j++)
                {
                    tex.SetPixel(i, tex.height-j-1, intToColor(imageData[i][j]));
                }
            }
            
            System.IO.File.WriteAllBytes(filename, tex.EncodeToPNG());
        }

        private static Color intToColor(int color)
        {
            return stringToColor(color.ToString("X8"));
        }

        private static Color stringToColor(string colorStr)
        {
            colorStr = colorStr.Replace("#", "");
            while (colorStr.Length < 8)
            {
                colorStr += "0";
            }

            string rStr = colorStr.Substring(0, 2);
            string gStr = colorStr.Substring(2, 2);
            string bStr = colorStr.Substring(4, 2);
            string aStr = colorStr.Substring(6, 2);

            float r = int.Parse(rStr, System.Globalization.NumberStyles.HexNumber) / (float)255;
            float g = int.Parse(gStr, System.Globalization.NumberStyles.HexNumber) / (float)255;
            float b = int.Parse(bStr, System.Globalization.NumberStyles.HexNumber) / (float)255;
            float a = int.Parse(aStr, System.Globalization.NumberStyles.HexNumber) / (float)255;

            Color color = new Color(r, g, b, a);

            return color;
        }

        public static int ColorToInt(Color color)
        {
            string rgbColor = ((int)(color.r * 255)).ToString("X2") + ((int)(color.g * 255)).ToString("X2") + ((int)(color.b * 255)).ToString("X2") + ((int)(color.a * 255)).ToString("X2");
            return int.Parse(rgbColor, System.Globalization.NumberStyles.HexNumber);
        }
    }
}
