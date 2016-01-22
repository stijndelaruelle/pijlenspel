using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ArrowCardGame
{
    public class DeckDefinition : ScriptableObject
    {
        [SerializeField]
        private List<CardDefinition> m_CardDefinitions;
        public List<CardDefinition> CardDefinitions
        {
            get { return m_CardDefinitions; }
        }
    }
}
