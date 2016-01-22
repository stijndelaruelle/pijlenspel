using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ArrowCardGame
{
    public class VisualDeck : MonoBehaviour
    {
        [SerializeField]
        private DeckDefinition m_DeckDefinition;

        [SerializeField]
        private VisualCard m_VisualCardPrefab;
        private List<VisualCard> m_VisualCards;

        [SerializeField]
        private RectTransform m_TableRoot;

        [SerializeField]
        private CardSlotGrid m_DefaultCardSlotGrid;

        private Deck m_Deck;
        public Deck Deck
        {
            get { return m_Deck; }
        }

        private void Awake()
        {
            if (m_DeckDefinition == null)
            {
                Debug.LogError("Visual deck doesn't have a Deckdefinition!", this);
                return;
            }

            if (m_VisualCardPrefab == null)
            {
                Debug.LogError("Visual deck doesn't have a VisualCardPrefab!", this);
                return;
            }

            m_Deck = new Deck(m_DeckDefinition);

            m_VisualCards = new List<VisualCard>();

            List<Card> cards = m_Deck.Cards;
            for (int i = cards.Count - 1; i >= 0; --i)
            {
                VisualCard newVisualCard = GameObject.Instantiate(m_VisualCardPrefab) as VisualCard;
                newVisualCard.Initialize(cards[i], m_TableRoot, m_DefaultCardSlotGrid);

                newVisualCard.transform.SetParent(gameObject.transform);
                newVisualCard.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                newVisualCard.transform.localRotation = Quaternion.identity;
                newVisualCard.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                m_VisualCards.Add(newVisualCard);
            }
        }
    }
}
