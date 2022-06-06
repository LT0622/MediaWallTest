using UnityEngine;
using UnityEngine.UI;
using SerialPortUtility;
using System;
using System.Diagnostics;
using DG.Tweening;

public class Check : MonoBehaviour
{
    public SerialPortUtilityPro port;
    public GameObject panel;
    public Text text;
    private float time;
    private bool reConnect;
    // Start is called before the first frame update
    void Start()
    {
        panel.SetActive(false);
        // AudioSettings.OnAudioConfigurationChanged += text.text = "音频设备断开";
    //    AudioSettings.OnAudioConfigurationChanged += Change_Audio();
    }
    void Change_Audio()
    {
        text.text = "音频设备断开";
    }
    // Update is called once per frame
    void Update()
    {
        if (!port.IsOpened())
        {
            time += Time.deltaTime;
            panel.SetActive(true);
            text.text = "串口断开，正在重新连接";
            panel.GetComponent<CanvasGroup>().DOFade(0.4f, 1).SetEase(Ease.Linear);
            if (time >= 3)
            {
                OpenPort();
                time = 0;
            }
        }
    }
    void OpenPort()
    {
        try
        {
            if (reConnect == false)
            {
                UnityEngine.Debug.Log("串口断开，正在重新连接");
                reConnect = true;
            }
            port.Open();
        }
        catch (Exception)
        {
        }
        if (port.IsOpened())
        {
            UnityEngine.Debug.Log("串口重新连接成功");
            reConnect = false;
            panel.GetComponent<CanvasGroup>().DOFade(0, 1f).SetEase(Ease.Linear).onComplete += () =>
            {
                text.text = "";
                panel.SetActive(false);
            };
        }
    }
    void CallPython()
    {
        Process process = new Process();// 创建一个进程对象
        // 设置启动程序为指定路径下的Python解释器，该程序可以运行Python脚本
        process.StartInfo.FileName = @"E:\Anaconda3\envs\Unity_Lab\python.exe";
        process.StartInfo.UseShellExecute = false;// 进行进程其他的设置
        process.StartInfo.Arguments = @"D:\Projects\PycharmProjects\HelloWorld.py";// 设置Python脚本路径和传入的参数
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.CreateNoWindow = true;// 不显示执行窗口        
        process.Start();// 启动进程
        // 获取进程运行的输出，
        process.BeginOutputReadLine();
        //添加结果输出委托
        process.OutputDataReceived += new DataReceivedEventHandler(GetData);
        process.WaitForExit();
        // 结果委托
        void GetData(object sender, DataReceivedEventArgs e)
        {
            // 结果不为空才打印（可以自己添加类型不同的处理委托）
            if (string.IsNullOrEmpty(e.Data) == false)
            {
                UnityEngine.Debug.Log(e.Data);
            }
        }
    }
}

