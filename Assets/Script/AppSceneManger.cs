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
    private string scene3_name = "3DModelScene";

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
            case 3: SceneManager.LoadScene(scene3_name); return;
            default: ExitApp(); return;
        }
    }

    public void ExitApp()
    {
#if UNITY_EDITOR
        // �����Ϳ��� ��� ��带 ����
        UnityEditor.EditorApplication.isPlaying = false;
#else
    // ����� �ۿ����� ����
    Application.Quit();

        // Android������ Quit ȣ�� �Ŀ��� ���� �ٷ� ������� ���� �� �־� �Ʒ� �ڵ� �߰�
    #if UNITY_ANDROID
            AndroidJavaObject activity = null;
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }
            activity.Call("finish");
    #endif

#endif
    }
}

