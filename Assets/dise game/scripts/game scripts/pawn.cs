using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class pawn : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        movePawn();
    }
    public List<SpriteRenderer> SpriteRenderersForColor = new List<SpriteRenderer>();
    public List<Image> imagesForColor = new List<Image>();

    public List<SpriteRenderer> SpriteRenderersSelection = new List<SpriteRenderer>();
    public List<Image> imagesSelection = new List<Image>();

    public ludoplaceholder ludoplaceholder;
    public ludogame ludogame
    {
        get
        {
            return ludoplaceholder.ludogame;
        }
    }
    public path restPath;
    public UnityEvent onStep;
    public pawnInfo pawnInfo
    {
        get
        {
            switch (pawnCount)
            {
                case 1:
                    return ludoplaceholder.ludoGamePlayerData.pawnInfo1;
                case 2:
                    return ludoplaceholder.ludoGamePlayerData.pawnInfo2;
                case 3:
                    return ludoplaceholder.ludoGamePlayerData.pawnInfo3;
                case 4:
                    return ludoplaceholder.ludoGamePlayerData.pawnInfo4;
            }
            return ludoplaceholder.ludoGamePlayerData.pawnInfo1;
        }
        set
        {
            switch (pawnCount)
            {
                case 1:
                    ludoplaceholder.ludoGamePlayerData.pawnInfo1 = value;
                    break;
                case 2:
                    ludoplaceholder.ludoGamePlayerData.pawnInfo2 = value;
                    break;
                case 3:
                    ludoplaceholder.ludoGamePlayerData.pawnInfo3 = value;
                    break;
                case 4:
                    ludoplaceholder.ludoGamePlayerData.pawnInfo4 = value;
                    break;
                default:
                    Debug.LogError("no pan find");
                    break;
            }
        }
    }
    public bool movebul;
    public int pawnCount;
    public CircleCollider2D CircleCollider2D;
    public movingSettings movingSettings;
    public UnityEvent onCompleted;

    void Update()
    {
        foreach(var SpriteRendererSelection in SpriteRenderersSelection)
        {
            SpriteRendererSelection.gameObject.SetActive(movebul);
        }
        foreach (var imageSelection in imagesSelection)
        {
            imageSelection.gameObject.SetActive(movebul);
        }
        if(CircleCollider2D != null)
        CircleCollider2D.enabled = movebul;
        
    }
    public void go(ludoplaceholder ludoplaceholder, path restPath, int pawnCount)
    {
        this.pawnCount = pawnCount;
        this.ludoplaceholder = ludoplaceholder;
        this.restPath = restPath;
        foreach (var SpriteRendererForColor in SpriteRenderersForColor)
        {
            SpriteRendererForColor.color = ludoplaceholder.color;
        }
        foreach (var imagesForColor in imagesForColor)
        {
            imagesForColor.color = ludoplaceholder.color;
        }
        gameObject.SetActive(true);
    }

    public void movePawn()
    {
        if (movebul)
        {
            movebul = !ludoplaceholder.movePawn(this);
        }
    }
    #region set values
    public void setValues(pawnState pawnState,int pos)
    {
        setPawnState(pawnState);
        setPos(pos);
    }
    public void setValues(pawnInfo pawnInfo)
    {
        setPawnState(pawnInfo.pawnState);
        setPos(pawnInfo.step);
    }
    public void setPawnState(pawnState pawnState)
    {
        var oldInfo = pawnInfo;
        var oldState = oldInfo.pawnState;
        if (oldState != pawnState)
        {
            oldInfo.pawnState = pawnState;
            pawnInfo = oldInfo;
            onPawnStateChanged(oldState, pawnState);

        }
    }
    public void setPos(int pos)
    {
        var oldInfo = pawnInfo;
        var oldPos = pawnInfo.step;
        if (oldPos != pos)
        {
            oldInfo.step = pos;
            pawnInfo= oldInfo;
            onPawnPosChanged(oldPos, pos);
        }
    }
    #endregion
    public void onPawnStateChanged(pawnState oldPawnState, pawnState newPawnState)
    {
        if (oldPawnState != newPawnState)
        {
            var movingSpeed = ludoplaceholder.ludogame.movingSpeed;
            if (oldPawnState == pawnState.dead && newPawnState == pawnState.alive)
            {
                //just alive
                var path = ludoplaceholder.placeHolderpath;
                if (path.Count>0)
                {
                    movingSettings.newPath(movingSpeed, path[0]);
                }
            }
            if (oldPawnState == pawnState.alive && newPawnState == pawnState.dead)
            {
                //just dead
                var path = ludoplaceholder.placeHolderpath;
                var newPath = new List<path>();
                if (pawnInfo.step>0)
                {
                    var p = path.GetRange(0, pawnInfo.step-1);
                    p.Reverse();
                    newPath.AddRange(p);
                }
                newPath.Add(restPath);
                movingSettings.newPath(movingSpeed*2.5f, newPath);
            }
            if (newPawnState == pawnState.complected)
            {
                //just complected
                movingSettings.newPath(movingSpeed,ludoplaceholder.innerPath[ludoplaceholder.innerPath.Count-1]);
            }
            onDataChanged();
        }
    }
    public void onPawnPosChanged(int oldPos,int newPos)
    {
        if (pawnInfo.pawnState == pawnState.alive || pawnInfo.pawnState == pawnState.complected)
        {
            var path = ludoplaceholder.placeHolderpath;
            var change = ((newPos - oldPos) );
            change += (change > 0) ? +1 : -1;
            //messageToUser.messagee($"oldPos {oldPos},newPos {newPos},change {change}", 20f);
            var movingSpeed = ludoplaceholder.ludogame.movingSpeed;

            if (oldPos < path.Count && newPos < path.Count && oldPos >= 0 && newPos >= 0)
            {
                movingSettings.newPath(movingSpeed, path.GetRange(oldPos, change));
            }
            onDataChanged();
        }
    }
    private void onDataChanged()
    {
        
    }
    public AudioClip onStepPlayAudioClip;
    private void FixedUpdate()
    {
        if (!movingSettings.moving)
            return;

        if (movingSettings.path == null || movingSettings.path.Count == 0)
        {
            movingSettings.moving = false;
            return;
        }


        Transform target = movingSettings.path[0].transform;
        Vector3 targetPosition = target.position;
        Vector3 direction = targetPosition - transform.position;

        float reachThreshold = 0.01f; // Adjust as needed

        if (direction.magnitude <= reachThreshold)
        {
            transform.position = targetPosition; // Snap to exact position
            movingSettings.path.RemoveAt(0);

            onStep.Invoke();

            if (onStepPlayAudioClip)
                audioSpaner.span(onStepPlayAudioClip);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, movingSettings.speedDelta);
         
        }
        if (movingSettings.path.Count == 0)
        {
            movingSettings.moving = false;
            onAnimationComplected();
        }
    }

    private void onAnimationComplected()
    {
        if (pawnInfo.pawnState == pawnState.complected)
        {
            onCompleted.Invoke();
            if (ludoplaceholder.isAllPawnsCompleted())
            {
                var playersdata = new List<ludoGamePlayerData>();
                playersdata.Add(ludogame.ludoplaceholderBlue.ludoGamePlayerData);
                playersdata.Add(ludogame.ludoplaceholderGreen.ludoGamePlayerData);
                playersdata.Add(ludogame.ludoplaceholderYellow.ludoGamePlayerData);
                playersdata.Add(ludogame.ludoplaceholderRed.ludoGamePlayerData);

                var data = new gameData() { playersData = playersdata };
                ludoplaceholder.ludogame.onComplectWindow.go(data);
            }
        }
    }

    public path getCurrentPath()
    {
        if(ludoplaceholder)
        switch(pawnInfo.pawnState)
        {
            case pawnState.alive:
                    var index = pawnInfo.step;
                    index = Mathf.Clamp(index, 0, ludoplaceholder.placeHolderpath.Count);
                return ludoplaceholder.placeHolderpath[index];
            case pawnState.dead:
                return restPath;
            case pawnState.complected:
                return ludoplaceholder.placeHolderpath[ludoplaceholder.placeHolderpath.Count-1];
        }
        return null;
    }
}
[Serializable]
public class movingSettings
{
    public bool moving;
    public List<path> path;
    public float speedDelta;
    public void newPath(float speedDelta, List<path> newPath)
    {
        this.path = newPath;
        this.speedDelta = speedDelta;
        moving = true;

    }
    public void newPath(float speedDelta, params path[] newPath)
    {
        this.path = new List<path>(newPath);
        this.speedDelta = speedDelta;
        moving = true;

    }
}
