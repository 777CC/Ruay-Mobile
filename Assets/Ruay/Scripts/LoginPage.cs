﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
//using UnityEngine.Advertisements;

public class LoginPage : MonoBehaviour {
    [SerializeField]
    private GameObject popup;
    [SerializeField]
    private CanvasGroup loading;
    //[SerializeField]
    //private CanvasGroup popupAlpha;
    [SerializeField]
    private Text statusMessage;
    [SerializeField]
    private RectTransform checkerImage;
    public void Start()
    {
        //For android status bar and navigation bar
        Screen.fullScreen = false;

        loading.gameObject.SetActive(true);
        LeanTween.alphaCanvas(loading, 1,0.3f).setDelay(0.3f);
        ConnectFacebook();
        //StartCoroutine(CheckScreenSize());
    }
    //IEnumerator CheckScreenSize()
    //{
    //    yield return new WaitForSeconds(0.1f);
        
    //        var worldCorners = new Vector3[4];
    //    checkerImage.GetWorldCorners(worldCorners);
    //    var result = new Rect(
    //                  worldCorners[0].x,
    //                  worldCorners[0].y,
    //                  worldCorners[2].x - worldCorners[0].x,
    //                  worldCorners[2].y - worldCorners[0].y);
    //    Debug.Log((int)result.width);
    //    Debug.Log((int)result.height);
    //    Debug.Log(Manager.Instance.ImageSizeHeight);
    //    Debug.Log(Manager.Instance.ImageSizeWidth);
    //    Debug.Log(Screen.width);
    //}
    public void ConnectFacebook()
    {
#if USE_FACEBOOK_LOGIN
        if (!FB.IsInitialized)
        {
            FB.Init(this.OnInitComplete);
            //FB.Init(delegate ()
            //{
            //    Debug.Log("starting thread");
            //    // shows to connect the current identityid or create a new identityid with facebook authentication
            //    Login();
            //});
        }
        else
        {
            Login();
        }
#else
        statusMessage.text = "Not Facebook.";
        Debug.Log("Not Facebook.");
#endif
    }
    private void OnInitComplete()
    {
        if(!FB.IsInitialized)
        {
            StartCoroutine(SetTimeout());
        }
        else
        {
            if(FB.IsLoggedIn)
            {
                LoginCognito();
            }
            else
            {
                Login();
            }
        }
    }
    void Login()
    {
        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, FacebookLoginCallback);
    }
    private void FacebookLoginCallback(IResult result)
    {
        Debug.Log("FB.Login completed");
        if (result.Error != null || !FB.IsLoggedIn)
        {
            Debug.LogError(result.Error);
            statusMessage.text = result.Error;
            popup.gameObject.SetActive(true);
        }
        else
        {
            //foreach (KeyValuePair<string, object> entry in result.ResultDictionary)
            //{
            //    Debug.Log(entry.Key + " : " + entry.Value);
            //}
            if (result.ResultDictionary.ContainsKey("user_id"))
            {
                Manager.Instance.facebookId = result.ResultDictionary["user_id"].ToString();
            }
            LoginCognito();
        }
    }
    private void LoginCognito()
    {
        Manager.Instance.DownloadHomeJson(null);
        Manager.Instance.OnSyncSuccess = HandleSyncSuccess;
        Manager.Instance.OnSyncFailure = HandleSyncFailure;
        Manager.Instance.LoginCognitoWithFacebook();
        StartCoroutine(SetTimeout());
    }
    private void HandleSyncSuccess(string e)
    {
        if (Manager.Instance.IsUserRegistered)
        {
            SceneManager.LoadScene("Home");
        }
        else
        {
            SceneManager.LoadScene("UserInfoEditor");
        }
    }
    private void HandleSyncFailure(string exception)
    {
        popup.gameObject.SetActive(true);
        statusMessage.text = "กรุณาลองใหม่อีกครั้ง\n" + exception;
        popup.GetComponentInChildren<Button>().onClick.AddListener(() => {
            SceneManager.LoadScene("Login");
        });
        //Manager.Instance.UpdateAppInfo();
    }
    private IEnumerator SetTimeout()
    {
        yield return new WaitForSeconds(60);
        //HandleSyncFailure("");
        Manager.Instance.UpdateAppInfo(()=> { });
        yield return new WaitForSeconds(15);
        popup.SetActive(true);
        statusMessage.text = "กรุณาลองใหม่อีกครั้ง\nหรือโหลดเวอร์ชั่นใหม่";
        popup.GetComponentInChildren<Button>().onClick.AddListener(() => {
            SceneManager.LoadScene("Login");
        });
    }
    //public void ShowRewardedAd()
    //{
    //    if (Advertisement.IsReady("rewardedVideo"))
    //    {
    //        var options = new ShowOptions { resultCallback = HandleShowResult };
    //        Advertisement.Show("rewardedVideo", options);
    //    }
    //}

    //private void HandleShowResult(ShowResult result)
    //{
    //    switch (result)
    //    {
    //        case ShowResult.Finished:
    //            Debug.Log("The ad was successfully shown.");
    //            //
    //            // YOUR CODE TO REWARD THE GAMER
    //            // Give coins etc.
    //            break;
    //        case ShowResult.Skipped:
    //            Debug.Log("The ad was skipped before reaching the end.");
    //            break;
    //        case ShowResult.Failed:
    //            Debug.LogError("The ad failed to be shown.");
    //            break;
    //    }
    //}
}