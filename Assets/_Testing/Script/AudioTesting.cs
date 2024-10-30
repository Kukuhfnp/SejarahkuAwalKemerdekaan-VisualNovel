using System.Collections;
using System.Collections.Generic;
using CHARACTERS;
using UnityEngine;

public class AudioTesting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Running());
    }
    
    Character CreateCharacter(string name) => CharacterManager.instance.CreateCharacter(name);

    IEnumerator Running2()
    {
        Character_Sprite Alya = CreateCharacter("Alya") as Character_Sprite;
        Alya.Show();

        //yield return new WaitForSeconds(0.5f); 
        
        AudioManager.instance.PlaySoundEffect("Audio/SFX/RadioStatic", loop: true);
        
        yield return new WaitForSeconds(1f);

        //Alya.Animate("Hop");
        //Alya.TransitionSprite(Alya.GetSprite("upset"));
        yield return Alya.Say("Waduh!");

        AudioManager.instance.StopSoundEffect("RadioStatic");
        //AudioManager.instance.PlayVoice("Audio/Voices/exclamation");
        
        Alya.Say("Ah tiba tiba mati");
    }
    IEnumerator Running()
    {
        yield return new WaitForSeconds(1);

        //Character_Sprite Tia = CreateCharacter("Tia") as Character_Sprite;
        Character_Sprite Alya = CreateCharacter("Alya") as Character_Sprite;

        GraphicPanelManager.instance.GetPanel("background").GetLayer(0,true).SetTexture("Graphics/BG Images/02_1");
        AudioManager.instance.PlayTrack("Audio/Ambience/RainyMood",0);
        AudioManager.instance.PlayTrack("Audio/Music/Calm", 1, pitch: 0.7f);

        Alya.Show();

        //AudioManager.instance.PlayVoice("Audio/Voices/exclamation");
        Alya.SetSprite(Alya.GetSprite("upset"));
        Alya.MoveToPosition(new Vector2(0.8f, -0.125f), speed: 0.5f);
        yield return Alya.Say("Aduhhh gimana ini....");

        AudioManager.instance.StopTrack(1);

        // GraphicPanelManager.instance.GetPanel("background").GetLayer(0,true).SetTexture("Graphics/BG Images/02_2");
        // AudioManager.instance.PlayTrack("Audio/Music/Happy", volumeCap: 0.1f);
    }
}
