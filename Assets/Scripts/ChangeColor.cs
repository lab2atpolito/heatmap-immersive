using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;
using System.Linq;

public class ChangeColor : MonoBehaviour
{



    Color[] colors =  { new Color(0, 0, 0, 0), 
        new Color(0, 0.9f, 0.2f, 0.5f), 
        new Color(0.9f, 1, 0.3f, 0.5f),
        new Color(0.9f, 0.7f, 0.1f, 0.5f), 
        new Color(1, 0, 0, 0.5f) };

    float[] pointranges = { 0, 0.25f, 0.5f, 0.75f, 1 };

    List<Vector2> coordinates;
    [SerializeField] string dataPath;
    [SerializeField] float instantTime;
    [SerializeField] float windowLength;
    [SerializeField] float frameRate = 25;
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] float _Diameter = 0.1f;




    float[] mPoints;
    int mHitCount;
    float[] intensities;




    List<Vector2> coor = new List<Vector2>();
    Dictionary<Vector2, float> CoordinateCount;
    int imageWidth = 5376;
    int imageHeight = 2688;

    float previousTime;
    Texture2D texture;
    Renderer r;
    // Start is called before the first frame update
    void Start()
    {
        texture = new Texture2D(imageWidth, imageHeight);
        r = GetComponent<Renderer>();
        r.material.mainTexture = texture;
    
        coor = GetCoordinates();
        CoordinateCount = GetCoordinateCount(coor);
        generateHeatMap(CoordinateCount);


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Vector2> GetCoordinates()
    {
        if (windowLength == 0)
        {
            windowLength = 2;
        }

        var reader = new StreamReader(@"D:\Unity Projects\Immersive_Movies\Eye Data\Eye Data 100\CustomEvents.csv");
        string headerLine = reader.ReadLine();

        List<Vector2> Newcoordinates = new List<Vector2>();
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (line.Equals(headerLine))
            {

                break;
            }
            else
            {
                var values = line.Split(';');
                float timestamp = (float)float.Parse(values[1]);
                float tStart;

                if ((instantTime - windowLength) < 0)
                {
                    tStart = 0;
                }
                else
                {
                    tStart = instantTime - windowLength;
                }

                if (timestamp >= tStart && timestamp <= instantTime)
                {
                    float x = float.Parse(values[3]);
                    float y = float.Parse(values[4]);
                    Newcoordinates.Add(new Vector2(x, y));
                }
            }


        }

        return Newcoordinates;
    }

    public Dictionary<Vector2, float> GetCoordinateCount(List<Vector2> coordinates)
    {
        Dictionary<Vector2, float> coor = new Dictionary<Vector2, float>();
        foreach (Vector2 v in coordinates)
        {
            if (coor.ContainsKey(v))
            {
                coor[v]++;
            }
            else
            {
                coor.Add(v, 1);
            }

        }

        return coor;

    }

    public Color getHeatForPixel(float weight)
    {
        if (weight <= pointranges[0])
        {
            return colors[0];
        }
        if (weight >= pointranges[4])
        {
            return colors[4];
        }
        for (int i = 1; i < 5; i++)
        {
            if (weight < pointranges[i]) //if weight is between this point and the point before its range
            {
                float dist_from_lower_point = weight - pointranges[i - 1];
                float size_of_point_range = pointranges[i] - pointranges[i - 1];

                float ratio_over_lower_point = dist_from_lower_point / size_of_point_range;

                //now with ratio or percentage (0-1) into the point range, multiply color ranges to get color

                Color color_range = colors[i] - colors[i - 1];

                Color color_contribution = color_range * ratio_over_lower_point;

                Color new_color = colors[i - 1] + color_contribution;
                return new_color;

            }
        }
        return colors[0];
    }

    float distsq(Vector2 a, Vector2 b)
    {
        float area_of_effect_size = _Diameter;

        return Mathf.Max(0.0f, 1.0f - Vector2.Distance(a, b) / area_of_effect_size) * Mathf.Max(0.0f, 1.0f - Vector2.Distance(a, b) / area_of_effect_size);
    }

    public void generateHeatMap(Dictionary<Vector2, float> coorCount)
    {
        Dictionary<Vector2, Color> pixelColors = new Dictionary<Vector2, Color>();

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                
                Vector2 pixel = new Vector2(x, y);
                Color col;
                if (pixelColors.ContainsKey(pixel))
                {
                    col = pixelColors[pixel];
                } else
                {
                    col = colors[0];
                    pixelColors.Add(pixel, col);
                }

                float totalWeight = 0;
                
                foreach(Vector2 gazePoint in coorCount.Keys)
                {
                    float pt_intensity = coorCount[gazePoint] / coorCount.Count;
                    totalWeight += pt_intensity * distsq(pixel, gazePoint);
                }

                Color heat = getHeatForPixel(totalWeight);
                pixelColors[pixel] = pixelColors[pixel] + heat;
                texture.SetPixel((int) pixel.x,(int) pixel.y, pixelColors[pixel]);

            }
            texture.Apply();

        }

        

    }
}
