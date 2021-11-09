using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clearButtonScript : MonoBehaviour
{
    public Mic mic;

    // ボタンが押された場合、今回呼び出される関数
    public void OnClick()
    {
        Debug.Log("clear押された!");  // ログを出力
        //GameObject mic = GameObject.Find("Mic");
        //mic.GetComponent<Mic>().Clear();
        mic.Clear();
    }
}