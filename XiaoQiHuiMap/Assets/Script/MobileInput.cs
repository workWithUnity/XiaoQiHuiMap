using UnityEngine;  
using System.Collections;  
  
public class MobileInput : MonoBehaviour {  
  
    //距离最值  
    public float MaxDistance=5F;  
    public float MinDistance=0.5F;  
    //缩放速率  
    public float ZoomSpeedS = 0.1F;
    public float ZoomSpeedM = 0.05F;
    public float ZoomSpeedL = 0.03F;

    public float ZoomSpeed = 0.1F;
    //观察目标  
    private Transform Target;

    [HideInInspector]
    public string str = "";

    //记录上一次手机触摸位置判断用户是在左放大还是缩小手势
    private Vector2 oldPosition1;
    private Vector2 oldPosition2;

    private Vector3 _vec3TargetScreenSpace;// 目标物体的屏幕空间坐标  
    private Vector3 _vec3TargetWorldSpace;// 目标物体的世界空间坐标  
    private Vector3 _vec3MouseScreenSpace;// 鼠标的屏幕空间坐标  
    private Vector3 _vec3Offset;// 偏移 

    private int downIndex = 0;
    private int dragIndex = 0;

    private Camera camera;
    private float Distance;

    public static MobileInput Instance;
    void Awake()
    {
        Target = GameObject.Find("Cube").transform;
        camera = this.gameObject.GetComponent<Camera>();
        Distance = camera.orthographicSize;
        Instance = this;

        ZoomSpeed = ZoomSpeedS;
    }

    void Start ()   
    {  
        //允许多点触控  
        Input.multiTouchEnabled=true;   
    }
    
#if UNITY_ANDROID || UNITY_IPHONE 
    void Update()
    {
        if (Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Moved)
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

        if(Input.touchCount>1)  
        {  
            //两只手指都处于移动状态  
            if (Input.touches[0].phase == TouchPhase.Moved || Input.touches[1].phase == TouchPhase.Moved)
            {
                //计算移动方向  
                var tempPosition1 = Input.GetTouch(0).position;
                var tempPosition2 = Input.GetTouch(1).position;

                //根据向量的大小判断当前手势是放大还是缩小  
                if(isEnlarge(oldPosition1,oldPosition2,tempPosition1,tempPosition2))
                {
                    Distance -= ZoomSpeed;
                }
                else
                {
                    Distance += ZoomSpeed;
                }
                    
                if (dragIndex++ != 0)
                {
                    //限制距离  
                    Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);
                    Camera.main.orthographicSize = Distance;
                }
                    
                //备份上一次触摸点的位置，用于对比
                oldPosition1 = tempPosition1;
                oldPosition2 = tempPosition2;
            }
            else
            {
                dragIndex = 0;
            }
        } 
    }

#endif

    //函数返回真为放大，返回假为缩小
     bool isEnlarge(Vector2 oP1,Vector2 oP2, Vector2 nP1,Vector2 nP2) 
    {
	    //函数传入上一次触摸两点的位置与本次触摸两点的位置计算出用户的手势
        var leng1 =Mathf.Sqrt((oP1.x-oP2.x)*(oP1.x-oP2.x)+(oP1.y-oP2.y)*(oP1.y-oP2.y));
        var leng2 =Mathf.Sqrt((nP1.x-nP2.x)*(nP1.x-nP2.x)+(nP1.y-nP2.y)*(nP1.y-nP2.y));
        if(leng1<leng2)
        {
    	     //放大手势
             return true;
        }else
        {
    	    //缩小手势
            return false;
        }
    }

}  
	
