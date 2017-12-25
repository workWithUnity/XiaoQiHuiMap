using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebMessage : MonoBehaviour {



    private static WebMessage _instance;
    public static WebMessage Instance { get { return _instance; } }

    void Awake()
    {
        if (_instance != null)
        {
            DestroyImmediate(this);
            return;
        }
        _instance = this;
    }


    /// <summary>
    /// 点击地图
    /// </summary>
    /// <param name="readAddr"></param>
    public static void ClickMap(string mapId)
    {
        Debug.LogError("WebClickMap:" + mapId);
#if UNITY_WEBGL
         Application.ExternalCall("UnityMessage", mapId);
#endif

    }

    /// <summary>
    /// ios回调到unity的方法
    /// </summary>
    /// <param name="base64">Base64.</param>
    public void SendUnityMessage(string param)
    {
        Debug.LogError("SendMessageByWeb:" + param);
    }
}
