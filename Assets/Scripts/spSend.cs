using UnityEngine;
using System;
using System.IO.Ports;
using UnityEngine.UI;
using System.Text;
using System.Threading;
using System.IO;
using LitJson;
using UnityEngine.Video;
using DG.Tweening;

[System.Serializable]
public class RFID
{
    public string port;
    public string[] rfid = new string[5];
    public string[] path = new string[5];
}
public class spSend : MonoBehaviour
{

    public SerialPort port;
    public RFID Rfid = new RFID();
    private string a;
    Thread dataReceiveThread;
    private string JsonPath;
    static public byte[] strSend = new byte[4] { 0x04, 0x01, 0xDC, 0x1E };
    int length = 0;
    string msg = "0401DC1E";
    public Button[] Buttons = new Button[5];
    public GameObject[] Anim = new GameObject[5];
    public VideoPlayer[] videoPlayer = new VideoPlayer[5];
    void Awake()
    {
        JsonPath = Application.dataPath + "/StreamingAssets/configData.text";
        load();
    }
    void Start()
    {

        //串口初始化 
        port = new SerialPort(Rfid.port, 19200, Parity.None, 8, StopBits.One);
        port.Open();
        if (port.IsOpen)
        {
            Debug.Log("串口打开成功");
        }
        else
        {
            Debug.Log("串口打开失败");
        }       
       
        dataReceiveThread = new Thread(new ThreadStart(DataReceiveFunction));//开启线程发送和接收
        dataReceiveThread.Start();

        Buttons[0].onClick.AddListener(OnClick_0);
        Buttons[1].onClick.AddListener(OnClick_1);
        Buttons[2].onClick.AddListener(OnClick_2);
        Buttons[3].onClick.AddListener(OnClick_3);
        Buttons[4].onClick.AddListener(OnClick_4);
        for (int i = 0; i < Buttons.Length; i++)
        {
            Buttons[i].GetComponent<Image>().DOFade(0, 2).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        }
      
    }

    //关闭串口
    void OnApplicationQuit()
    {
        port.Close();
        Application.Quit();
    }

    //发送
    public void SendData(byte[] data)
    {
        if (port.IsOpen)
        {

            for (int i = 0; i < data.Length; i++)
            {
                a += data[i].ToString();
            }
            port.Write(data, 0, data.Length);
         //   Debug.Log("send:" + a);
            a = null;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //  SendData(strSend);
            length = 0;
            SendMsg(msg);           
        }
        
    }

    public void SendMsg(string s)
    {
        string msg = s;
        byte[] cmd = new byte[1024 * 1024 * 3];
        cmd = Convert16(msg);
        SendData(cmd);
    }
    private byte[] Convert16(string strText)
    {
        strText = strText.Replace(" ", "");
        byte[] bText = new byte[strText.Length / 2];
        for (int i = 0; i < strText.Length / 2; i++)
        {
            bText[i] = Convert.ToByte(Convert.ToInt32(strText.Substring(i * 2, 2), 16));
        }
        return bText;
    }
    void DataReceiveFunction()
    {
        byte[] buffer = new byte[1024];
        
        while (true)
        {
            if (port != null && port.IsOpen)
            {
                
                    SendMsg(msg);//发送
                    Thread.Sleep(100);
                try
                {
                    length += port.Read(buffer, length, buffer.Length - length);//接收字节
                   // Debug.Log(length + "," + buffer[0]);
                    if (length == buffer[0])
                    {
                        check_rfid(buffer);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.GetType() != typeof(ThreadAbortException))
                    {
                    }
                }
            }
            Thread.Sleep(100);
        }
    }
    void check_rfid(byte[] buffer)
    {
        int length = buffer[0];
        int cards_number = (length - 5) / 8;
        byte[][] vector = new byte[cards_number][];

       //  Debug.Log(cards_number);
        for (int i = 0; i < cards_number; i++)
        {
            //  vector[i] = buffer.split(4 + i * 8, 4 + (i + 1) * 8 - 1);
            vector[i] = new byte[8];
            Array.Copy(buffer, 4 + i * 8, vector[i], 0, 8);
        }

        string UUID = "";
        for(int i = 0; i < 8; i++)
        {
            UUID += vector[0][i].ToString("X");
        }
        Debug.Log("接收到RFID："+UUID);
    }
    void load()//读取
    {
        if (File.Exists(JsonPath))
        {
            //创建一个关闭StreamReader，用来读取流
            StreamReader sr = new StreamReader(JsonPath);
            //将流赋值给jsonstr
            string jsonstr = sr.ReadToEnd();
            //关闭
            sr.Close();
            //将字符串jsonstr转化为RFID对象
            Rfid = JsonMapper.ToObject<RFID>(jsonstr);
        }
        else
        {
            Debug.Log("json文件不存在");
        }
    }
    private void OnClick_0()
    {
        SendMsg(msg);
        anim(0);
    }
    private void OnClick_1()
    {
        SendMsg(msg);
        anim(1);
    }
    private void OnClick_2()
    {
        SendMsg(msg);
        anim(2);
    }
    private void OnClick_3()
    {
        SendMsg(msg);
        anim(3);
    }
    private void OnClick_4()
    {
        SendMsg(msg);
        anim(4);
    }
    private void anim(int i)
    {
        //  Anim.GetComponent<RawImage>().color = new Color(1, 1, 1, 0);
        //    videoPlayer.Stop();       
        Anim[i].GetComponent<RawImage>().DOFade(1, 0.5f);
        DOTween.To(v => { }, 0, 0, 0.5f).onComplete += () => { videoPlayer[i].Play(); };
    }

}