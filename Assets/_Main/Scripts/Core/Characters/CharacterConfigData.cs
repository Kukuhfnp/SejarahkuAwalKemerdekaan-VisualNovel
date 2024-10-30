using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using DIALOGUE;
using TMPro;
using UnityEngine;

namespace CHARACTERS
{
    [System.Serializable]
    public class CharacterConfigData
    {
        public string name;
        public string alias;
        public Character.CharacterType characterType;

        public Color nameColor;
        public Color dialogueColor;

        public TMP_FontAsset nameFont;
        public TMP_FontAsset dialogueFont;

        public float nameFontSize;
        public float dialogueFontSize;

        [SerializedDictionary("Path / ID", "Sprite")]
        public SerializedDictionary<string, Sprite> sprites = new SerializedDictionary<string, Sprite>();

        public CharacterConfigData Copy()
        {
            CharacterConfigData result = new CharacterConfigData();

            result.name = name;
            result.alias = alias;
            result.characterType = characterType;
            result.nameFont = nameFont;
            result.dialogueFont = dialogueFont;

            result.nameColor = new Color(nameColor.r, nameColor.g, nameColor.b, 1f);
            result.dialogueColor = new Color(dialogueColor.r, dialogueColor.g, 1f);

            result.dialogueFontSize = dialogueFontSize;
            result.nameFontSize = nameFontSize;

            return result;  
        }

        private static Color defaultColor => DialogueSystems.instance.config.defaultTextColor;
        private static TMP_FontAsset defaultFont => DialogueSystems.instance.config.defaultFont;

        public static CharacterConfigData Default
        {
            get
            {
                CharacterConfigData result = new CharacterConfigData();

                result.name = "";
                result.alias = "";
                result.characterType = Character.CharacterType.Text;

                result.nameFont = defaultFont;
                result.dialogueFont = defaultFont;
                result.nameColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b, defaultColor.a);
                result.dialogueColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b, defaultColor.a);

                result.dialogueFontSize = DialogueSystems.instance.config.defaultDialogueFontSize;
                result.nameFontSize = DialogueSystems.instance.config.defaultNameFontSize;

                return result;
            }
        }
    }
}