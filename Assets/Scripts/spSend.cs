using UnityEngine;
using System;
using System.IO.Ports;
using UnityEngine.UI;
using System.Threading;
using System.IO;
using LitJson;
using UnityEngine.Video;
using DG.Tweening;
using System.Collections;

[System.Serializable]
public class RFID
{
    public string Port;
    public string[] rfid = new string[5];
    public string[] path = new string[5];
}
public class spSend : MonoBehaviour
{
    public SerialPort Port;
    public RFID RFID = new RFID();
    Thread dataReceiveThread;
    private string _JsonPath;
    public byte[] SendData = new byte[4] { 0x04, 0x01, 0xDC, 0x1E };
    private int _length = 0;
    private string RfidRead = "";
    public string RfidNow = "";//读取到的RFID
    public Button[] Buttons = new Button[5];
    public GameObject[] Anims = new GameObject[5];
    public VideoPlayer[] VideoPlayers = new VideoPlayer[5];
    public AudioSource Audio;//音频播放
    void Awake()
    {
        _JsonPath = Application.dataPath + "/StreamingAssets/configData.text";
        load();
    }
    void Start()
    {

        Port = new SerialPort(RFID.Port, 19200, Parity.None, 8, StopBits.One);
        Port.Open();
        if (Port.IsOpen)
        {
            Debug.Log("串口打开成功");
        }
        else
        {
            Debug.Log("串口打开失败");
        }
        dataReceiveThread = new Thread(new ThreadStart(DataReceiveFunction));//开启线程发送和接收
        dataReceiveThread.Start();

        Buttons[0].onClick.AddListener(OnClick_0);//按钮事件监听
        Buttons[1].onClick.AddListener(OnClick_1);
        Buttons[2].onClick.AddListener(OnClick_2);
        Buttons[3].onClick.AddListener(OnClick_3);
        Buttons[4].onClick.AddListener(OnClick_4);
        for (int i = 0; i < Buttons.Length; i++)
        {
            Buttons[i].GetComponent<Image>().DOFade(0, 2).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        }

    }
    void OnApplicationQuit()
    {
        Port.Close();
        Application.Quit();
    }
    private void Update()
    {
        if (RfidNow != RfidRead)
        {
            RfidNow = RfidRead;
            if (RfidNow == "")
            {
                Debug.Log("拿走RFID");
                Anim_stop();
                Audio.Pause();
            }
            else
            {
                Debug.Log("接收到RFID：" + RfidNow);
                int i;
                for (i = 0; i < RFID.rfid.Length; i++)
                {
                    if (RfidNow == RFID.rfid[i])
                    {
                        StartCoroutine(LoadAudio(RFID.path[i]));
                        Buttons[i].onClick.Invoke();//按钮按下
                        Debug.Log("正确的RFID，播放音频：" + RFID.path[i]);
                        break;
                    }
                }
                if (i == RFID.rfid.Length)
                {
                    Debug.Log("没有该RFID");
                }

            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Port.Open();
            Debug.Log("键盘操作，串口打开");
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Port.Close();
            Debug.Log("键盘操作，串口关闭");
        }
    }
    void DataReceiveFunction()//接收数据
    {
        byte[] buffer = new byte[1024];

        while (true)
        {
            if (Port != null && Port.IsOpen)
            {
                Port.Write(SendData, 0, SendData.Length);
                Thread.Sleep(100);
                try
                {
                    _length += Port.Read(buffer, _length, buffer.Length - _length);//接收字节                                 
                    if (_length == buffer[0] && _length != 5)
                    {
                        check_rfid(buffer);
                    }
                    else
                    {
                        RfidRead = "";
                    }
                    _length = 0;
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
        int _length = buffer[0];
        int cards_number = (_length - 5) / 8;
        byte[][] vector = new byte[cards_number][];

        //  Debug.Log(cards_number);
        for (int i = 0; i < cards_number; i++)
        {
            //  vector[i] = buffer.split(4 + i * 8, 4 + (i + 1) * 8 - 1);
            vector[i] = new byte[8];
            Array.Copy(buffer, 4 + i * 8, vector[i], 0, 8);
        }

        string UUID = "";
        for (int i = 0; i < 8; i++)
        {
            UUID += vector[0][i].ToString("X");//rfid
        }
        RfidRead = UUID;

    }
    void load()//读取
    {
        if (File.Exists(_JsonPath))
        {
            StreamReader sr = new StreamReader(_JsonPath);
            string jsonstr = sr.ReadToEnd();
            sr.Close();
            RFID = JsonMapper.ToObject<RFID>(jsonstr);
        }
        else
        {
            Debug.Log("json文件不存在");
        }
    }
    #region 按钮事件绑定
    private void OnClick_0()
    {
        RfidRead = RFID.rfid[0];
        Anim_play(0);
    }
    private void OnClick_1()
    {
        RfidRead = RFID.rfid[1];
        Anim_play(1);
    }
    private void OnClick_2()
    {
        RfidRead = RFID.rfid[2];
        Anim_play(2);
    }
    private void OnClick_3()
    {
        RfidRead = RFID.rfid[3];
        Anim_play(3);
    }
    private void OnClick_4()
    {
        RfidRead = RFID.rfid[4];
        Anim_play(4);
    }
    #endregion
    private void Anim_play(int i)
    {
        Anims[i].GetComponent<RawImage>().DOFade(1, 0.5f);
        VideoPlayers[i].Play();
    }
    private void Anim_stop()
    {
        for (int i = 0; i < Anims.Length; i++)
        {
            Anims[i].GetComponent<RawImage>().DOFade(0, 0.5f);
            DOTween.To(v => { }, 0, 0, 0.5f).onComplete += () => { VideoPlayers[i].Stop(); };
        }
    }
    IEnumerator LoadAudio(string backPath)//加载音频
    {
        WWW www = new WWW(Application.dataPath + "/StreamingAssets/music/" + backPath);
        yield return www;
        AudioClip clip = www.GetAudioClip();
        Audio.clip = clip;
        Audio.Play();
    }
}