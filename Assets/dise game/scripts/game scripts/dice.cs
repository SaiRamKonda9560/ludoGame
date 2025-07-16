using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class dice : MonoBehaviour
{

    public diceData diceData;
    public Action<int> onDiseRolled;
    public Action<int> onDiceRolledStarted;
    public Action<int> onSelectingStarted;
    public const float selectingDalay = 0.2f;
    public Button Button;
    public List<Sprite> diceRollSprites;
    public List<Sprite> diceNumberSprites;
    public AudioClip rollAudio;

    public float animationTime;
    private bool animateDice;
    private float animationStartTime;
    public ludogame ludogame;
    public void startRolingAnmi()
    {
        animationStartTime = Time.time;
        animateDice = true;

    }
    public Image markImage;
    private void Update()
    {
        markImage.gameObject.SetActive(canRoll());
        if (animateDice)
        {
            var d = Time.time - animationStartTime;
            if(d < 0 || d > animationTime)
            {
                animateDice = false;    
            }
            else
            {
                var index = Mathf.Clamp(Mathf.RoundToInt((d/animationTime)* diceRollSprites.Count),0,diceRollSprites.Count-1);
                Button.image.sprite = diceRollSprites[index];

            }

        }
        else
        {
            if (diceData.value > 0 && (diceData.value-1) < diceNumberSprites.Count && diceNumberSprites.Count > 0)
            {
                Button.image.sprite = diceNumberSprites[(diceData.value - 1)];
            }
        }
    }
    public void onButtonClicked()
    {
        if (canRoll())
        {
            //if can roll
            diceData.isRolling = true;
            roll();
        }
    }
    public bool callingSelecting;
    public void rolled()
    {
        diceData.isRolling = false;
        if (diceData.isMyTurn) 
        {
            onDiseRolled?.Invoke(diceData.value);
            callingSelecting = true;
            Invoke("startSelecting", selectingDalay);
        }
        else
        {
            diceData.isSelecting = false;
        }
    }
    void startSelecting()
    {
        diceData.isSelecting = true;
        callingSelecting = false;
        onSelectingStarted?.Invoke(diceData.value);
    }
    public bool canRoll()
    {
        if (diceData.isMyTurn)
        {
            if (diceData.isRolling || diceData.isSelecting || callingSelecting || ludogame.isAnyPawnMoving)
            {
                return false;
            }
            else
            {
                return true;

            }
        }
        else
        {
            return false;   
        }
    }
    public void clearData()
    {
        diceData.isSelecting = false;
        diceData.isRolling = false;
        diceData.isMyTurn = false;
    } 
    public void roll()
    {
        diceData.value = UnityEngine.Random.Range(1, 7);
        startRolingAnmi();
        audioSpaner.span(rollAudio);
        Invoke("rolled", animationTime);
        if (diceData.isMyTurn)
        {
            onDiceRolledStarted?.Invoke(diceData.value);
        }
    }
    public void roll(int value)
    {
        diceData.value = value;
        startRolingAnmi();
        audioSpaner.span(rollAudio);
        Invoke("rolled", animationTime);
        if (diceData.isMyTurn)
        {
            onDiceRolledStarted?.Invoke(diceData.value);
        }
    }


}
