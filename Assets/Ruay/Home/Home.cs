using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
[SerializeField]
public class Home : MonoBehaviour {
    //public Image photo;
    [SerializeField]
    List<Page> pages;
    // Use this for initialization
    void Start () {
        //pages[0] = new Page();
        //pages[0].Name = "Home";
        //pages[1] = new Page();
        //pages[1].Name = "Test";
        
       Debug.Log(JsonUtility.ToJson(this));
        string home = Manager.Instance.homeJson;
        if (!string.IsNullOrEmpty(home))
        {
            JsonUtility.FromJsonOverwrite(home, this);
        }
        else
        {
            Manager.Instance.OnHomeDownloadSuccess = Downloaded;
            Manager.Instance.DownloadHomeJson();
        }
    }

    void Downloaded(string json)
    {
        JsonUtility.FromJsonOverwrite(json, this);
        Debug.Log(pages.Count);
        Page homePage = pages.Find(page => page.Name == "Home1");
        if(homePage != null)
        {
            Debug.Log(homePage.Cards.Length);
        }
        else
        {
        }
    }

	// Update is called once per frame
	//void Update () {
 //       if (Input.GetMouseButtonDown(0))
 //       {
 //           Vector3[] corners = new Vector3[4];
 //           photo.rectTransform.GetWorldCorners(corners);
 //           Rect newRect = new Rect(corners[0], corners[2] - corners[0]);
 //           Debug.Log(newRect.Contains(Input.mousePosition));
 //           Debug.Log(newRect.width);
 //           Debug.Log(Screen.width);
 //       }
 //   }
//    public void 
    public void TestLambda()
    {
        Manager.Instance.LambdaClient.InvokeAsync(new Amazon.Lambda.Model.InvokeRequest()
        {
            FunctionName = "poll",
            Payload = "{}"
        },
        (responseObject) =>
        {
            if (responseObject.Exception == null)
            {
                Debug.Log(System.Text.Encoding.ASCII.GetString(responseObject.Response.Payload.ToArray()));
            }
            else
            {
                Debug.Log(responseObject.Exception);
            }
        }
        );
    }
}
