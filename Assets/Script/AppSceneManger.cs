using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppSceneManger : Singlton<AppSceneManger>
{
    [SerializeField]private string currentSceneName = "";

    private string scene0_name = "LoginScene";
    private string scene1_name = "MainScene";
    private string scene2_name = "CameraScene"; 

    protected override void Awake()
    {
        base.Awake();

        currentSceneName = SceneManager.GetActiveScene().name;
    }

    public void ChangeScene(int sceneNum)//이후에 비동기나 동기의 방식을 선택해서 변경이 필요하지만 현재는 기능을 우선해서 제작
    {
        switch (sceneNum)
        {
            case 0: SceneManager.LoadScene(scene0_name); return;
            case 1: SceneManager.LoadScene(scene1_name); return;
            case 2: SceneManager.LoadScene(scene2_name); return;
            default: return;
        }
    }
}
