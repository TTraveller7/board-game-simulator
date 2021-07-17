using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BGS.UNO
{
    public class MultiGame : MonoBehaviourPun, IPunObservable
    {
        [Header("Basic")]
        [SerializeField] GameObject Canvas;
        [SerializeField] GameObject gameUI;
        SettingsUI uiScript;

        [Header("Containers")]
        [SerializeField]GameObject deck;
        DeckMul deckScript;

        [SerializeField] GameObject discard;

        [SerializeField] GameObject currentHand;
        [SerializeField] GameObject handPrefab;
        List<GameObject> hands;

        [SerializeField] GameObject playersObj;
        [SerializeField] GameObject playerPrefab;
        List<GameObject> players;
        public GameObject CurrentPlayer { get { return players[currentPlayerIndex]; } }

        [Header("Button & Icons")]
        [SerializeField] GameObject directionIcons;
        [SerializeField] Button nextTurnButton;

        int numOfPlayer;
        int currentPlayerIndex;
        bool antiClockWise;

        public delegate void TurnStart();
        public static event TurnStart TurnStartHandler;

        public delegate void TurnEnd();
        public static event TurnEnd TurnEndHandler;

        private void Start()
        {
            Screen.orientation = ScreenOrientation.Landscape;
            numOfPlayer = GameStatus.NumOfPlayers;
            uiScript = gameUI.GetComponent<SettingsUI>();

            // Initialize deck, discard, currenthand;
            deckScript = deck.GetComponent<DeckMul>();
            deckScript.Initialize(DealCard);

            discard.GetComponent<Discard>().Initialize(currentHand, GetComponent<UNOInfo>());

            currentHand.GetComponent<CurrentHand>().Initialize(discard, deck, OnTurnStart);

            // Initialize hands
            hands = new List<GameObject>();
            for (int i = 0; i < numOfPlayer - 1; i++)
            {
                GameObject a_Hand = Instantiate(handPrefab, Canvas.transform);
                a_Hand.GetComponent<Hand>().Initialize(i);
                hands.Add(a_Hand);
            }
            Hand.SetPositions(hands);

            // Initialize players
            players = new List<GameObject>();
            for (int i = 0; i < numOfPlayer; i++)
            {
                GameObject a_Player = Instantiate(playerPrefab, playersObj.transform);
                a_Player.GetComponent<Player>().Initialize(GameStatus.GetNameOfPlayer(i + 1), currentHand, GetComponent<UNOInfo>());
                players.Add(a_Player);
            }

            // Initialize settings ui
            gameUI.GetComponent<SettingsUI>().Initialize();


            if (GameStatus.isNewGame)
            {
                currentPlayerIndex = 0;
                antiClockWise = true;

                // Deal 7 cards to each player
                foreach (GameObject player in players)
                    player.GetComponent<Player>().Cards.AddRange(DealCards(7, player));
                gameUI.GetComponent<SettingsUI>().AddLog("UNO: New game.");
            }
            else
            {
                if (SaveLoadManager.OnLoadHandler != null)
                    SaveLoadManager.OnLoadHandler(SaveLoadManager.tempSD);
                gameUI.GetComponent<SettingsUI>().AddLog("UNO: Load complete.");
            }

            // Initialize rules script
            GetComponent<Rules>().enabled = GameStatus.useRules;
            //if (GameStatus.useRules)
                //GetComponent<Rules>().Initialize(currentHand, discard, deck, this);

            // Initialize direaction Icons
            directionIcons.GetComponent<DirectionIcons>().Interactable = !GameStatus.useRules;
            directionIcons.GetComponent<DirectionIcons>().DirectionIconToggle(antiClockWise);

            /////////
            // Initialize Game start handler

            if (GameStatus.useRules)
                TurnStartHandler += GetComponent<Rules>().OnDraw2Draw4Played_Start;

            /////////
            // Initialize Game end handler

            // Get cards from hand to player
            foreach (GameObject player in players)
                TurnEndHandler += player.GetComponent<Player>().GetCardsFromHand;
            // If use rules, check isNextTurn
            if (GameStatus.useRules)
                TurnEndHandler += GetComponent<Rules>().IsNextTurn;

            // Set next currentPlayerIndex
            TurnEndHandler += SetCurrentPlayerIndex;
            if (GameStatus.useRules)
                TurnEndHandler += GetComponent<Rules>().OnDraw2Draw4Played_End;

            // Set Hand
            TurnEndHandler += SetHand;
            // Place cards on hand
            foreach (GameObject player in players)
                TurnEndHandler += player.GetComponent<Player>().PlaceCards;
            // Enable current hand cover
            TurnEndHandler += currentHand.GetComponent<CurrentHand>().EnableCover;

            // Initialize first turn
            SetHand();
            nextTurnButton.interactable = false;
            foreach (GameObject player in players)
                player.GetComponent<Player>().PlaceCards();
            currentHand.GetComponent<CurrentHand>().EnableCover();

            GameStatus.isNewGame = true;
        }

        private void OnDisable()
        {
            Screen.orientation = ScreenOrientation.Portrait;

            TurnEndHandler -= SetCurrentPlayerIndex;
            TurnEndHandler -= SetHand;
        }

        public void OnTurnStart()
        {
            // If without rules, enable next turn button
            if (!GameStatus.useRules)
                nextTurnButton.interactable = true;
            // Enable deck draw
            deck.GetComponent<Deck>().Interactable = true;

            if (TurnStartHandler != null)
                TurnStartHandler();
        }

        public void OnTurnEnd()
        {
            // Disable next turn button
            nextTurnButton.interactable = false;
            // Disable deck draw
            deck.GetComponent<Deck>().Interactable = false;

            if (TurnEndHandler != null)
                TurnEndHandler();
        }

        /**
         * <summary>
         * Deal cards from deck into a transform.
         * </summary>
         * <return>A list of cards.</return>
         * <param name="num">Number of cards to deal.</param>
         * <param name="player">Player who take the cards</param>
         */
        public List<GameObject> DealCards(int num, GameObject player)
        {
            return deckScript.DrawCards(num, player.transform);
        }

        /// <summary>
        /// Get index of next player based on direction.
        /// </summary>
        /// <param name="index">Player index.</param>
        /// <returns>Index of next player based on direction.</returns>
        int NextPlayer(int index)
        {
            if (antiClockWise)
                return index == (numOfPlayer - 1)
                    ? 0
                    : index + 1;
            else
                return index == 0
                    ? numOfPlayer - 1
                    : index - 1;
        }

        // helper
        void SetCurrentPlayerIndex()
        {
            currentPlayerIndex = NextPlayer(currentPlayerIndex);
        }

        /// <summary>
        /// Assign player to hand/currentHand.
        /// </summary>
        void SetHand()
        {
            players[currentPlayerIndex].GetComponent<Player>().isCurrentPlayer = true;
            players[currentPlayerIndex].GetComponent<Player>().SetCurrentHandName();

            int index = currentPlayerIndex;

            if (antiClockWise)
                for (int i = 0; i < numOfPlayer - 1; i++)
                {
                    index = NextPlayer(index);
                    players[index].GetComponent<Player>().Hand = hands[i];
                }
            else
                for (int i = numOfPlayer - 2; i >= 0; i--)
                {
                    index = NextPlayer(index);
                    players[index].GetComponent<Player>().Hand = hands[i];
                }
        }

        

        /// <summary>
        /// Deal a card to currenthand.
        /// </summary>
        
        public void DealCard()
        {
            List<GameObject> list = currentHand.GetComponent<CurrentHand>().Cards;
            list.AddRange(DealCards(1, currentHand));
            currentHand.GetComponent<CurrentHand>().Cards = list;
        }
        
        
        #region RPCs

        [PunRPC]
        public void ToggleDirection()
        {
            antiClockWise = !antiClockWise;
            directionIcons.GetComponent<DirectionIcons>().DirectionIconToggle(antiClockWise);
            uiScript.AddLog("Direction reversed.");
        }

        #endregion


        #region IPunObservable Implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}



