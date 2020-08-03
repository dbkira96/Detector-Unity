using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEditor;

public class DetectionController : MonoBehaviour
{
    public UnityEngine.UI.Text txtDebug;
    public Camera camera;
    private bool askDetection=false;
    private readonly string baseurl = "http://192.168.1.141:5000/";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnPostRender()
    {
        if (askDetection)
        {
            askDetection = false;
            RenderTexture renderTexture = camera.targetTexture;

            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0,0,renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);
            byte[] array = renderResult.EncodeToJPG();
        }
    }
    IEnumerator AskDetection(byte[] img)
    {
        string url = baseurl + "detect/synth1";
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
        camera.targetTexture = RenderTexture.GetTemporary(416, 416, 16);
        askDetection = true;
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

    public void OnTestButton() {
        StartCoroutine(getText());
        
    }
}
