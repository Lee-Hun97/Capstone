using UnityEngine;
using System.Diagnostics;
using System.IO;
using UnityEngine.UIElements;

public class UseRealityCapture : MonoBehaviour
{
    public string rcPath = "C:\\Program Files\\Capturing Reality\\RealityCapture\\RealityCapture.exe";
    public string imageFolder = "C:\\CapturedImages"; //�ڵ������� �ٲ۴ٸ� ��ΰ� ����Ǿ�� �Ѵ�.
    public string outputModel = "C:\\Output\\model.obj";
    public string projectPath = "C:\\Output\\myProject.rcproj";
    string rclFilePath = @"C:/Work/auto_script.rcl";
    public string objPath = "C:/Output/model.obj";

    public void RunRCProcess()
    {
        if (!File.Exists(rcPath))
        {
            UnityEngine.Debug.LogError("RealityCapture ���� ������ ã�� �� �����ϴ�.");
            return;
        }

        //���� �ڵ�
        //ProcessStartInfo psi = new ProcessStartInfo();
        //psi.FileName = rcPath;
        //psi.Arguments = $"-addFolder \"C:\\CapturedImages\" -align -run \"CalculateModel\" -exportModel \"C:\\Output\\model.obj\"";
        //psi.RedirectStandardOutput = true;
        //psi.RedirectStandardError = true;
        //psi.UseShellExecute = false;
        //psi.CreateNoWindow = true;

        //Process process = new Process();
        //process.StartInfo = psi;
        //process.OutputDataReceived += (sender, args) => UnityEngine.Debug.Log(args.Data);
        //process.ErrorDataReceived += (sender, args) => UnityEngine.Debug.LogError(args.Data);
        //process.Start();
        //process.BeginOutputReadLine();
        //process.BeginErrorReadLine();
        //process.WaitForExit();

        //UnityEngine.Debug.Log("RealityCapture �۾� �Ϸ�!");

        ProcessStartInfo psi = new ProcessStartInfo();
        psi.FileName = "C:/Program Files/Capturing Reality/RealityCapture/RealityCapture.exe"; // RC ���
        psi.Arguments = "-newScene "; //\"C:/Work/MyProject.rcproj\"
        psi.UseShellExecute = false;
        psi.CreateNoWindow = true;
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;

        using (Process process = Process.Start(psi))
        {
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            UnityEngine.Debug.Log("Output: " + output);
            if (!string.IsNullOrEmpty(error))
            {
                UnityEngine.Debug.LogError("Error: " + error);
            }
        }

        CreateRealityCaptureScript(imageFolder, outputModel, projectPath, rclFilePath);

        //ProcessStartInfo psi2 = new ProcessStartInfo();
        //psi2.FileName = "C:/Program Files/Capturing Reality/RealityCapture/RealityCapture.exe";
        //psi2.Arguments = $"-proj \"{projectPath}\" -rcl \"{rclFilePath}\"";
        //psi2.UseShellExecute = false;
        //psi2.CreateNoWindow = true;
        //Process.Start(psi2);
    }

    private void CreateRealityCaptureScript(string imageFolderPath, string outputModelPath, string projectFilePath, string rclFilePath)
    {
        string rclScript = $@"
        import ""{imageFolderPath}""
        align
        calculateNormalModel
        exportModel ""{outputModelPath}""
        save ""{projectFilePath}""
        exit
        ";

        // ���Ϸ� ����
        File.WriteAllText(rclFilePath, rclScript);
    }

    //private void LoadObjModel(string path)
    //{
    //    var importer = new OBJLoader();
    //    GameObject model = importer.ImportFile(path);

    //    if (model != null)
    //    {
    //        model.transform.position = Vector3.zero;
    //        Debug.Log("�� �ε� �Ϸ�!");
    //    }
    //    else
    //    {
    //        Debug.LogError("�� �ε� ����");
    //    }
    //}
}
