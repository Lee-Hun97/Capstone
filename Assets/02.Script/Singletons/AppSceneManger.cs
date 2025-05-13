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

    public void ChangeScene(Scene_name sceneNum)//이후에 비동기나 동기의 방식을 선택해서 변경이 필요하지만 현재는 기능을 우선해서 제작
    {
        switch (sceneNum)
        {
            case Scene_name.LoginScene: SceneManager.LoadScene(scene0_name); return;
            case Scene_name.MainScene: SceneManager.LoadScene(scene1_name); return;
            case Scene_name.CameraScene: SceneManager.LoadScene(scene2_name); return;
            case Scene_name.ModelScene: SceneManager.LoadScene(scene3_name); return;
            case Scene_name.MRScene: SceneManager.LoadScene(scene4_name); return;
            default: ExitApp(); return; //조건을 더 추가해서 예외가 발생하면 종료되는 문제를 제거해야함
        }
    }

    public void ExitApp()
    {
#if UNITY_EDITOR
        // 에디터에서 재생 모드를 종료
        UnityEditor.EditorApplication.isPlaying = false;
#else
    // 빌드된 앱에서는 종료
    Application.Quit();

        // Android에서는 Quit 호출 후에도 앱이 바로 종료되지 않을 수 있어 아래 코드 추가
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

