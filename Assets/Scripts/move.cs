using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class move : MonoBehaviour
{
    private float time;
    // Start is called before the first frame update
    void Start()
    {
        if (transform.GetComponent<Text>().fontSize == 80)
        {
            time = 6;
        }
        else
        {
            time = 5;
        }
        transform.DOLocalMoveX(-1400, time).SetEase(Ease.Linear).OnComplete(Destroy);
    }
    void Destroy()
    {
        Destroy(transform.gameObject);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
