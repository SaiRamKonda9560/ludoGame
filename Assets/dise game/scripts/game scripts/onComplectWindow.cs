using TMPro;
using UnityEngine;

public class onComplectWindow : MonoBehaviour
{
    public TMP_Text heading_Text;
    public nakamaClient nakamaClient;
    public gameData gameData;
    public void go(gameData gameData)
    {
        this.nakamaClient = nakamaClient;
        this.gameData = gameData;
        setData();
    }
    void setData()
    {
        if (gameData != null && nakamaClient!=null)
        {
            if (nakamaClient.session!=null)
            {
                var Username = nakamaClient.session.Username;
                foreach (var data in gameData.playersData)
                {
                    var userName = data.userName;
                    if (userName!="")
                    {
                        var isWin = data.isWin();
                        if (Username == data.userName)
                        {
                            //my user
                            if (isWin)
                            {
                                heading_Text.text = "you win !";
                            }
                            else
                            {
                                heading_Text.text = "you loose !";
                            }
                        }
                        else
                        {
                            //other user

                        }
                    }

                }
                gameObject.SetActive(true);
            }

        }
        else
        {

        }
    }
    public void goBack()
    {
        if(nakamaClient!=null)
        {
            nakamaClient.closeMatch();
        }
    }
    public void rematch()
    {
        if (nakamaClient != null)
        {
            var session = nakamaClient.session;
            var socket = nakamaClient.socket;
            var match = nakamaClient.match;
            if(match != null)
            {

            }

        }
    }

}
