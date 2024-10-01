using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Marsion.UI
{
    public class UI_DeckBuilder : UI_Popup
    {
        DeckBuildState currentState;

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

            Managers.Client.OnGameStarted -= GameStart;
            Managers.Client.OnGameStarted += GameStart;

            Managers.Builder.OnUpdateDeckBuildingState -= UpdateSelections;
            Managers.Builder.OnUpdateDeckBuildingState += UpdateSelections;

            contentsDictionary = new Dictionary<string, GameObject>();

            UpdateState();
            InitDeck();
            InitSelections();
            UpdateSelections();
        }

        private void Clear()
        {
            Managers.Client.OnGameStarted -= GameStart;
            Managers.Builder.OnUpdateDeckBuildingState -= UpdateSelections;
        }

        private void GameStart()
        {
            Clear();
            ClosePopupUI();
        }

        private void UpdateState()
        {
            currentState = Managers.Builder.State;

            Text_Count.text = $"남은 횟수 : {currentState.Count}";
            Managers.Logger.Log<UI_DeckBuilder>("UpdateState");
            Text_DeckCount.text = $"{currentState.Deck.Count}";

            if (Managers.Builder.IsComplete)
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
            }
        }

        private void InitDeck()
        {
            foreach (var card in currentState.Deck)
            {
                AddCard(card);
            }

            SortContentByCost();
        }

        private void InitSelections()
        {
            Button_Left.Button.onClick.AddListener(() =>
            {
                OnSelect(0);
                SortContentByCost();
                Managers.Builder.Select(0);
            });

            Button_Center.Button.onClick.AddListener(() =>
            {
                OnSelect(1);
                SortContentByCost();
                Managers.Builder.Select(1);
            });

            Button_Right.Button.onClick.AddListener(() =>
            {
                OnSelect(2);
                SortContentByCost();
                Managers.Builder.Select(2);
            });
        }

        private void UpdateSelections()
        {
            UpdateState();

            switch (Managers.Builder.Type)
            {
                case DeckBuilder.SelectType.Legendary:
                    SetOneSelection();
                    break;
                case DeckBuilder.SelectType.Table:
                    SetOneSelection();
                    break;
                case DeckBuilder.SelectType.Exchange:
                    SetExchangeSelection();
                    break;
            }
        }

        private void SetOneSelection()
        {
            Panel_LeftExchange.SetActive(false);
            Button_Left.Setup(currentState.Selections[0]);
            Button_Left.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

            Panel_CenterExchange.SetActive(false);
            Button_Center.Setup(currentState.Selections[1]);
            Button_Center.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

            Panel_RightExchange.SetActive(false);
            Button_Right.Setup(currentState.Selections[2]);
            Button_Right.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        }

        private void SetExchangeSelection()
        {
            Panel_LeftExchange.GetComponentInChildren<Content_Card>().Setup(currentState.SubSelections[0]);
            Panel_LeftExchange.SetActive(true);
            Button_Left.Setup(currentState.Selections[0]);
            Button_Left.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -70);

            Panel_CenterExchange.GetComponentInChildren<Content_Card>().Setup(currentState.SubSelections[1]);
            Panel_CenterExchange.SetActive(true);
            Button_Center.Setup(currentState.Selections[1]);
            Button_Center.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -70);

            Panel_RightExchange.GetComponentInChildren<Content_Card>().Setup(currentState.SubSelections[2]);
            Panel_RightExchange.SetActive(true);
            Button_Right.Setup(currentState.Selections[2]);
            Button_Right.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -70);
        }

        private void OnSelect(int index)
        {
            switch (Managers.Builder.Type)
            {
                case DeckBuilder.SelectType.Legendary:
                    AddCard(currentState.Selections[index]);
                    break;
                case DeckBuilder.SelectType.Table:
                    AddCard(currentState.Selections[index]);
                    break;
                case DeckBuilder.SelectType.Exchange:
                    RemoveCard(currentState.SubSelections[index]);
                    AddCard(currentState.Selections[index]);
                    break;
            }
        }

        private void AddCard(Card card)
        {
            if (contentsDictionary.TryGetValue(card.Name, out var value))
            {
                value.GetComponent<Content_Card>().IncreaseCount();
            }
            else
            {
                var content_Card = Managers.UI.MakeSubItem<Content_Card>(Content_Root.transform);
                content_Card.Setup(card);
                content_Card.IncreaseCount();
                contentsDictionary.Add(card.Name, content_Card.gameObject);
            }
        }

        private void RemoveCard(Card card)
        {
            if (contentsDictionary.TryGetValue(card.Name, out var value))
            {
                bool IsZero = value.GetComponent<Content_Card>().DecreaseCount();
                if (IsZero)
                {
                    Managers.Resource.Destroy(value);
                    contentsDictionary.Remove(card.Name);
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
            Managers.Builder.Ready();
        }
    }
}