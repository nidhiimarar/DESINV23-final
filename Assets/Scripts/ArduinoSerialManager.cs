using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO.Ports;
using System.Threading;

public class ArduinoSerialManager : MonoBehaviour
{
    [SerializeField] private string portName = "COM3"; // Windows: COM3, COM4 etc. Mac/Linux: /dev/ttyUSB0
    [SerializeField] private int baudRate = 9600;

    private SerialPort serialPort;
    private Thread serialThread;
    private bool isRunning = false;

    // Scene name → single character command sent to Arduino
    private const string CMD_OCEAN      = "O";
    private const string CMD_LIGHTHOUSE = "L";
    private const string CMD_MENU       = "M";

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject); // persist across scenes
        SceneManager.sceneLoaded += OnSceneLoaded;
        OpenPort();
    }

    void OpenPort()
    {
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.ReadTimeout  = 100;
            serialPort.WriteTimeout = 100;
            serialPort.Open();

            isRunning = true;
            serialThread = new Thread(SerialReadLoop) { IsBackground = true };
            serialThread.Start();

            Debug.Log($"[Arduino] Port {portName} opened.");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[Arduino] Could not open port: {e.Message}");
        }
    }

    // Background thread — keeps the main thread from freezing if Arduino is slow
    void SerialReadLoop()
    {
        while (isRunning && serialPort != null && serialPort.IsOpen)
        {
            try
            {
                string line = serialPort.ReadLine();
                Debug.Log($"[Arduino] Received: {line}");
            }
            catch (System.TimeoutException) { } // normal, just no data right now
            catch (System.Exception e)
            {
                Debug.LogWarning($"[Arduino] Read error: {e.Message}");
            }
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "Ocean":      SendCommand(CMD_OCEAN);      break;
            case "Lighthouse": SendCommand(CMD_LIGHTHOUSE); break;
            default:           SendCommand(CMD_MENU);       break; // main menu or any other scene
        }
    }

    public void SendCommand(string cmd)
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                serialPort.WriteLine(cmd);
                Debug.Log($"[Arduino] Sent: {cmd}");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[Arduino] Write error: {e.Message}");
            }
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        isRunning = false;

        if (serialPort != null && serialPort.IsOpen)
            serialPort.Close();
    }
}