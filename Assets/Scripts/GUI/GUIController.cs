using Mirror.Discovery;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GUIController : MonoBehaviour
{
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
        CustomNetworkManager.Instance.SetDestinationIP(ipAddressInput.text);
        CustomNetworkManager.Instance.StartClient();
    }

    public void OnHostButtonClick()
    {
        CustomNetworkManager.Instance.StartHost();
    }

    public void Log(string log)
    {
        Debug.Log(log);
    }
}
