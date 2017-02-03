using UnityEngine;
using UnityEngine.UI;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections;
using System.IO;

[RequireComponent(typeof(Button))]
public class HomeScrollCallView : EnhancedScrollerCellView {
    const string PhotoDirName = "Photo";
    public Text NameText;
    public GameObject Mask;
    public RawImage Photo;
    public Button button;
    public Image BG;
    public Image Line;
    public delegate void GetPhoto(string name, Texture tex);
    Thread loadPhoto;
    Thread savePhoto;
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
            case CardType.NameOnly:
                SetView(false, data.Photo, false, TextAnchor.MiddleLeft);
                break;
            case CardType.NameWithBG:
                SetView(true, data.Photo, false, TextAnchor.MiddleLeft);
                break;
            case CardType.NameWithLine:
                SetView(false, data.Photo, true, TextAnchor.MiddleLeft);
                break;
            default:
                break;
        }
    }
    void SetPhoto(string name,Texture tex)
    {
        Photo.texture = tex;
    }
    void SetView(bool isBG,string photoName,bool isLine, TextAnchor nameAli)
    {
        BG.enabled = isBG;
        
        if (!string.IsNullOrEmpty(photoName))
        {
            StartCoroutine(LoadPhoto(name, SetPhoto));
            Mask.SetActive(true);
        }
        else
        {
            Mask.SetActive(false);
            Photo.texture = null;
        }
        Line.enabled = isLine;
        NameText.alignment = nameAli;
    }
    string dataDir
    {
        get
        {
            string path;
#if UNITY_EDITOR
            path = "file:" + Application.persistentDataPath;
#elif UNITY_ANDROID
         path = "jar:file://"+ Application.persistentDataPath;
#elif UNITY_IOS
         path = "file:" + Application.persistentDataPath;
#else
         //Desktop (Mac OS or Windows)
         path = "file:"+ Application.persistentDataPath;
#endif
            return path;
        }
    }
    IEnumerator LoadPhoto(string name, GetPhoto getPhoto)
    {
        string subPath = Path.Combine(PhotoDirName, name);
        string ioDir = Path.Combine(Application.persistentDataPath, PhotoDirName);
        string ioPath = Path.Combine(ioDir, name);
        string wwwfilePath = Path.Combine(dataDir, subPath);
        string url = Path.Combine(Manager.Instance.AppUrl, name);

        WWW www;
        if (!File.Exists(ioPath))
        {
            Debug.Log("not Exists : " + wwwfilePath);
            www = new WWW(url);
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                Texture2D tex = www.texture;
                //Sprite sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                getPhoto(name, www.texture);
                if (!Directory.Exists(ioDir))
                {
                    Directory.CreateDirectory(ioDir);
                }
                var save = new Thread(() => File.WriteAllBytes(Path.Combine(ioDir, name), www.bytes));
                save.Start();
            }
            else
            {
                getPhoto(name, null);
            }
        }
        else
        {
            Debug.Log("Exists : " + wwwfilePath);
            //www = new WWW(wwwfilePath);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(File.ReadAllBytes(Path.Combine(ioDir, name)));
            tex.Apply();
            getPhoto(name, tex);
            yield return null;
        }
    }
}
