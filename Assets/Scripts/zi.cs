using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class zi : MonoBehaviour
{
    public TextAsset text;
    public GameObject textPrefab;
    public GameObject panel;
    private string[] str;
    private int txt_no;
    private int txt_size;
    private int[] vector = new int[6] { 400, 500, 600, 700, 900, 1000 };
    private int i;
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
    }

    // Update is called once per frame
    void Update()
    {

    }
    void Inst()
    {
        Rand();
        GameObject text = Instantiate(textPrefab, new Vector3(2200, vector[i], 0), Quaternion.identity);
        if (i < 5)
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
            text.GetComponent<Text>().fontSize = 60;
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
