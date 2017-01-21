﻿using UnityEngine;
using System.Collections;
using Amazon;
using Amazon.CognitoSync;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentity.Model;
using Amazon.CognitoSync.SyncManager;
using Amazon.Lambda;
using Amazon.Lambda.Model;
//using Amazon.S3;
//using Amazon.S3.Model;
using Facebook.Unity;
using System.Collections.Generic;
using System;

public class Manager : Singleton<Manager>
//public class Manager:MonoBehaviour
{
    const string PrefsName = "UserInfo";
    protected Manager() { }
    void Start()
    {
        
        Debug.Log("Awake" + PlayerPrefs.GetString(PrefsName));
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString(PrefsName)))
        {
            Debug.Log("Load user info from PlayerPrefs. \n" + PlayerPrefs.GetString(PrefsName));
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(PrefsName), Instance);
        }
        if (Instance.GetComponent<UnityInitializer>() == null)
        {
            UnityInitializer.AttachToGameObject(Instance.gameObject);
        }
        SetAWS();
    }
    //void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        Handheld.Vibrate();
    //    }
    //}
    public void UpdateUserInfo()
    {
        double time = GetUnixTime();
        if ((updateTime + updateTimeCount < time) || (updateTime - updateTimeCount > time))
        {
            Debug.Log("UpdateUserInfo : " + time + " : " + ((updateTime + updateTimeCount) - time));
            Debug.Log("tel " + tel + " + " + birthday);
            PutToDataset("updateTime", time.ToString("0"));
            PutToDataset("facebookId", facebookId);
            PutToDataset("firstName", firstName);
            PutToDataset("lastName", lastName);
            PutToDataset("tel", tel.ToString());
            PutToDataset("inviteBy", inviteBy);
            PutToDataset("birthday", birthday.ToString());
            PutToDataset("gender", gender);
            PutToDataset("interests", interests);
            UserInfo.SynchronizeAsync();
            foreach (KeyValuePair<string, string> entry in UserInfo.ActiveRecords)
            {
                Debug.Log("ar " + entry.Key + " : " + entry.Value);
            }
            foreach (Record rec in UserInfo.Records)
            {
                Debug.Log("rec " + rec.Key + " : " + rec.Value);
            }
        }
        else
        {
            Debug.Log("No UpdateUserInfo : " + time + " : " + updateTime + updateTimeCount);
            OnSyncSuccess(string.Empty);
        }
        Save();
    }
    #region UserData
    private string[] UserInfoKeys = { "firstName", "lastName", "gender", "tel", "birthday", "interests" };
    public string facebookId = string.Empty;
    public int satang = 0;
    public string firstName = string.Empty;
    public string lastName = string.Empty;
    public string inviteBy = string.Empty;
    public string inviteName = string.Empty;
    public double updateTime = 0;
    public int dailyRawardTime;
    public string email = string.Empty;
    public string gender = string.Empty;
    [SerializeField]
    public int tel = 0;
    [SerializeField]
    public int birthday = 0;
    public string interests = string.Empty;
    public Ticket[] Tickets;
    public Reward[] Rewards;
    public string[] Event;
    #endregion
    
    #region AppInfo
    //Cognito
    [SerializeField]
    private string dataSetName = "Info";
    //S3
    //public const string AppInfoS3BucketName = "testchainkchoonoi", AppInfoFileName = "Appinfo.txt";
    //const string appInfoUrl = @"";
    const string appInfoUrl = @"https://s3-ap-southeast-1.amazonaws.com/ruay/Appinfo.json";
    //const string info = "Appinfo.json";
    [SerializeField]
    private string appUrl = @"https://s3-ap-southeast-1.amazonaws.com/ruay/";
    [SerializeField]
    private string homeFlieName = "home.json";
    [NonSerialized]
    public string homeJson;
    //const string 
    [SerializeField]
    private string IdentityPoolId = "ap-northeast-1:d2051844-b612-4173-a365-838a4da96036";
    //public string IdentityPoolId = "ap-northeast-1:d2051844-b000-0000-a365-838a4da96000";
    [SerializeField]
    private string regionName = RegionEndpoint.APNortheast1.SystemName;
    //private int updateTimeCount = 86400000;//milliseconds in Day.
    [SerializeField]
    private double updateTimeCount = 180000;
    #endregion
    public void Save()
    {
        string save = JsonUtility.ToJson(Instance);
        PlayerPrefs.SetString(PrefsName, save);
        PlayerPrefs.Save();
        Debug.Log("Save : " + save);
        //System.IO.File.WriteAllText(System.IO.Path.Combine(Application.streamingAssetsPath, "Appinfo.txt"), JsonUtility.ToJson(Instance));
    }
    public bool IsUserRegistered
    {
        get
        {
            bool IsUserRegistered = true;
            for(int i = 0; i< UserInfoKeys.Length;i++)
            {
                string val = UserInfo.Get(UserInfoKeys[i]);
                int isZero;
                int.TryParse(val, out isZero);
                if (string.IsNullOrEmpty(val) || val == "0")
                {
                    Debug.Log("NullOrEmpty :" + UserInfoKeys[i]);
                    IsUserRegistered = false;
                }
            }
            return IsUserRegistered;
        }
    }
    public void UpdateAppInfo()
    {
        Debug.Log("UpdateAppInfo");
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
        WWW www = new WWW(appInfoUrl);
        yield return www;
        Debug.Log(www.text);
        if (www.responseHeaders.Count > 0)
        {
            foreach (KeyValuePair<string, string> entry in www.responseHeaders)
            {
                Debug.Log(entry.Value + "=" + entry.Key);
            }
        }
        try {
            JsonUtility.FromJsonOverwrite(www.text, Instance);
            SetAWS();
            Save();
        }catch
        {
            Debug.Log("cannot update appinfo");
        }
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
    private Coroutine homeJsonCoroutine;
    public void DownloadHomeJson()
    {
        if(homeJsonCoroutine == null)
        {
            homeJsonCoroutine = StartCoroutine(downloadHomeJson());
        }
    }
    IEnumerator downloadHomeJson()
    {
        WWW www = new WWW(appUrl + homeFlieName);
        //WWW www = new WWW(appUrl);
        Debug.Log(www.url);
        yield return !www.isDone;
        homeJson = www.text;
        if (OnHomeDownloadSuccess != null)
        {
            OnHomeDownloadSuccess(www.text);
        }
        Debug.Log(homeJson);
    }
    public OnSyncCallback OnHomeDownloadSuccess;
    public OnSyncCallback OnError;
    public void Error(string e)
    {
        if (OnError != null)
        {
            OnError(e);
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
    //private IAmazonS3 s3Client;
    //public IAmazonS3 S3Client
    //{
    //    get
    //    {
    //        if (s3Client == null)
    //        {
    //            SetAWS();
    //        }
    //        //test comment
    //        return s3Client;
    //    }
    //}
    public void LoginCognitoWithFacebook()
    {
        Credentials.AddLogin("graph.facebook.com", AccessToken.CurrentAccessToken.TokenString);
        UpdateUserInfo();
    }
    private void PutToDataset(string key,string val)
    {
        if (!string.IsNullOrEmpty(val)){
            UserInfo.Put(key, val);
        }
    }
    private void HandleSyncSuccess(object sender, SyncSuccessEventArgs e)
    {
        Debug.Log("HandleSyncSuccess :" + e.UpdatedRecords);
        double time;
        if (double.TryParse(UserInfo.Get("updateTime"), out time))
        {
            updateTime = time;
        }
        firstName = UserInfo.Get("firstName");
        lastName = UserInfo.Get("lastName");
        gender = UserInfo.Get("gender");
        int number;
        if (int.TryParse(UserInfo.Get("tel"), out number))
        {
            tel = number;
        }
        int date;
        if (int.TryParse(UserInfo.Get("birthday"), out date))
        {
            birthday = date;
        }
        interests = UserInfo.Get("interests");
        Save();
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
        Debug.Log("SetAWS");
        region = RegionEndpoint.GetBySystemName(regionName);
        credentials = new CognitoAWSCredentials(IdentityPoolId, region);
        syncManager = new CognitoSyncManager(credentials, new AmazonCognitoSyncConfig { RegionEndpoint = region });
        userInfo = SyncManager.OpenOrCreateDataset(dataSetName);
        userInfo.OnSyncSuccess += HandleSyncSuccess;
        userInfo.OnSyncFailure += HandleSyncFailure;
        UserInfo.OnSyncConflict = HandleSyncConflict;
        lambdaClient = new AmazonLambdaClient(credentials, region);
        //s3Client = new AmazonS3Client(credentials, region);
    }
    private double GetUnixTime()
    {
        //DateTime baseDate = new DateTime(1970, 1, 1);
        //TimeSpan diff = DateTime.UtcNow - baseDate;
        //return diff.TotalMilliseconds.ToString();
        //return ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
        return (DateTime.UtcNow.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds + updateTimeCount);
    }
    #endregion
}