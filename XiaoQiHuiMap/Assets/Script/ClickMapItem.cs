using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickMapItem : MonoBehaviour {

    public Color selectedColor = Color.yellow;
    private Color curColor;
    MeshRenderer meshRender;
    // Use this for initialization
    private void Awake()
    {
        meshRender = this.gameObject.GetComponent<MeshRenderer>();
        if (meshRender != null)
        {
            curColor = meshRender.material.color;
            CommonTools.AddOrGetComponent<MeshCollider>(this.gameObject);
        }
        
    }
    public void OnClick()
    {
        meshRender = this.gameObject.GetComponent<MeshRenderer>();
        if (meshRender != null)
        {
            DataManager.Instance.str += "OnClick: " + this.name;
            meshRender.material.color = selectedColor;
        }
        try
        {
            IOSMessage.ClickMap(this.gameObject.name);
            AndroidMessage.ClickMap(this.gameObject.name);
            WebMessage.ClickMap(this.gameObject.name);
        }
        catch (Exception ex)
        {
            Debug.LogError("errr:" + ex.ToString());
        }
        
    }

    public void CancelClick()
    {
        if (meshRender != null)
        {
            meshRender.material.color = curColor;
        }
        
    }
}


public static class CommonTools
{
    public static T AddOrGetComponent<T>(this GameObject go) where T : Component 
    {
        
        T t= go.GetComponent<T>();
        if (t == null)
        {
            t = go.gameObject.AddComponent<T>();
        }
        return t;
    }
}