using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIALOGUE;

namespace TESTING
{
    public class Testing_Architect : MonoBehaviour
    {
        DialogueSystems ds;
        TextArchitect architect;

        public TextArchitect.BuildMethod bm = TextArchitect.BuildMethod.instant;

        string[] lines = new string[5]
        {
            "This is a random line of dialogue",
            "I want to say something, come over here",
            "The world is a crazy place sometimes.",
            "Don't lose hope, things will get better!",
            "It's a bird? It's a plane? No! - It's Super Sheltie!"
        };
        
        // Start is called before the first frame update
        void Start()
        {
            ds = DialogueSystems.instance;
            architect = new TextArchitect(ds.dialogueContainer.dialogueText);
            architect.buildMethod = TextArchitect.BuildMethod.fade;
            architect.speed = 0.5f;
        }

        // Update is called once per frame
        void Update()
        {   
            if(bm != architect.buildMethod)
            {
                architect.buildMethod = bm;
                architect.Stop();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                architect.Stop();
            }
            
            string Longline = "this is very long line and you cant read this with super fast, pokoknya ini kalimat panjang karena gatau mau bahas apa. Bingung juga jadinya ini bakal gimana klo gatau yaudah lah ya gitu aja sekian dan terimakasih.";
            if(Input.GetKeyDown(KeyCode.Space))
            {
                if(architect.isBuilding)
                {
                    if(!architect.hurryUp)
                    {
                        architect.hurryUp = true;
                    }
                    else
                    {
                        architect.ForceComplete();
                    }
                }
                else
                    //architect.Build(lines[Random.Range(0, lines.Length)]);
                    architect.Build(Longline);
            }
            else if(Input.GetKeyDown(KeyCode.A))
            {
                //architect.Append(lines[Random.Range(0, lines.Length)]);
                architect.Append(Longline);
            }
        }
    }
}