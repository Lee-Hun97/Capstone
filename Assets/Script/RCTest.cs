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

        // �ڵ� ����
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

        // ���� �̸� ���� (������ ������)
        if (!Directory.Exists(inputFolder))
        {
            UnityEngine.Debug.LogError("�Է� �̹��� ������ �������� �ʽ��ϴ�: " + inputFolder);
            return;
        }
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }

        // RealityCapture ���� ���� ����
        ProcessStartInfo psi = new ProcessStartInfo();
        psi.FileName = @"C:\Program Files\Capturing Reality\RealityCapture\RealityCapture.exe"; //���̸� ��ġ ����, ����� rc �� �����ϴ� ��ġ������ ���߿��� �������� ����� ����
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

        // ����
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
