using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Marsion
{
    public class UI_Relay : MonoBehaviour
    {
        [SerializeField] TMP_InputField inputField;
        [SerializeField] GameObject panel;

        public void Start()
        {
            Managers.Server.OnConnect -= HidePanel;
            Managers.Server.OnConnect += HidePanel;
        }

        public void OnDisable()
        {
            Managers.Server.OnConnect -= HidePanel;
        }

        public async void OnClickHost()
        {
            Managers.Logger.Log<UI_Relay>("Start Host");

            inputField.text = await StartHostWithRelay(2);
        }

        public async void OnClickGuest()
        {
            bool result = await StartClientWithRelay(inputField.text);

            Managers.Logger.Log<UI_Relay>($"Try connect with join code : {inputField.text}.");

            if (result)
            {
                Managers.Logger.LogWarning<UI_Relay>("Connection succeed.");
            }
            else
                Managers.Logger.LogWarning<UI_Relay>("Connection failed.");
        }

        private void HidePanel()
        {
            panel.SetActive(false);
        }

        private async Task<string> StartHostWithRelay(int maxConnecitions=2)
        {
            await UnityServices.InitializeAsync();

            if(!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnecitions);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

            return NetworkManager.Singleton.StartHost() ? joinCode : null;
        }

        private async Task<bool> StartClientWithRelay(string joinCode)
        {
            //Initialize the Unity Services engine
            await UnityServices.InitializeAsync();
            //Always authenticate your users beforehand
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                //If not already logged, log the user in
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            // Join allocation
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
            // Configure transport
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            // Start client
            return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
        }
    }
}