using System;
using System.Collections.Generic;
using UnityEngine;

public class pawn : MonoBehaviour
{
    public SpriteRenderer SpriteRendererForColor;
    public SpriteRenderer SpriteRendererSelection;
    public ludoplaceholder ludoplaceholder;
    public path restPath;
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
    void Update()
    {
        SpriteRendererSelection.gameObject.SetActive(movebul);
        CircleCollider2D.enabled = movebul;
    }
    public void go(ludoplaceholder ludoplaceholder, path restPath, int pawnCount)
    {
        this.pawnCount = pawnCount;
        this.ludoplaceholder = ludoplaceholder;
        this.restPath = restPath;
        SpriteRendererForColor.color = ludoplaceholder.color;
    }
    private void OnMouseDown()
    {
        if (movebul)
        {
            ludoplaceholder.pawnSelected(this);
            movebul=false;
        }
    }
    public void setValues(pawnState pawnState,int pos)
    {
        setPawnState(pawnState);
        setPos(pos);
    }
    public void setValues(pawnInfo pawnInfo)
    {
        setPawnState(pawnInfo.pawnState);
        setPos(pawnInfo.pos);
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
        var oldPos = pawnInfo.pos;
        if (oldPos != pos)
        {
            oldInfo.pos = pos;
            pawnInfo= oldInfo;
            onPawnPosChanged(oldPos, pos);
        }
    }

    public movingSettings movingSettings;
    public void onPawnStateChanged(pawnState oldPawnState, pawnState newPawnState)
    {
        if (oldPawnState != newPawnState)
        {
            movingSettings.path.Clear();
            if (oldPawnState == pawnState.dead && newPawnState == pawnState.alive)
            {
                //just alive
                var path = ludoplaceholder.mainPath;
                if (path.Count>0)
                {
                    movingSettings.path.Add(path[0]);
                    movingSettings.moving = true;
                }
            }
            if (oldPawnState == pawnState.alive && newPawnState == pawnState.dead)
            {
                //just dead
                movingSettings.path.Add(restPath.GetComponent<path>());
                movingSettings.moving = true;
            }
            if (newPawnState == pawnState.complected)
            {
                //just complected
                movingSettings.path.Add(ludoplaceholder.innerPath[ludoplaceholder.innerPath.Count-1]);
                movingSettings.moving = true;
            }
            onDataChanged();
        }
    }
    public void onPawnPosChanged(int oldPos,int newPos)
    {
        if (pawnInfo.pawnState == pawnState.alive)
        {
            var path = ludoplaceholder.mainPath;
            if (oldPos < path.Count && newPos < path.Count && oldPos >= 0 && newPos >= 0)
            {
                movingSettings.path.Clear();
                movingSettings.path.AddRange(path.GetRange(oldPos, newPos - oldPos));
                movingSettings.moving = true;
            }
            onDataChanged();
        }
    }
    private void onDataChanged()
    {
        
    }
    private void FixedUpdate()
    {
        if (movingSettings.moving)
        {
            if (movingSettings.path.Count > 0)
            {
                var path = movingSettings.path[0];
                var position = path.transform.position;
                transform.position = Vector3.MoveTowards(transform.position,position,ludoplaceholder.ludogame.movingSpeed);
                if(transform.position == position)
                {
                    //reached
                    movingSettings.path.RemoveAt(0);
                }
                else
                {
                    //not reached

                }

            }
            if (movingSettings.path.Count==0)
            {
                movingSettings.moving = false;
            }
        }
    }
    public path getCurrentPath()
    {
        if(ludoplaceholder)
        switch(pawnInfo.pawnState)
        {
            case pawnState.alive:
                return ludoplaceholder.list[pawnInfo.pos];
            case pawnState.dead:
                return restPath;
            case pawnState.complected:
                return ludoplaceholder.list[ludoplaceholder.list.Count-1];
        }
        return null;
    }
}
[Serializable]
public class movingSettings
{
    public bool moving;
    public List<path> path;
}
