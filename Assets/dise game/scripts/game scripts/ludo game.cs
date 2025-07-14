using Nakama;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ludogame : MonoBehaviour
{
    [Header("refferenc")]
    public List<path> path = new List<path>();
    public pawn pawnPrefab;
    public ludoplaceholder ludoplaceholderBlue;
    public ludoplaceholder ludoplaceholderRed;
    public ludoplaceholder ludoplaceholderGreen;
    public ludoplaceholder ludoplaceholderYellow;
    public dice dice;
    public nakamaClient nakamaClient;
    public bool isAnyPawnMoving
    {
        get
        {
            return (ludoplaceholderBlue.isAnyPawnMoving()|| ludoplaceholderRed.isAnyPawnMoving()|| ludoplaceholderGreen.isAnyPawnMoving()|| ludoplaceholderYellow.isAnyPawnMoving());
        }
    }
    [ContextMenu("names")]
    public void setNames()
    {
        int t = 0;
        foreach (var path in path)
        {
            path.transform.name = t.ToString();
            t++;
        }
    }
    private void Start()
    {
        dice.onDiceRolledStarted += onDiceRolledStarted;
        dice.onSelectingStarted += onSelectingStarted;
    }
    public int playerCount;
    public int Turn;
    public int whosTurn
    {
        get
        {
            return Turn;
        }
        set
        {
            if(Turn != value)
            {
                Turn = value;
                if(Turn > (playerCount-1))
                {
                    Turn = 0;
                }
                if (Turn==getPlayerIndux())
                {
                    //if my turn started

                }
                else
                {
                    
                }
            }
        }
    }
    public int rollDiseAnimation;
    public string dataJson;
    bool allConneted;
    public bool allPlayersConneted
    {
        get { return allConneted; }
        set
        {
            if (value != allConneted)
            {
                allConneted = value;
                if (value)
                {
                    onAllPlayersConneted();
                }
                else
                {

                }
            }
        }
    }
    public void onAllPlayersConneted()
    {
        //order players by user name
        userPresenceList = userPresenceList.OrderBy(s => s.Key).ToDictionary(pair => pair.Key, pair => pair.Value);
        spanPawns();
        ludoplaceholderBlue.onPawnMoved = onPawnMoved;
    }
    //StartGame is called by nakama clint after match matched
    public void StartGame(nakamaClient nakamaClient)
    {
        if (nakamaClient)
        {
            this.nakamaClient = nakamaClient;
            if (checkNakama())
            {
                if (nakamaClient.match != null)
                {
                    var match = nakamaClient.match;
                    var socket = nakamaClient.socket;
                    var client = nakamaClient.client;
                    var session = nakamaClient.session;
                    var Self = match.Self;
                    var UserId = match.Self.UserId;
                    var Username = match.Self.Username;



                    socket.ReceivedMatchState += ReceivedMatchState;
                    socket.ReceivedMatchPresence += ReceivedMatchPresence;


                    userPresenceList = new Dictionary<string, IUserPresence>();
                    userPresenceList.Add(Username, Self);

                    ludoplaceholderBlue.ludoGamePlayerData.userName = Username;
                    whosTurn = 0;
                    playerCount = 2;
                    allPlayersConneted = false;
                    sendPing();
                }
            }
        }
    }
    public float movingSpeed = 0.03f;
    //next Player Turn 
    public void nextPlayerTurn(bool sendDataToPlayers)
    {
        whosTurn++;
        if (whosTurn==getPlayerIndux())
        {
            dice.clearData();
        }
        if (sendDataToPlayers)
        {
            sendMessageToPlayers("nextOne");
        }
        ludoplaceholderBlue.disSelectAllPawns();
    }
    //span pawns
    public void spanPawns()
    {
        ludoplaceholderBlue.spanPawns(this);
        ludoplaceholderGreen.spanPawns(this);
        //ludoplaceholderRed.spanPawns(this);
        //ludoplaceholderYellow.spanPawns(this); 
    }

    public pawn addKill;

    private void Update()
    {

        if (addKill != null)
        {
            var data = new gameData();
            addKill.setValues(pawnState.dead, 1);
            data.addPlayerData(addKill.ludoplaceholder.ludoGamePlayerData);
            sendMessageToPlayers("data:"+JsonUtility.ToJson(data));
            addKill=null;
        }

        allPlayersConneted = (userPresenceList.Count == playerCount);
        if (allPlayersConneted)
        {
            //all players connected
            if (checkNakama())
            {
                var match = nakamaClient.match;
                var socket = nakamaClient.socket;
                var client = nakamaClient.client;
                var session = nakamaClient.session;
                if (match != null)
                {
                    //handle game code
                    if (whosTurn == getPlayerIndux())
                    {
                        //my turn
                        dice.diceData.isMyTurn = true;
                        if (dice.canRoll())
                        {
                        }
                        if (dice.diceData.isSelecting)
                        {
                            onSelecting();
                        }
                    }
                    else
                    {
                        //other player turn
                        dice.diceData.isMyTurn = false;
                    }


                }
                else
                {
                    //Debug.Log("matchIs");
                }

                updateData();
            }


        }
        else
        {
            sendPing();
        }
        if (rollDiseAnimation > 0)
        {
            rollDiseAnimation = Mathf.Clamp(rollDiseAnimation, 0, 6);
            if(!dice.diceData.isMyTurn)
            {
                dice.roll(rollDiseAnimation);
            }
            rollDiseAnimation = 0;
        }
    }
    void updateData()
    {
        if (dataJson != "")
        {
            print(dataJson);
            var gameData = JsonUtility.FromJson<gameData>(dataJson);
            if (gameData != null)
            {
                if (gameData.playersData != null)
                {
                    foreach (var data in gameData.playersData)
                    {
                        var userName = data.userName;
                        if (userName == ludoplaceholderBlue.ludoGamePlayerData.userName)
                        {
                            //my data
                            ludoplaceholderBlue.updatepawnData(data);
                            print("myplayer data updated");

                        }
                        else
                        {
                            ludoplaceholderGreen.updatepawnData(data);
                            print("ludoplaceholderGreen data updated");

                        }

                    }
                }

            }
            dataJson = "";
        }

    }
    #region call backs
    public void onSelectingStarted(int value)
    {
        var pawns = ludoplaceholderBlue.getValiedPawns(dice.diceData.value, true);
        if(pawns.Count == 0)
        {
            nextPlayerTurn(true);
        }
    }
    public  void onSelecting()
    {

    }
    public void onSelectingComplected()
    {

    }
    public void onDiceRolledStarted(int value)
    {
        sendMessageToPlayers("rolled:" + value);
    }
    public void onPawnMoved(pawn pawn)
    {
        
        if (pawn )
        {
            var myPath = pawn.getCurrentPath();
            var allOtherPawns = new List<pawn>();
            allOtherPawns.AddRange(ludoplaceholderGreen.getAllPawns());
            allOtherPawns.AddRange(ludoplaceholderRed.getAllPawns());
            allOtherPawns.AddRange(ludoplaceholderYellow.getAllPawns());

            var data = new gameData();
            List<pawn> killedPawns =new List<pawn>();
            foreach(pawn otherPawn in allOtherPawns)
            {
                if (otherPawn)
                {
                    var holder = otherPawn.ludoplaceholder;
                    if (holder)
                    {
                        var path = otherPawn.getCurrentPath();
                        if (path != null && myPath != null)
                        {
                            if (path.gameObject == myPath.gameObject)
                            {
                                //in same path
                                otherPawn.setValues(pawnState.dead,1);
                                data.addPlayerData(otherPawn.ludoplaceholder.ludoGamePlayerData);
                                killedPawns.Add(otherPawn);
                            }
                        }
                    }
                }
            }

            data.addPlayerData(ludoplaceholderBlue.ludoGamePlayerData);
            sendMessageToPlayers("data:" + JsonUtility.ToJson(data));
            if (dice.diceData.value == 6 || killedPawns.Count > 0)
            {
                dice.diceData.isSelecting = false;
            }
            else
            {
                nextPlayerTurn(true);
                dice.clearData();
            }
        }

    }
    #endregion

    #region nakama methods
    public Dictionary<string,IUserPresence> userPresenceList = new Dictionary<string,IUserPresence>();
    //resive data from players
    public void ReceivedMatchState(IMatchState matchState)
    {
        var message = Encoding.UTF8.GetString(matchState.State);
        var MatchId = matchState.MatchId;
        var UserPresence = matchState.UserPresence;
        var Username = UserPresence.Username;
        var UserId = UserPresence.UserId;
        if (userPresenceList.Count != playerCount)
        {
            if (message == "ping")
            {
                if (!userPresenceList.ContainsKey(UserId))
                {
                    userPresenceList.Add(Username, UserPresence);
                }
                ludoplaceholderGreen.ludoGamePlayerData.userName = Username;
                print("ping");
            }
        }
        if (allConneted)
        {
            //all players loaded
            if (message.StartsWith("rolled:"))
            {
                var value = message.Substring("rolled:".Length, message.Length - "rolled:".Length);
                var valueInt = Convert.ToInt32(value);
                rollDiseAnimation = valueInt;
            }
            else if (message.StartsWith("data:"))
            {
                dataJson = message.Substring("data:".Length, message.Length - "data:".Length);
            }
            else if (message == "nextOne")
            {
                nextPlayerTurn(false);
            }
        }
    }
    float lastPingTime;
    [ContextMenu("ping")]
    public void sendPing()
    {
        if ((Time.unscaledTime-lastPingTime) > 0.5f)
        {
            sendMessageToPlayers("ping");
            lastPingTime = Time.unscaledTime;
        }
    }
    //get my index in players
    public int getPlayerIndux()
    {
        if (checkNakama())
        {
            int i = 0;
            foreach (var user in userPresenceList)
            {
                if (nakamaClient.session.Username == user.Value.Username)
                {
                    return i;
                }
                i++;
            }
        }
        return -1;
    }
    //matchPresence to handle joins , leaves
    public void ReceivedMatchPresence(IMatchPresenceEvent matchPresenceEvent)
    {
        if (allPlayersConneted)
        {
            foreach (var userPresence in matchPresenceEvent.Leaves)
            {
                if (userPresenceList.ContainsKey(userPresence.Username))
                {
                    userPresenceList.Remove(userPresence.Username);
                    Debug.Log("player disconneted "+ userPresence.Username);
                }
            }
        }
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
    //send message to all clints
    void sendMessageToPlayers(string data)
    {
        if (checkNakama())
        {
            if (nakamaClient.match != null)
            {
                nakamaClient.sendMatchMessage(data);
            }
        }
    }
    #endregion
}
public class gameData
{
    public List<ludoGamePlayerData> playersData;
    public void addPlayerData(ludoGamePlayerData playerData)
    {
        if (playersData == null)
        {
            playersData = new List<ludoGamePlayerData>();
        }
        playersData.Add(playerData);
    }
}
[Serializable]
public struct ludoGamePlayerData
{
    public string userName;
    public diceData diceData;
    public pawnInfo pawnInfo1;
    public pawnInfo pawnInfo2;
    public pawnInfo pawnInfo3;
    public pawnInfo pawnInfo4;
}
[Serializable]  
public struct diceData
{
    public bool isMyTurn;
    public bool isRolling;
    public bool isSelecting;
    public int value;

}
[Serializable]
public struct pawnInfo
{
    public pawnState pawnState;
    public int pos;
    public void setValues(pawnState pawnState,int pos)
    {
        this.pawnState = pawnState;
        this.pos = pos;
    }
    public void setPawnState(pawnState pawnState)
    {
        this.pawnState = pawnState;
    }
    public void setPos(int pos)
    {
        this.pos = pos;
    }
}
public enum pawnState
{
    dead, alive, complected
}