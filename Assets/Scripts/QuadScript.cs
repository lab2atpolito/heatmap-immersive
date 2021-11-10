using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;
using System.Linq;
using System;
using UnityEngine.UI;

public enum TrackedKind { Gaze, Eye};
public class QuadScript : MonoBehaviour
{
    public TrackedKind Tracking = TrackedKind.Eye;

    List<Vector2> coordinates;
    [SerializeField] string dataPath;
    [SerializeField] float instantTime;
    [SerializeField] float windowLength=2;
    [SerializeField] float frameRate = 25;
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] Slider timeSlider;
    [SerializeField] InputField window;
    Material mMaterial;

    MeshRenderer mMeshRenderer;


    float[] mPoints;
    int mHitCount;
   float[] intensities;




    List<Vector2> coor = new List<Vector2>();
    Dictionary<Vector2, float> CoordinateCount;
    float imageWidth = 5376;
    float imageHeight = 2688;

    float previousTime;
    float previousWindowLength;

   








    void Start()
    {

        mPoints = new float[1022];
        intensities = new float[511];
        mHitCount = 0;
        instantTime = 0;
        previousTime = instantTime;
        previousWindowLength = windowLength;
        mMeshRenderer = GetComponent<MeshRenderer>();
        mMaterial = mMeshRenderer.material;


        coor = GetCoordinates();
        CoordinateCount = GetCoordinateCount(coor);
        mMaterial.SetFloat("_TotalHits", coor.Count);
        generateHeatMap(CoordinateCount);

        window.onEndEdit.AddListener(delegate { ValueChangeCheck(); });










    }

    void Update()
    {
        instantTime = (float) videoPlayer.time;
        

        if (instantTime != previousTime || windowLength!=previousWindowLength)
        {
            mHitCount = 0;
            coor = GetCoordinates();
            CoordinateCount = GetCoordinateCount(coor);
            mMaterial.SetFloat("_TotalHits", coor.Count);
            generateHeatMap(CoordinateCount);
            previousTime = instantTime;
            previousWindowLength = windowLength;
        }




    }



    public void generateHeatMap(Dictionary<Vector2, float> coorCount)
    {

        
            foreach (Vector2 coor in coorCount.Keys)
            {
                var x = coor.x / imageWidth;
                var y = coor.y / imageHeight;
                float intensity = CoordinateCount[coor];
                addHitPoint(x * 4 - 2, y * 4 - 2, intensity, mHitCount);
                
            }
        
        
    }

    public void addHitPoint(float xp, float yp, float intensity, int Count)
    {
       if(Count >= 510)
        {
            Count = 0;
            
        }
        mPoints[Count * 2] = xp;
        mPoints[Count * 2 + 1] = yp;
        intensities[Count] = intensity;


        mHitCount++;
        
        mMaterial.SetInt("_HitCount", mHitCount);
        mMaterial.SetFloatArray("_Hits", mPoints);
        mMaterial.SetFloatArray("_Intensities", intensities);
        

    }

    public List<Vector2> GetCoordinates()
    {
        if (windowLength == 0)
        {
            windowLength = 2;
        }

        var reader = new StreamReader(dataPath);
        string headerLine = reader.ReadLine();

        List<Vector2> Newcoordinates = new List<Vector2>();
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (line.Equals(headerLine))
            {

                continue;
            }
            else
            {
                var values = line.Split(';');
                if (values.Count() > 1)
                {
                    float timestamp = (float)float.Parse(values[1]);
                    float tStart;
                    string dataType = " Planar_" + Tracking.ToString() + "_Point";

                    if ((instantTime - windowLength) < 0)
                    {
                        tStart = 0;
                    }
                    else
                    {
                        tStart = instantTime - windowLength;
                    }

                    if (timestamp >= tStart && timestamp <= instantTime && values[2].Equals(dataType))
                    {
                        float x = float.Parse(values[3]);
                        float y = float.Parse(values[4]);
                        Newcoordinates.Add(new Vector2(x, y));
                    }

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
            } else
            {
                coor.Add(v, 1);
            }

        }

        //in case too much data eliminate the ones with less hit
        /*if (coor.Count > 511)
        {
            var coorSorted = from entry in coor orderby entry.Value descending select entry;
            coor = coorSorted.Take(511).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }*/

        return coor;




    }

    public Dictionary<Vector2, float> GetCoordinateCount(Dictionary<Vector2, float> coordinates, int count)
    {
        Dictionary<Vector2, float> coor = new Dictionary<Vector2, float>();

     
         List<Vector2> pixels = coordinates.Keys.ToList().GetRange(count * 500, Math.Min(500*(count+1), coordinates.Count ));
    
         

        foreach(Vector2 p in pixels)
        {
            coor.Add(p, coordinates[p]);
        }
        return coor;
       

       
       




    }


    public void ValueChangeCheck()
    {
        windowLength = float.Parse(window.textComponent.text);
        if (windowLength> videoPlayer.frameCount * 25)
        {
            windowLength = videoPlayer.frameCount * 25;
        }
    }




}
