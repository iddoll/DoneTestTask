using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

public class LoadVideo : MonoBehaviour
{
    [SerializeField] private string nameOfFile;
    [SerializeField] private VideoPlayer videoPlayer;

    private void Start()
    {
        StartCoroutine(LoadVideoCoroutine());
    }

    IEnumerator LoadVideoCoroutine()
    {
        AssetBundleRequest m_videoToLoadFromBundle;
        
        TextAsset m_loadedVideo;
        
        m_videoToLoadFromBundle = LoaderAssetBundle.instance.bundle.LoadAssetAsync<TextAsset>(nameOfFile);

        yield return m_videoToLoadFromBundle;
        
        m_loadedVideo = (TextAsset)m_videoToLoadFromBundle.asset;
        
        File.WriteAllBytes(Path.Combine(Application.streamingAssetsPath,nameOfFile + ".mp4"), m_loadedVideo.bytes);
        videoPlayer.url = Path.Combine(Application.streamingAssetsPath, nameOfFile + ".mp4");
    }
}

