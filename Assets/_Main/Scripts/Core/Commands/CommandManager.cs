using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;
using UnityEngine.Events;
using Unity.VisualScripting;
using CHARACTERS;

namespace COMMANDS
{
    public class CommandManager : MonoBehaviour
    {
        private const char SUB_COMMAND_IDENTIFIER = '.';
        public const string DATABASE_CHARACTER_BASE = "characters";
        public const string DATABASE_CHARACTER_SPRITE = "characters_sprite";
        public static CommandManager instance {get; private set;}

        private CommandDatabase database;
        private Dictionary<string, CommandDatabase> subDatabases = new Dictionary<string, CommandDatabase>();

        private List<CommandProcess> activeProcesses = new List<CommandProcess>();
        private CommandProcess topProcess => activeProcesses.Last();

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;

                database = new CommandDatabase();

                Assembly assembly = Assembly.GetExecutingAssembly();
                Type[] extensionTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(CMD_DatabaseExtension))).ToArray();

                foreach(Type extension in extensionTypes)
                {
                    MethodInfo extendMethod = extension.GetMethod("Extend");
                    extendMethod.Invoke(null, new object[] { database });
                }
            }
            else
                DestroyImmediate(gameObject);
        }

        public CoroutineWrapper Execute(string commandName, params string[] args)
        {
            if(commandName.Contains(SUB_COMMAND_IDENTIFIER))
                return ExecuteSubCommand(commandName, args);

            Delegate command = database.GetCommand(commandName);

            if(command == null)
                return null;

            return StartProcess(commandName, command, args);
        }

        private CoroutineWrapper ExecuteSubCommand(string commandName, string[] args)
        {
            string[] parts = commandName.Split(SUB_COMMAND_IDENTIFIER);
            string databaseName = string.Join(SUB_COMMAND_IDENTIFIER, parts.Take(parts.Length - 1));
            string subCommandName = parts.Last();

            if(subDatabases.ContainsKey(databaseName))
            {
                Delegate command = subDatabases[databaseName].GetCommand(subCommandName);
                if(command != null)
                {
                    return StartProcess(commandName, command, args);
                }
                else
                {
                    Debug.LogError($"No Command called '{subCommandName}' was found in sub database '{databaseName}'");
                    return null;
                }
            }

            string characterName = databaseName;
            //if we've made it here then we should try to run as a character command
            if(CharacterManager.instance.HasCharacter(databaseName))
            {
                List<string> newArgs = new List<string>(args);
                newArgs.Insert(0, characterName);
                args = newArgs.ToArray();

                return ExecuteCharacterCommand(subCommandName, args);
            }
            
            Debug.LogError($"No sub database called '{databaseName}' exist! Command '{subCommandName}' could not be run.");
            return null;
        }

        private CoroutineWrapper ExecuteCharacterCommand(string commandName, params string[] args)
        {
            Delegate command = null;

            CommandDatabase dbBase = subDatabases[DATABASE_CHARACTER_BASE];
            CommandDatabase dbSprite = subDatabases[DATABASE_CHARACTER_BASE];
            if (dbBase.HasCommand(commandName))
            {
                command = dbBase.GetCommand(commandName);
                return StartProcess(commandName, command, args);
            }
            else if(dbSprite.HasCommand(commandName))
            {
                command = dbSprite.GetCommand(commandName);
                return StartProcess(commandName, command, args);
            }

            CharacterConfigData characterConfigData = CharacterManager.instance.GetCharacterConfig(args[0]);
            switch (characterConfigData.characterType)
            {
                case Character.CharacterType.Sprite:
                case Character.CharacterType.SpriteSheet:
                    dbBase = subDatabases[DATABASE_CHARACTER_BASE];
                    dbSprite = subDatabases[DATABASE_CHARACTER_SPRITE];
                    break;
            }

            if (dbBase.HasCommand(commandName))
            {
                command = dbBase.GetCommand(commandName);
            }
            else if(dbSprite.HasCommand(commandName))
            {
                command = dbSprite.GetCommand(commandName);
            }
            // command = dbBase.GetCommand(commandName);

            if(command != null)
                return StartProcess(commandName, command, args);

            Debug.LogError($"Command manager was unable to execute command '{commandName}' on character '{args[0]}'. The character name or command may be invalid.");
            return null;
        }

        private CoroutineWrapper StartProcess(string commandName, Delegate command, string[] args)
        {
            System.Guid processId = System.Guid.NewGuid();
            CommandProcess cmd = new CommandProcess(processId, commandName, command, null, args, null);
            activeProcesses.Add(cmd);

            Coroutine co = StartCoroutine(RunningProcess(cmd));

            cmd.runningProcess = new CoroutineWrapper(this, co);

            return cmd.runningProcess;
        }

        public void StopCurrentProcess()
        {
            if(topProcess != null)
                KillProcess(topProcess);
        }

        public void StopAllProcess()
        {
            foreach(var c in activeProcesses)
            {
                if(c.runningProcess != null && !c.runningProcess.isDone)
                    c.runningProcess.Stop();
                
                c.onTerminateAction?.Invoke();
            }

            activeProcesses.Clear();
        }

        private IEnumerator RunningProcess(CommandProcess process)
        {
            yield return WaitingForProcessToComplete(process.command, process.args);

            KillProcess(process);
        }

        public void KillProcess(CommandProcess cmd)
        {
            activeProcesses.Remove(cmd);

            if(cmd.runningProcess != null && !cmd.runningProcess.isDone)
                cmd.runningProcess.Stop();
            
            cmd.onTerminateAction?.Invoke();
        }

        private IEnumerator WaitingForProcessToComplete(Delegate command, string[] args)
        {
            if(command is Action)
                command.DynamicInvoke();

            else if(command is Action<string>)
                command.DynamicInvoke(args.Length == 0 ? string.Empty : args[0]);

            else if(command is Action<string[]>)
                command.DynamicInvoke((object)args);
            
            else if ( command is Func<IEnumerator>)
                yield return ((Func<IEnumerator>)command)();

            else if ( command is Func<string, IEnumerator>)
                yield return ((Func<string, IEnumerator>)command)(args.Length == 0 ? string.Empty : args[0]);

            else if ( command is Func<string[],IEnumerator>)
                yield return ((Func<string[],IEnumerator>)command)(args);
        }

        public void AddTerminationActionToCurrentProcess(UnityAction action)
        {
            CommandProcess process = topProcess;

            if(process == null)
                return;
            
            process.onTerminateAction = new UnityEvent();
            process.onTerminateAction.AddListener(action);
        }

        public CommandDatabase CreateSubDatabase(string name)
        {
            name = name.ToLower();

            if(subDatabases.TryGetValue(name, out CommandDatabase db))
            {
                Debug.LogWarning($"A database by the name of '{name}' is already exist!.");
                return db;
            }
            CommandDatabase newDatabase = new CommandDatabase();
            subDatabases.Add(name, newDatabase);

            return newDatabase;
        }
    }   
}
