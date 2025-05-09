using UnityEngine;
using System.Diagnostics;
using System.IO;
using UnityEngine.UIElements;
using UnityEditor.PackageManager;
using System;

public class RCTest : MonoBehaviour
{
    string scriptPath = @"C:/Work/myscript.rcscript";

    private void Start()
    {
        CreateRealityCaptureScript2();
        UnityEngine.Debug.Log("1");
    }

    private void CreateRealityCaptureScript2()//굳이 필요한가?
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
        selectComponent 0
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
        psi.Arguments = string.Join(" ", new string[] //원하는 명령어를 미리 스크립트로 만들어서 저장하는 것
        {
            "-newScene",
            "-addFolder \"" + inputFolder + "\"",
            "-align",
            //"-selectComponent", "-index", "0", //-> 이미 사용했던 방식을 재사용 가능
            //"-reconstructModel", "-detailLevel", "Normal", -> 존재하지 않는 cli 명령어
            "-calculateNormalModel",
            "-save", "\"" + projectPath + "\"",
            "-exportModel", "Model_1", "\"" + outputModelPath + "\"", //기존적으로 모델의 이름을 Model1 으로 생성하기 때문에
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
    }
}
