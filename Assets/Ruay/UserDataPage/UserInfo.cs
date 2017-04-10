using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Amazon.CognitoSync.SyncManager;
public class UserInfo : MonoBehaviour
{
    public RectTransform FormRect;
    public GameObject Loading;
    public Animator FormAnimator;
    public Button Next;
    const string NextStr = "Next";
    public Button Back;
    const string BackStr = "Back";


    [SerializeField]
    InputField firstnameField;
    [SerializeField]
    InputField lastnameField;
    [SerializeField]
    InputField inviteByField;
    [SerializeField]
    InputField phoneNumberField;

    [SerializeField]
    Toggle maleToggle;
    [SerializeField]
    Toggle femaleToggle;
    [SerializeField]
    Toggle AnotherToggle;
    [SerializeField]
    InputField genderField;
    [SerializeField]
    InterestController interestController;
    private int pageIndex = 1;
    private void Start()
    {
        firstnameField.text = Manager.Instance.firstName;
        lastnameField.text = Manager.Instance.lastName;
        phoneNumberField.text = Manager.Instance.phoneNumber;

        if (!string.IsNullOrEmpty(Manager.Instance.inviteBy))
        {
            inviteByField.text = Manager.Instance.inviteBy;
            inviteByField.readOnly = true;
        }
        else
        {
            inviteByField.onValueChanged.AddListener(((text) => {
                Manager.Instance.inviteBy = text;
            }));
        }
        if (Manager.Instance.gender == "male")
        {
            maleToggle.isOn = true;
        }
        else if (Manager.Instance.gender == "female")
        {
            femaleToggle.isOn = true;
        }
        else if (!string.IsNullOrEmpty(Manager.Instance.gender))
        {
            Debug.Log("gender : " + Manager.Instance.gender);
            AnotherToggle.isOn = true;
            AnotherToggle.Select();
            genderField.text = Manager.Instance.gender;
        }
        if (!string.IsNullOrEmpty(Manager.Instance.interests))
        {
            Manager.Instance.UpdateAppInfo(() =>
            {
                string[] selecteds = Manager.Instance.interests.Split('#');
                for (int i = 0; i < selecteds.Length; i++)
                {
                    Debug.Log(selecteds[i]);
                    if (selecteds[i].Length > 0)
                    {
                        interestController.userInterests.Add(selecteds[i]);
                    }
                }
                if (interestController.userInterests.Count > 0)
                {
                    interestController.Reload(Manager.Instance.AppInterests);
                }
            });
        }
        else
        {
            Manager.Instance.UpdateAppInfo(() =>
            {
                if(Manager.Instance.AppInterests != null)
                {
                    Debug.Log("AppInterests : " + Manager.Instance.AppInterests.Length);
                    interestController.Reload(Manager.Instance.AppInterests);
                }
            });
        }
        firstnameField.onValueChanged.AddListener(((text) => {
            Manager.Instance.firstName = text;
        }));
        lastnameField.onValueChanged.AddListener(((text) => {
            Manager.Instance.lastName = text;
        }));
        phoneNumberField.onValueChanged.AddListener(((text) => {
            Manager.Instance.phoneNumber = text;
        }));
        
    }
    public void NextMove()
    {
        //Next.interactable = false;
        //Back.interactable = false;
        LeanTween.moveX(FormRect, FormRect.anchoredPosition.x - 1080, 0.25f).setEaseInOutSine().setOnComplete(() => {
            // Next.interactable = true;
            // Back.interactable = true;
        });
    }
    public void BackMove()
    {
        Next.interactable = false;
        Back.interactable = false;
        LeanTween.moveX(FormRect, FormRect.anchoredPosition.x + 1080, 0.25f).setEaseInOutSine().setOnComplete(() => {
            Next.interactable = true;
            Back.interactable = true;
        });
    }
    void NamePageValidtation()
    {
        bool isValid = true;
        string message = string.Empty;
        if (Manager.Instance.firstName == string.Empty)
        {
            message += "-กรุณาใส่ชื่อ\n";
            isValid = false;
        }
        if (string.IsNullOrEmpty(Manager.Instance.lastName))
        {
            message += "-กรุณาใส่นามสกุล\n";
            isValid = false;
        }
        if (string.IsNullOrEmpty(Manager.Instance.phoneNumber))
        {
            message += "-กรุณาใส่เบอร์มือถือ\n";
            isValid = false;
        }

        if (isValid)
        {
            Validated();
        }
        else
        {
            Manager.Instance.DialogPopup("", message, null, null);
        }
    }
    void BirthDayValidtation()
    {
        if (Manager.Instance.birthday <= 0)
        {
            Manager.Instance.DialogPopup("", "-กรุณาใส่วันเกิด\n", null, null);
        }
        else
        {
            Validated();
        }
    }
    void GenderValidation()
    {
        if (string.IsNullOrEmpty( Manager.Instance.gender))
        {
            Manager.Instance.DialogPopup("", "-กรุณาใส่เพศ\n", null, null);
        }
        else
        {
            Validated();
        }
    }
    void InterestValidation()
    {
        bool isValid = true;
        string message = string.Empty;
        if (interestController.userInterests.Count < 3)
        {
            message += "-กรุณาเลือกสิ่งที่สนใจอย่างน้อย 3 อย่าง";
            isValid = false;
        }
        if (isValid)
        {
            SetInterest(interestsToString(interestController.userInterests));
            Loading.gameObject.SetActive(true);
            Manager.Instance.OnSyncSuccess = HandleSyncSuccess;
            Manager.Instance.OnSyncFailure = HandleSyncFailure;
            Manager.Instance.UpdateUserInfo(false);
        }
        else
        {
            Manager.Instance.DialogPopup("", message, null, null);
        }
    }
    private string interestsToString(List<string> names)
    {
        string str = string.Empty;
        names.ForEach(delegate (string name)
        {
            str += "#" + name;
        });
        return str;
    }
    private void Validated()
    {
        pageIndex++;
        NextMove();
        //FormAnimator.SetTrigger(NextStr);
    }
    private void HandleSyncSuccess(string e)
    {
        Manager.Instance.DownloadHomeJson(() => {
            SceneManager.LoadScene("Home");
        });
    }
    private void HandleSyncFailure(string exception)
    {
        Manager.Instance.DialogPopup("", "กรุณาลองใหม่อีกครั้ง\nสามารถแคปหน้าจอส่งพนักงานได้\n" + exception, () => {
            SceneManager.LoadScene("Login");
        }, null);
    }
    public void NextPage()
    {
        switch(pageIndex)
        {
            case 1:
                NamePageValidtation();
                break;
            case 2:
                BirthDayValidtation();
                break;
            case 3:
                GenderValidation();
                break;
            case 4:
                InterestValidation();
                break;
        }
        Manager.Instance.Save();
    }
    public void BackPage()
    {
        if(pageIndex == 1)
        {
            BackToLogin();
        }
        else
        {
            BackMove();
        }
        pageIndex--;
        Debug.Log(pageIndex);
    }
    public void BackToLogin()
    {
        SceneManager.LoadScene("Login");
    }
    public void SetBirthday(int data)
    {
        Manager.Instance.birthday = data;
    }
    public void SetGender(string data)
    {
        Manager.Instance.gender = data;
    }
    private void SetInterest(string data)
    {
        Manager.Instance.interests = data;
    }
}
