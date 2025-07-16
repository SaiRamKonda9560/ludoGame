
using TMPro;
using UnityEngine;

public class messageToUser : MonoBehaviour
{
    // Start is called before the first frame update
    public static messageToUser mainMessageToUser;
    public TMP_Text TMP_TextForMessage;
    string message;
    float endTime=0f;
    public Color textColor = Color.white;
    void Start()
    {
        
    }

    // Update is called once per frame

    void FixedUpdate()
    {
        mainMessageToUser = this;
        var timePassed=endTime-Time.unscaledTime;
        if(timePassed<=0)
        {
            //end 
            TMP_TextForMessage.enabled=(false);

        }
        else
        {
            //start
            if (TMP_TextForMessage)
            {
                TMP_TextForMessage.enabled=(true);
                TMP_TextForMessage.text=message;
                TMP_TextForMessage.color= textColor;

            }

        }

    }

    public static void printMessage(string message)
    {
        printMessage(message,4);
    }
    public static void printMessage(string message, float time)
    {
        if(Time.unscaledTime < mainMessageToUser.endTime)
        {
            mainMessageToUser.endTime = Time.unscaledTime + time;
            var newLineCount = 0;
            foreach(var chear in mainMessageToUser.message.ToCharArray())
            {
                if (chear == '\n')
                {
                    newLineCount++;
                }
            }
            mainMessageToUser.message += ("\n"+message);
        }
        else
        {
            mainMessageToUser.endTime = Time.unscaledTime + time;
            mainMessageToUser.message = message;
        }

    }
    public static void printMessageNew(string message, float time = 1.5f)
    {
        mainMessageToUser.endTime = Time.unscaledTime + time;
        mainMessageToUser.message = message;
    }
    public static void messagee(string message, float time = 1.5f)
    {
        var messageToUser = (messageToUser)GameObject.FindFirstObjectByType(typeof(messageToUser));
        if(messageToUser != null )
        {
            messageToUser.printMessage(message, time);

        }
    }

}
