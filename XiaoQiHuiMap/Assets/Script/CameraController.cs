using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public float zoomSpeed = 1;
    public GameObject map;
    Vector3 currPosition; //拖拽前的位置
    Vector3 newPosition; //拖拽后的位置
	// Use this for initialization
	void Start () {

	}

    void Update()
    {
        //鼠标滚轮的效果
        //Camera.main.fieldOfView 摄像机的视野
        //Camera.main.orthographicSize 摄像机的正交投影
        //Zoom out
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            this.transform.position += new Vector3(0, 0, zoomSpeed);
        }
        //Zoom in
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            this.transform.position -= new Vector3(0, 0, zoomSpeed);
        }
       
        
    }
    void OnMouseDrag()
    {
        //1：把物体的世界坐标转为屏幕坐标 (依然会保留z坐标)
        currPosition = Camera.main.WorldToScreenPoint(map.transform.position);

        //2：更新物体屏幕坐标系的x,y
        currPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, currPosition.z);

        //3：把屏幕坐标转为世界坐标
        newPosition = Camera.main.ScreenToWorldPoint(currPosition);

        //4：更新物体的世界坐标
        map.transform.position = newPosition;
    }

}
