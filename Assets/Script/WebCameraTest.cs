using UnityEngine;
using UnityEngine.UI;

public class WebCameraTest : MonoBehaviour
{

    public RawImage rawImage;

    WebCamTexture webCamTexture;

    void Start()
    {
        webCamTexture = new WebCamTexture();
        rawImage.texture = webCamTexture;
        webCamTexture.Play();
    }

    void Update()
    {
    }
}
