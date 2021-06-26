using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UNO
{
    public class Hand : MonoBehaviour, IHand
    {
        [SerializeField] GameObject playerText;
        [SerializeField] GameObject playerTextLeft;
        [SerializeField] GameObject playerTextRight;

        Text playerName;
        public string PlayerName
        {
            get { return playerName.text; }
            set { playerName.text = value; }
        }

        [SerializeField] int num; // Starts from 0
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

        public void Initialize(int num)
        {
            this.num = num;
            gameObject.name = ToString();

            cards = new List<GameObject>();
        }

        static Position position1 = new Position()
        {
            anchoredPosition = new Vector2(-40f, 0f),
            anchor = new Vector2(1f, 0.5f),
            rotation = Quaternion.Euler(0f, 0f, 90f)
        };

        static Position position2 = new Position()
        {
            anchoredPosition = new Vector2(234f, -40f),
            anchor = new Vector2(0.5f, 1f),
            rotation = Quaternion.Euler(0f, 0f, 180f)
        };

        static Position position3 = new Position()
        {
            anchoredPosition = new Vector2(0f, -40f),
            anchor = new Vector2(0.5f, 1f),
            rotation = Quaternion.Euler(0f, 0f, 180f)
        };

        static Position position4 = new Position()
        {
            anchoredPosition = new Vector2(-234f, -40f),
            anchor = new Vector2(0.5f, 1f),
            rotation = Quaternion.Euler(0f, 0f, 180f)
        };

        static Position position5 = new Position()
        {
            anchoredPosition = new Vector2(40f, 0f),
            anchor = new Vector2(0f, 0.5f),
            rotation = Quaternion.Euler(0f, 0f, -90f)
        };

        // Set hand positions
        
        public static void SetPositions(List<GameObject> hands)
        {
            switch (hands.Count)
            {
                case 1: 
                    SetPosition(hands[0], position3); break;
                case 2: 
                    SetPosition(hands[0], position2); SetPosition(hands[1], position4); break;
                case 3: 
                    SetPosition(hands[0], position1); SetPosition(hands[1], position3); 
                    SetPosition(hands[2], position5); break;
                case 4:
                    SetPosition(hands[0], position1); SetPosition(hands[1], position2);
                    SetPosition(hands[2], position4); SetPosition(hands[3], position5); break;
                case 5:
                    SetPosition(hands[0], position1); SetPosition(hands[1], position2);
                    SetPosition(hands[2], position3); SetPosition(hands[3], position4);
                    SetPosition(hands[4], position5); break;
            }
        }

        static void SetPosition(GameObject hand, Position position) 
        {
            RectTransform rect = hand.GetComponent<RectTransform>();
            rect.anchorMin = position.anchor;
            rect.anchorMax = position.anchor;
            rect.rotation = position.rotation;
            rect.anchoredPosition = position.anchoredPosition;

            hand.GetComponent<Hand>().SetTextPosition(rect.rotation.eulerAngles.z);
        }

        // Set text position

        void SetTextPosition(float zRotation)
        {
            switch (zRotation)
            {
                case 90f: playerTextRight.SetActive(true); playerName = playerTextRight.GetComponent<Text>(); break;
                case 180f: playerText.SetActive(true); playerName = playerText.GetComponent<Text>(); break;
                case 270f: playerTextLeft.SetActive(true); playerName = playerTextLeft.GetComponent<Text>(); break;
                default: playerText.SetActive(true); playerName = playerText.GetComponent<Text>();break;
            }
        }

        // Set cards positions

        void PlaceCards()
        {
            float width = GetComponent<RectTransform>().rect.width;
            float cardWidth = cards[0].GetComponent<RectTransform>().rect.width;

            float d = cards.Count <= 1 ? 0f : (width - cardWidth) / cards.Count - 1;
            d = d > cardWidth + 10f ? cardWidth + 10f : d;

            float x = -(cardWidth + ((cards.Count - 1) * d)) / 2f + 0.5f * cardWidth;
            float y = 10f;

            foreach (GameObject card in cards)
            {
                card.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
                card.GetComponent<RectTransform>().rotation = this.GetComponent<RectTransform>().rotation;
                x += d;

                card.GetComponent<CardReaction>().enabled = false;
            }
        }

        public void GiveCards(GameObject player, out List<GameObject> cards)
        {
            cards = new List<GameObject>(this.cards);
            foreach (GameObject card in this.cards)
                card.transform.SetParent(player.transform);
            this.cards = new List<GameObject>();
        }

        public override string ToString()
        {
            return "Hand" + num.ToString();
        }
    }

    public struct Position 
    {
        public Vector2 anchoredPosition;
        public Vector2 anchor;
        public Quaternion rotation;
    }
}