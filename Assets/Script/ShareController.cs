using UnityEngine;
using System.IO;
using System.Collections;
using SocialConnector;
using UnityEngine.UI;

public class ShareController : MonoBehaviour
{
    public GameObject button;
    public GameObject button2;
    public GameObject button3;

    public void Share()
    {
        //スクリーンショットを撮る時ボタンを消したい
        button.SetActive(false);
        button2.SetActive(false);
        button3.SetActive(false);
        StartCoroutine(_Share());

    }

    public IEnumerator _Share()
    {
        string imgPath = Application.persistentDataPath + "/image.png";

        // 前回のデータを削除
        File.Delete(imgPath);

        //スクリーンショットを撮影
        ScreenCapture.CaptureScreenshot("image.png");



        // 撮影画像の保存が完了するまで待機
        while (true)
        {
            if (File.Exists(imgPath))
            {
                Debug.Log("ボタン戻す");
                button.SetActive(true);
                button2.SetActive(true);
                button3.SetActive(true);
                break;
            }

            yield return null;
            Debug.Log("ボタン戻す");
            button.SetActive(true);
            button2.SetActive(true);


        }



        // 投稿する
        string tweetText = "Otononaで撮ったよ! #オトノナ ";
        string tweetURL = null;
        SocialConnector.SocialConnector.Share(tweetText, tweetURL, imgPath);


    }
}
