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

    public void ChangeScene(int sceneNum)//���Ŀ� �񵿱⳪ ������ ����� �����ؼ� ������ �ʿ������� ����� ����� �켱�ؼ� ����
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
