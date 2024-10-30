using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;
using DIALOGUE;
using TMPro;

namespace TESTING
{
    public class TestCharacter : MonoBehaviour
    {
        private Character CreateCharacter(string name) => CharacterManager.instance.CreateCharacter(name);

        public TMP_FontAsset tempFont;
        // Start is called before the first frame update
        void Start()
        {
            //Character FemaleStudent = CharacterManager.instance.CreateCharacter("Female Pink Hair");
            // Character Elen = CharacterManager.instance.CreateCharacter("Elen");
            // Character Stella = CharacterManager.instance.CreateCharacter("Stella");
            // Character Stella2 = CharacterManager.instance.CreateCharacter("Stella");
            // Character Bambi = CharacterManager.instance.CreateCharacter("Bambi");
            StartCoroutine(Test());
        }

        IEnumerator Test()
        {

            Character_Sprite Alya = CreateCharacter("Alya") as Character_Sprite;
            Character_Sprite Tia = CreateCharacter("Tia") as Character_Sprite;
            
            Alya.SetPosition(new Vector2(0.1f, -0.35f));
            Tia.SetPosition(new Vector2(0.9f, 0));

            yield return new WaitForSeconds(1);

            yield return Tia.Flip(0.5f);

            // yield return Tia.FaceLeft(immediate: true);

            Alya.UnHighlight();
            yield return Tia.Say("Kamu kenapa sih daritadi kayak panik gitu?");

            Tia.UnHighlight();
            Alya.Highlight();
            yield return Alya.Say("Emmmm... Jadi ini Kak, Sebentar lagi aku ada ujian Sejarah.");

            Alya.UnHighlight();
            Tia.Highlight();
            yield return Tia.Say("Oalah itu doang, emang topiknya apaan nanti ujiannya?");

            Tia.UnHighlight();
            Alya.Highlight();
            yield return Alya.Say("Topiknya ini kak, Menjelang kemerdekaan indonesia.");

            Alya.UnHighlight();
            Tia.Highlight();
            yield return Tia.Say("Yaelah itu mah gampang, sini kakak bantu kamu belajar.");

            Tia.UnHighlight();
            Alya.Highlight();
            yield return Alya.Say("Serius Kak? Horeeeeee....");
            Alya.Animate("Hop");

            yield return null;
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
            // Alya.Show();
            // Tia.Show();
            //Alya.SetPosition(new Vector2(0.175f, 0f));
            //Tia.SetPosition(new Vector2(0.825f, 0.75f));
            // Character_Sprite Tia = CreateCharacter("Tia") as Character_Sprite;
            // Alya.SetPosition(new Vector2(0.5f, -0.5f));
            // Tia.isVisible = false;

            // yield return new WaitForSeconds(2);

            // Sprite AlyaSprite = Alya.GetSprite("female student 2 - surprised");
            // yield return Alya.TransitionSprite(AlyaSprite,0,0.75f);

            // yield return new WaitForSeconds(1);
            
            // Alya.MoveToPosition(new Vector2(0.175f, -0.5f));
            // Tia.Show();
            // yield return Tia.MoveToPosition(new Vector2(0.825f, 0.75f));

            // Alya.TransitionSprite(Alya.GetSprite("female student 2 - happy"));

            // Sprite TiaSprite = Tia.GetSprite("Sprite F PinkH Professional Neutral02");
            // Tia.TransitionSprite(TiaSprite,0,0.75f);
            //yield return Tia.TransitionColor(Color.red, speed: 0.1f);
            // yield return Tia.TransitionColor(Color.blue);
            // yield return Tia.TransitionColor(Color.yellow);
            // yield return Tia.TransitionColor(Color.white);


            // Alya.SetPosition(new Vector2(0.1f, -0.35f));
            // Tia.SetPosition(new Vector2(0.9f, 0));

            // yield return new WaitForSeconds(1);

            // yield return Tia.Flip(0.5f);

            // yield return Alya.FaceRight(immediate: true);

            // // yield return Tia.FaceLeft(immediate: true);

            // Alya.UnHighlight();
            // yield return Tia.Say("Kamu kenapa sih daritadi kayak panik gitu?");

            // Tia.UnHighlight();
            // Alya.Highlight();
            // yield return Alya.Say("Emmmm... Jadi ini Kak, Sebentar lagi aku ada ujian Sejarah.");

            // Alya.UnHighlight();
            // Tia.Highlight();
            // yield return Tia.Say("Oalah itu doang, emang topiknya apaan nanti ujiannya?");

            // Tia.UnHighlight();
            // Alya.Highlight();
            // yield return Alya.Say("Topiknya ini kak, Menjelang kemerdekaan indonesia.");

            // Alya.UnHighlight();
            // Tia.Highlight();
            // yield return Tia.Say("Yaelah itu mah gampang, sini kakak bantu kamu belajar.");