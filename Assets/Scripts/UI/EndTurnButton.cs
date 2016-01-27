using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ArrowCardGame
{
    public class EndTurnButton : MonoBehaviour
    {
        [SerializeField]
        private Button m_Button;

        private void Start()
        {
            Table.Instance.GetState(TurnPhaseType.PlayedCard).EnterPhaseEvent += OnEnterRequestedPhase;
            Table.Instance.GetState(TurnPhaseType.PlayedCard).ExitPhaseEvent += OnExitRequestedPhase;

            m_Button.interactable = false;
        }

        private void OnDestroy()
        {
            if (Table.Instance == null)
                return;

            Table.Instance.GetState(TurnPhaseType.PlayedCard).EnterPhaseEvent -= OnEnterRequestedPhase;
            Table.Instance.GetState(TurnPhaseType.PlayedCard).ExitPhaseEvent -= OnExitRequestedPhase;
        }

        private void OnEnterRequestedPhase()
        {
            bool enabled = (Table.Instance.GetCurrentPlayerType() != PlayerType.AI);
            m_Button.interactable = enabled;
        }

        private void OnExitRequestedPhase()
        {
            m_Button.interactable = false;
        }
    }
}
