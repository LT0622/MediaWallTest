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
using System.Collections;

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
    private string read_rfid="";
    public string read_rfid0="";//读取到的RFID
    public Button[] Buttons = new Button[5];
    public GameObject[] Anim = new GameObject[5];
    public VideoPlayer[] videoPlayer = new VideoPlayer[5];
    public AudioSource Audio;//音频播放
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
       
            if (read_rfid0 != read_rfid)
            {
                read_rfid0 = read_rfid;
                if (read_rfid0 == "")
                {
                    Debug.Log("拿走RFID");
                    Audio.Pause();
                }
                else
                {
                    Debug.Log("接收到RFID：" + read_rfid0);
                    int none = 0;//与本地rfid数组内查找，没有则计数加一
                    for(int i = 0; i < Rfid.rfid.Length; i++)
                    {
                        if (read_rfid0 == Rfid.rfid[i])
                        {
                            StartCoroutine(LoadAudio(Rfid.path[i]));
                            Debug.Log("正确的RFID，播放音频："+ Rfid.path[i]);                         
                        }
                        else
                        {
                            none++;
                        }
                    }
                   if(none== Rfid.rfid.Length)
                    {
                        Debug.Log("没有该RFID");
                    }
                    
                }
            }
           
        
       if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            port.Open();
            Debug.Log("键盘操作，串口打开");
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            port.Close();
            Debug.Log("键盘操作，串口关闭");
        }

    }

    public void SendMsg(string s)//发送数据
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
    void DataReceiveFunction()//接收数据
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
                    //length = port.Read(buffer, 0, buffer.Length);
                    length += port.Read(buffer, length, buffer.Length - length);//接收字节                                 
                   // Debug.Log(length + "," + buffer[0]);                  
                    if (length == buffer[0]&&length!=5)
                    {
                        check_rfid(buffer);
                    }
                    else
                    {                    
                        read_rfid = "";
                    }
                    length = 0;
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
    void check_rfid(byte[] buffer)//检查数据
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
            UUID += vector[0][i].ToString("X");//rfid
        }
        read_rfid = UUID;
      
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
        read_rfid = Rfid.rfid[0];
        anim(0);
    }
    private void OnClick_1()
    {
        read_rfid = Rfid.rfid[1];
        anim(1);
    }
    private void OnClick_2()
    {
        read_rfid = Rfid.rfid[2];
        anim(2);
    }
    private void OnClick_3()
    {
        read_rfid = Rfid.rfid[3];
        anim(3);
    }
    private void OnClick_4()
    {
        read_rfid = Rfid.rfid[4];
        anim(4);
    }
    private void anim(int i)
    {             
        Anim[i].GetComponent<RawImage>().DOFade(1, 0.5f);
        DOTween.To(v => { }, 0, 0, 0.5f).onComplete += () => { videoPlayer[i].Play(); };
    }
    IEnumerator LoadAudio(string backPath)//加载音频
    {

        WWW www = new WWW(Application.dataPath + "/StreamingAssets/music/"+backPath);

        yield return www;
        AudioClip clip = www.GetAudioClip();
        Audio.clip = clip;
        Audio.Play();
    }
}