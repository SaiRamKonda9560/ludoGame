using UnityEngine;
using Nakama;
using System;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Text;
using TMPro;
using System.Collections.Generic;
public class nakamaClient : MonoBehaviour
{
    public string serverIp= "127.0.0.1";
    void Start()
    {
        //connetToServer();
    }
    public IClient client;
    public ISession session;
    public ISocket socket;
    public IApiAccount account;
    public bool IsConnected;
    public bool IsConnecting;
    
    public void connetToServer()
    {
        //connect using defalt keys
        client = new Nakama.Client("http", serverIp, 7350, "defaultkey");
        AuthenticateWithDevice();
        
    }
    public string newId;
    public TMP_InputField inputFieldForIp;
    public string setNewIdAndStartServer
    {
        set
        {
            serverIp=inputFieldForIp.text;
            if(serverIp == "")
            {
                serverIp = "127.0.0.1";
            }
            newId = value;
            connetToServer();
        }
    }
    public async void AuthenticateWithDevice()
    {
        // If the user's device ID is already stored, grab that - alternatively get the System's unique device identifier.
        var deviceId = PlayerPrefs.GetString("deviceId", SystemInfo.deviceUniqueIdentifier);
        // If the device identifier is invalid then let's generate a unique one.
        if (deviceId == SystemInfo.unsupportedIdentifier)
        {
            deviceId = System.Guid.NewGuid().ToString();
        }
        // Save the user's device ID to PlayerPrefs so it can be retrieved during a later play session for re-authenticating.
        if(!string.IsNullOrEmpty(newId))
        {
            deviceId = newId;
            PlayerPrefs.SetString("authDeviceToken", "");
        }
        PlayerPrefs.SetString("deviceId", deviceId);
        // Authenticate with the Nakama server using Device Authentication.
        try
        {
            var storedAuthToken = PlayerPrefs.GetString("authDeviceToken");
            var refreshToken = PlayerPrefs.GetString("nakama.refreshToken");
            if (string.IsNullOrEmpty(storedAuthToken))
            {
                session = await client.AuthenticateDeviceAsync(deviceId);
                //save authToken
                PlayerPrefs.SetString("authDeviceToken", session.AuthToken);
                Debug.Log("Authenticated with Device ID");
            }
            else
            {
                session = Session.Restore(storedAuthToken);
                if (session.IsExpired || session.HasExpired(DateTime.UtcNow.AddDays(1)))
                {
                    try
                    {
                        // Attempt to refresh the existing session.
                        session = await client.SessionRefreshAsync(session);
                    }
                    catch (ApiResponseException)
                    {
                        // Couldn't refresh the session so reauthenticate.
                        session = await client.AuthenticateDeviceAsync(deviceId);
                        PlayerPrefs.SetString("nakama.refreshToken", session.RefreshToken);
                    }

                    PlayerPrefs.SetString("authDeviceToken", session.AuthToken);
                }
            }


        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error authenticating with Device ID: {0}", ex.Message);
        }


        //create socket
        if (session != null && client != null)
        {
            socket = client.NewSocket();
            socket.Closed+=Closed;
            socket.Connected += Connected;

            await socket.ConnectAsync(session);


        }
    }
    //called if socket connected
    void Connected()
    {
        print("connected");
        getAccount();
        socket.ReceivedMatchmakerMatched += matchFound;

    }
    //called if socket closed
    void Closed()
    {
        print("closed");
    }
    public async void setUserName()
    {
        if (socket != null && session!=null)
        {
            await client.UpdateAccountAsync(session, nameInputField.text);
            getAccount();
        }
    }
    public async void getAccount()
    {
        if (socket != null && session != null)
        {
            account = await client.GetAccountAsync(session);
            onAccountLoaded();
        }
    }
    [Header("events")]
    public UnityEvent onConnected;
    public UnityEvent onDisConnected;


    private void Update()
    {
        if(socket != null)
        {
            if(IsConnected != socket.IsConnected)
            {
                IsConnected = socket.IsConnected;
                if(IsConnected)
                {
                    onConnected.Invoke();
                }
                else
                {
                    onDisConnected.Invoke();
                }
            }
            if(IsConnecting != socket.IsConnecting)
            {
                IsConnecting = socket.IsConnecting;
            }
        }
        else
        {
            IsConnected = false;
            IsConnecting = false;
        }

        if (IsConnected)
        {
            if (match==null)
            {
                connectedRoot.gameObject.SetActive(true);

            }
            else
            {
                connectedRoot.gameObject.SetActive(false);

            }
            notConnectedRoot.gameObject.SetActive(false);

        }
        else
        {
            connectedRoot.gameObject.SetActive(false);
            notConnectedRoot.gameObject.SetActive(true);
            if (match != null)
            {
                match = null;
            }

        }
        autoMatchWindow.gameObject.SetActive(match!=null|| matchmakerTicket != null);
        if (account != null)
        {
            nameTmp.text = account.User.Username;
        }
    }

    private void OnDestroy()
    {
        closeMatch();
    }
    public async void closeMatch()
    {
        autoMatchWindow.gameObject.SetActive(false);
        if (socket != null && match != null )
        {
            await socket.LeaveMatchAsync(match);
        }
        matchmakerTicket = null;
        match = null;
    }
    #region match
    [Header("match")]
    public Image autoMatchWindow;
    public IMatchmakerTicket matchmakerTicket;
    public IMatch match;
    public async void AddMatchmakerAsync(string query)
    {
        if (socket != null)
        {
            socket.ReceivedMatchPresence += ReceivedMatchPresence;
            matchmakerTicket = await socket.AddMatchmakerAsync(query, 2,2);
        }
        else
        {

        }
    }
    public Dictionary<string, IUserPresence> userPresences = new Dictionary<string, IUserPresence>();
    public void ReceivedMatchPresence(IMatchPresenceEvent matchPresenceEvent)
    {
        if(userPresences==null)
        {
            userPresences = new Dictionary<string, IUserPresence>();
        }
        foreach(var userPresence in matchPresenceEvent.Joins)
        {
            if (userPresences.ContainsKey(userPresence.Username))
            {
                userPresences[userPresence.Username] =  userPresence;
            }
            else
            {
                userPresences.Add(userPresence.Username, userPresence);

            }
        }
        foreach (var userPresence in matchPresenceEvent.Leaves)
        {
            if (userPresences.ContainsKey(userPresence.Username))
            {
                userPresences.Remove(userPresence.Username);

            }
        }
    }
    public void matchFound(IMatchmakerMatched matchmakerMatched)
    {
        if (socket != null)
        {
            joinMatch(matchmakerMatched);
        }
    }
    public async void joinMatch(IMatchmakerMatched matchmakerMatched)
    {
        match = await socket.JoinMatchAsync(matchmakerMatched);
        socket.ReceivedMatchState += ReceivedMatchState;
        ludogame.StartGame(this);
    }
    public async void joinMatch(string matchId)
    {
        match = await socket.JoinMatchAsync(matchId);
        socket.ReceivedMatchState += ReceivedMatchState;
        ludogame.StartGame(this);
    }
    public async void sendMatchMessage(string data)
    {
        if(match!=null&&socket!=null)
        {
            await socket.SendMatchStateAsync(match.Id, 1,Encoding.UTF8.GetBytes(data));
        }
    }
    public async void sendMatchData(byte[] data)
    {
        if (match != null && socket != null && data!=null)
        {
            await socket.SendMatchStateAsync(match.Id, 1, data);
        }
    }
    public void ReceivedMatchState(IMatchState matchState)
    {
        var data = Encoding.UTF8.GetString(matchState.State);
        var MatchId = matchState.MatchId;
        var UserPresence = matchState.UserPresence;
        var Username = UserPresence.Username;
        var UserId = UserPresence.UserId;

    }
    public void autoMatch()
    {
        AddMatchmakerAsync("*");
        
    }
    #endregion

    #region ref
    public TMP_Text nameTmp;
    public TMP_InputField nameInputField;
    public Transform connectedRoot;
    public Transform notConnectedRoot;
    public ludogame ludogame;
    public void onAccountLoaded()
    {
        if (account != null)
        {
            //nameTmp.text = account.User.Username;
            nameInputField.text = account.User.Username;

        }
        else
        {
            Debug.Log("account is null");
        }
    }
    #endregion
}
