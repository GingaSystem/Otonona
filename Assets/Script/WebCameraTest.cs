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
        float scaleY = webCamTexture.videoVerticallyMirrored ? -1f : 1f;
        rawImage.transform.localScale = new Vector3(1f, scaleY, 1f);
        int orient = -webCamTexture.videoRotationAngle;
        rawImage.transform.localEulerAngles = new Vector3(0f, 0f, orient);
    }
}
