using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class dice : MonoBehaviour
{
    public diceData diceData;
    public TMP_Text text;
    public Action<int> onDiseRolled;
    public Action<int> onDiceRolledStarted;
    public Action<int> onSelectingStarted;
    public const float rollDalay = 0.5f;
    public const float selectingDalay = 0.5f;
    public Button Button;
    public Image markImage;
    private void Update()
    {
        markImage.gameObject.SetActive(canRoll());
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
    public void rolled()
    {
        diceData.isRolling = false;
        text.text = diceData.value.ToString();
        if(diceData.isMyTurn) 
        {
            onDiseRolled?.Invoke(diceData.value);
            Invoke("startSelecting", selectingDalay);
        }
        else
        {
            diceData.isSelecting = false;
        }
    }
    void startSelecting()
    {
        onSelectingStarted?.Invoke(diceData.value);
        diceData.isSelecting = true;
    }
    public bool canRoll()
    {
        if (diceData.isMyTurn)
        {
            if (diceData.isRolling || diceData.isSelecting)
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
        text.text = "...";
        Invoke("rolled", rollDalay);
        if (diceData.isMyTurn)
        {
            onDiceRolledStarted?.Invoke(diceData.value);
        }
    }
    public void roll(int value)
    {
        diceData.value = value;
        text.text = "...";
        Invoke("rolled", rollDalay);
        if (diceData.isMyTurn)
        {
            onDiceRolledStarted?.Invoke(diceData.value);
        }
    }
}
