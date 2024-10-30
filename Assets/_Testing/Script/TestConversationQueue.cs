using System.Collections;
using System.Collections.Generic;
using DIALOGUE;
using UnityEngine;

public class TestConversationQueue : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Running());
    }

    IEnumerator Running()
    {
        List<string> lines = new List<string>()
        {
            "Line 1.",
            "Line 2.",
            "Line 3."
        };

        yield return DialogueSystems.instance.Say(lines);

        DialogueSystems.instance.Hide();
    }

    // Update is called once per frame
    void Update()
    {
        List<string> lines = new List<string>();
        Conversation conversation = null;

        if (Input. GetKeyDown(KeyCode.Q))
        {
            lines = new List<string>()
            {
                "This is the start of an enqueued conversation.",
                "We can keep it going!"
            };
            conversation = new Conversation(lines);
            DialogueSystems.instance. conversationManager. Enqueue(conversation);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            
            lines = new List<string>()
            {
                "This is an important conversation!",
                "August 26, 2023 is international.dog day!"
            };
            conversation = new Conversation(lines);
            DialogueSystems. instance. conversationManager. EnqueuePriority(conversation);
        }
    }
}
