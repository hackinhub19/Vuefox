

namespace GoogleARCore.Examples.CloudAnchors
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.Networking.Match;
    using UnityEngine.Networking.Types;
    using UnityEngine.UI;

    
#pragma warning disable 618
    [RequireComponent(typeof(NetworkManager))]
#pragma warning restore 618
    public class NetworkManagerUIController : MonoBehaviour
    {
        
        public Canvas LobbyScreen;

        
        public Text SnackbarText;

       
        public GameObject CurrentRoomLabel;

        >
        public CloudAnchorsExampleController CloudAnchorsExampleController;

       
        public GameObject RoomListPanel;

        
        public Text NoPreviousRoomsText;

       
        public GameObject JoinRoomListRowPrefab;

        
        private const int k_MatchPageSize = 5;

       
#pragma warning disable 618
        private NetworkManager m_Manager;
#pragma warning restore 618

       
        private string m_CurrentRoomNumber;

        
        private List<GameObject> m_JoinRoomButtonsPool = new List<GameObject>();

        
        public void Awake()
        {
            
            for (int i = 0; i < k_MatchPageSize; i++)
            {
                GameObject button = Instantiate(JoinRoomListRowPrefab);
                button.transform.SetParent(RoomListPanel.transform, false);
                button.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(0, -100 - (200 * i));
                button.SetActive(true);
                button.GetComponentInChildren<Text>().text = string.Empty;
                m_JoinRoomButtonsPool.Add(button);
            }

#pragma warning disable 618
            m_Manager = GetComponent<NetworkManager>();
#pragma warning restore 618
            m_Manager.StartMatchMaker();
            m_Manager.matchMaker.ListMatches(
                startPageNumber: 0,
                resultPageSize: k_MatchPageSize,
                matchNameFilter: string.Empty,
                filterOutPrivateMatchesFromResults: false,
                eloScoreTarget: 0,
                requestDomain: 0,
                callback: _OnMatchList);
            _ChangeLobbyUIVisibility(true);
        }

       
        public void OnCreateRoomClicked()
        {
            m_Manager.matchMaker.CreateMatch(m_Manager.matchName, m_Manager.matchSize,
                                           true, string.Empty, string.Empty, string.Empty,
                                           0, 0, _OnMatchCreate);
        }

       
        public void OnRefhreshRoomListClicked()
        {
            m_Manager.matchMaker.ListMatches(
                startPageNumber: 0,
                resultPageSize: k_MatchPageSize,
                matchNameFilter: string.Empty,
                filterOutPrivateMatchesFromResults: false,
                eloScoreTarget: 0,
                requestDomain: 0,
                callback: _OnMatchList);
        }

        
        public void OnAnchorInstantiated(bool isHost)
        {
            if (isHost)
            {
                SnackbarText.text = "Hosting Cloud Anchor...";
            }
            else
            {
                SnackbarText.text =
                    "Cloud Anchor added to session! Attempting to resolve anchor...";
            }
        }

       
        public void OnAnchorHosted(bool success, string response)
        {
            if (success)
            {
                SnackbarText.text = "Cloud Anchor successfully hosted! Tap to place more stars.";
            }
            else
            {
                SnackbarText.text = "Cloud Anchor could not be hosted. " + response;
            }
        }

       
        public void OnAnchorResolved(bool success, string response)
        {
            if (success)
            {
                SnackbarText.text = "Cloud Anchor successfully resolved! Tap to place more stars.";
            }
            else
            {
                SnackbarText.text =
                    "Cloud Anchor could not be resolved. Will attempt again. " + response;
            }
        }

        
        public void ShowErrorMessage(string errorMessage)
        {
            SnackbarText.text = errorMessage;
        }

       
#pragma warning disable 618
        private void _OnJoinRoomClicked(MatchInfoSnapshot match)
#pragma warning restore 618
        {
            m_Manager.matchName = match.name;
            m_Manager.matchMaker.JoinMatch(match.networkId, string.Empty, string.Empty,
                                         string.Empty, 0, 0, _OnMatchJoined);
            CloudAnchorsExampleController.OnEnterResolvingModeClick();
        }

        
#pragma warning disable 618
        private void _OnMatchList(
            bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
#pragma warning restore 618
        {
            m_Manager.OnMatchList(success, extendedInfo, matches);
            if (!success)
            {
                SnackbarText.text = "Could not list matches: " + extendedInfo;
                return;
            }

            if (m_Manager.matches != null)
            {
                
                foreach (GameObject button in m_JoinRoomButtonsPool)
                {
                    button.SetActive(false);
                    button.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                    button.GetComponentInChildren<Text>().text = string.Empty;
                }

                NoPreviousRoomsText.gameObject.SetActive(m_Manager.matches.Count == 0);

               
                int i = 0;
#pragma warning disable 618
                foreach (var match in m_Manager.matches)
#pragma warning restore 618
                {
                    if (i >= k_MatchPageSize)
                    {
                        break;
                    }

                    var text = "Room " + _GetRoomNumberFromNetworkId(match.networkId);
                    GameObject button = m_JoinRoomButtonsPool[i++];
                    button.GetComponentInChildren<Text>().text = text;
                    button.GetComponentInChildren<Button>().onClick.AddListener(() =>
                        _OnJoinRoomClicked(match));
                    button.SetActive(true);
                }
            }
        }

        
#pragma warning disable 618
        private void _OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
#pragma warning restore 618
        {
            m_Manager.OnMatchCreate(success, extendedInfo, matchInfo);
            if (!success)
            {
                SnackbarText.text = "Could not create match: " + extendedInfo;
                return;
            }

            m_CurrentRoomNumber = _GetRoomNumberFromNetworkId(matchInfo.networkId);
            _ChangeLobbyUIVisibility(false);
            SnackbarText.text = "Find a plane, tap to create a Cloud Anchor.";
            CurrentRoomLabel.GetComponentInChildren<Text>().text = "Room: " + m_CurrentRoomNumber;
        }

        
#pragma warning disable 618
        private void _OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
#pragma warning restore 618
        {
            m_Manager.OnMatchJoined(success, extendedInfo, matchInfo);
            if (!success)
            {
                SnackbarText.text = "Could not join to match: " + extendedInfo;
                return;
            }

            m_CurrentRoomNumber = _GetRoomNumberFromNetworkId(matchInfo.networkId);
            _ChangeLobbyUIVisibility(false);
            SnackbarText.text = "Waiting for Cloud Anchor to be hosted...";
            CurrentRoomLabel.GetComponentInChildren<Text>().text = "Room: " + m_CurrentRoomNumber;
        }

        
        private void _ChangeLobbyUIVisibility(bool visible)
        {
            LobbyScreen.gameObject.SetActive(visible);
            CurrentRoomLabel.gameObject.SetActive(!visible);
            foreach (GameObject button in m_JoinRoomButtonsPool)
            {
                bool active = visible && button.GetComponentInChildren<Text>().text != string.Empty;
                button.SetActive(active);
            }
        }

        private string _GetRoomNumberFromNetworkId(NetworkID networkID)
        {
            return (System.Convert.ToInt64(networkID.ToString()) % 10000).ToString();
        }
    }
}
