using System;
using UnityEngine;

public class MusicPlayerController : MonoBehaviour {
    
    private static MusicPlayerController instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}