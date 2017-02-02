using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

public class Home : MonoBehaviour {
    //public Image photo;
    public TextAsset text;
    public Texture2D testSP;
    // Use this for initialization
    [SerializeField]
    public Page[] pages;
    void Start () {
        //string test = JsonUtility.ToJson(this);
        //Debug.Log("test : " + test.Length);
        //string path = Application.streamingAssetsPath + @"\test.txt";
        //File.WriteAllText(path, test);
        //string ttt =  File.ReadAllText(path);
        //Debug.Log("ttt : " + ttt.Length);
        //JsonUtility.FromJsonOverwrite(ttt, this);
        //JsonUtility.FromJsonOverwrite(text.text, this);
        //pages = JsonHelper.getJsonArray<Page>(text.text);
        //Debug.Log(JsonUtility.ToJson(this));
        StartCoroutine(TestWWW());
    }
    IEnumerator TestWWW()
    {
        WWW www = new WWW("chainchoonoi.com/home.json");
        yield return www;
        Debug.Log(www.text);
    }
    public void TestLambda()
    {
        //Manager.Instance.LambdaClient.InvokeAsync(new Amazon.Lambda.Model.InvokeRequest()
        //{
        //    FunctionName = "poll",
        //    Payload = "{}"
        //},
        //(responseObject) =>
        //{
        //    if (responseObject.Exception == null)
        //    {
        //        Debug.Log(System.Text.Encoding.ASCII.GetString(responseObject.Response.Payload.ToArray()));
        //    }
        //    else
        //    {
        //        Debug.Log(responseObject.Exception);
        //    }
        //}
        //);
    }
}
