using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEditor;
using UnityEngine.Rendering;


public class DetectionController : MonoBehaviour
{
    public UnityEngine.UI.Text txtDebug;
    
    public Texture2D ScreenshotTexture { get; private set; }
       private int _screenshotTextureH=1080;
       private int _screenshotTextureW=1920;
        private void Awake()
        {  
            ScreenshotTexture = new Texture2D(_screenshotTextureW, _screenshotTextureH, TextureFormat.RGB24, false);
        }
 
    private readonly string baseurl = "http://192.168.1.141:5000/";
    // Start is called before the first frame update
    IEnumerator AskDetection()
    {
        yield return new WaitForEndOfFrame();
        RenderTexture renderTexture = RenderTexture.GetTemporary(Screen.width,Screen.height,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Default,1);
        ScreenCapture.CaptureScreenshotIntoRenderTexture(renderTexture);
        RenderTexture.active = renderTexture;
        
        
        txtDebug.text="we bello hguaglio";
        ScreenshotTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height),0, 0);
        ScreenshotTexture.Apply();
        
        byte[] img = ScreenshotTexture.EncodeToJPG();
        
            
        string url = baseurl + "predict/synth1";
        List<IMultipartFormSection> data = new List<IMultipartFormSection>();
        data.Add(new MultipartFormFileSection("image",img,"img.jpg","image/jpeg"));
        UnityWebRequest request = UnityWebRequest.Post(url, data);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError(request.error);
            yield break;

        }
        JSONNode response = JSON.Parse(request.downloadHandler.text);
        txtDebug.text = response;
    }
    public void Detect()
    { 
        txtDebug.text="asking for detection";
       
        StartCoroutine(AskDetection());
        //txtDebug.text = "asking detection";
        
       
    }
    IEnumerator getText() {
        string url = baseurl + "test";
        
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError(request.error);
            yield break;

        }
        JSONNode data = JSON.Parse(request.downloadHandler.text);
        txtDebug.text = data["content"];

    }
}
