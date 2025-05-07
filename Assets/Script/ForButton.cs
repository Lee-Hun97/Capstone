using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForButton : MonoBehaviour
{
    public void CallChangeSceneFunction(int i)
    {
        AppSceneManger.Instance.ChangeScene(i);
    }
}
