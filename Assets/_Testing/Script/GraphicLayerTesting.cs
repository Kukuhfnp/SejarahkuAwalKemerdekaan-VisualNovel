using System.Collections;
using System.Collections.Generic;
using CHARACTERS;
using DIALOGUE;
using UnityEngine;

public class GraphicLayerTesting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Running());
    }
    IEnumerator Running()
    {
        GraphicPanel panel = GraphicPanelManager.instance.GetPanel("Background");
        GraphicLayer layer0 = panel.GetLayer(0, true);
        GraphicLayer layer1 = panel.GetLayer(1, true);

        layer0.SetVideo("Graphics/BG Videos/Nebula");
        layer1.SetTexture("Graphics/BG Images/spaceshipinterior");

        yield return new WaitForSeconds(2);

        GraphicPanel cinematic = GraphicPanelManager.instance.GetPanel("Cinematic");
        GraphicLayer cinLayer = cinematic.GetLayer(0,true);

        Character Alya = CharacterManager.instance.CreateCharacter("Alya", true);

        yield return Alya.Say("Wahhh aku ada di luar angkasaaaa!!!");

        cinLayer.SetTexture("Graphics/Gallery/pup");

        yield return DialogueSystems.instance.Say("Narrator", "we truly dont deserve dogs");

        cinLayer.Clear();

        yield return new WaitForSeconds(1);

        panel.Clear();
    }
}
