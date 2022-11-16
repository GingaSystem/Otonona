using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDraw : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    // LineRendererコンポーネントをゲームオブジェクトにアタッチする
    var lineRenderer = gameObject.AddComponent<LineRenderer>();

    var positions = new Vector3[]{
        new Vector3(-2.5f, 0, 0),               // 開始点
        new Vector3(-2.0f, 0, 0),               // 終了点
    };
    var positions2 = new Vector3[]{
        new Vector3(-1.5f, 0, 0),               // 開始点
        new Vector3(-1.0f, 0, 0),               // 終了点
    };

    lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    lineRenderer.startColor = Color.red;
    lineRenderer.endColor = Color.green;
    lineRenderer.numCapVertices = 10; //角を丸く
    lineRenderer.startWidth = 0.1f;                   // 開始点の太さを0.1にする
    lineRenderer.endWidth = 0.1f;                     // 終了点の太さを0.1にする
    lineRenderer.SetPositions(positions);             // 線を引く場所を指定する
    }

    // Update is called once per frame
    void Update()
    {
        var positions2 = new Vector3[]{
        new Vector3(-1.5f, 0, 0),               // 開始点
        new Vector3(-1.0f, 0, 0),               // 終了点
    };

       // draw(positions2);
    }
    void draw(Vector3[] positions){
    var lineRenderer = gameObject.AddComponent<LineRenderer>();
    lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    lineRenderer.startColor = Color.red;
    lineRenderer.endColor = Color.green;
    lineRenderer.numCapVertices = 10; //角を丸く
    lineRenderer.startWidth = 0.1f;                   // 開始点の太さを0.1にする
    lineRenderer.endWidth = 0.1f;                     // 終了点の太さを0.1にする
    lineRenderer.SetPositions(positions);    
    }
}
