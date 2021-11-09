using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cameraButtonScript : MonoBehaviour
{

    // ボタンが押された場合、今回呼び出される関数
    public void OnClick()
    {
        Debug.Log("camera押された!");  // ログを出力
        GameObject sc = GameObject.Find("ShareController");
        sc.GetComponent<ShareController>().Share(); //ここなにやってるのだろ
    }
}