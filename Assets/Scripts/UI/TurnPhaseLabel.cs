using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ArrowCardGame
{
    public class TurnPhaseLabel : MonoBehaviour
    {
        [SerializeField]
        private List<TurnPhaseType> m_TurnPhase;

        [SerializeField]
        private Text m_Text;

        private void Start()
        {
            foreach (TurnPhaseType turnPhase in m_TurnPhase)
            {
                Table.Instance.GetState(turnPhase).EnterPhaseEvent += OnEnterRequestedPhase;
                Table.Instance.GetState(turnPhase).ExitPhaseEvent += OnExitRequestedPhase;
            }

        }

        private void OnDestroy()
        {
            if (Table.Instance == null)
                return;

            foreach (TurnPhaseType turnPhase in m_TurnPhase)
            {
                Table.Instance.GetState(turnPhase).EnterPhaseEvent -= OnEnterRequestedPhase;
                Table.Instance.GetState(turnPhase).ExitPhaseEvent -= OnExitRequestedPhase;
            }
        }

        private void OnEnterRequestedPhase()
        {
            m_Text.fontSize = 35;
            m_Text.color = Color.white;
        }

        private void OnExitRequestedPhase()
        {
            m_Text.fontSize = 25;
            m_Text.color = new Color(0.25f, 0.25f, 0.25f, 1.0f);
        }
    }
}
