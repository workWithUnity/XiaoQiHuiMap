using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidMessage : MonoBehaviour {

    private static AndroidMessage _instance;
    public static AndroidMessage Instance { get { return _instance; } }

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
        Debug.LogError("AndroidClickMap:"+mapId);
#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                //调用Android插件中UnityTestActivity中StartActivity0方法，stringToEdit表示它的参数
                jo.Call("StartActivity0", mapId);
            }
        }
#endif
    }


    /// <summary>
    /// Android回调到unity的方法
    /// </summary>
    /// <param name="base64">Base64.</param>
    public void SendUnityMessage(string param)
    {
        Debug.LogError("SendMessageByIOS:" + param);
    }


}
