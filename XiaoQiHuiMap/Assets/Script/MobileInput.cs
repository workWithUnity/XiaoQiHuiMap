using UnityEngine;  
using System.Collections;  
  
public class MobileInput : MonoBehaviour {  
  
    //观察目标  
    public Transform Target;  
  
    //观察距离  
    public float Distance = 2F;  
  
    //距离最值  
    public float MaxDistance=5F;  
    public float MinDistance=0.5F;  
  
    //缩放速率  
    public float ZoomSpeed=0.1F;  
  
    //旋转速度  
    public float SpeedX=12;
    public float SpeedY = 12;  
    //角度限制  
    private float  MinLimitY = 5;  
    private float  MaxLimitY = 180;  
      
    //旋转角度  
    private float mX=0;  
    private float mY=0;  
  
    //当前手势  
    private Vector2 mPos;

    void Awake()
    {
        Distance = this.Target.transform.position.z;
    }

    void Start ()   
    {  
        //允许多点触控  
        Input.multiTouchEnabled=true;   
    }
    public string str ="";
#if UNITY_ANDROID || UNITY_IPHONE 
    void Update ()   
    {  
        if(!Target) return;  
  
  
        //单点触控  
        if(Input.touchCount==1)  
        {  
            //手指处于移动状态  
            if(Input.touches[0].phase==TouchPhase.Moved)  
            {
                mX = Input.GetAxis("Mouse X") * SpeedX * 0.02F;
                mY = Input.GetAxis("Mouse Y") * SpeedY * 0.02F;
                str = "mX:" + Input.GetAxis("Mouse X") + " " + "mY:" + Input.GetAxis("Mouse Y") ;
                Debug.LogError(str);
                //计算相机的角度和位置  
                Target.transform.position += new Vector3(-mX, -mY, 0);  
            }  
        }  
        //多点触控  
        else if(Input.touchCount>1)  
        {  
            //两只手指都处于移动状态  
            if(Input.touches[0].phase==TouchPhase.Moved || Input.touches[1].phase==TouchPhase.Moved)  
            {  
                //计算移动方向  
                Vector2 mDir=Input.touches[1].position-Input.touches[0].position;  
                //根据向量的大小判断当前手势是放大还是缩小  
                if(mDir.sqrMagnitude>mPos.sqrMagnitude){  
                    Distance+=ZoomSpeed;  
                }else{  
                    Distance-=ZoomSpeed;  
                }  
                //限制距离  
                Distance=Mathf.Clamp(Distance,MinDistance,MaxDistance);  
                //更新当前手势  
                mPos=mDir;
                Target.transform.position = new Vector3(Target.position.x, Target.position.y, Distance);
            }  
        }  
     
    }  
#endif
    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 500, 150), str);
    }
  
    //角度限制  
    private float ClampAngle (float angle,float min,float max)   
    {  
        if (angle < -360) angle += 360;  
        if (angle >  360) angle -= 360;  
        return Mathf.Clamp (angle, min, max);  
    }  
}  
	
