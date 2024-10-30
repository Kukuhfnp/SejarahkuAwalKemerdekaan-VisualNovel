using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static DIALOGUE.LogicalLines.LogicalLineUtils.Encapsulation;
using static DIALOGUE.LogicalLines.LogicalLineUtils.Conditions;

namespace DIALOGUE.LogicalLines
{
    public class LL_Conditioin : ILogicalLine
    {
        public string keyword =>"if";
        private const string Else = "else";
        private readonly string [] CONTAINERS = new string[] { "(", ")" };

        public IEnumerator Execute(DIALOGUE_LINE line)
        {
            string rawCondition = ExtractCondition(line.rawData.Trim());
            bool conditionResult = EvaluateCondition(rawCondition);

            Conversation currentConversation = DialogueSystems.instance.conversationManager.conversation;
            int currentProgress = DialogueSystems.instance.conversationManager.conversationProgress;

            EncapsulatedData ifData = RipEncapsulationData(currentConversation, currentProgress, false, parentStartingIndex: currentConversation.fileStartIndex);
            EncapsulatedData elseData = new EncapsulatedData();

            if (ifData.endingIndex + 1 < currentConversation.Count)
            {
                string nextLine = currentConversation.GetLines()[ifData.endingIndex + 1].Trim() ;
                if (nextLine == Else)
                {
                    elseData = RipEncapsulationData(currentConversation, ifData.endingIndex + 1, false, parentStartingIndex: currentConversation.fileStartIndex);
                }
            }

            currentConversation.SetProgress(elseData.isNull ? ifData.endingIndex : elseData.endingIndex);

            EncapsulatedData selData = conditionResult ? ifData : elseData;
            
            if(!selData.isNull && selData.lines.Count > 0) //seldata.start index && seldata.endindex kudunya?
            {
                //remove the header and encapsulator lines from the conversation index
                selData.startingIndex += 2;//remove header and starting encapsulator
                selData.endingIndex -= 1;//Remove ending encapsulator

                Conversation newConversation = new Conversation(selData.lines, file: currentConversation.file, fileStartIndex: selData.startingIndex, fileEndIndex: selData.endingIndex);
                DialogueSystems.instance.conversationManager.EnqueuePriority(newConversation);
            }

            yield return null;
        }

        public bool Matches(DIALOGUE_LINE line)
        {
            return line.rawData.Trim().StartsWith(keyword);
        }  

        private string ExtractCondition(string line)
        {
            int startIndex = line.IndexOf(CONTAINERS[0]) + 1;
            int endIndex = line.IndexOf(CONTAINERS[1]);
            return line.Substring(startIndex, endIndex - startIndex).Trim();
        }
    }
}