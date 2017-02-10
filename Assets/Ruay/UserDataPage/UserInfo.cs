using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Amazon.CognitoSync.SyncManager;

public class UserInfo : MonoBehaviour
{
    public GameObject Loading;
    public Animator AllPageAni;
    public Button Next;
    const string NextStr = "Next";
    public Button Back;
    const string BackStr = "Back";


    [SerializeField]
    InputField nameField;
    [SerializeField]
    InputField lastnameField;
    [SerializeField]
    InputField inviteByField;
    [SerializeField]
    InputField telField;

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
    void Start()
    {
        nameField.text = Manager.Instance.firstName;
        lastnameField.text = Manager.Instance.lastName;
        if (Manager.Instance.tel != 0)
        {
            telField.text = Manager.Instance.tel.ToString("0000000000");
        }
        inviteByField.text = Manager.Instance.inviteBy;
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
            string[] selecteds = Manager.Instance.interests.Split('#');
            for (int i = 0; i < selecteds.Length; i++)
            {
                Debug.Log(selecteds[i]);
                if (selecteds[i].Length >0)
                {
                    interestController.userInterests.Add(selecteds[i]);
                }
            }
        }
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
        if (Manager.Instance.lastName == string.Empty)
        {
            message += "-กรุณาใส่นามสกุล\n";
            isValid = false;
        }
        if (Manager.Instance.tel == 0)
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
        bool isValid = true;
        string message = string.Empty;
        if (Manager.Instance.birthday == 0)
        {
            message += "-กรุณาใส่วันเกิด\n";
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
    void GenderValidation()
    {
        bool isValid = true;
        string message = string.Empty;
        if (Manager.Instance.gender == string.Empty)
        {
            message += "-กรุณาใส่เพศ\n";
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
            SetInterest(InterestsToString(interestController.userInterests));
            Loading.gameObject.SetActive(true);
            Manager.Instance.OnSyncSuccess = HandleSyncSuccess;
            Manager.Instance.OnSyncFailure = HandleSyncFailure;
            Manager.Instance.UpdateUserInfo();
        }
        else
        {
            Manager.Instance.DialogPopup("", message, null, null);
        }
    }
    private string InterestsToString(List<string> names)
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
        AllPageAni.SetTrigger(NextStr);
    }
    private void HandleSyncSuccess(string e)
    {
        SceneManager.LoadScene("Home");
    }
    private void HandleSyncFailure(string exception)
    {
        Manager.Instance.DialogPopup("", "กรุณาลองใหม่อีกครั้ง\nสามารถแคปหน้าจอส่งพนักงานได้\n" + exception, () => {
            SceneManager.LoadScene("Login");
        }, null);
    }
    public void NextPage()
    {
        //Debug.Log(AllPageAni.GetCurrentAnimatorStateInfo(0).);
        if (AllPageAni.GetCurrentAnimatorStateInfo(0).IsName("Base.NamePage"))
        {
            NamePageValidtation();
        }
        else if (AllPageAni.GetCurrentAnimatorStateInfo(0).IsName("Base.BirthDay"))
        {
            BirthDayValidtation();
        }
        else if (AllPageAni.GetCurrentAnimatorStateInfo(0).IsName("Base.Gender"))
        {
            GenderValidation();
        }
        else if (AllPageAni.GetCurrentAnimatorStateInfo(0).IsName("Base.Interest"))
        {
            InterestValidation();
        }
        else
        {
            AllPageAni.SetTrigger(NextStr);
        }
        Manager.Instance.Save();
    }
    public void BackPage()
    {
        if (AllPageAni.GetCurrentAnimatorStateInfo(0).IsName("NamePage"))
        {
            BackToLogin();
        }
        else
        {
            AllPageAni.SetTrigger(BackStr);
        }
    }
    public void BackToLogin()
    {
        SceneManager.LoadScene("Login");
    }
    public void SetFirstName(string data)
    {
        Manager.Instance.firstName = data;
    }
    public void SetLastName(string data)
    {
        Manager.Instance.lastName = data;
    }
    public void SetInviteBy(string name)
    {
        Manager.Instance.inviteBy = name;
    }
    public void SetBirthday(int data)
    {
        Manager.Instance.birthday = data;
    }
    public void SetGender(string data)
    {
        Manager.Instance.gender = data;
    }
    public void SetTel(string data)
    {
        int number;
        if (int.TryParse(data, out number))
        {
            Manager.Instance.tel = number;
        }
    }
    private void SetInterest(string data)
    {
        Manager.Instance.interests = data;
    }
}
