using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using TMPro;
using Marsion.UI;
using System.Collections;
using UnityEngine.UI;

namespace Marsion
{
    public class UI_Connect : UI_Popup
    {
        [SerializeField] bool IsRelay;
        [SerializeField] TMP_InputField inputField;
        [SerializeField] GameObject panel;

        [SerializeField] Button Button_Host;
        [SerializeField] Button Button_Guest;
        [SerializeField] Button Button_StartDraft;

        [SerializeField] GameObject content_Host;
        [SerializeField] GameObject content_Guest;
        [SerializeField] GameObject content_DeckWarning_1;
        [SerializeField] GameObject content_DeckWarning_2;
        [SerializeField] GameObject content_StartDraft;

        public override void Init()
        {

        }

        public void Clear()
        {

        }

        public async void OnClickHost()
        {
            Managers.Logger.Log<UI_Connect>("Start Host", colorName: ColorCodes.CommonUI);

            Button_Host.interactable = false;
            Button_Guest.interactable = false;

            if (IsRelay)
            {
                inputField.text = await StartHostWithRelay(2);
            }
            else
            {
                Managers.Network.StartHost();
            }

            StartCoroutine(DisplayContentsWithDelay(1f, true));
        }

        public async void OnClickGuest()
        {
            Managers.Logger.Log<UI_Connect>("Start Guest", colorName: ColorCodes.CommonUI);

            Button_Host.interactable = false;
            Button_Guest.interactable = false;

            if (IsRelay)
            {
                bool result = await StartClientWithRelay(inputField.text);

                Managers.Logger.Log<UI_Connect>($"Try connect with join code : {inputField.text}.");

                if (result)
                {
                    Managers.Logger.Log<UI_Connect>("Connection succeed.");
                }
                else
                    Managers.Logger.Log<UI_Connect>("Connection failed.");
            }
            else
            {
                Managers.Network.StartClient();
            }

            StartCoroutine(DisplayContentsWithDelay(1f, false));
        }

        public void OnClickStartDraft()
        {
            Button_StartDraft.interactable = false;

            Managers.UI.ClosePopupUI(this);

            Managers.Logger.Log<UI_Connect>($"Start Draft", colorName: ColorCodes.CommonUI);
            Managers.Client.Draft.RequestStartDraft();
        }

        private IEnumerator DisplayContentsWithDelay(float delay, bool isHost)
        {
            if (isHost)
                content_Host.SetActive(true);
            else
                content_Guest.SetActive(true);

            yield return new WaitForSeconds(delay);

            content_DeckWarning_1.SetActive(true);

            yield return new WaitForSeconds(delay);

            content_DeckWarning_2.SetActive(true);

            yield return new WaitForSeconds(delay);

            content_StartDraft.SetActive(true);
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