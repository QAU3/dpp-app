using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using System.Net;

public class UIManager : MonoBehaviour
{

    public InputField discoveryMessage, ipAdressField, portField;
    public Button findServerBtn, connectBtn;
    public ServerDiscovery serverDiscovery;
    public Toggle toggle;
    string _ip;
    int _port;

    public void Awake()
    {
        discoveryMessage.text = "findServer";
        ipAdressField.text = "";
        portField.text = "6540";

        ipAdressField.onEndEdit.AddListener(GetIPAdress);
        portField.onEndEdit.AddListener(GetPort);
        findServerBtn.onClick.AddListener(FindServerAction);
        connectBtn.onClick.AddListener(ConnectAction);
        toggle.onValueChanged.AddListener(DefineFontType);

    }

    private void DefineFontType(bool arg0)
    {
        int font_type = 1;
        if (arg0==false) { font_type = 0; }
        PlayerPrefs.SetInt("FontType", font_type);
    }

    private void ConnectAction()
    {
    
        SceneManager.LoadScene("ViewerScene");
        

    }

    private void FindServerAction()
    {
        serverDiscovery.allCommands = true;
        serverDiscovery.clientCommand = discoveryMessage.text;
        serverDiscovery.FindServer(SetIP);
    }

    private void SetIP(IPEndPoint obj)
    {
        string _IPADRESS= obj.Address.ToString();
        ipAdressField.text = _IPADRESS;
        _ip = _IPADRESS;
        PlayerPrefs.SetString("SERVER_IP", _IPADRESS);

    }

    private void GetPort(string arg0)
    {
        _port = int.Parse(arg0);
    }

    private void GetIPAdress(string arg0)
    {
        _ip = arg0;
    }
}
