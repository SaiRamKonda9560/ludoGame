using Nakama;
using TMPro;
using UnityEngine;

public class createroom : MonoBehaviour
{
    public TMP_InputField InputField;
    public nakamaClient nakamaClient;
    public TMP_Text infoTmpText;
    private void OnEnable()
    {
        setText("", Color.black, -1f);
    }
    public async void onClicked()
    {
        if (checkNakama())
        {
            if (nakamaClient.match != null || nakamaClient.matchmakerTicket != null)
            {
                try
                {
                    nakamaClient.closeMatch();
                }
                catch
                {

                }
            }

            var roomName = InputField.text;
            if (roomName == "")
            {
                setText("enter room name", Color.black, 1);
            }
            else
            {
                try
                {
                    setText("creating", Color.black, -1f);
                    var match = await nakamaClient.socket.CreateMatchAsync(roomName);
                    GUIUtility.systemCopyBuffer = (match.Id);
                    nakamaClient.joinMatch(match.Id);
                    setText("connected", Color.black, -1f);
                }
                catch
                {
                    setText("error", Color.black, 3f);
                }

            }
        }
    }
    public void setText(string text, Color color, float time)
    {
        infoTmpText.text = text;
        infoTmpText.color = color;
        if (time > 0)
        {
            Invoke("disableText", time);

        }
    }
    void disableText()
    {
        infoTmpText.text = "";
    }
    //check Nakama connection
    bool checkNakama()
    {
        if (nakamaClient)
        {
            if (nakamaClient.socket != null && nakamaClient.client != null && nakamaClient.session != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}
