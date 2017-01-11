using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
public class LoginPage : MonoBehaviour {
    [SerializeField]
    private GameObject popup;
    [SerializeField]
    private Text statusMessage;
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
            popup.SetActive(true);
        }
        else
        {
            Manager.Instance.LoginCognitoWithFacebook();
            //LoadPhoto();
            if (Manager.Instance.IsUserRegistered)
            {
                SceneManager.LoadScene("Home");
            }
            else
            {
                SceneManager.LoadScene("UserDataEditor");
            }
        }
    }
}
