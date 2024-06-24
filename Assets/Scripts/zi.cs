using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using RenderHeads.Media.AVProVideo;

public class zi : MonoBehaviour
{
    public TextAsset text;
    public GameObject textPrefab;
    public GameObject panel;
    private string[] str;
    private int txt_no;
    private int txt_size;
    private int[] vector = new int[6] { 200, 500, 600, 700, 900, 1000 };
    private int i;
    public MediaPlayer mediaPlayer;
    public DisplayUGUI DisplayUGUI;
    // Start is called before the first frame update
    void Start()
    {
        str = text.text.Split('\n');
        InvokeRepeating("Inst", 0, 6);
        InvokeRepeating("Inst", 0, 4);
        InvokeRepeating("Inst", 1, 6);
        InvokeRepeating("Inst", 1, 4);
        InvokeRepeating("Inst", 2, 6);
        InvokeRepeating("Inst", 2, 4);
        DisplayUGUI.color = new Color(1, 1, 1, 0);
        mediaPlayer.Control.Stop();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            mediaPlayer.Control.Play();
            panel.GetComponent<CanvasGroup>().DOFade(0, 1).SetEase(Ease.Linear).onComplete += () =>
            {
                DisplayUGUI.DOFade(1, 1).SetEase(Ease.Linear);
            };
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            DisplayUGUI.DOFade(0, 1).SetEase(Ease.Linear).onComplete += () =>
            {
                mediaPlayer.Control.Seek(0);
                mediaPlayer.Control.Stop();
                panel.GetComponent<CanvasGroup>().DOFade(1, 2).SetEase(Ease.Linear);
            };
        }
    }
    void Inst()
    {
        Rand();
        GameObject text = Instantiate(textPrefab, new Vector3(2200, vector[i], 0), Quaternion.identity);
        if (i < vector.Length - 1)
        {
            i++;
        }
        else
        {
            i = 0;
        }
        text.transform.parent = panel.transform;//把实例化的物体放到父物体之下
        text.GetComponent<Text>().text = str[txt_no];
        if (txt_size == 0)
        {
            text.GetComponent<Text>().fontSize = 50;
        }
        else
        {
            text.GetComponent<Text>().fontSize = 80;
        }
    }
    void Rand()
    {
        txt_no = Random.Range(0, str.Length);
        txt_size = Random.Range(0, 2);
    }
}
