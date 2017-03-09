using System.Text;
using UnityEngine;
using UnityEngine.UI;
public class SetInviteNamePopup : Popup
{
    [SerializeField]
    private InputField inviteField;
    [SerializeField]
    private Button summitButton;
    private LambdaSetInviteName payloadObj;
    public override void Start()
    {
        base.Start();
        payloadObj = new LambdaSetInviteName();
        payloadObj.oldInviteName = Manager.Instance.inviteName;
        inviteField.text = Manager.Instance.inviteName;
        inviteField.onEndEdit.AddListener((text) => {
            Manager.Instance.inviteName = text;
            summitButton.interactable = !string.IsNullOrEmpty(text) && payloadObj.oldInviteName != text;
        });
        summitButton.onClick.AddListener(() =>
        {
            SetInviteName();
        });
        Show();
    }
    public void SetInviteName()
    {
        payloadObj.newInviteName = inviteField.text;
        Manager.Instance.LambdaClient.InvokeAsync(new Amazon.Lambda.Model.InvokeRequest()
        {
            FunctionName = "SetInviteName",
            Payload = JsonUtility.ToJson(payloadObj)
        },
        (responseObject) =>
        {
            if (responseObject.Exception == null)
            {
                string res = Encoding.ASCII.GetString(responseObject.Response.Payload.ToArray());
                Manager.Instance.inviteName = payloadObj.newInviteName;
                Manager.Instance.Save();
                Manager.Instance.SetSettingPage();
                Manager.Instance.DialogPopup("เรียบร้อย :)", payloadObj.newInviteName, null, null);
            }
            else
            {
                Manager.Instance.DialogPopup("ไม่สำเร็จ", "ชื่อซ้ำ ลองใช้ชื่ออื่นน่ะ", null, null);
            }
        });
    }
}
