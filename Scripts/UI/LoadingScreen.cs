using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LoadingScreen : MonoBehaviour
{
    public Image loadingAnimation;
    public Sprite[] frames;
    public  float fps = 10.0f;

    void Update() {
        if (loadingAnimation == null) return;

        int index = (int)(Time.time * fps);
        index = index % frames.Length;
        if (frames[index] == null) return;
        loadingAnimation.sprite = frames[index];
    }

}
