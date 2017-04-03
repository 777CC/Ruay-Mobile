using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Amazon;
using Amazon.CognitoSync;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentity.Model;
using Amazon.CognitoSync.SyncManager;
using Facebook.Unity;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Threading;
using Amazon.Lambda;

public class Manager : Singleton<Manager>
//public class Manager:MonoBehaviour
{
    const string PrefsName = "UserInfo";
    protected Manager() { }
    void Start()
    {
        //PlayerPrefs.DeleteAll();
        if (Instance == this || Instance == null)
        {
            if (!string.IsNullOrEmpty(PlayerPrefs.GetString(PrefsName)))
            {
                Debug.Log("Load user info from PlayerPrefs. \n" + PlayerPrefs.GetString(PrefsName));
                JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(PrefsName), Instance);
                AdsInit();
            }
            else
            {
                UpdateAppInfo(null);
            }
            if (Instance.GetComponent<UnityInitializer>() == null)
            {
                UnityInitializer.AttachToGameObject(Instance.gameObject);
            }
            SetAWS();
#if !UNITY_EDITOR
            SceneManager.LoadScene("Login");
#endif
        }
    }
#region UserData
    private string[] UserInfoKeys = { "fbProfilePicture", "firstName", "lastName", "phoneNumber", "birthday", "gender", "zodiac", "interests" };
    public string facebookId = string.Empty;
    public int satang = 0;
    public string fbProfilePicture = string.Empty;
    public string firstName = string.Empty;
    public string lastName = string.Empty;
    public string inviteBy = string.Empty;
    public string inviteName = string.Empty;
    public double updateTime = 0;
    public int dailyRawardTime;
    public string email = string.Empty;
    public string gender = string.Empty;
    public string phoneNumber = string.Empty;
    [SerializeField]
    public int birthday = 0;
    public string interests = string.Empty;
    public int zodiac = 0;
    [SerializeField]
    private List<Ticket> tickets;
    [SerializeField]
    private List<Reward> rewards;
    public string[] Event;
#endregion

#region AppInfo
    //Cognito
    const string appInfoUrl = @"https://s3-ap-southeast-1.amazonaws.com/ruay/Appinfo.json";
    //const string info = "Appinfo.json";
    [SerializeField]
    private string appUrl = @"https://chainchoonoi.com/";
    public string AppUrl
    {
        get
        {
            return appUrl;
        }
    }
    [SerializeField]
    private string dataSetName = "Info";
    [SerializeField]
    private string homeFlieName = "home.json";
    [SerializeField]
    private string androidAdUnitId = "ca-app-pub-4416122709352572/4608987845";
    private string iosAdUnitId = "ca-app-pub-4416122709352572/4608987845";
    //const string 
    [SerializeField]
    private string identityPoolId = "ap-northeast-1:d2051844-b612-4173-a365-838a4da96036";
    [SerializeField]
    private string regionName = RegionEndpoint.APNortheast1.SystemName;
    //private int updateTimeCount = 86400000;//milliseconds in Day.
    [SerializeField]
    private double updateTimeCount = 180000;
    [SerializeField]
    private Interest[] appInterests;
    public Interest[] AppInterests
    {
        get
        {
            return appInterests;
        }
    }
    public const string HomePageName = "Home";
    const string SettingPageId = "Setting";
    const string SettingPageName = "การตั้งค่า";
    const string MyTicketsPageId = "MyTickets";
    const string MyTicketsPageName = "สลากของคุณ";
    const string MyRewardsPageId = "MyRewards";
    const string MyRewardsPageName = "ของขวัญ";
    private const float imageW = 1032f;
    private const float screenW = 1080f;
    private const float screenHRatio = 2.048f;
    int imageSizeWidth, imageSizeHeight;
    public int ImageSizeWidth
    {
        get
        {
            if (imageSizeWidth == 0)
            {
                imageSizeWidth = (int)((imageW * Screen.width) / (screenW));
            }
            return imageSizeWidth;
        }
    }
    public int ImageSizeHeight
    {
        get
        {
            if (imageSizeHeight == 0)
            {
                imageSizeHeight = (int)(ImageSizeWidth / screenHRatio);
            }
            return imageSizeHeight;
        }
    }
    [SerializeField]
    private List<Page> pages;
    [SerializeField]
    private Item[] rounds;
    [SerializeField]
    private Item[] items;
    public IEnumerator DownloadJson(string url, Action<string> callback)
    {
        WWW www = new WWW(url);
        yield return www;
        if (callback != null)
        {
            Debug.Log(www.text);
            callback(www.text);
        }
    }
    public delegate void GetPage(Page page);
    public void GetPageById(string pageId, GetPage getPage)
    {
        if (pages != null)
        {
            if (getPage != null)
            {
                getPage(pages.Find(page => page.id == pageId));
            }
        }
        else
        {
            StartCoroutine(WaitForDonwloadHomeJson(pageId, getPage));
        }
    }
    IEnumerator WaitForDonwloadHomeJson(string pageId, GetPage getPage)
    {
        yield return new WaitUntil(() => pages != null);
        getPage(pages.Find(page => page.id == pageId));
    }
    public delegate void GetItem(Item item);
    public delegate void GetTicket(Ticket ticket, Item round);
    public delegate void GetReward(Reward reward, Item item);
    public void GetRoundById(string roundId, GetItem getRound)
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
            StartCoroutine(WaitForDonwloadRounds(roundId, getRound));
        }
    }
    IEnumerator WaitForDonwloadRounds(string roundId, GetItem getItem)
    {
        yield return new WaitUntil(() => pages != null);
        getItem(Array.Find(rounds, p => p.id == roundId));
    }
    public void GetTicketByIndex(int index, GetTicket getTicket)
    {
        if (tickets != null)
        {
            if (getTicket != null && index < tickets.Count)
            {
                getTicket(tickets[index], Array.Find(rounds, r => r.id == tickets[index].roundId));
            }
        }
    }
    public void GetRewardByIndex(int Index, GetReward getReward)
    {
        if (rewards != null)
        {
            if (getReward != null && Index < rewards.Count)
            {
                getReward(rewards[Index], Array.Find(items, r => r.id == rewards[Index].itemId));
            }
        }
    }
    public void GetItemById(string id, GetItem getItem)
    {
        if (items != null)
        {
            if (getItem != null)
            {
                getItem(Array.Find(items, p => p.id == id));
            }
        }
        else
        {
            StartCoroutine(WaitForDonwloadItems(id, getItem));
        }
    }
    IEnumerator WaitForDonwloadItems(string id, GetItem getItem)
    {
        yield return new WaitUntil(() => pages != null);
        getItem(Array.Find(items, p => p.id == id));
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
            for (int i = 0; i < UserInfoKeys.Length; i++)
            {
                string val = GetUserInfo(UserInfoKeys[i]);
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
        StartCoroutine(DownloadJson(appInfoUrl, (data)=> {
            try
            {
                Debug.Log("Update appinfo : " + data);
                JsonUtility.FromJsonOverwrite(data, Instance);
                SetAWS();
                Save();
            }
            catch (Exception e)
            {
                Debug.Log("cannot update appinfo : " + e);
            }
            if (callback != null)
            {
                callback();
            }
        }));
        //StartCoroutine(DownloadAppInfo(callback));

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
    public void ClearUserInfo()
    {
        FB.LogOut();
        PlayerPrefs.DeleteAll();
        Caching.CleanCache();
        Manager newManager = new Manager();
        JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(newManager), Instance);
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
        StartCoroutine(DownloadJson(appUrl + homeFlieName, (data) => {
            try
            {
                JsonUtility.FromJsonOverwrite(data, Instance);
                SetPages();
                SetSettingPage();
                SetMyTicketsPage();
                SetMyRewardsPage();
                Save();
                if (callback != null)
                {
                    callback();
                }
            }
            catch
            {
                Debug.Log("cannot update appinfo");
            }
        }));
    }
    void SetPages()
    {
        double timeNow = GetUnixTime();
        pages.ForEach((page) =>
        {
            //Debug.Log(JsonUtility.ToJson(page));
            page.cards.ForEach((card) => {
                //Debug.Log(JsonUtility.ToJson(card));
                if (card.startTime != 0 && card.endTime != 0)
                {
                    if (card.startTime > timeNow || card.endTime < timeNow)
                    {
                        page.cards.Remove(card);
                    }
                }
            });
            if (page.id == HomePageName)
            {
                Card ad = new Card();
                ad.viewType = CardType.Ad;
                int i = 0;
                page.cards.Insert(i, ad);
            }
            //if (page.name != HomePageName && page.name != MyTicketsName && page.name != MyRewardsName)
            if (page.id != MyTicketsPageId && page.id != MyRewardsPageId)
            {
                page.cards.Insert(0, BackPageCard(page.title));
            }
        });
    }
    public void SetSettingPage()
    {
        Page page = pages.Find(p => p.id == SettingPageId);
        if (page == null)
        {
            page = new Page();
        }
        page.id = SettingPageId;
        page.title = SettingPageName;
        page.cards = new List<Card>();
        page.cards.Add(BackPageCard(page.title));
        Card userInfoCard = new Card();
        userInfoCard.title = "แก้ไขข้อมูลของคุณ";
        userInfoCard.actionType = "UserInfo";
        userInfoCard.viewType = CardType.NameWithBGCenter;
        page.cards.Add(userInfoCard);
        //invite head card.
        Card inviteHeadCard = new Card();
        inviteHeadCard.title = "ชื่อผู้เชิญของคุณ :";
        inviteHeadCard.viewType = CardType.NameOnlyLeft;
        page.cards.Add(inviteHeadCard);
        //invite name card.
        Card inviteValCard = new Card();
        if (string.IsNullOrEmpty(inviteName))
        {
            inviteValCard.title = "ตั้งชื่อผู้เชิญ กดแก้ไข";
        }
        else
        {
            inviteValCard.title = inviteName;
        }
        inviteValCard.viewType = CardType.NameOnlyCenter;
        page.cards.Add(inviteValCard);
        //invite name change card.
        Card inviteChangeCard = new Card();
        inviteChangeCard.title = "แก้ไข";
        inviteChangeCard.actionType = "SetInviteName";
        inviteChangeCard.viewType = CardType.NameWithBGCenter;
        page.cards.Add(inviteChangeCard);
        page.cards.Add(BackPageCard(string.Empty));
        page.cards.Add(BackPageCard(string.Empty));
        //Quit card.
        Card quitCard = new Card();
        quitCard.title = "ออกจากระบบ";
        quitCard.actionType = "Quit";
        quitCard.viewType = CardType.NameWithBGCenter;
        page.cards.Add(quitCard);
        if (!pages.Contains(page))
        {
            pages.Add(page);
        }
    }
    void SetMyTicketsPage()
    {
        Page page = pages.Find(p => p.id == MyTicketsPageId);
        if (page == null)
        {
            page = new Page();
        }
        page.id = MyTicketsPageId;
        page.title = MyTicketsPageName;
        page.cards = new List<Card>();
        page.cards.Add(BackPageCard(page.title));
        if (tickets != null)
        {
            if (tickets.Count > 0)
            {
                for (int i = 0; i < tickets.Count; i++)
                {
                    Card card = new Card();
                    Item round = Array.Find(rounds, r => r.id == tickets[i].roundId);
                    //card.name = round.name;
                    if (round.choices != null)
                    {
                        Choice choice = Array.Find(round.choices, c => c.choiceValue == tickets[i].reserveNumber);
                        if (!string.IsNullOrEmpty(choice.choiceName))
                        {
                            card.title = choice.choiceName;
                        }
                        else
                        {
                            card.title = tickets[i].reserveNumber.ToString();
                        }
                    }
                    else
                    {
                        card.title = tickets[i].reserveNumber.ToString();
                    }
                    card.title += " " + (string.IsNullOrEmpty(tickets[i].announced) ? "ยังไม่ประกาศ" : tickets[i].announced);
                    card.actionType = "Ticket";
                    card.actionValue = i.ToString();
                    card.viewType = CardType.NameWithLine;
                    page.cards.Add(card);
                }
            }
            else
            {
                SetEmptyTicket(page.cards);
            }
        }
        else
        {
            SetEmptyTicket(page.cards);
        }
        if (!pages.Contains(page))
        {
            pages.Add(page);
        }
    }
    void SetEmptyTicket(List<Card> cards)
    {
        Card card = new Card();
        card.title = "ไม่มีอะ ลองแลกสลากดูซิ";
        card.viewType = CardType.NameOnlyRight;
        cards.Add(card);
    }
    void SetMyRewardsPage()
    {
        Page page = pages.Find(p => p.id == MyRewardsPageId);
        if (page == null)
        {
            page = new Page();
        }
        page.id = MyRewardsPageId;
        page.title = MyRewardsPageName;
        page.cards = new List<Card>();
        page.cards.Add(BackPageCard(page.title));
        if (rewards != null)
        {
            if (rewards.Count > 0)
            {
                for (int i = 0; i < rewards.Count; i++)
                {
                    Card card = new Card();
                    Item round = Array.Find(items, r => r.id == rewards[i].itemId);
                    card.title = round.title;
                    card.actionType = "Reward";
                    card.actionValue = i.ToString();
                    card.viewType = CardType.NameWithLine;
                    page.cards.Add(card);
                }
            }
            else
            {
                SetEmptyReward(page.cards);
            }
        }
        else
        {
            SetEmptyReward(page.cards);
        }
        if (!pages.Contains(page))
        {
            pages.Add(page);
        }
    }
    void SetEmptyReward(List<Card> cards)
    {
        Card card = new Card();
        card.title = "ไม่มีอะ ลองแลกของขวัญดูน่ะ";
        card.viewType = CardType.NameOnlyRight;
        cards.Add(card);
    }
    Card BackPageCard(string pageName)
    {
        Card card = new Card();
        card.title = pageName;
        card.viewType = CardType.Header;
        card.actionType = string.Empty;
        card.actionValue = string.Empty;
        return card;
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
    public void LoginCognitoWithFacebook()
    {
        Credentials.AddLogin("graph.facebook.com", AccessToken.CurrentAccessToken.TokenString);
        UpdateUserInfo(true);
    }
    public void UpdateUserInfo(bool isCheckTime)
    {
        double time = GetUnixTime();
        if(!isCheckTime || (updateTime + updateTimeCount < time) || (updateTime - updateTimeCount > time))
        {
            Debug.Log("UpdateUserInfo : " + time + " : " + ((updateTime + updateTimeCount) - time));
            Debug.Log("tel " + phoneNumber + " + " + birthday);
            PutToDataset("updateTime", time.ToString("0"));
            PutToDataset("facebookId", facebookId);
            PutToDataset("fbProfilePicture", fbProfilePicture);
            PutToDataset("firstName", firstName);
            PutToDataset("lastName", lastName);
            PutToDataset("phoneNumber", phoneNumber);
            PutToDataset("inviteBy", inviteBy);
            PutToDataset("birthday", birthday.ToString());
            PutToDataset("gender", gender);
            PutToDataset("zodiac", zodiac.ToString());
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
            HandleSyncSuccess(null, null);
        }
        Save();
    }
    public void PutToDataset(string key, string val)
    {
        if (!string.IsNullOrEmpty(val)) {
            UserInfo.Put(key, val);
        }
    }
    private void HandleSyncSuccess(object sender, SyncSuccessEventArgs e)
    {
        if (e != null)
        {
            updateTime = GetUnixTime();
        }
        int s;
        if (int.TryParse(GetUserInfo("satang"), out s))
        {
            satang = s;
        }
        fbProfilePicture = GetUserInfo("fbProfilePicture");
        firstName = GetUserInfo("firstName");
        lastName = GetUserInfo("lastName");
        gender = GetUserInfo("gender");
        phoneNumber = GetUserInfo("phoneNumber");
        Debug.Log("inviteName : " + GetUserInfo("inviteName") + " inviteBy : " + GetUserInfo("inviteBy"));
        inviteBy = GetUserInfo("inviteBy");
        inviteName = GetUserInfo("inviteName");
        int date;
        if (int.TryParse(GetUserInfo("birthday"), out date))
        {
            birthday = date;
        }
        int zo;
        if (int.TryParse(GetUserInfo("zodiac"), out zo))
        {
            zodiac = zo;
        }
        interests = GetUserInfo("interests");
        Ticket[] tk = JsonHelper.getJsonArray<Ticket>(GetUserInfo("tickets"));
        Reward[] rw = JsonHelper.getJsonArray<Reward>(GetUserInfo("rewards"));
        if (tk != null)
        {
            tickets = new List<Ticket>(tk);
        }
        if (rw != null)
        {
            rewards = new List<Reward>(rw);
        }
        Save();
        SetSettingPage();
        SetMyTicketsPage();
        SetMyRewardsPage();
        if (OnSyncSuccess != null)
        {
            OnSyncSuccess(string.Empty);
        }
        //AdsInit();
    }
    private void HandleSyncFailure(object sender, SyncFailureEventArgs e)
    {
        Debug.Log("HandleSyncFailure" + e.Exception.ToString());
        if (OnSyncFailure != null)
        {
            OnSyncFailure(e.Exception.ToString());
        }
    }
    string GetUserInfo(string name)
    {
        if (UserInfo.ActiveRecords.ContainsKey(name))
        {
            Record rec = UserInfo.GetRecord(name);
            if (!rec.IsDeleted) {
                return rec.Value;
            }
        }
        return string.Empty;
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
        if (credentials == null)
        {
            credentials = new CognitoAWSCredentials(identityPoolId, region);
        }
        else
        {
            if(credentials.IdentityPoolId != identityPoolId)
            {
                credentials = new CognitoAWSCredentials(identityPoolId, region);
            }
        }
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

    public void DialogPopup(string header, string desc, Action onAccept, Action onCancel)
    {
        DialogPopup popup = Instantiate(Resources.Load<GameObject>("DialogPopup"), FindObjectOfType<Canvas>().transform, false).GetComponent<DialogPopup>();
        popup.SetDialog(header, desc, onAccept, onCancel);
    }
    public int GetRoundAmountById(string id)
    {
        int amount = 0;
        if (tickets != null)
        {
            tickets.FindAll((tk) => tk.roundId == id).ForEach((tk) =>
            {
                amount += tk.amount;
            });
        }
        return amount;
    }
#region App Service
    public void BuyRound(string id, int number, int amount, Action onSuccess)
    {
        LambdaBuyTicket ticket = new LambdaBuyTicket();
        ticket.roundId = id;
        ticket.reserveNumber = number;
        ticket.amount = amount;
        LambdaClient.InvokeAsync(new Amazon.Lambda.Model.InvokeRequest()
        {
            FunctionName = "BuyTicket",
            Payload = JsonUtility.ToJson(ticket)
        },
        (responseObject) =>
        {
            if (responseObject.Exception == null)
            {
                string res = Encoding.ASCII.GetString(responseObject.Response.Payload.ToArray());
                Ticket newTicket = JsonUtility.FromJson<Ticket>(res);
                if (!string.IsNullOrEmpty(newTicket.roundId))
                {
                    tickets.Add(newTicket);
                    SetMyTicketsPage();
                    satang -= Array.Find(rounds, (r) => r.id == newTicket.roundId).price * newTicket.amount;
                    Save();
                    if (onSuccess != null)
                    {
                        onSuccess();
                    }
                    DialogPopup("รับสลากเรียบร้อย", "ดูได้ที่หน้าสลากของคุณ", null, null);
                }
                else
                {
                    DialogPopup("ไม่สามารถรับได้", res, null, null);
                }
            }
            else
            {
                DialogPopup("ไม่สามารถรับได้", responseObject.Exception.ToString(), null, null);
            }
        });
    }

    public void BuyItem(string id, int choice, int amount, Action onSuccess)
    {
        LambdaBuyReward reward = new LambdaBuyReward();
        reward.itemId = id;
        reward.choice = choice;
        reward.amount = amount;
        Debug.Log(JsonUtility.ToJson(reward));
        LambdaClient.InvokeAsync(new Amazon.Lambda.Model.InvokeRequest()
        {
            FunctionName = "BuyReward",
            Payload = JsonUtility.ToJson(reward)
        },
        (responseObject) =>
        {
            if (responseObject.Exception == null)
            {
                string res = Encoding.ASCII.GetString(responseObject.Response.Payload.ToArray());
                Reward newReward = JsonUtility.FromJson<Reward>(res);
                if (!string.IsNullOrEmpty(newReward.itemId))
                {
                    rewards.Add(newReward);
                    SetMyRewardsPage();

                    satang -= Array.Find(rounds, (r) => r.id == newReward.itemId).price * newReward.amount;
                    Save();
                    if (onSuccess != null)
                    {
                        onSuccess();
                    }
                    DialogPopup("แลกของขวัญเรียบร้อย", "สามารถดูได้ที่หน้าของขวัญ", null, null);
                }
                else
                {
                    DialogPopup("ไม่สามารถรับได้", res, null, null);
                }
            }
            else
            {
                DialogPopup("ไม่สามารถรับได้", responseObject.Exception.ToString(), null, null);
            }
        });
    }
#endregion

#region Ads
    GoogleMobileAdBanner adsBanner;
    void AdsInit()
    {
        Debug.Log("AdsInit :" + Instance.androidAdUnitId);
#if UNITY_ANDROID
        GoogleMobileAdSettings.Instance.Android_BannersUnitId = androidAdUnitId;
#elif UNITY_IOS
        GoogleMobileAdSettings.Instance.IOS_BannersUnitId = iosAdUnitId;
#endif

        //Required
        GoogleMobileAd.Init();

        //Optional, add data for better ad targeting
        if (gender == "male")
        {
            GoogleMobileAd.SetGender(GoogleGender.Male);
        }else if(gender == "female")
        {
            GoogleMobileAd.SetGender(GoogleGender.Female);
        }
        else
        {
            GoogleMobileAd.SetGender(GoogleGender.Unknown);
        }
        Array.ForEach<string>(interests.Split('#'), (keyword) => {
            if (string.IsNullOrEmpty(keyword))
            {
                GoogleMobileAd.AddKeyword(keyword);
            }
        });
        var day = FromUnixTime(birthday);
        GoogleMobileAd.SetBirthday(day.Year,(AndroidMonth)(day.Month - 1), day.Day);
        GoogleMobileAd.TagForChildDirectedTreatment(false);

        adsBanner = GoogleMobileAd.CreateAdBanner(0,0, GADBannerSize.MEDIUM_RECTANGLE);

        //listening for banner to load example using C# actions:
        //banner2.OnLoadedAction += OnBannerLoadedAction;
        adsBanner.ShowOnLoad = false;
    }
    public void ShowAd(Rect rect,Action<GoogleMobileAdBanner> onComplete)
    {
        if (adsBanner != null)
        {
            if (adsBanner.IsLoaded)
            {
                adsBanner.Show();
                adsBanner.SetBannerPosition((int)(rect.x + ((rect.width - adsBanner.width) / 2)), (int)(rect.y + ((rect.height - adsBanner.height) / 2)));
                if(onComplete!= null)
                {
                    onComplete(adsBanner);
                }
            }
            else
            {
                //AdsInit();
                adsBanner.ShowOnLoad = true;
                adsBanner.OnLoadedAction += onComplete;
            }
        }
        else
        {
            AdsInit();
            adsBanner.ShowOnLoad = true;
            adsBanner.OnLoadedAction += onComplete;
        }
    }

    public void HideAd()
    {
        if (adsBanner != null)
        {
            adsBanner.Hide();
        }
    }
    public void SetAdPosition(Rect rect)
    {
        if (adsBanner != null)
        {
            adsBanner.SetBannerPosition((int)(rect.x + ((rect.width - adsBanner.width) /2)), (int)(rect.y + ((rect.height - adsBanner.height) / 2)));
        }
    }
    private DateTime FromUnixTime(long unixTime)
    {
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return epoch.AddMilliseconds(unixTime);
    }
#endregion
}