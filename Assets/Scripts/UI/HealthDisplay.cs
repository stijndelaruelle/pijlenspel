using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ArrowCardGame
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField]
        private DamageableObject m_DamageableObject;

        [SerializeField]
        private Text m_Text;

        private void Awake()
        {
            m_DamageableObject.HealthChangedEvent += OnHealthChanged;
        }

        private void OnHealthChanged(int oldHealth, int newHealth)
        {
            m_Text.text = newHealth.ToString();
        }
    }
}
