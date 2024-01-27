using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//tosaaa_3 developer console script
//just add it to an empty gameobject

public class DeveloperConsole : MonoBehaviour
{
    public float consoleHeightPercentage = 0.5f;
    public float consoleHeight = 300f;
    public float labelWidth = 100f;
    public float labelHeight = 20f;
    public float inputFieldWidth = 200f;
    public float inputFieldHeight = 20f;
    public float buttonWidth = 80f;
    public float buttonHeight = 20f;
    public float fontSize = 16f;

    private string inputString = "";
    private List<string> consoleLog = new List<string>();
    private bool isConsoleVisible = false;

    private List<Command> commands = new List<Command>();

    private void Start()
    {
        RegisterCommands();
    }

    private void Update()
    {
        HandleInput();
    }

    private void OnGUI()
    {
        if (isConsoleVisible)
        {
            DrawConsole();
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            isConsoleVisible = !isConsoleVisible;

            if (!isConsoleVisible)
            {
                inputString = ""; // Clear input field when hiding the console
            }
        }

        if (isConsoleVisible)
        {
            // Use Input.GetKeyDown for Enter key
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                ExecuteCommand(inputString);
                inputString = ""; // Clear input field after executing a command
            }
        }
    }

    private void DrawConsole()
    {
    float consoleHeightPixels = Screen.height * consoleHeightPercentage;
    if (consoleHeight > 0)
    {
        consoleHeightPixels = Mathf.Min(consoleHeight, Screen.height);
    }

    GUI.Box(new Rect(0, 0, Screen.width, consoleHeightPixels), "Developer Console");

    GUI.Label(new Rect(10, consoleHeightPixels + 10, labelWidth, labelHeight), "Command:");
    inputString = GUI.TextField(new Rect(labelWidth + 10, consoleHeightPixels + 5, inputFieldWidth, inputFieldHeight), inputString);

    if (GUI.Button(new Rect(labelWidth + inputFieldWidth + 20, consoleHeightPixels + 5, buttonWidth, buttonHeight), "Execute") || Event.current.keyCode == KeyCode.Return)
    {
        ExecuteCommand(inputString);
        inputString = "";
        Event.current.Use(); // Consume the event to prevent other components from using it
    }

    // Display console logs with configurable font size
    GUI.skin.label.fontSize = (int)fontSize;
    for (int i = 0; i < consoleLog.Count; i++)
    {
        float labelY = consoleHeightPixels + 40 + i * (fontSize + 5);
        GUI.Label(new Rect(10, labelY, Screen.width - 20, fontSize + 5), consoleLog[i]);
    }
    }


    private void RegisterCommands()
    {
        AddCommand("hello", () => LogToConsole("Hello, Developer!"));
        AddCommand("clear", () => consoleLog.Clear());
        AddCommand("your_custom_command1", () => LogToConsole("Executing your_custom_command1"));
        AddCommand("your_custom_command2", () => LogToConsole("Executing your_custom_command2"));
        AddCommand("slowmotion", () => SetSlowMotion());
        AddCommand("reloadmap", () => ReloadMap());
        // Add more commands as needed
    }

    private void AddCommand(string command, Action action)
    {
        commands.Add(new Command(command, action));
    }

    private void ExecuteCommand(string command)
    {
        Command foundCommand = commands.Find(cmd => cmd.CommandName.Equals(command, StringComparison.OrdinalIgnoreCase));

        if (foundCommand != null)
        {
            foundCommand.Action.Invoke();
        }
        else
        {
            LogToConsole("Unknown command: " + command);
        }
    }

    private void LogToConsole(string message)
    {
        consoleLog.Add("> " + message);
    }

    private void SetSlowMotion()
    {
        Time.timeScale = 0.1f; // You can adjust the value (0.5f for half speed, 0.1f for very slow, etc.)
        LogToConsole("Slow motion activated!");
    }
    
    private void ReloadMap()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    private class Command
    {
        public string CommandName { get; }
        public Action Action { get; }

        public Command(string commandName, Action action)
        {
            CommandName = commandName;
            Action = action;
        }
    }
}

//dev console by tosaaa_3
//if ur not a dev, leave
