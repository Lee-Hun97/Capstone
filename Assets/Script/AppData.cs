using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AppData : Singlton<AppData>
{
    public string ID;
    public string PW;

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Debug.Log("AppData destroyed");
    }

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("AppData Initialized");
    }


    public void SetInfo(string id, string pw)
    {
        ID = id;
        PW = pw;
    }
    public void GetInfo()
    {
        Debug.Log($"{ID}, {PW}");
    }
}
