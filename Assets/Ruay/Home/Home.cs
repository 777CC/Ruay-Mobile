using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class Home : MonoBehaviour {
    //public Image photo;
    public TextAsset text;
    // Use this for initialization
    [SerializeField]
    public Page[] pages;
    void Start () {
        //pages = JsonHelper.getJsonArray<Page>(text.text);
        //Debug.Log(JsonUtility.ToJson(this));
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
