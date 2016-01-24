using UnityEngine;
using System.Collections;

namespace ArrowCardGame
{
    public delegate void VoidIntIntDelegate(int oldValue, int newValue);

    public class DamageableObject : MonoBehaviour
    {
        [SerializeField]
        private int m_Health;
        public int Health
        {
            get { return m_Health; }
        }

        private int m_OriginalHealth;

        [SerializeField]
        private int m_MaxHealth;

        //Events
        private VoidIntIntDelegate m_HealthChangedEvent;
        public VoidIntIntDelegate HealthChangedEvent
        {
            get { return m_HealthChangedEvent; }
            set { m_HealthChangedEvent = value; }
        }

        private void Start()
        {
            m_OriginalHealth = m_Health;
            Table.Instance.StartGameEvent += OnStartGame;
        }

        private void OnDestroy()
        {
            if (Table.Instance != null)
                Table.Instance.StartGameEvent -= OnStartGame;
        }

        public void ModifyHealth(int amount)
        {
            int oldHealthValue = m_Health;
            m_Health += amount;

            if (m_Health < 0)
                m_Health = 0;

            if (m_HealthChangedEvent != null)
                m_HealthChangedEvent(oldHealthValue, m_Health);
        }

        private void OnStartGame(PlayerType playerType1, PlayerType playerType2)
        {
            ModifyHealth(m_OriginalHealth - m_Health);
        }
    }
}
