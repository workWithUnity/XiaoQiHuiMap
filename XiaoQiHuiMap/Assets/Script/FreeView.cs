using UnityEngine;
using System.Collections;

//定义鼠标按键枚举  
enum MouseButton
{
    //鼠标左键  
    MouseButton_Left = 0,
    //鼠标右键  
    MouseButton_Right = 1,
    //鼠标中键  
    MouseButton_Midle = 2
}

public class FreeView : MonoBehaviour
{

    //鼠标缩放距离最值  
    public float MaxDistance = -0.4f;
    public float MinDistance = -4f;
    //鼠标缩放速率  
    public float ZoomSpeed = 2F;

    //观察目标  
    private Transform Target;

    //屏幕坐标  
    private Vector3 mScreenPoint;
    //坐标偏移  
    private Vector3 mOffset;

    private Camera camera;

    private Vector3 _vec3TargetScreenSpace;// 目标物体的屏幕空间坐标  
    private Vector3 _vec3TargetWorldSpace;// 目标物体的世界空间坐标  
    private Vector3 _vec3MouseScreenSpace;// 鼠标的屏幕空间坐标  
    private Vector3 _vec3Offset;// 偏移 

    private int downIndex = 0;
    private float Distance;
    void Awake()
    {
        Target = GameObject.Find("Cube").transform;
        camera = this.gameObject.GetComponent<Camera>();
        Distance = camera.orthographicSize;

    }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN ||UNITY_WEBGL
    void Update()
    {
            if (Input.GetMouseButton((int)MouseButton.MouseButton_Left))
            {
                if (downIndex++ == 0)
                {
                    // 把目标物体的世界空间坐标转换到它自身的屏幕空间坐标 
                    _vec3TargetScreenSpace = Camera.main.WorldToScreenPoint(Target.position);

                    // 存储鼠标的屏幕空间坐标（Z值使用目标物体的屏幕空间坐标） 
                    _vec3MouseScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _vec3TargetScreenSpace.z);

                    // 计算目标物体与鼠标物体在世界空间中的偏移量 
                    _vec3Offset = Target.position - Camera.main.ScreenToWorldPoint(_vec3MouseScreenSpace);
                }

                // 存储鼠标的屏幕空间坐标（Z值使用目标物体的屏幕空间坐标）
                _vec3MouseScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _vec3TargetScreenSpace.z);

                // 把鼠标的屏幕空间坐标转换到世界空间坐标（Z值使用目标物体的屏幕空间坐标），加上偏移量，以此作为目标物体的世界空间坐标
                _vec3TargetWorldSpace = Camera.main.ScreenToWorldPoint(_vec3MouseScreenSpace) + _vec3Offset;

                // 更新目标物体的世界空间坐标 
                Target.position = _vec3TargetWorldSpace;
            }
            else
            {
                downIndex = 0;
            }

            //鼠标中键平移  
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                //鼠标滚轮缩放  
                Distance += Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed;
                Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);
                //设置相机视口
                camera.orthographicSize = Distance;
            }
    }

#endif
}