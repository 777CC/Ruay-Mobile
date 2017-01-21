﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
public class LoginPage : MonoBehaviour {
    [SerializeField]
    private GameObject popup;
    [SerializeField]
    private CanvasGroup loading;
    //[SerializeField]
    //private CanvasGroup popupAlpha;
    [SerializeField]
    private Text statusMessage;
    public void Start()
    {
        loading.gameObject.SetActive(true);
        LeanTween.alphaCanvas(loading, 1,0.3f).setDelay(0.3f);
        ConnectFacebook();
    }
    public void ConnectFacebook()
    {
#if USE_FACEBOOK_LOGIN
        statusMessage.text = "Connecting to Facebook";
        if (!FB.IsInitialized)
        {
            FB.Init(delegate ()
            {
                Debug.Log("starting thread");
                // shows to connect the current identityid or create a new identityid with facebook authentication
                Login();
            });
        }
        else
        {
            Login();
        }
#else
                 statusMessage.text = "Not Facebook.";
#endif
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
            Manager.Instance.DownloadHomeJson();
            Manager.Instance.OnSyncSuccess = HandleSyncSuccess;
            Manager.Instance.OnSyncFailure = HandleSyncFailure;
            Manager.Instance.LoginCognitoWithFacebook();
            StartCoroutine(SetTimeout());
        }
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
        Manager.Instance.UpdateAppInfo();
        yield return new WaitForSeconds(15);
        popup.SetActive(true);
        statusMessage.text = "กรุณาลองใหม่อีกครั้ง\nหรือโหลดเวอร์ชั่นใหม่";
        popup.GetComponentInChildren<Button>().onClick.AddListener(() => {
            SceneManager.LoadScene("Login");
        });
    }
}