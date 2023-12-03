using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Xml;
using System.Threading.Tasks;
using System;

public enum InputAction
{
    Up,
    Down,
    Right,
    Left,
    Jump,
    Attack,
    Dash,
    Confirm,
    Return,
}

public static class Controls
{
    private static Dictionary<InputAction, KeyCode[]> _bindedKeys = new()
    {
        { InputAction.Up, new KeyCode[] { KeyCode.UpArrow, KeyCode.W } },
        { InputAction.Down, new KeyCode[] { KeyCode.DownArrow, KeyCode.S } },
        { InputAction.Left, new KeyCode[] { KeyCode.LeftArrow, KeyCode.A } },
        { InputAction.Right, new KeyCode[] { KeyCode.RightArrow, KeyCode.D } },
        { InputAction.Jump, new KeyCode[] { KeyCode.Z, KeyCode.J } },
        { InputAction.Attack, new KeyCode[] { KeyCode.X, KeyCode.K } },
        { InputAction.Dash, new KeyCode[] { KeyCode.C, KeyCode.L } },
        { InputAction.Confirm, new KeyCode[] { KeyCode.Return, KeyCode.None } },
        { InputAction.Return, new KeyCode[] { KeyCode.Escape, KeyCode.None } },
    };
    public static void Set(InputAction action, KeyCode key) => _bindedKeys[action][0] = key;
    public static void SetAlt(InputAction action, KeyCode key) => _bindedKeys[action][1] = key;
    public static KeyCode Get(InputAction action) => _bindedKeys[action][0];
    public static KeyCode GetAlt(InputAction action) => _bindedKeys[action][1];
    public static void SaveControls()
    {
        using XmlWriter writer = XmlWriter.Create("Controls.xml");
        writer.WriteStartDocument();
        writer.WriteStartElement("Controls");

        foreach (var pair in _bindedKeys)
        {
            writer.WriteStartElement("Control");

            writer.WriteStartElement("InputAction");
            writer.WriteString(((int)pair.Key).ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("KeyCode");
            writer.WriteString(((int)pair.Value[0]).ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("AltKeyCode");
            writer.WriteString(((int)pair.Value[1]).ToString());
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        writer.WriteEndElement();
        writer.WriteEndDocument();
        writer.Flush();
    }
    public static async Task LoadControls()
    {
        XmlReaderSettings settings = new();
        settings.Async = true;

        using XmlReader reader = XmlReader.Create("Controls.xml", settings);
        while (await reader.ReadAsync())
        {
            if (reader.NodeType != XmlNodeType.Element || reader.Name != "Control")
                continue;

            int inputAction = -1;
            int keyCode = -1;
            int altKeyCode = -1;

            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Control")
                {
                    if (inputAction != -1 && keyCode != -1 && altKeyCode != -1)
                    {
                        Set((InputAction)inputAction, (KeyCode)keyCode);
                        SetAlt((InputAction)inputAction, (KeyCode)altKeyCode);

                        inputAction = -1;
                        keyCode = -1;
                        altKeyCode = -1;
                    }
                }

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "InputAction":
                            if (await reader.ReadAsync() && reader.NodeType == XmlNodeType.Text)
                                inputAction = Int32.Parse(reader.Value);
                            break;
                        case "KeyCode":
                            if (await reader.ReadAsync() && reader.NodeType == XmlNodeType.Text)
                                keyCode = Int32.Parse(reader.Value);
                            break;
                        case "AltKeyCode":
                            if (await reader.ReadAsync() && reader.NodeType == XmlNodeType.Text)
                                altKeyCode = Int32.Parse(reader.Value);
                            break;
                    }
                }
            }
        }
    }
}

public class ControlsActions : MonoBehaviour
{
    public static ControlsActions instance;

    public delegate void OnControlChangedHandler(InputAction action);
    public OnControlChangedHandler OnControlChanged;

    [SerializeField]
    private GameObject _messagePrefab;

    private void Awake() => instance = this;

    public void ChangeBinding(int action)
    {
        ShowChangeBindingMessage(action, false);
    }

    public void ChangeAltBinding(int action)
    {
        ShowChangeBindingMessage(action, true);
    }

    private void ShowChangeBindingMessage(int action, bool isAlt)
    {
        GameObject message = Instantiate(_messagePrefab, GameObject.FindGameObjectWithTag("Messages").transform);
        message.GetComponent<WaitForButtonMap>().SetMappingInfo(this, action, isAlt);
    }

    public void Apply()
    {
        Controls.SaveControls();
    }

    public void Back()
    {
        SceneManager.LoadScene("Settings");
    }
}
