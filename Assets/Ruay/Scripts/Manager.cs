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
using System.Collections.Generic;

public class Manager : Singleton<Manager>
//public class Manager:MonoBehaviour
{
    const string PrefsName = "UserInfo";
    protected Manager() { }

    void Awake()
    {
        if (PlayerPrefs.GetString(PrefsName) != string.Empty)
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(PrefsName), Instance);
        }
    }
    void Start()
    {
        UnityInitializer.AttachToGameObject(gameObject);
    }
    #region UserData
    private string[] UserInfoNames = { "firstName", "lastName", "inviteBy", "inviteName", "email", "gender", "tel", "birthday", "interestings"};
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
    //S3
    //public const string AppInfoS3BucketName = "testchainkchoonoi", AppInfoFileName = "Appinfo.txt";
    const string AppInfoUrl = @"https://s3-ap-northeast-1.amazonaws.com/testchainkchoonoi/Appinfo.txt";
    public string IdentityPoolId = "ap-northeast-1:d2051844-b612-4173-a365-838a4da96036";
    public string regionName = RegionEndpoint.APNortheast1.SystemName;
    #endregion
    public void Save()
    {
        PlayerPrefs.SetString(PrefsName, JsonUtility.ToJson(Instance));
        //System.IO.File.WriteAllText(System.IO.Path.Combine(Application.streamingAssetsPath, "Appinfo.txt"), JsonUtility.ToJson(Instance));
    }
    public bool IsUserRegistered
    {
        get
        {
            bool IsUserRegistered = true;
            for(int i = 0; i< UserInfoNames.Length;i++)
            {
                if (string.IsNullOrEmpty(UserInfo.Get(UserInfoNames[i])))
                {
                    IsUserRegistered = false;
                }
            }
            return IsUserRegistered;
        }
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
    private IEnumerator DownloadAppInfo()
    {
        WWW www = new WWW(AppInfoUrl);
        yield return www;
        JsonUtility.FromJsonOverwrite(www.text, Instance);
        SetAWS();
    }
    #region AWS
    private Dataset userInfo;
    private Dataset UserInfo
    {
        get
        {
            if (userInfo == null)
            {
                SetAWS();
            }
            return userInfo;
        }
    }
    public delegate void OnSyncCallback(string e);
    public OnSyncCallback OnSyncSuccess;
    public OnSyncCallback OnSyncFailure;
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
    public void LoginCognitoWithFacebook()
    {
        Credentials.AddLogin("graph.facebook.com", AccessToken.CurrentAccessToken.TokenString);
    }
    public void SyncCognito()
    {
        //Set UserInfo
        PutToDataset("firstName", firstName);
        PutToDataset("lastName", lastName);
        PutToDataset("tel", tel.ToString());
        PutToDataset("inviteBy", inviteBy);
        //UserInfo.Put("email", "");
        PutToDataset("birthday", birthday.ToString());
        PutToDataset("gender", gender);
        PutToDataset("interest", interestings);
        UserInfo.SynchronizeAsync();
    }
    private void PutToDataset(string name,string val)
    {
        if (!string.IsNullOrEmpty(val)){
            UserInfo.Put(name, val);
        }
    }
    private void HandleSyncSuccess(object sender, SyncSuccessEventArgs e)
    {
        Debug.Log("HandleSyncSuccess");
        if (OnSyncSuccess != null)
        {
            OnSyncSuccess(string.Empty);
        }
    }
    private void HandleSyncFailure(object sender, SyncFailureEventArgs e)
    {
        Debug.Log("HandleSyncFailure" + e.Exception.ToString());
        if (OnSyncFailure != null)
        {
            OnSyncFailure(e.Exception.ToString());
        }
    }
    private bool HandleSyncConflict(Amazon.CognitoSync.SyncManager.Dataset dataset, List<SyncConflict> conflicts)
    {
        List<Record> resolvedRecords = new List<Record>();
        foreach (SyncConflict conflictRecord in conflicts)
        {
            // This example resolves all the conflicts using ResolveWithRemoteRecord 
            // SyncManager provides the following default conflict resolution methods:
            //      ResolveWithRemoteRecord - overwrites the local with remote records
            //      ResolveWithLocalRecord - overwrites the remote with local records
            //      ResolveWithValue - for developer logic  
            resolvedRecords.Add(conflictRecord.ResolveWithRemoteRecord());
        }

        // resolves the conflicts in local storage
        dataset.Resolve(resolvedRecords);

        //updateUIValue();

        // on return true the synchronize operation continues where it left,
        //      returning false cancels the synchronize operation
        return true;
    }

    private void SetAWS()
    {
        region = RegionEndpoint.GetBySystemName(regionName);
        credentials = new CognitoAWSCredentials(IdentityPoolId, region);
        syncManager = new CognitoSyncManager(credentials, new AmazonCognitoSyncConfig { RegionEndpoint = region });
        userInfo = SyncManager.OpenOrCreateDataset(DataSetName);
        userInfo.OnSyncSuccess += HandleSyncSuccess;
        userInfo.OnSyncFailure += HandleSyncFailure;
        UserInfo.OnSyncConflict = HandleSyncConflict;
        lambdaClient = new AmazonLambdaClient(credentials, region);
        s3Client = new AmazonS3Client(credentials, region);
    }
    #endregion
}