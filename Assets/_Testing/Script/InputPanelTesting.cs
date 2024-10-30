using System.Collections;
using System.Collections.Generic;
using CHARACTERS;
using UnityEngine;

public class InputPanelTesting : MonoBehaviour
{
    public InputPanel inputPanel;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Running());
    }

    IEnumerator Running()
    {
        Character Tia = CharacterManager.instance.CreateCharacter("Tia", revealAfterCreation: true);

        yield return Tia.Say("Hi! Whats your name?");

        inputPanel.Show("What is your name?");

        while(inputPanel.isWaitingOnUserInput)
            yield return null;

        string characterName = inputPanel.lastinput;

        yield return Tia.Say($"it's very nice to meet you, {characterName}!");
    }
}
