using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{

    private VideoPlayer _videoPlayer;


    private void Start()
    {
        _videoPlayer = GetComponent<VideoPlayer>();

        _videoPlayer.loopPointReached += EndVideo;
        
    }

    private void EndVideo(VideoPlayer videoPlayer) { }
}
