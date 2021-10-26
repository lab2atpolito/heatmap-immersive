using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ExpFidelity : MonoBehaviour
{
    [SerializeField] int nPax = 1;
    [SerializeField] float percentageOfminimumTime = 0.5f;
    [SerializeField]
    void Start()
    {
        var reader = new StreamReader(@"D:\Unity Projects\Immersive_Movies\Eye Data\Eye Data 100\CustomEvents.csv");
        string headerLine = reader.ReadLine();

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

            }

        }
    }

        // Update is called once per frame
        void Update()
    {
        
    }
}
