using UnityEngine;
using System.Diagnostics;
using System.IO;
using UnityEngine.UIElements;
using UnityEditor.PackageManager;
using System;
using UnityEditor.SearchService;

public class RCTest : MonoBehaviour
{
    string scriptPath = @"C:/Work/myscript.rcscript";

    private void Start()
    {
        CreateRealityCaptureScript2();
        UnityEngine.Debug.Log("1");
    }

    private void CreateRealityCaptureScript2()
    {
        string outputDir = @"C:/Work";
        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        // 자동 생성
        File.WriteAllText(scriptPath,
        @"
        newProject
        addFolder ""C:/Work/InputImages""
        alignImages
        calculateNormalModel
        simplifyModel maxTriangles=200000
        exportModel ""C:/Work/Output/model.obj""
        save ""C:/Work/MyProject.rcproj""
        exit
        ");
    }

    public void RunRealityCapture()
    {
        string inputFolder = @"C:/Work/CapturedImages";
        string outputFolder = @"C:/Work/Output";
        string projectPath = @"C:/Work/MyProject";
        string outputModelPath = Path.Combine(outputFolder, "model.obj");

        UnityEngine.Debug.Log("2");

        // 폴더 미리 생성 (없으면 에러남)
        if (!Directory.Exists(inputFolder))
        {
            UnityEngine.Debug.LogError("입력 이미지 폴더가 존재하지 않습니다: " + inputFolder);
            return;
        }
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }

        // RealityCapture 실행 정보 구성
        ProcessStartInfo psi = new ProcessStartInfo();
        psi.FileName = @"C:\Program Files\Capturing Reality\RealityCapture\RealityCapture.exe"; //앱이면 위치 변경, 현재는 rc 가 존재하는 위치이지만 나중에는 서버에서 사용할 예정
        psi.Arguments = string.Join(" ", new string[]
{
    "-newScene",
    "-addFolder \"" + inputFolder + "\"",
    "-align",
    "-calculateNormalModel",
    "-selectComponent \"Component 0\"",
    "-selectModel \"Model 1\"",
    "-simplify",
    "-exportModel \"Model 1\"","\"" + outputModelPath + "\"",
    "-save \"" + projectPath + "\"",
    "-quit"
});

        psi.UseShellExecute = false;
        psi.CreateNoWindow = true;
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;

        // 실행
        Process process = Process.Start(psi);
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        UnityEngine.Debug.Log("RealityCapture Output:\n" + output);
        if (!string.IsNullOrEmpty(error))
        {
            UnityEngine.Debug.LogError("RealityCapture Error:\n" + error);
        }

        AppSceneManger.Instance.ChangeScene(3);
    }
}
