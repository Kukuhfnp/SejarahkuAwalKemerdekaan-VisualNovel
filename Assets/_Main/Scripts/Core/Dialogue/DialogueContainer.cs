using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace DIALOGUE
{
    [System.Serializable]
    public class DialogueContainer
    {
        public GameObject root;
        public NameContainer nameContainer;
        public TextMeshProUGUI dialogueText;

        private CanvasGroupController cgController;

        public void SetDialogueColor(Color color) => dialogueText.color = color;
        public void SetDialogueFont(TMP_FontAsset font) => dialogueText.font = font;

        public void SetDialogueFontSize(float size) => dialogueText.fontSize = size;

        private bool initialize = false;

        public void Initialize()
        {
            if(initialize)
                return;
            
            cgController = new CanvasGroupController(DialogueSystems.instance, root.GetComponent<CanvasGroup>());
        }
        
        public bool isVisible => cgController.isVisible;
        public Coroutine Show(float speed = 1f, bool immediate = false) => cgController.Show(speed,immediate);
        public Coroutine Hide(float speed = 1f, bool immediate = false) => cgController.Hide(speed,immediate);
    }
}