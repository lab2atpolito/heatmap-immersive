using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Video;

public class DataCollector : MonoBehaviour
{
    // Start is called before the first frame update
    List<Vector2> coordinates;
    [SerializeField] string dataPath;
    [SerializeField] float instantTime;
    [SerializeField] float windowLength;
    [SerializeField] float frameRate = 25;
    [SerializeField] VideoPlayer videoPlayer;

    public List<Vector2> GetCoordinates()
    {
        if(windowLength == 0)
        {
            windowLength = 10000;
        }

        var reader = new StreamReader(@"D:\Unity Projects\Immersive_Movies\Eye Data\Eye Data 100\CustomEvents.csv");
        string headerLine = reader.ReadLine();

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (line.Equals(headerLine)) {

                break;
            } else
            {
                var values = line.Split(';');
                float timestamp = (float)float.Parse(values[1]);
                float tStart;

                if ((instantTime - windowLength) < 0)
                {
                    tStart = 0.000f;
                } else
                {
                    tStart = instantTime - windowLength;
                }
                
                if (timestamp>=tStart && timestamp <= instantTime)
                {
                    float x = float.Parse(values[3]);
                    float y = float.Parse(values[4]);
                    coordinates.Add(new Vector2(x, y));
                }
            }
           

        }

        return coordinates;
    }


}
