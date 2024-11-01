using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Marsion.UI
{
    public class UI_DraftPanel : UI_Popup
    {
        DraftState CurrentState;

        [SerializeField] Button Button_Ready;

        [SerializeField] Button_Card Button_Left;
        [SerializeField] Button_Card Button_Center;
        [SerializeField] Button_Card Button_Right;

        [SerializeField] GameObject Content_Root;

        [SerializeField] GameObject Panel_Left;
        [SerializeField] GameObject Panel_LeftExchange;

        [SerializeField] GameObject Panel_Center;
        [SerializeField] GameObject Panel_CenterExchange;

        [SerializeField] GameObject Panel_Right;
        [SerializeField] GameObject Panel_RightExchange;

        [SerializeField] GameObject Panel_Status;

        [SerializeField] TMP_Text Text_Count;
        [SerializeField] TMP_Text Text_State;
        [SerializeField] TMP_Text Text_DeckCount;

        Dictionary<string, GameObject> contentsDictionary;

        public override void Init()
        {
            base.Init();

            Managers.Client.Draft.OnStateUpdate += UpdateState;
            //Managers.Client.Game.OnGameStarted += ClosePopupUI;
            Managers.Client.GameEx.OnGameStarted += ClosePopupUI;

            contentsDictionary = new Dictionary<string, GameObject>();

            InitState(Managers.Client.Draft.State);
            InitButtons();
            InitDeck();
            UpdatePanel();
        }

        private void Clear()
        {
            //Managers.Client.Game.OnGameStarted -= ClosePopupUI;
            Managers.Client.GameEx.OnGameStarted -= ClosePopupUI;
            Managers.Client.Draft.OnStateUpdate -= UpdateState;
        }

        private void InitState(DraftState state)
        {
            CurrentState = state;
        }

        private void InitButtons()
        {
            Button_Left.Button.onClick.AddListener(() =>
            {
                OnClickButton(0);
                SortContentByCost();
                Managers.Client.Draft.Select(0);
            });

            Button_Center.Button.onClick.AddListener(() =>
            {
                OnClickButton(1);
                SortContentByCost();
                Managers.Client.Draft.Select(1);
            });

            Button_Right.Button.onClick.AddListener(() =>
            {
                OnClickButton(2);
                SortContentByCost();
                Managers.Client.Draft.Select(2);
            });

            Button_Ready.onClick.AddListener(Ready);
        }

        private void InitDeck()
        {
            foreach (var soID in CurrentState.CurrentDeck)
            {
                AddCard(soID);
            }

            SortContentByCost();
        }

        private void UpdateState()
        {
            CurrentState = Managers.Client.Draft.State;

            UpdatePanel();
        }

        private void UpdatePanel()
        {
            Text_Count.text = $"남은 횟수 : {CurrentState.Count}";
            Text_DeckCount.text = $"{CurrentState.CurrentDeck.Count}";

            if (CurrentState.IsComplete)
            {
                Panel_Left.SetActive(false);
                Panel_Center.SetActive(false);
                Panel_Right.SetActive(false);
                Panel_Status.SetActive(true);

                Button_Ready.interactable = true;
            }
            else
            {
                Button_Ready.interactable = false;

                UpdateSelections();
            }
        }

        private void UpdateSelections()
        {
            switch (CurrentState.Type)
            {
                case SelectType.Legendary:
                    SetOneSelection();
                    break;
                case SelectType.Table:
                    SetOneSelection();
                    break;
                case SelectType.Exchange:
                    SetExchangeSelection();
                    break;
            }
        }

        private void SetOneSelection()
        {
            Panel_LeftExchange.SetActive(false);
            Button_Left.Setup(CurrentState.CurrentSelections[0]);
            Button_Left.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

            Panel_CenterExchange.SetActive(false);
            Button_Center.Setup(CurrentState.CurrentSelections[1]);
            Button_Center.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

            Panel_RightExchange.SetActive(false);
            Button_Right.Setup(CurrentState.CurrentSelections[2]);
            Button_Right.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        }

        private void SetExchangeSelection()
        {
            Panel_LeftExchange.GetComponentInChildren<Content_Card>().Setup(CurrentState.CurrentSubSelections[0]);
            Panel_LeftExchange.SetActive(true);
            Button_Left.Setup(CurrentState.CurrentSelections[0]);
            Button_Left.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -70);

            Panel_CenterExchange.GetComponentInChildren<Content_Card>().Setup(CurrentState.CurrentSubSelections[1]);
            Panel_CenterExchange.SetActive(true);
            Button_Center.Setup(CurrentState.CurrentSelections[1]);
            Button_Center.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -70);

            Panel_RightExchange.GetComponentInChildren<Content_Card>().Setup(CurrentState.CurrentSubSelections[2]);
            Panel_RightExchange.SetActive(true);
            Button_Right.Setup(CurrentState.CurrentSelections[2]);
            Button_Right.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -70);
        }

        private void OnClickButton(int index)
        {
            switch (CurrentState.Type)
            {
                case SelectType.Legendary:
                    AddCard(CurrentState.CurrentSelections[index]);
                    break;
                case SelectType.Table:
                    AddCard(CurrentState.CurrentSelections[index]);
                    break;
                case SelectType.Exchange:
                    RemoveCard(CurrentState.CurrentSubSelections[index]);
                    AddCard(CurrentState.CurrentSelections[index]);
                    break;
            }
        }   

        private void AddCard(string soID)
        {
            if (contentsDictionary.TryGetValue(soID, out var value))
            {
                value.GetComponent<Content_Card>().IncreaseCount();
            }
            else
            {
                var content_Card = Managers.UI.MakeSubItem<Content_Card>(Content_Root.transform);
                content_Card.Setup(soID);
                content_Card.IncreaseCount();
                contentsDictionary.Add(soID, content_Card.gameObject);
            }
        }

        private void RemoveCard(string soID)
        {
            if (contentsDictionary.TryGetValue(soID, out var value))
            {
                bool IsZero = value.GetComponent<Content_Card>().DecreaseCount();
                if (IsZero)
                {
                    Managers.Resource.Destroy(value);
                    contentsDictionary.Remove(soID);
                }
            }
        }

        private void SortContentByCost()
        {
            // Content 안의 모든 자식 객체(Card 컴포넌트를 가진)를 배열로 가져오기
            var cards = Content_Root.transform.GetComponentsInChildren<Content_Card>();

            // Cost 값을 기준으로 내림차순 정렬
            var sortedCards = cards.OrderBy(card => card.Cost).ToArray();

            // 정렬된 순서로 자식들의 순서를 재정렬
            for (int i = 0; i < sortedCards.Length; i++)
            {
                sortedCards[i].transform.SetSiblingIndex(i);
            }
        }

        public void Ready()
        {
            Text_State.text = "상대를 기다리는 중!";
            Button_Ready.interactable = false;
            Managers.Client.Draft.Ready();
        }
    }
}