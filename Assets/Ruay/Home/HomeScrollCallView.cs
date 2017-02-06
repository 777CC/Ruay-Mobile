using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.IO;
using System.Threading;

[RequireComponent(typeof(Button))]
public class HomeScrollCallView : EnhancedScrollerCellView{
    [SerializeField]
    private Text HeadText;
    [SerializeField]
    private Text NameText;
    [SerializeField]
    private GameObject Mask;
    [SerializeField]
    private RawImage photo;
    public Texture Photo
    {
        get
        {
            return photo.texture;
        }
    }
    [SerializeField]
    private Button button;
   public  Button.ButtonClickedEvent OnClick
    {
        get
        {
            return button.onClick;
        }
    }
    [SerializeField]
    private Image BG;
    [SerializeField]
    private Image Line;
    const string PhotoDirName = "Photo";
    public void SetData(Card data)
    {
        NameText.text = data.Name;
        switch (data.ViewType)
        {
            case CardType.PhotoWithName:
                SetView(true, data.Photo, false, TextAnchor.MiddleLeft);
                break;
            case CardType.PhotoOnly:
                SetView(true, data.Photo, false, TextAnchor.MiddleLeft);
                break;
            case CardType.IconWithName:
                SetView(true, data.Photo, false, TextAnchor.MiddleLeft);
                break;
            case CardType.IconOnly:
                SetView(true, data.Photo, false, TextAnchor.MiddleLeft);
                break;
            case CardType.NameOnlyLeft:
                SetView(false, data.Photo, false, TextAnchor.MiddleLeft);
                break;
            case CardType.NameOnlyCenter:
                SetView(false, data.Photo, false, TextAnchor.MiddleCenter);
                break;
            case CardType.NameOnlyRight:
                SetView(false, data.Photo, false, TextAnchor.MiddleRight);
                break;
            case CardType.NameWithBG:
                SetView(true, data.Photo, false, TextAnchor.MiddleLeft);
                break;
            case CardType.NameWithLine:
                SetView(false, data.Photo, true, TextAnchor.MiddleLeft);
                break;
            case CardType.Header:
                SetView(false, data.Photo, false, TextAnchor.MiddleLeft);
                HeadText.text = data.Name;
                NameText.text = "<";
                break;
            default:
                break;
        }
    }
    void SetPhoto(string name,Texture tex)
    {
        photo.texture = tex;
    }
    void SetView(bool isBG,string photoName,bool isLine, TextAnchor nameAli)
    {
        BG.enabled = isBG;
        if (!string.IsNullOrEmpty(photoName))
        {
            StartCoroutine(LoadPhoto(photoName, SetPhoto));
            Mask.SetActive(true);
        }
        else
        {
            photo.texture = null;
            Mask.SetActive(false);
        }
        Line.enabled = isLine;
        NameText.alignment = nameAli;
    }
    public delegate void GetPhoto(string name, Texture tex);
    string dataPath
    {
        get
        {
            string path;
#if UNITY_EDITOR
            path = "file:" + Application.persistentDataPath;
#elif UNITY_ANDROID
            path = @"file://"+  Application.persistentDataPath;
#elif UNITY_IOS
            path = "file:" + Application.persistentDataPath;
#else
            //Desktop (Mac OS or Windows)
            path = "file:"+ Application.persistentDataPath;
#endif
            return path;
        }
    }
    IEnumerator LoadPhoto(string photoName, GetPhoto getPhoto)
    {
        string subDir = Path.Combine(PhotoDirName, photoName);
        string ioDir = Path.Combine(Application.persistentDataPath, PhotoDirName);
        string ioPath = Path.Combine(ioDir, photoName);
        string wwwfilePath = dataPath + Path.DirectorySeparatorChar + PhotoDirName + Path.DirectorySeparatorChar + photoName;
        string url = Path.Combine(Manager.Instance.AppUrl, photoName);
        
        if (!File.Exists(ioPath))
        {
            Debug.Log("load web :" + url);
            WWW www = new WWW(url);
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                getPhoto(photoName, www.texture);
                if (!Directory.Exists(ioDir))
                {
                    Directory.CreateDirectory(ioDir);
                }
                Debug.Log(photoName + " " + www.texture.width + "   ");
                byte[] data = www.bytes;
                var save = new Thread(() => File.WriteAllBytes(Path.Combine(ioDir, photoName), data));
                save.Start();
            }
            else
            {
                Debug.Log(www.error);
            }
        }
        else
        {
            Debug.Log("load file :" + wwwfilePath);
            WWW www = new WWW(wwwfilePath);
            yield return www;
            getPhoto(photoName, www.texture);
        }
    }
    public void ClearImage()
    {
        StopAllCoroutines();
        photo.texture = null;
        HeadText.text = string.Empty;
        NameText.text = string.Empty;
    }
}
