using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class UserData : MonoBehaviour {
    public GameObject Popup;
    public GameObject Loading;
    public Text PopupText;
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
    InterestingsController InterestingsController;
    void Start()
    {
        nameField.text = Manager.Instance.firstName;
        lastnameField.text = Manager.Instance.lastName;
        if (Manager.Instance.tel != 0)
        {
            telField.text = Manager.Instance.tel.ToString("0000000000");
        }
        inviteByField.text = Manager.Instance.inviteBy;
        if(Manager.Instance.gender == "ชาย")
        {
            maleToggle.isOn = true;
        }
        else if (Manager.Instance.gender == "หญิง")
        {
            femaleToggle.isOn = true;
        }
        else if(Manager.Instance.gender != string.Empty)
        {
            AnotherToggle.isOn = true;
            AnotherToggle.Select();
            genderField.text = Manager.Instance.gender;
        }
    }
    void NamePageValidtation()
    {
        bool isValid = true;
        PopupText.text = string.Empty;
        if (Manager.Instance.firstName == string.Empty)
        {
            PopupText.text += "-กรุณาใส่ชื่อ\n";
            isValid = false;
        }
        if (Manager.Instance.lastName == string.Empty)
        {
            Debug.Log("last");
            PopupText.text += "-กรุณาใส่นามสกุล\n";
            isValid = false;
        }
        if (Manager.Instance.tel == 0)
        {
            Debug.Log("last");
            PopupText.text += "-กรุณาใส่เบอร์มือถือ\n";
            isValid = false;
        }

        if (isValid)
        {
            Validated();
        }
        else
        {
            Popup.gameObject.SetActive(true);
        }
    }
    public void GenderValidation()
    {
        bool isValid = true;
        PopupText.text = string.Empty;
        if (Manager.Instance.gender == string.Empty)
        {
            PopupText.text += "-กรุณาใส่เพศ\n";
            isValid = false;
        }
        if (isValid)
        {
            Validated();
        }
        else
        {
            Popup.gameObject.SetActive(true);
        }
    }
    void InterestingsValidation()
    {
        bool isValid = true;
        PopupText.text = string.Empty;
        if (InterestingsController.userInterestings.Count < 3)
        {
            PopupText.text += "-กรุณาเลือกสิ่งที่สนใจอย่างน้อย 3 อย่าง";
            isValid = false;
        }
        if (isValid)
        {
            Manager.Instance.interestings = InterestingsToString(InterestingsController.userInterestings);
            Loading.gameObject.SetActive(true);
            //Validated();
        }
        else
        {
            Popup.gameObject.SetActive(true);
        }
    }
    private string InterestingsToString(List<string> names)
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
    public void NextPage()
    {
        //Debug.Log(AllPageAni.GetCurrentAnimatorStateInfo(0).);
        if (AllPageAni.GetCurrentAnimatorStateInfo(0).IsName("Base.NamePage"))
        {
            NamePageValidtation();
        }
        else if (AllPageAni.GetCurrentAnimatorStateInfo(0).IsName("Base.Gender"))
        {
            GenderValidation();
        }
        else if (AllPageAni.GetCurrentAnimatorStateInfo(0).IsName("Base.Interestings"))
        {
            InterestingsValidation();
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
    public void SetInteresting(string data)
    {
        Manager.Instance.interestings = data;
    }
    public void AddInteresting(string data)
    {
        Manager.Instance.interestings += "#" + data;
    }
    public void RemoveInteresting(string data)
    {
        Manager.Instance.interestings = Manager.Instance.interestings.Replace("#" + data, "");
    }
}
