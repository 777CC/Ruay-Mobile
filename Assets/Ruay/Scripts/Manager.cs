using UnityEngine;
using System.Collections;
using Amazon;
using Amazon.CognitoSync;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentity.Model;
using Amazon.CognitoSync.SyncManager;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Facebook.Unity;
public class Manager : Singleton<Manager>
{
    const string PrefsName = "Player";
    protected Manager() {
        //Debug.Log(json);
    }//UserData = new User(); } // guarantee this will be always a singleton only - can't use the constructor!

    void Awake()
    {
        //string json = JsonUtility.ToJson(Manager.Instance);
        //Debug.Log(json);
        if (PlayerPrefs.GetString(PrefsName) != string.Empty)
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(PrefsName), Instance);
        }
        else
        {

        }
        UnityInitializer.AttachToGameObject(Instance.gameObject);
        SetAWS();
    }
    #region UserData
    public int satang = 0;
    public string firstName = string.Empty;
    public string lastName = string.Empty;
    public string inviteBy = string.Empty;
    public string inviteName = string.Empty;
    public int updateTime;
    public int dailyRawardTime;
    public string email = string.Empty;
    public string gender = string.Empty;
    public int tel = 0;
    public int birthday = 0;
    public string interestings = string.Empty;
    public Ticket[] Tickets;
    public Reward[] Rewards;
    public string[] Event;
    #endregion

    #region AppInfo
    //Cognito
    public string DataSetName = "Info";
    private Dataset userInfo;
    public Dataset UserInfo
    {
        get
        {
            if (userInfo == null)
            {
                userInfo = SyncManager.OpenOrCreateDataset(DataSetName);
            }
            return userInfo;
        }
    }
    //S3
    //public const string AppInfoS3BucketName = "testchainkchoonoi", AppInfoFileName = "Appinfo.txt";
    const string AppInfoUrl = @"https://s3-ap-northeast-1.amazonaws.com/testchainkchoonoi/Appinfo.txt";
    //public string IdentityPoolId = "ap-northeast-1:d2051844-b612-4173-a365-838a4da96036";
    public string IdentityPoolId;
    public string regionName = RegionEndpoint.APNortheast1.SystemName;
    #endregion
    public void Save()
    {
        PlayerPrefs.SetString(PrefsName, JsonUtility.ToJson(Instance));
        System.IO.File.WriteAllText(System.IO.Path.Combine(Application.streamingAssetsPath, "Appinfo.txt"), JsonUtility.ToJson(Instance));
    }

    private IEnumerator DownloadAppInfo()
    {
        WWW www = new WWW(AppInfoUrl);
        yield return www;
        JsonUtility.FromJsonOverwrite(www.text, Instance);
        SetAWS();
    }

    public void UpdateAppInfo()
    {
        Debug.Log(S3Client == null);
        StartCoroutine(DownloadAppInfo());
        //S3Client.GetObjectAsync(AppInfoS3BucketName, AppInfoFileName, (responseObj) =>
        //{
        //    if (responseObj.Exception == null)
        //    {
        //        var response = responseObj.Response;
        //        if (response.ResponseStream != null)
        //        {
        //            System.IO.StreamReader reader = new System.IO.StreamReader(response.ResponseStream);
        //            string data = reader.ReadToEnd();
        //            JsonUtility.FromJsonOverwrite(System.IO.File.ReadAllText(System.IO.Path.Combine(Application.streamingAssetsPath, "Appinfo.txt")), Instance);
        //            SetAWS();
        //            Debug.Log("OK");
        //        }
        //        else
        //        {
        //            Debug.Log("Oop!! : " + responseObj.Exception);
        //        }
        //    }
        //    else
        //    {
        //        Debug.Log("Oop!! : " + responseObj.Exception);
        //    }
        //});   
    }

    private RegionEndpoint region;
    private RegionEndpoint Region
    {
        get
        {
            if (region == null)
            {
                SetAWS();
            }
            return region;
        }
    }
    private CognitoSyncManager syncManager;
    private CognitoSyncManager SyncManager
    {
        get
        {
            if (syncManager == null)
            {
                SetAWS();
            }
            return syncManager;
        }
    }
    private CognitoAWSCredentials credentials;
    private CognitoAWSCredentials Credentials
    {
        get
        {
            if (credentials == null)
                SetAWS();
            return credentials;
        }
    }
    private IAmazonLambda lambdaClient;
    public IAmazonLambda LambdaClient
    {
        get
        {
            if (lambdaClient == null)
            {
                SetAWS();
            }
            return lambdaClient;
        }
    }
    private IAmazonS3 s3Client;
    public IAmazonS3 S3Client
    {
        get
        {
            if (s3Client == null)
            {
                SetAWS();
            }
            //test comment
            return s3Client;
        }
    }
    public void FBLogin()
    {
        Credentials.AddLogin("graph.facebook.com", AccessToken.CurrentAccessToken.TokenString);
    }
    private void SetAWS()
    {
        region = RegionEndpoint.GetBySystemName(regionName);
        credentials = new CognitoAWSCredentials(IdentityPoolId, Region);
        syncManager = new CognitoSyncManager(Credentials, new AmazonCognitoSyncConfig { RegionEndpoint = Region });
        lambdaClient = new AmazonLambdaClient(Credentials, Region);
        s3Client = new AmazonS3Client(Credentials, Region);
    }
}