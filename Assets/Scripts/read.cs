using UnityEngine;
using UnityEngine.UI;
using SerialPortUtility;
using System.IO;
using LitJson;
using DG.Tweening;
using UnityEngine.Video;

[System.Serializable]
public class RFID
{
    public string port;
    public string[] rfid = new string[5];
    public string[] path = new string[5]; 
}
public class read : MonoBehaviour
{
    public SerialPortUtilityPro port;
    public RFID Rfid = new RFID();
    private string JsonPath;
    public Button[] Buttons = new Button[5];
    public GameObject[] Anim=new GameObject[5];
    public VideoPlayer[] videoPlayer=new VideoPlayer[5];
    
    void Awake()
    {
        JsonPath = Application.dataPath + "/StreamingAssets/configData.text";             
        load();
    }
    void Start()
    {
        try
        {
        port.SerialNumber = Rfid.port; //读取json文件里的端口号
        port.Open();
        }
        catch (System.ArgumentNullException)
        {

        }
        Buttons[0].onClick.AddListener(OnClick_0);
        Buttons[1].onClick.AddListener(OnClick_1);
        Buttons[2].onClick.AddListener(OnClick_2);
        Buttons[3].onClick.AddListener(OnClick_3);
        Buttons[4].onClick.AddListener(OnClick_4);
        for(int i = 0; i < Buttons.Length; i++)
        {
            Buttons[i].GetComponent<Image>().DOFade(0, 2).SetEase(Ease.Linear).SetLoops(-1,LoopType.Yoyo);
        }
    }

    [System.Obsolete]
    public void GetValue(object data)
    {
        var info = data as string;
        if (info == null)
        {          
            return;
        }
        info = info[0].ToString();//得到RFID
        for(int i = 0; i < Rfid.rfid.Length;i++)
        {
            if (info == Rfid.rfid[i])
            {
                port.SendMessage(Rfid.path[i]);
                anim(i);
            }           
        }
       
    }
    // Update is called once per frame
    void Update()
    {
       
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
        port.SendMessage(Rfid.path[0]);     
        anim(0);
    }
    private void OnClick_1()
    {
        port.SendMessage(Rfid.path[1]);    
        anim(1);
    }
    private void OnClick_2()
    {
        port.SendMessage(Rfid.path[2]);     
        anim(2);
    }
    private void OnClick_3()
    {
        port.SendMessage(Rfid.path[3]);     
        anim(3);
    }
    private void OnClick_4()
    {
        port.SendMessage(Rfid.path[4]);    
        anim(4);
    }
    private void anim(int i)
    {
      //  Anim.GetComponent<RawImage>().color = new Color(1, 1, 1, 0);
    //    videoPlayer.Stop();       
        Anim[i].GetComponent<RawImage>().DOFade(1, 0.5f);
        DOTween.To(v => { }, 0, 0, 0.5f).onComplete += () => {videoPlayer[i].Play();};
    }
   
}
