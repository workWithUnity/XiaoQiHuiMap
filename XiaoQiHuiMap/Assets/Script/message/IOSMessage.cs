using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class IOSMessage : MonoBehaviour
{
	[DllImport ("__Internal")]
	private static extern void _iosClickMap(string mapId);

    private static IOSMessage _instance;
    public static IOSMessage Instance { get { return _instance; } }

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
        Debug.LogError("IosClickMap:" + mapId);
#if UNITY_IPHONE && !UNITY_EDITOR
        _iosClickMap(mapId);
#endif
    }

	/// <summary>
	/// ios回调到unity的方法
	/// </summary>
	/// <param name="base64">Base64.</param>
	public void SendUnityMessage(string param)
	{
        Debug.LogError("SendMessageByIOS:" + param);
	}

    
}
