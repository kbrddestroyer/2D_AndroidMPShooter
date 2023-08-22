using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GUIController : MonoBehaviour
{
    [Header("Requred objects")]
    [SerializeField] private CustomNetworkManager networkManager;
    [Header("GUI")]
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject directConnectionUI;
    [SerializeField] private TMP_InputField ipAddressInput;

    public void OnConnectDialogSwitch()
    {
        mainUI.SetActive(!mainUI.activeInHierarchy);
        directConnectionUI.SetActive(!directConnectionUI.activeInHierarchy);
    }

    public void OnConnectButtonClick()
    {
        networkManager.setDestinationIP(ipAddressInput.text);
        networkManager.ConnectPlayer();
    }

    public void OnHostButtonClick()
    {
        networkManager.ConnectHost();
    }
}
