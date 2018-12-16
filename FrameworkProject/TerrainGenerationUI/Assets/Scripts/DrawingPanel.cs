using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TerrainGeneration_PluginBase;
using System;
using System.IO;

public class DrawingPanel : MonoBehaviour {

    private Texture2D tex;
    private Vector3 previousMousePos;
    private Vector3 currentMousePos;
    private bool mousePressed = false;
    private SketchTool currentTool;
    private Color toolColor;
    private Color baseColor;
    public GameObject Camera;

    public void Init(int width, int height, string baseColorStr, int[][] value)
    {
        baseColor = stringToColor(baseColorStr);
        tex = new Texture2D(width, height);

        if(value != null && value.Length == width && value.Length > 0 && value[0].Length == height)
        {
            for (int i = 0; i < tex.width; i++)
            {
                for (int j = 0; j < tex.height; j++)
                {
                    tex.SetPixel(i, j, intToColor(value[i][j]));
                }
            }
        }
        else
        {
            for (int i = 0; i < tex.width; i++)
            {
                for (int j = 0; j < tex.height; j++)
                {
                    tex.SetPixel(i, j, baseColor);
                }
            }
        }

        tex.Apply();
        gameObject.GetComponent<RawImage>().texture = tex;
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        gameObject.GetComponent<RectTransform>().transform.position = new Vector3(-75, 0, 0);
    }

    public void LoadFile() 
    {
        /*string path = EditorUtility.OpenFilePanel("Overwrite with png", "", "png");
        if (path.Length != 0)
        {
            byte[] fileData = File.ReadAllBytes(path);
            tex.LoadImage(fileData);
        }*/
    }

    void Update()
    {
 
        if (tex != null)
        {
            if (Input.GetButton("Fire1") && RectTransformUtility.RectangleContainsScreenPoint(gameObject.GetComponent<RectTransform>(), Input.mousePosition, null))
            {
                bool mouseWasPressed = false;
                if (!mousePressed)
                {
                    mouseWasPressed = true;
                    previousMousePos = Input.mousePosition;
                    currentMousePos = Input.mousePosition;
                    mousePressed = true;
                }
                else
                {
                    previousMousePos = currentMousePos;
                    currentMousePos = Input.mousePosition;
                }
                draw(mouseWasPressed);
            }
            else
            {
                if (mousePressed)
                {
                    previousMousePos = currentMousePos;
                    currentMousePos = Input.mousePosition;
                    mousePressed = false;
                    draw(false);
                }
            }
        }
    }

    internal int[][] GetImageData()
    {
        int[][] values = new int[tex.width][];
        for(int x = 0; x < tex.width; x++)
        {
            values[x] = new int[tex.height];
            for(int y = 0; y < tex.height; y++)
            {
                values[x][y] = ColorToInt(tex.GetPixel(x, y));
            }
        }

        return values;
    }

    public static int ColorToInt(Color color)
    {
        string rgbColor = ((int)(color.r * 255)).ToString("X2") + ((int)(color.g * 255)).ToString("X2") + ((int)(color.b * 255)).ToString("X2") + ((int)(color.a * 255)).ToString("X2");
        return int.Parse(rgbColor, System.Globalization.NumberStyles.HexNumber);
    }

    private Color intToColor(int color)
    {
        return stringToColor(color.ToString("X8"));
    }

    public void SetTool(SketchTool sketchTool)
    {
        Color color = Color.black;
        switch (sketchTool.Type)
        {
            case SketchToolType.Brush:
                color = stringToColor(((SketchToolBrush)sketchTool).Color);
                break;
            case SketchToolType.Circle:
                color = stringToColor(((SketchToolCircle)sketchTool).Color);
                break;
            case SketchToolType.Circunference:
                color = stringToColor(((SketchToolCircunference)sketchTool).Color);
                break;
            case SketchToolType.Eraser:
                color = baseColor;
                break;
        }
        toolColor = color;
        currentTool = sketchTool;
    }

    private void draw(bool mouseWasPressed)
    {
        if (currentTool != null && toolColor != null)
        {
            Vector2 previousLocalPoint = new Vector2();
            Vector2 currentLocalPoint = new Vector2();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(gameObject.GetComponent<RectTransform>(), previousMousePos, null, out previousLocalPoint);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(gameObject.GetComponent<RectTransform>(), currentMousePos, null, out currentLocalPoint);

            switch (currentTool.Type)
            {
                case SketchToolType.Brush:
                    drawLine(new Vector2((int)previousLocalPoint.x - tex.width/2, (int)previousLocalPoint.y - tex.height/2),
                    new Vector2((int)currentLocalPoint.x - tex.width/2, (int)currentLocalPoint.y - tex.height/2),
                    toolColor, (int)Math.Floor(((SketchToolBrush)currentTool).Width/2d));
                    break;
                case SketchToolType.Circle:
                    if (mouseWasPressed)
                    {
                        circle((int)currentLocalPoint.x - tex.width / 2, (int)currentLocalPoint.y - tex.height / 2, ((SketchToolCircle)currentTool).Size, toolColor);
                    }
                    break;
                case SketchToolType.Circunference:
                    if (mouseWasPressed)
                    {
                        circunference((int)currentLocalPoint.x - tex.width / 2, (int)currentLocalPoint.y - tex.height / 2, ((SketchToolCircunference)currentTool).Size, toolColor);
                    }
                    break;
                case SketchToolType.Eraser:
                    drawLine(new Vector2((int)previousLocalPoint.x - tex.width / 2, (int)previousLocalPoint.y - tex.height / 2),
                    new Vector2((int)currentLocalPoint.x - tex.width / 2, (int)currentLocalPoint.y - tex.height / 2),
                    toolColor, (int)Math.Floor(((SketchToolEraser)currentTool).Size/2d));
                    break;
            }

            tex.Apply();
        }
    }

    private void drawLine(Vector2 p1, Vector2 p2, Color col, int thickness)
    {
        Vector2 t = p1;
        float frac = 1 / Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2) + Mathf.Pow(p2.y - p1.y, 2));
        float ctr = 0;

        while ((int)t.x != (int)p2.x || (int)t.y != (int)p2.y)
        {
            t = Vector2.Lerp(p1, p2, ctr);
            ctr += frac;
            //tex.SetPixel((int)t.x, (int)t.y, col);
            circle((int)t.x, (int)t.y, thickness, col);
        }

        circle((int)p2.x, (int)p2.y, thickness, col);
    }

    private void circle(int cx, int cy, int r, Color col)
    {
        int x, y, px, nx, py, ny, d;

        for (x = 0; x <= r; x++)
        {
            d = (int)Mathf.Ceil(Mathf.Sqrt(r * r - x * x));
            for (y = 0; y <= d; y++)
            {
                px = cx + x;
                nx = cx - x;
                py = cy + y;
                ny = cy - y;

                tex.SetPixel(px, py, col);
                tex.SetPixel(nx, py, col);

                tex.SetPixel(px, ny, col);
                tex.SetPixel(nx, ny, col);

            }
        }
    }

    private void circunference(int cx, int cy, int r, Color col)
    {
        int x, y, px, nx, py, ny, d;

        for (x = 0; x <= r; x++)
        {
            d = (int)Mathf.Ceil(Mathf.Sqrt(r * r - x * x));
            //for (y = 0; y <= d; y++)
            //{
                y = 0;    

                px = cx + x;
                nx = cx - x;
                py = cy + y;
                ny = cy - y;

                tex.SetPixel(px, py, col);
                tex.SetPixel(nx, py, col);

                tex.SetPixel(px, ny, col);
                tex.SetPixel(nx, ny, col);

            y = d;

            px = cx + x;
            nx = cx - x;
            py = cy + y;
            ny = cy - y;

            tex.SetPixel(px, py, col);
            tex.SetPixel(nx, py, col);

            tex.SetPixel(px, ny, col);
            tex.SetPixel(nx, ny, col);

            //}
        }
    }

    public static Color stringToColor(string colorStr)
    {
        colorStr = colorStr.Replace("#", "");
        while(colorStr.Length < 8)
        {
            colorStr += "0";
        }

        string rStr = colorStr.Substring(0, 2);
        string gStr = colorStr.Substring(2, 2);
        string bStr = colorStr.Substring(4, 2);
        string aStr = colorStr.Substring(6, 2);

        float r = int.Parse(rStr, System.Globalization.NumberStyles.HexNumber)/(float)255;
        float g = int.Parse(gStr, System.Globalization.NumberStyles.HexNumber) / (float)255;
        float b = int.Parse(bStr, System.Globalization.NumberStyles.HexNumber) / (float)255;
        float a = int.Parse(aStr, System.Globalization.NumberStyles.HexNumber) / (float)255;

        Color color = new Color(r, g, b, a);

        return color;
    }
}
