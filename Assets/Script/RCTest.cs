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

    private void CreateRealityCaptureScript2()//���� �ʿ��Ѱ�?
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
        psi.Arguments = string.Join(" ", new string[] //���ϴ� ��ɾ �̸� ��ũ��Ʈ�� ���� �����ϴ� ��
        {
            "-newScene",
            "-addFolder \"" + inputFolder + "\"",
            "-align",
            //"-selectComponent", "-index", "0", //-> �̹� ����ߴ� ����� ���� ����
            //"-reconstructModel", "-detailLevel", "Normal", -> �������� �ʴ� cli ��ɾ�
            "-calculateNormalModel",
            "-save", "\"" + projectPath + "\"",
            "-exportModel", "Model_1", "\"" + outputModelPath + "\"", //���������� ���� �̸��� Model1 ���� �����ϱ� ������
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
    }
}
