using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DIALOGUE
{
    [System.Serializable]
    /// <summary>
    /// Merupakan sebuah kotak yang menyimpan nama teks dari layar atau screen. Dan termasuk ke bagian dialog container.
    /// </summary>
    public class NameContainer
    {
        [SerializeField] private GameObject root;
        [field:SerializeField] public TextMeshProUGUI nameText {get; private set;} 

        public void Show(string nameToShow = "")
        {
            root.SetActive(true);

            if (nameToShow != string.Empty)
                nameText.text = nameToShow;
        }

        public void Hide()
        {
            root.SetActive(false);
        }

        public void SetNameColor(Color color) => nameText.color = color;
        public void SetNameFont(TMP_FontAsset font) => nameText.font = font;
        public void SetNameFontSize(float size) => nameText.fontSize = size;
    }
}