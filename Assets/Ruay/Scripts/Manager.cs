using UnityEngine;
using System.Collections;
using Amazon;
using Amazon.CognitoSync;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentity.Model;
using Amazon.CognitoSync.SyncManager;
//using Amazon.S3;
//using Amazon.S3.Model;
using Facebook.Unity;
using System.Collections.Generic;
using System;
using System.Text;

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
    [SerializeField]
    private Ticket[] tickets;
    [SerializeField]
    private Reward[] Rewards;
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
    //const string 
    [SerializeField]
    private string IdentityPoolId = "ap-northeast-1:d2051844-b612-4173-a365-838a4da96036";
    //public string IdentityPoolId = "ap-northeast-1:d2051844-b000-0000-a365-838a4da96000";
    [SerializeField]
    private string regionName = RegionEndpoint.APNortheast1.SystemName;
    //private int updateTimeCount = 86400000;//milliseconds in Day.
    [SerializeField]
    private double updateTimeCount = 180000;
    const string HomePageName = "Home";
    const string MyTicketName = "MyTickets";
    [SerializeField]
    private List<Page> pages;
    [SerializeField]
    private Round[] rounds;
    public delegate void GetPage(Page page);
    public void GetPageByName(string pageName,GetPage getPage)
    {
        if (pages != null)
        {
            if (getPage != null)
            {
               getPage(pages.Find(page => page.Name == pageName));
            }
        }
        else
        {
            StartCoroutine( WaitForDonwloadHomeJson(pageName,getPage));
        }
    }
    IEnumerator WaitForDonwloadHomeJson(string pageName,GetPage getPage)
    {
        yield return new WaitUntil(()=> pages != null);
        getPage(pages.Find(page => page.Name == pageName));
    }
    public delegate void GetRound(Round round);
    public delegate void GetTicket(Ticket item,Round round);
    public void GetRoundById(string roundId, GetRound getRound)
    {
        if (pages != null)
        {
            if (getRound != null)
            {
                getRound(Array.Find(rounds, p => p.id == roundId));
            }
        }
        else
        {
            StartCoroutine(WaitForDonwloadHomeJson(roundId, getRound));
        }
    }
    public void GetTicketById(int ticketIndex, GetTicket getTicket)
    {
        Debug.Log("GetTicketById :" + ticketIndex);
        if (tickets != null)
        {
            if (getTicket != null && ticketIndex < tickets.Length)
            {
                getTicket(tickets[ticketIndex], Array.Find(rounds, r => r.id == tickets[ticketIndex].roundId));
            }
        }
    }
    IEnumerator WaitForDonwloadHomeJson(string roundId, GetRound getRound)
    {
        yield return new WaitUntil(() => pages != null);
        getRound(Array.Find(rounds, p => p.id == roundId));
    }
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
    public void UpdateAppInfo(System.Action callback)
    {
        Debug.Log("UpdateAppInfo");
        StartCoroutine(DownloadAppInfo(callback));
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
    private IEnumerator DownloadAppInfo(System.Action callback)
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
            callback();
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
    public void DownloadHomeJson(Action callback)
    {
        StartCoroutine(downloadHomeJson(callback));
    }
    IEnumerator downloadHomeJson(Action callback)
    {
        WWW www = new WWW(appUrl + homeFlieName);
        Debug.Log(www.url);
        yield return www;
        Debug.Log(www.text);
        JsonUtility.FromJsonOverwrite(www.text, Instance);
        SetPages();
        SetTicketPage();
        if (callback != null)
        {
            callback();
        }
    }
    void SetPages()
    {
        pages.ForEach((page) =>
        {
            if (page.Name != HomePageName && page.Name != MyTicketName)
            {
                page.Cards.Insert(0, BackPageCard);
            }
        });
    }
    void SetTicketPage()
    {
        Page page = pages.Find(p => p.Name == MyTicketName);
        if(page == null)
        {
            page = new Page();
            page.Cards = new List<Card>();
        }
        page.Cards.Clear();
        page.Cards.Add(BackPageCard);
        if (tickets != null)
        {
            if (tickets.Length > 0)
            {
                for (int i = 0; i < tickets.Length; i++)
                {
                    Card card = new Card();
                    Round round = Array.Find(rounds, r => r.id == tickets[i].roundId);
                    card.Name = round.Name;
                    card.NextPage = "Ticket" + i;
                    card.ViewType = CardType.NameWithLine;
                    page.Cards.Add(card);
                    Debug.Log(JsonUtility.ToJson(card));
                }
            }
            else
            {
                SetEmptyTicket(page.Cards);
            }
        }
        else
        {
            SetEmptyTicket(page.Cards);
        }
        page.Name = MyTicketName;
        if (!pages.Contains(page))
        {
            pages.Add(page);
        }
    }
    void SetEmptyTicket(List<Card> cards)
    {
        Card card = new Card();
        card.Name = "ไม่มีสลากอะ";
        card.ViewType = CardType.NameWithoutBG;
        cards.Add(card);
    }
    Card BackPageCard
    {
        get
        {
            Card card = new Card();
            card.Name = "< กลับ";
            card.ViewType = CardType.NameWithoutBG;
            card.NextPage = "Back";
            return card;
        }
    }
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
    //private IAmazonLambda lambdaClient;
    //public IAmazonLambda LambdaClient
    //{
    //    get
    //    {
    //        if (lambdaClient == null)
    //        {
    //            SetAWS();
    //        }
    //        return lambdaClient;
    //    }
    //}
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
            //foreach (KeyValuePair<string, string> entry in UserInfo.ActiveRecords)
            //{
            //    Debug.Log("ar " + entry.Key + " : " + entry.Value);
            //}
            //foreach (Record rec in UserInfo.Records)
            //{
            //    Debug.Log("rec " + rec.Key + " : " + rec.Value);
            //}
        }
        else
        {
            Debug.Log("No UpdateUserInfo : " + time + " : " + updateTime + updateTimeCount);
            OnSyncSuccess(string.Empty);
        }
        Save();
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
        tickets = JsonHelper.getJsonArray<Ticket>(UserInfo.Get("tickets"));
        SetTicketPage();
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
        //lambdaClient = new AmazonLambdaClient(credentials, region);
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

    #region App Service
    public void BuyRound(string round,string number,int amount)
    {
        Debug.Log("Buy :" + round + " : " + number + " : "+ amount);
    }
    #endregion
}