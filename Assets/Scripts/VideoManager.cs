using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] Text time;

    // Start is called before the first frame update
    void Start()
    {
        slider.maxValue = (float)videoPlayer.clip.frameCount / 25;
        time.text = FormatTime(slider.value);
        slider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        //videoPlayer.Pause();

    }


    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.S)){
            string nameScreenshot = videoPlayer.clip.name+ ".png";
            ScreenCapture.CaptureScreenshot(nameScreenshot, 2);
        }

    }

    public void ValueChangeCheck()
    {
        time.text = FormatTime(slider.value);
        videoPlayer.Play();
        videoPlayer.frame = (long)slider.value * 25;
        videoPlayer.Pause();


    }



    public string FormatTime(float time)
    {
        TimeSpan t = TimeSpan.FromSeconds(time);

        string answer = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                        t.Hours,
                        t.Minutes,
                        t.Seconds);
        return answer;
    }


}
