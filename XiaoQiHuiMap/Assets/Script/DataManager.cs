using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using System;
using System.Text;
using System.Security.Cryptography;
public class DataManager : MonoBehaviour {

    public float bigArea = 1.4f;
    public float middleArea = 0.8f;
    public float smallArea = 0.4f;

    public int fontSize_1 = 40;
    public int fontSize_2 = 40;
    public int fontSize_3 = 30;

    private static DataManager _instance;
    public static DataManager Instance { get { return _instance; } }

    int modleLayer;
    [HideInInspector]
    public int UI3D_S;
    [HideInInspector]
    public int UI3D_M;
    [HideInInspector]
    public int UI3D_L;
    private ClickMapItem lastClickMapItem;
    [HideInInspector]
    public GameObject canvas_S;
    [HideInInspector]
    public GameObject canvas_M;
    [HideInInspector]
    public GameObject canvas_L;

    public const string MD5KEY = "FileMD5";
    private Dictionary<string, string> mapInfoDic = new Dictionary<string, string>();
    private int reConnectionNum = 0;
    private bool canDownload = false;
    void Awake()
    {
        if (_instance != null)
        {
            DestroyImmediate(this);
        }

        IOSMessage.ClickMap("GameLaunch");
        AndroidMessage.ClickMap("GameLaunch");
        WebMessage.ClickMap("GameLaunch");

        _instance = this;
        modleLayer = LayerMask.NameToLayer("Modle");
        UI3D_S = LayerMask.NameToLayer("3DUI_S");
        UI3D_M = LayerMask.NameToLayer("3DUI_M");
        UI3D_L = LayerMask.NameToLayer("3DUI_L");

        canvas_S = GameObject.Find("Cube/MapRoot/TitleCanvas_S");
        canvas_M = GameObject.Find("Cube/MapRoot/TitleCanvas_M");
        canvas_L = GameObject.Find("Cube/MapRoot/TitleCanvas_L");
        canvas_S.gameObject.layer = DataManager.Instance.UI3D_S;
        canvas_M.gameObject.layer = DataManager.Instance.UI3D_M;
        canvas_L.gameObject.layer = DataManager.Instance.UI3D_L;

        StartCoroutine(LoadXml());
        lastDistance = Camera.main.orthographicSize;

        curCullingMask = Camera.main.cullingMask;
    }
    private Ray ray;
    private RaycastHit hit;  

    private double t1; 
    private double t2; 
    private float lastDistance = -1;

    private float curCullingMask;
    
    void Update()
    {
        if (canDownload)
        {
            Debug.LogError("=======LoadXml=======");
            canDownload = false;
            StartCoroutine(LoadXml());
        }

        float distance = Camera.main.orthographicSize;
        if (distance != lastDistance)
        {
            if (distance > DataManager.Instance.bigArea)
            {
                Camera.main.cullingMask |= (1 << UI3D_S);
                Camera.main.cullingMask &= ~(1 << UI3D_M);
                Camera.main.cullingMask &= ~(1 << UI3D_L);

                canvas_S.gameObject.SetActive(true);
                canvas_M.gameObject.SetActive(false);
                canvas_L.gameObject.SetActive(false);
                MobileInput.Instance.ZoomSpeed = MobileInput.Instance.ZoomSpeedS;
            }
            else if (distance > DataManager.Instance.middleArea)
            {
                Camera.main.cullingMask &= ~(1 << UI3D_S);
                Camera.main.cullingMask |= (1 << UI3D_M);
                Camera.main.cullingMask &= ~(1 << UI3D_L);

                canvas_S.gameObject.SetActive(false);
                canvas_M.gameObject.SetActive(true);
                canvas_L.gameObject.SetActive(false);

                MobileInput.Instance.ZoomSpeed = MobileInput.Instance.ZoomSpeedM;
            }
            else if (distance > DataManager.Instance.smallArea)
            {
                Camera.main.cullingMask &= ~(1 << UI3D_S);
                Camera.main.cullingMask &= ~(1 << UI3D_M);
                Camera.main.cullingMask |= (1 << UI3D_L);

                canvas_S.gameObject.SetActive(false);
                canvas_M.gameObject.SetActive(false);
                canvas_L.gameObject.SetActive(true);

                MobileInput.Instance.ZoomSpeed = MobileInput.Instance.ZoomSpeedL;
            }
            curCullingMask = Camera.main.cullingMask;
        }
        

        lastDistance = distance;

        if (Input.GetKeyDown(KeyCode.Home) || Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetMouseButtonDown(0))
        {
            t2 = Time.realtimeSinceStartup;
            if (t2 - t1 < 0.5f)
            {
                if (Input.touches.Length == 1)
                {
                    //摄像机发射射线到屏幕点
                    ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
                    if (Physics.Raycast(ray, out hit, modleLayer))
                    {
                        SwitchMap(hit.transform.gameObject);
                    }
                }
                else
                {
                    //摄像机发射射线到屏幕点
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit, modleLayer))
                    {
                        SwitchMap(hit.transform.gameObject);
                    }
                }
            }
            t1 = t2;
        }
    }
    [HideInInspector]
    public string str;
    void OnGUI()
    {
        //GUIStyle gui = new GUIStyle();
        //gui.fontSize = 50;
        //GUI.Label(new Rect(0, 0, 500, 150), str, gui);
    }

    public void SwitchMap(GameObject obj)
    {
        ClickMapItem mapItem = obj.GetComponent<ClickMapItem>();
        if (mapItem != null)
        {
            if (lastClickMapItem != null)
            {
                lastClickMapItem.CancelClick();
            }
            mapItem.OnClick();
            lastClickMapItem = mapItem;
        }
    }

    public string LoadText(string id)
    {
        if (mapInfoDic.ContainsKey(id))
        {
            return mapInfoDic[id];
        }
        else
        {
            return id;
        }
        
    }
#if UNITY_WEBGL
    IEnumerator LoadXml()
    {
       
        string curTargetPath = Application.streamingAssetsPath + "/mapData.xml";
        Debug.LogError(curTargetPath);
        WWW www = new WWW(curTargetPath);
        yield return www;
        string xmlstr;
        if (www.isDone && String.IsNullOrEmpty(www.error))
        {
            string curMD5 = EncryptWithMD5(www.bytes).Trim();
            xmlstr = System.Text.Encoding.UTF8.GetString(www.bytes);
        }
        else
        {
            Debug.LogError("www failed");
            yield break;
        }
        www.Dispose();

        //创建xml文档
        XmlDocument xml = new XmlDocument();
        xml.LoadXml(xmlstr);
        //得到objects节点下的所有子节点
        XmlNodeList xmlNodeList = xml.SelectSingleNode("content").ChildNodes;
        //遍历所有子节点
        foreach (XmlElement xl1 in xmlNodeList)
        {
            string id = xl1.GetAttribute("id");
            string name = xl1.GetAttribute("name");

            if (!mapInfoDic.ContainsKey(id))
            {
                mapInfoDic.Add(id,name);
            }
        }
        FollowUI[] list = GameObject.FindObjectsOfType<FollowUI>();
        for (int i =0 ;i < list.Length; i++)
        {
            list[i].Refresh();
        }
    }
#else
        IEnumerator LoadXml()
        {

            string targetPath = GetLocalPath("Documents") + "mapData.xml";
            //string curTargetPath = Application.streamingAssetsPath + "/mapData.xml";
            string curTargetPath = "http://139.196.195.151/Public/images/data.xml";

            string tLocalMD5 = "";
            if (File.Exists(targetPath))
            {
                string targetPathWww = targetPath;
#if UNITY_ANDROID && !UNITY_EDITOR

#else
                targetPathWww = "file://" + targetPath;
#endif
                WWW www1 = new WWW(targetPathWww);
                yield return www1;
                if (www1.isDone)
                {
                    tLocalMD5 = EncryptWithMD5(www1.bytes);
                }
                www1.Dispose();
            }
            string xmlstr = null;
            str += "tLocalMD5 :" + tLocalMD5;
            WWW www = new WWW(curTargetPath);
            yield return www;
            if (www.isDone && String.IsNullOrEmpty(www.error))
            {
                string curMD5 = EncryptWithMD5(www.bytes).Trim();
                Debug.LogError(tLocalMD5 + " " + curMD5);
                xmlstr = System.Text.Encoding.UTF8.GetString(www.bytes);
                if (curMD5 != tLocalMD5)
                {
                    Debug.LogError("update New resoure!");
                    try
                    {
                        if (File.Exists(targetPath))
                        {
                            File.Delete(targetPath);
                        }
                        FileStream fileStream = File.Create(targetPath);
                        fileStream.Write(www.bytes, 0, www.bytes.Length);
                        fileStream.Flush();
                        fileStream.Close();
                        PlayerPrefs.SetString(MD5KEY, curMD5);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.ToString());
                    }
                }
                else
                {
                    Debug.LogError("No New resoure!");
                }

            }
            else
            {
                Debug.LogError("www failed");
                if (tLocalMD5 == "" && reConnectionNum++ < 4)
                {
                    canDownload = true;
                }
                yield break;
            }
            www.Dispose();
            yield return null;

            //创建xml文档
            XmlDocument xml = new XmlDocument();
            XmlReaderSettings set = new XmlReaderSettings();
            set.IgnoreComments = true;//这个设置是忽略xml注释文档的影响。有时候注释会影响到xml的读取
#if UNITY_WEBGL
            xml.LoadXml(xmlstr);
#else
            xml.Load(XmlReader.Create(targetPath, set));
#endif
            //得到objects节点下的所有子节点
            XmlNodeList xmlNodeList = xml.SelectSingleNode("content").ChildNodes;
            //遍历所有子节点
            foreach (XmlElement xl1 in xmlNodeList)
            {
                string id = xl1.GetAttribute("id");
                string name = xl1.GetAttribute("name");

                if (!mapInfoDic.ContainsKey(id))
                {
                    mapInfoDic.Add(id, name);
                }
            }

            FollowUI[] list = GameObject.FindObjectsOfType<FollowUI>();
            for (int i = 0; i < list.Length; i++)
            {
                list[i].Refresh();
            }
        }
#endif
    public static string GetLocalPath(string file_name)
    {
        string path = "";
#if UNITY_EDITOR
        path = Application.dataPath;
#elif UNITY_ANDROID
	  path = Application.persistentDataPath;
#elif UNITY_IPHONE
	  path = Application.persistentDataPath;
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
       path =  Application.dataPath;
#else
       path =  Application.dataPath;
#endif
        string dataFilePath = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        dataFilePath = dataFilePath.Substring(0, dataFilePath.LastIndexOf(Path.DirectorySeparatorChar));
        if (dataFilePath.EndsWith(".app"))
        {
            dataFilePath = dataFilePath.Substring(0, dataFilePath.LastIndexOf(Path.DirectorySeparatorChar));
        }

        string dataPath = dataFilePath + Path.DirectorySeparatorChar + file_name;
        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }
        return dataPath + Path.DirectorySeparatorChar;
    }

    public static string EncryptWithMD5(byte[] sor)
    {
        //byte[] sor = Encoding.UTF8.GetBytes(source);
        MD5 md5 = MD5.Create();
        byte[] result = md5.ComputeHash(sor);
        StringBuilder strbul = new StringBuilder(40);
        for (int i = 0; i < result.Length; i++)
        {
            strbul.Append(result[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位

        }
        return strbul.ToString();
    }
}
