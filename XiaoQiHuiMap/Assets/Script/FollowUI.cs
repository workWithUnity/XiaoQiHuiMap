using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowUI : MonoBehaviour
{
    public Transform target;
    public Vector3 offsetPos = Vector3.zero;
    private float frontSize;
    private float scaleSize = 0.0015f;
	// Use this for initialization
	void Start () {
       Text text = target.GetComponent<Text>();
       text.fontSize = 20;
       GameTools.AddClickEvent(target.gameObject, Text_ClickEvent);
	}
    void Text_ClickEvent()
    {
        string[] array = this.target.name.Split('_');
        if (array.Length == 2)
        {
            IOSMessage.ClickMap(array[1]);
            AndroidMessage.ClickMap(array[1]);
        }
    }
	// Update is called once per frame
	void Update () {
        if (target != null)
        {

            Vector3 player2DPosition = Camera.main.WorldToScreenPoint(transform.position);
            //target.position = player2DPosition + offsetPos;
            target.position = transform.position + offsetPos;
            target.rotation = Camera.main.transform.rotation;
            

            float distance = Mathf.Abs(Camera.main.transform.position.z);
            target.localScale = Vector3.one * distance * scaleSize;

            if (distance > 1.4)
            {
                if (target.name.Contains("txt_1"))
                {
                    target.gameObject.GetComponent<Text>().enabled = true;
                }
                else
                {
                    target.gameObject.GetComponent<Text>().enabled = false;
                }
            }
            else if (distance > 0.8f)
            {
                if (target.name.Contains("txt_2"))
                {
                    target.gameObject.GetComponent<Text>().enabled = true;
                }
                else
                {
                    target.gameObject.GetComponent<Text>().enabled = false;
                }
            }
            else if (distance > 0.4f)
            {
                if (target.name.Contains("txt_3"))
                {
                    target.gameObject.GetComponent<Text>().enabled = true;
                }
                else
                {
                    target.gameObject.GetComponent<Text>().enabled = false;
                }
            }
            //血条超出屏幕就不显示  
            if (player2DPosition.x > Screen.width || player2DPosition.x < 0 || player2DPosition.y > Screen.height || player2DPosition.y < 0)
            {
                target.gameObject.SetActive(false);
            }
            else
            {
                target.gameObject.SetActive(true);
            }  
        }
	}
}
