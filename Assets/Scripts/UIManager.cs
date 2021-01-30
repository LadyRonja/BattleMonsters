using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject startMenu;
    public InputField usernameField;
    public InputField IPField;

    public Text attemptingConnectIP;

    //Singleton pattern
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    //Update debug window displaying in ui where the client is attempting to connect
    private void Start()
    {
        if (attemptingConnectIP != null)
        {
            attemptingConnectIP.text = "Attempting to connect to IP: " + Client.instance.ip;
        }
    }

    //When attempting to connect to the server, make the UI non-interactable
    public void OnConnectedToServer()
    {
        startMenu.SetActive(false);
        usernameField.interactable = false;
        IPField.interactable = false;
        Client.instance.ConnectedToServer();
    }


}
