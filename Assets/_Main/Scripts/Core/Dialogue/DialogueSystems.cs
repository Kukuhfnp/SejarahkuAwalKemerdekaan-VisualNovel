using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CHARACTERS;
using System.Runtime.InteropServices;

namespace DIALOGUE
{
    public class DialogueSystems : MonoBehaviour
    {
        [SerializeField] private DialogueSystemConfigurationSO _config;
        public DialogueSystemConfigurationSO config => _config; //untuk memanggil kelas DialogueSystemConfigurationSO

        public DialogueContainer dialogueContainer = new DialogueContainer(); //untuk memanggil kelas DialogueContainer agar variable dan juga fungsi di dalam kelas ini dapat digunakan
        public ConversationManager conversationManager {get; private set;}
        private TextArchitect architect; //variable untuk memanggil kelas TextArchitect
        private AutoReader autoReader; //variable untuk memanggil kelas AutoReader sehingga fungsi yang terdapat pada kelas AutoReader dapat dipanggil
        [SerializeField] private CanvasGroup mainCanvas; //untuk menyimpan nilai variable yang nantinya akan diisi melalui inspector unity

        public static DialogueSystems instance {get; private set;}

        public delegate void DialogueSystemsEvent();

        public event DialogueSystemsEvent onUserPrompt_Next; //digunakan untuk melanjutkan teks percakapan jika menerima inputan dari pemain
        public event DialogueSystemsEvent onClear; //digunakan untuk clear atau menghapus percakapan

        public bool isRunningConversation => conversationManager.isRunning; //indikator untuk menentukan apakah konversasi atau percakapan sedang berjalan atau tidak

        public DialogueContinuePrompt prompt; //untuk memanggil kelas DialogueContinuePrompt
        private CanvasGroupController cgController; //untuk mengatur canvas utama apakah akan ditampilkan atau tidak

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                Initialize();
            }
            else
                DestroyImmediate(gameObject);
        }

        bool _initialized = false;
        private void Initialize()
        {
            if(_initialized)
                return;

            architect = new TextArchitect(dialogueContainer.dialogueText);
            conversationManager = new ConversationManager(architect);

            cgController = new CanvasGroupController(this, mainCanvas);
            dialogueContainer.Initialize();

            if(TryGetComponent(out autoReader))
                autoReader.Initialize(conversationManager);
        }

        public void OnUserPrompt_Next()
        {
            //? digunakan agar jk terdapat nilai null maka perintah tidak melakukan apa apa sehingga tidak menimbulkan eror
            onUserPrompt_Next?.Invoke();

            if(autoReader != null && autoReader.isOn)
                autoReader.Disable();
        }

        public void OnSystemPrompt_Next()
        {
            //? digunakan agar jk terdapat nilai null maka perintah tidak melakukan apa apa sehingga tidak menimbulkan eror
            onUserPrompt_Next?.Invoke();
        }

        public void OnSystemPrompt_Clear()
        {
            onClear ? .Invoke();
        }

        public void OnStartViewingHistory()
        {
            prompt.Hide();
            autoReader.allowToggle = false;
            conversationManager.allowUserPrompts = false;

            if (autoReader. isOn)
                autoReader.Disable();
        }

        public void OnStopViewingHistory()
        {
            prompt.Show();
            autoReader.allowToggle = true;
            conversationManager.allowUserPrompts = true;
        }

        public void ApplySpeakerDataToDialogueContainer(string speakerName)
        {
            Character character = CharacterManager.instance.GetCharacter(speakerName);
            CharacterConfigData config = character != null ? character.config : CharacterManager.instance.GetCharacterConfig(speakerName);

            ApplySpeakerDataToDialogueContainer(config); 
        }

        public void ApplySpeakerDataToDialogueContainer(CharacterConfigData config)
        {
            dialogueContainer.SetDialogueColor(config.dialogueColor);
            dialogueContainer.SetDialogueFont(config.dialogueFont);
            dialogueContainer.SetDialogueFontSize(config.dialogueFontSize * this.config.dialogueFontScale);
            dialogueContainer.nameContainer.SetNameColor(config.nameColor);
            dialogueContainer.nameContainer.SetNameFont(config.nameFont);
            dialogueContainer.nameContainer.SetNameFontSize(config.nameFontSize);

        }

        public void ShowSpeakerName(string speakerName = "")
        {
            if(speakerName.ToLower() != "narrator")
                dialogueContainer.nameContainer.Show(speakerName);
            else
            {
                HideSpeakerName();
                dialogueContainer.nameContainer.nameText.text = "";
            }
                
        }
        public void HideSpeakerName() => dialogueContainer.nameContainer.Hide();
        
        public Coroutine Say(string speaker, string dialogue)
        {
            List<string> conversation = new List<string>() { $"{speaker} \"{dialogue}\"" };
            return Say(conversation);
        }

        public Coroutine Say(List<string> lines, string filePath = "")
        {
            Conversation conversation = new Conversation(lines, file: filePath);
            return conversationManager.StartConversation(conversation);
        }
        public Coroutine Say(Conversation conversation)
        {
            return conversationManager.StartConversation(conversation);
        }

        public bool isVisible => cgController.isVisible;
        public Coroutine Show(float speed = 1f, bool immediate = false) => cgController.Show(speed,immediate);

        public Coroutine Hide(float speed = 1f, bool immediate = false) => cgController.Hide(speed,immediate);
    }
}