using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ludoplaceholder : MonoBehaviour
{
    public ludogame ludogame;
    public Color color;

    public path restPath1;
    public path restPath2;
    public path restPath3;
    public path restPath4;

    [NonSerialized] public pawn pawn1;
    [NonSerialized] public pawn pawn2;
    [NonSerialized] public pawn pawn3;
    [NonSerialized] public pawn pawn4;

    public ludoGamePlayerData ludoGamePlayerData;
    public Action<pawn> onPawnMoved;
    [Header("path")]
    public List<path> innerPath;
    public List<path> placeHolderpath = new List<path>();
    public GameObject noSignal;
    public void setSignal(bool signal)
    {
        if (noSignal)
        {
            noSignal.SetActive(signal);
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


    }
    public void setPawnsScale(float scale)
    {
        var ludoBordRectTransofrm = ludogame.GetComponent<RectTransform>();
        if (ludoBordRectTransofrm)
        {
            var boardSizeDelta = ludoBordRectTransofrm.sizeDelta;
            if (pawn1)
            {
                var pawn1RectTransofrm = pawn1.GetComponent<RectTransform>();
                pawn1RectTransofrm.sizeDelta = boardSizeDelta*scale;
            }
            if (pawn1)
            {
                var pawn2RectTransofrm = pawn2.GetComponent<RectTransform>();
                pawn2RectTransofrm.sizeDelta = boardSizeDelta * scale;
            }
            if (pawn1)
            {
                var pawn3RectTransofrm = pawn3.GetComponent<RectTransform>();
                pawn3RectTransofrm.sizeDelta = boardSizeDelta * scale;
            }
            if (pawn1)
            {
                var pawn4RectTransofrm = pawn4.GetComponent<RectTransform>();
                pawn4RectTransofrm.sizeDelta = boardSizeDelta * scale;
            }
        }

    }
    public void updatepawnData(ludoGamePlayerData newLudoGamePlayerData)
    {
        pawn1.setValues(newLudoGamePlayerData.pawnInfo1);
        pawn2.setValues(newLudoGamePlayerData.pawnInfo2);
        pawn3.setValues(newLudoGamePlayerData.pawnInfo3);
        pawn4.setValues(newLudoGamePlayerData.pawnInfo4);
    }
    #region selecting
    //get new positions
    public int totalStepsCount
    {
        get { return placeHolderpath.Count; }
    }
    public int totalInnerSteps
    {
        get { return innerPath.Count; }
    }

    public void Start()
    {
    }
    public bool canPawnMove(pawn pawn, int number, bool applyValue = false)
    {
        var valied = false;
        if (pawn)
        {
            var pawnInfo = pawn.pawnInfo;
            int playerCurrentStep = pawnInfo.step;
            var pawnState = pawnInfo.pawnState;
            switch (pawnState)
            {
                case pawnState.dead:
                    if (number == 6)
                    {
                        if (applyValue)
                        {
                            pawn.setValues(pawnState.alive,0);
                        }
                        valied = true;
                    }
                    break;
                case pawnState.alive:
                    var targateSteps = number + playerCurrentStep;
                    if (targateSteps < totalStepsCount)
                    {
                        // in range
                        bool isCompleted = (targateSteps == (totalStepsCount-1));

                        if (applyValue)
                        {
                            if (isCompleted)
                            {
                                pawn.setValues(pawnState.complected,targateSteps);
                            }
                            else
                            {
                                pawn.setPos(targateSteps);
                            }
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
    public void disSelectAllPawns()
    {
        pawn1.movebul = false;
        pawn2.movebul = false;
        pawn3.movebul = false;
        pawn4.movebul = false;
    }
    //after mouse selected
    public bool movePawn(pawn pawn)
    {
        var moved = false;
        if (ludogame.dice.diceData.isMyTurn && ludogame.dice.diceData.isSelecting)
        {
            moved = canPawnMove(pawn, ludogame.dice.diceData.value, true);
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
        return moved;
    }

    public bool isAllPawnsCompleted()
    {
        return ludoGamePlayerData.isWin();
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
