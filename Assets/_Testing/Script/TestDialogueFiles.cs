using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using VISUALNOVEL;

public class TestDialogueFiles : MonoBehaviour
{
    [SerializeField] private TextAsset fileToRead = null;
// Start is called before the first frame update
        void Start()
        {
            StartConversation();
        }

        void StartConversation()
        {
            string fullPath = AssetDatabase.GetAssetPath(fileToRead) ;

            int resourcesIndex = fullPath.IndexOf("Resources/");
            string relativePath = fullPath.Substring(resourcesIndex + 10);

            string filePath = Path.ChangeExtension(relativePath, null);

            VNManager.instance.LoadFile(filePath);
        }

        private void Update()
        {
            // if (Input. GetKeyDown(KeyCode.DownArrow))
            //     DialogueSystems.instance.dialogueContainer.Hide();

            // else if (Input. GetKeyDown(KeyCode.UpArrow))
            //     DialogueSystems.instance.dialogueContainer.Show();
        }
}