using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ludoplaceholder : MonoBehaviour
{
    public int spanIndex;
    public int endIndex;
    public int InsideOffest=-2;
    public int localPosition;

    public path restPath1;
    public path restPath2;
    public path restPath3;
    public path restPath4;

    public pawn pawn1;
    public pawn pawn2;
    public pawn pawn3;
    public pawn pawn4;

    public Color color;
    public ludoGamePlayerData ludoGamePlayerData;
    public ludogame ludogame;
    public Action<pawn> onPawnMoved;

    public List<path> innerPath;
    public List<path> list = new List<path>();
    public List<path> mainPath
    {
        get
        {
            if (ludogame)
            {
                list.Clear();
                var paths= ludogame.path;
                var startIndex = spanIndex;
                endIndex = spanIndex + InsideOffest;
                list.AddRange(paths.GetRange(startIndex, (paths.Count - startIndex)));
                if (endIndex+1 > 0)
                {
                    list.AddRange(paths.GetRange(0, endIndex+1));
                }
                list.AddRange(innerPath);
            }
            return list;
        }
    }
    //K 128
    //spans pawns
    public void spanPawns(ludogame ludogame)
    {
        this.ludogame = ludogame;
        var pawnsGameObject = transform.Find("pawns");
        if (pawnsGameObject == null)
        {
            pawnsGameObject = new GameObject("pawns").transform;
            pawnsGameObject.parent = transform;
        }
        destroyAllChilds(pawnsGameObject);
        pawn pawnPrefab= ludogame.pawnPrefab;

        var span1 = Instantiate(pawnPrefab.gameObject, pawnsGameObject);
        span1.transform.position = restPath1.position;
        pawn1 = span1.GetComponent<pawn>();
        pawn1.go(this, restPath1, 1);

        var span2 = Instantiate(pawnPrefab.gameObject, pawnsGameObject);
        span2.transform.position = restPath2.position;
        pawn2 = span2.GetComponent<pawn>();
        pawn2.go(this, restPath2, 2);

        var span3 = Instantiate(pawnPrefab.gameObject, pawnsGameObject);
        span3.transform.position = restPath3.position;
        pawn3 = span3.GetComponent<pawn>();
        pawn3.go(this, restPath3, 3);

        var span4 = Instantiate(pawnPrefab.gameObject, pawnsGameObject);
        span4.transform.position = restPath4.position;
        pawn4 = span4.GetComponent<pawn>();
        pawn4.go(this, restPath4, 4);

        var d= mainPath;

    }

    #region update
    public void updatepawnData(ludoGamePlayerData newLudoGamePlayerData)
    {
        pawn1.setValues(newLudoGamePlayerData.pawnInfo1);
        pawn2.setValues(newLudoGamePlayerData.pawnInfo2);
        pawn3.setValues(newLudoGamePlayerData.pawnInfo3);
        pawn4.setValues(newLudoGamePlayerData.pawnInfo4);
    }

    #endregion

    #region selecting
    //get new positions
    public int totalSteps
    {
        get { return list.Count; }
    }

    public int totalInnerSteps
    {
        get { return innerPath.Count; }
    }

    public bool canPawnMove(pawn pawn, int number, bool applyValue = false)
    {
        var valied = false;
        if (pawn)
        {
            var pawnInfo = pawn.pawnInfo;
            int playerCurrentSteps = pawnInfo.pos;
            var pawnState = pawnInfo.pawnState;
            switch (pawnState)
            {
                case pawnState.dead:
                    if (number == 6)
                    {
                        if (applyValue)
                        {
                            pawn.setValues(pawnState.alive,1);
                        }
                        valied = true;
                    }
                    break;
                case pawnState.alive:
                    var targateSteps = number + playerCurrentSteps;
                    if (targateSteps <= totalSteps)
                    {
                        // in range
                        if (applyValue)
                        {
                            pawn.setPos(targateSteps);
                        }
                        valied = true;
                    }
                    else
                    {
                        //out of range
                    }
                    break;
                case pawnState.complected:
                    valied = false;
                    break;
            }
        }
        return valied;
    }
    public List<pawn> getValiedPawns(int number, bool select = false)
    {
        List<pawn> pawns = new List<pawn>();
        if (canPawnMove(pawn1, number))
        {
            pawns.Add(pawn1);
            pawn1.movebul = true;
        }
        if (canPawnMove(pawn2, number))
        {
            pawns.Add(pawn2);
            pawn2.movebul = true;

        }
        if (canPawnMove(pawn3, number))
        {
            pawns.Add(pawn3);
            pawn3.movebul = true;

        }
        if (canPawnMove(pawn4, number))
        {
            pawns.Add(pawn4);
            pawn4.movebul = true;

        }
        return pawns;
    }
    public void selectValiedPawns(int number)
    {
        pawn1.movebul = (canPawnMove(pawn1, number));
        pawn2.movebul = (canPawnMove(pawn2, number));
        pawn3.movebul = (canPawnMove(pawn3, number));
        pawn4.movebul = (canPawnMove(pawn4, number));
    }
    public void disSelectAllPawns()
    {
        pawn1.movebul = false;
        pawn2.movebul = false;
        pawn3.movebul = false;
        pawn4.movebul = false;
    }

    //after mouse selected
    public void pawnSelected(pawn pawn)
    {
        if (ludogame.dice.diceData.isMyTurn && ludogame.dice.diceData.isSelecting)
        {
            var moved = canPawnMove(pawn, ludogame.dice.diceData.value, true);
            if (moved)
            {
                onPawnMoved?.Invoke(pawn);
            }
            disSelectAllPawns();
        }
        else
        {
            disSelectAllPawns();
        }
    }

    #endregion


    public List<pawn> getAllPawns()
    {
        var list = new List<pawn>();
        if (pawn1 != null && pawn2 != null && pawn3 != null && pawn4 != null)
        {
            list.AddRange(new List<pawn>() { pawn1, pawn2, pawn3, pawn4 });
        }
        return list;
    }
    public bool isAnyPawnMoving()
    {
        if (pawn1 && pawn2 && pawn3 && pawn4)
        {
            return (pawn1.movingSettings.moving || pawn2.movingSettings.moving || pawn3.movingSettings.moving || pawn4.movingSettings.moving);
        }
        else
        {
            return false;
        }
    }
    private void destroyAllChilds(Transform transform)
    {
        if(transform)
        foreach(var child in transform.GetComponentsInChildren<Transform>())
        {
            if (transform != child.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
