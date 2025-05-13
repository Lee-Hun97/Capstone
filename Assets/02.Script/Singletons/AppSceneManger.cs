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
    private string scene4_name = "MR_CameraScene";

    protected override void Awake()
    {
        base.Awake();
        currentSceneName = SceneManager.GetActiveScene().name; 
        Debug.Log("AppSceneManger Initialized");
    }

    protected override void Start()
    {
        base.Start();
    }

    public void ChangeScene(Scene_name sceneNum)//���Ŀ� �񵿱⳪ ������ ����� �����ؼ� ������ �ʿ������� ����� ����� �켱�ؼ� ����
    {
        switch (sceneNum)
        {
            case Scene_name.LoginScene: SceneManager.LoadScene(scene0_name); return;
            case Scene_name.MainScene: SceneManager.LoadScene(scene1_name); return;
            case Scene_name.CameraScene: SceneManager.LoadScene(scene2_name); return;
            case Scene_name.ModelScene: SceneManager.LoadScene(scene3_name); return;
            case Scene_name.MRScene: SceneManager.LoadScene(scene4_name); return;
            default: ExitApp(); return; //������ �� �߰��ؼ� ���ܰ� �߻��ϸ� ����Ǵ� ������ �����ؾ���
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

