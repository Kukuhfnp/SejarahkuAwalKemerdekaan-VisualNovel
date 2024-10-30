using CHARACTERS;
using COMMANDS;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DIALOGUE.LogicalLines;

namespace DIALOGUE
{
    public class ConversationManager
    {
        private DialogueSystems dialogueSystems => DialogueSystems.instance;
        private Coroutine process = null;
        public bool isRunning => process != null;
        public bool isOnLogicalLine { get; private set; } = false;

        public TextArchitect architect = null;    
        private bool userPrompt = false;

        private LogicalLineManager logicalLineManager;

        public Conversation conversation => (conversationQueue.isEmpty() ? null : conversationQueue.top);
        public int conversationProgress => (conversationQueue.isEmpty() ? -1 : conversationQueue.top.GetProgress());
        private ConversationQueue conversationQueue;

        public bool allowUserPrompts = true;

        public ConversationManager(TextArchitect architect)
        {
            this.architect = architect;
            dialogueSystems.onUserPrompt_Next += OnUserPrompt_Next;

            logicalLineManager = new LogicalLineManager();

            conversationQueue = new ConversationQueue();
        }

        public Conversation[] GetConversationQueue() => conversationQueue.GetReadOnly();

        public void Enqueue(Conversation conversation) => conversationQueue.Enqueue(conversation);
        public void EnqueuePriority(Conversation conversation) => conversationQueue.EnqueuePriority(conversation);

        private void OnUserPrompt_Next()
        {
            if(allowUserPrompts)
                userPrompt = true;
        }

        public Coroutine StartConversation(Conversation conversation)
        {
            StopConversation();
            conversationQueue.Clear();

            Enqueue(conversation);

            process = dialogueSystems.StartCoroutine(RunningConversation());

            return process;
        }

        public void StopConversation()
        {
            if(!isRunning)
                return;
            
            dialogueSystems.StopCoroutine(process);
            process = null;
        }

        IEnumerator RunningConversation()
        {
            while(!conversationQueue.isEmpty())
            {
                Conversation currentConversation = conversation;

                if(currentConversation.HasReachedEnd())
                {
                    conversationQueue.Dequeue();
                    continue;
                }

                string rawLine = currentConversation.CurrentLine();

                //Tidak akan menampilkan blank line atau menjalankan sebuah logic pada blank line tersebut
                if(string.IsNullOrWhiteSpace(rawLine))
                {
                    TryAdvanceConversation(currentConversation);
                    continue;
                }

                DIALOGUE_LINE line = DialogueParser.Parse(rawLine);

                if(logicalLineManager.TryGetLogic(line, out Coroutine logic))
                {
                    isOnLogicalLine = true;
                    yield return logic;
                }
                else
                {
                    //menampilkan dialog
                    if(line.hasDialogue)
                        yield return Line_RunDialogue(line);

                    //Run any commands
                    if(line.hasCommands)
                        yield return Line_RunCommands(line);

                    //wait for user input if dialogue was in this line
                    if(line.hasDialogue)
                    {
                        //wait for user input
                        yield return WaitForUserInput();

                        CommandManager.instance.StopAllProcess();

                        dialogueSystems.OnSystemPrompt_Clear();
                    }
                }

                TryAdvanceConversation(currentConversation);
                isOnLogicalLine = false;
            }

            process = null;
        }

        private void TryAdvanceConversation(Conversation conversation)
        {
            conversation.IncrementProgress();

            if(conversation != conversationQueue.top)
                return;

            if (conversation.HasReachedEnd())
                conversationQueue.Dequeue();
        }

        IEnumerator Line_RunDialogue(DIALOGUE_LINE line)
        {
            //menampilkan atau sembunyikan nama speaker
            if(line.hasSpeaker)
            {
                HandleSpeakerLogic(line.speakerData);
            }

            if(!dialogueSystems.dialogueContainer.isVisible)
                dialogueSystems.dialogueContainer.Show();

            //Membangun dialog
            yield return BuildLineSegments(line.dialogueData);
        }

        private void HandleSpeakerLogic(DL_SPEAKER_DATA speakerData)
        {
            bool characterMustBeCreated = (speakerData.makeCharacterEnter || speakerData.isCastingPosition || speakerData.isCastingExpression);

            Character character = CharacterManager.instance.GetCharacter(speakerData.name, createIfDoesNotExist: characterMustBeCreated);

            if(speakerData.makeCharacterEnter && (!character.isVisible && !character.isRevealing))
                character.Show();

            //Add character name to UI
            dialogueSystems.ShowSpeakerName(TagManager.Inject(speakerData.displayName));

            DialogueSystems.instance.ApplySpeakerDataToDialogueContainer(speakerData.name);

            //Cast position
            if(speakerData.isCastingPosition)
                character.MoveToPosition(speakerData.castPosition);
            
            //Cast Expression
            if(speakerData.isCastingExpression)
            {
                foreach(var ce in speakerData.CastExpressions)
                    character.OnReceiveCastingExpression(ce.layer, ce.expression);
            }
            
        }

        IEnumerator Line_RunCommands(DIALOGUE_LINE line)
        {
            List<DL_COMMAND_DATA.Command> commands = line.commandData.commands;

            foreach(DL_COMMAND_DATA.Command command in commands)
            {
                if(command.waitForCompletion || command.name == "wait")
                {
                    CoroutineWrapper cw = CommandManager.instance.Execute(command.name, command.arguments);
                    while(!cw.isDone)
                    {
                        if(userPrompt)
                        {
                            CommandManager.instance.StopCurrentProcess();
                            userPrompt = false;
                        }
                        yield return null;
                    }
                }
                    
                else
                    CommandManager.instance.Execute(command.name, command.arguments);
            }

            yield return null;
        }

        IEnumerator BuildLineSegments(DL_DIALOGUE_DATA line)
        {
            for(int i = 0; i < line.segments.Count; i++)
            {
                DL_DIALOGUE_DATA.DIALOGUE_SEGMENT segment = line.segments[i];

                yield return WaitForDialogueSegmentSignalToBeTriggered(segment);

                yield return BuildDialogue(segment.dialogue, segment.appendText);
            }
        }

        public bool isWaitingOnAutoTimer { get; private set; } = false; 

        IEnumerator WaitForDialogueSegmentSignalToBeTriggered(DL_DIALOGUE_DATA.DIALOGUE_SEGMENT segment)
        {
            switch(segment.startSignal)
            {
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.C:
                    yield return WaitForUserInput();
                    dialogueSystems.OnSystemPrompt_Clear();
                    break;
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.A:
                    yield return WaitForUserInput();
                    break;
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.WC:
                    isWaitingOnAutoTimer = true;
                    yield return new WaitForSeconds(segment.signalDelay);
                    isWaitingOnAutoTimer = false;
                    dialogueSystems.OnSystemPrompt_Clear();
                    break;
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.WA:
                    isWaitingOnAutoTimer = true;
                    yield return new WaitForSeconds(segment.signalDelay);
                    isWaitingOnAutoTimer = false;
                    break;
                default:
                    break;

            }
        }

        IEnumerator BuildDialogue(string dialogue, bool append = false)
        {
            dialogue = TagManager.Inject(dialogue);

            //Membangun dialog
            if(!append)
                architect.Build(dialogue);
            else
                architect.Append(dialogue);

            while(architect.isBuilding)
            {
                if(userPrompt)
                {
                    if(!architect.hurryUp)
                        architect.hurryUp = true;
                    else
                        architect.ForceComplete();

                    userPrompt = false;
                }
                yield return null;
            }
        }

        IEnumerator WaitForUserInput()
        {
            dialogueSystems.prompt.Show();

            while(!userPrompt)
                yield return null;
            
            dialogueSystems.prompt.Hide();

            userPrompt = false;
        }
    }
}