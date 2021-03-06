using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BGS.UNO
{
    public class CurrentHandMul : MonoBehaviourPun, ICurrentHand
    {
        [Header("References")]
        [SerializeField] GameObject discard;
        [SerializeField] MultiGame gameScript;

        [Header("Components")]
        [SerializeField] Text playerName;
        public string PlayerName
        {
            get { return playerName.text; }
            set { playerName.text = value; }
        }

        // Cards
        List<GameObject> cards;
        public List<GameObject> Cards
        {
            get { return cards; }
            set
            {
                if (value[0].GetComponent<Card>() != null)
                    cards = value;
                PlaceCards();
            }
        }

        GameObject highlightedCard;
        public GameObject HighlightedCard
        {
            get { return highlightedCard; }
            set
            {
                if (value == null)
                {
                    highlightedCard = null;
                    discard.GetComponent<Outline>().enabled = false;
                }
                else
                {
                    if (highlightedCard != null)
                        highlightedCard.GetComponent<CardReaction>().PutBack();
                    highlightedCard = value;
                    discard.GetComponent<Outline>().enabled = true;
                }
            }
        }

        public void Initialize()
        {
            cards = new List<GameObject>();

            name = ToString();
        }

        void OnDisable()
        {
            MultiGame.TurnStartHandler -= EnableCardReaction;
            MultiGame.TurnEndHandler -= DisableCardReaction;
        }

        #region Cards Methods

        void PlaceCards()
        {
            // If there is no cards to place, return
            if (cards.Count == 0) return;

            // If there is a highlighted card, put it back
            if (highlightedCard != null)
            {
                highlightedCard.GetComponent<CardReaction>().PutBack();
                highlightedCard = null;
            }

            float width = GetComponent<RectTransform>().rect.width;
            float cardWidth = cards[0].GetComponent<RectTransform>().rect.width;

            // Distance between cards
            float d = cards.Count <= 1 ? 0f : (width - cardWidth) / cards.Count - 1;
            d = d > cardWidth + 10f ? cardWidth + 10f : d;

            // Distance to left and right
            float x = -(cardWidth + ((cards.Count - 1) * d)) / 2f + 0.5f * cardWidth;

            // Distance to bottom
            float y = 10f;

            foreach (GameObject card in cards)
            {
                // Place cards from left to right
                card.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
                card.GetComponent<RectTransform>().rotation = Quaternion.identity;
                x += d;
            }
        }

        /// <summary>
        /// Play the highlighted card.
        /// </summary>
        /// <remarks>
        /// Put the card to discard, call game script to sync with other clients.
        /// </remarks>
        public void PlayCard()
        {
            discard.GetComponent<DiscardMul>().CardToPile(highlightedCard);
            gameScript.SyncPlayCard(cards.IndexOf(highlightedCard));

            // Remove references
            cards.Remove(highlightedCard);
            highlightedCard = null;
            PlaceCards();
        }

        #endregion

        #region Helpers

        public void SkipTurn()
        {
            if (highlightedCard != null)
            {
                highlightedCard.GetComponent<CardReaction>().PutBack();
                HighlightedCard = null;
            }
            DisableCardReaction();
        }

        public void EnableCardReaction()
        {
            foreach (GameObject card in cards)
            {
                card.GetComponent<CardReaction>().enabled = true;
                card.GetComponent<CardReaction>().Initialize(gameObject);
            }
        }

        public void EnableLastCardReaction()
        {
            cards[cards.Count - 1].GetComponent<CardReaction>().enabled = true;
            cards[cards.Count - 1].GetComponent<CardReaction>().Initialize(gameObject);
        }

        public void DisableCardReaction()
        {
            if (highlightedCard != null)
            {
                highlightedCard.GetComponent<CardReaction>().PutBack();
                HighlightedCard = null;
            }
            foreach (GameObject card in cards)
                card.GetComponent<CardReaction>().enabled = false;
        }

        public override string ToString()
        {
            return "CurrentHand";
        }

        #endregion

        #region IContainer Implementation

        public void TransferAllCards(Transform parent, out List<GameObject> transferedCards)
        {
            foreach (GameObject card in cards)
                card.transform.SetParent(parent);

            transferedCards = new List<GameObject>(cards);

            cards = new List<GameObject>();
        }

        public void TakeCards(List<GameObject> cards)
        {
            this.cards.AddRange(cards);
            foreach (GameObject card in cards)
                card.transform.SetParent(transform);
            PlaceCards();
        }

        #endregion
    }
}

