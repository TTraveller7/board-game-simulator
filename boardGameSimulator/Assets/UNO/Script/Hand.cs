using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNO
{
    public class Hand : MonoBehaviour
    {
        [SerializeField] int num; // Starts from 0
        List<GameObject> cards;

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

        static void SetPosition(GameObject Hand, Position position) 
        {
            RectTransform rect = Hand.GetComponent<RectTransform>();
            rect.anchorMin = position.anchor;
            rect.anchorMax = position.anchor;
            rect.rotation = position.rotation;
            rect.anchoredPosition = position.anchoredPosition;
        }

        // Set cards positions

        public void PlaceCards(List<GameObject> cards)
        {
            float x = -((cards.Count-1) / 2f) * 20f;
            float y = 10f;
            foreach (GameObject card in cards)
            {
                GameObject a_Card = card.GetComponent<Card>().Copy(transform);
                this.cards.Add(a_Card);
                a_Card.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
                x += 20f;
            }
        }

        // public void 

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