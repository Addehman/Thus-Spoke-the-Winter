using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class viewPortScript : MonoBehaviour
{
    public Vector3 raw;
    public Vector3 ScreenToWorld;
    public Vector3 WorldToScreen;
    public Vector3 ViewToScreen;
    public Vector3 ViewToWorld;
    public Vector3 ScreenToView;
    public Vector3 WorldToView;


    // Update is called once per frame
    void Update()
    {
        raw = Input.mousePosition;
        ScreenToWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        WorldToScreen = Camera.main.WorldToScreenPoint(Input.mousePosition);
        ViewToScreen = Camera.main.ViewportToScreenPoint(Input.mousePosition);
        ViewToWorld = Camera.main.ViewportToWorldPoint(Input.mousePosition);
        ScreenToView = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        WorldToView = Camera.main.WorldToViewportPoint(Input.mousePosition);

    }
}