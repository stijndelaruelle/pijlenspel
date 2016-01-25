using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ArrowCardGame
{
    public class NewTurnLabel : MonoBehaviour
    {
        [SerializeField]
        private Text m_Text;

        private bool m_AllPlayersAreHuman;
        private bool m_AllPlayersAreAI;
        private Coroutine m_RoutineHandle;

        private void Start()
        {
            Table.Instance.StartGameEvent += OnStartGame;
            Table.Instance.StartTurnEvent += OnStartTurn;
        }

        private void OnDestroy()
        {
            if (Table.Instance == null)
                return;

            Table.Instance.StartGameEvent -= OnStartGame;
            Table.Instance.StartTurnEvent -= OnStartTurn;
        }

        private void OnStartGame(PlayerType playerType1, PlayerType playerType2)
        {
            m_AllPlayersAreHuman = false;
            m_AllPlayersAreAI = false;

            if (playerType1 == PlayerType.Human && playerType2 == PlayerType.Human)
                m_AllPlayersAreHuman = true;

            if (playerType1 == PlayerType.AI && playerType2 == PlayerType.AI)
                m_AllPlayersAreAI = true;

            m_Text.enabled = false;
        }

        private void OnStartTurn(int playerID, PlayerType playerType)
        {
            if (m_RoutineHandle != null)
                StopCoroutine(m_RoutineHandle);

            string sentence = "";
            if (m_AllPlayersAreHuman)
            {
                sentence = "Player " + playerID + "'s Turn";
            }
            else if (m_AllPlayersAreAI)
            {
                sentence = "Computer " + playerID + "'s Turn";
            }
            else
            {
                if (playerType == PlayerType.Human)
                {
                    sentence = "Your turn";
                }
                else
                {
                    sentence = "Opponent's\nturn";
                }
            }

            m_Text.enabled = true;
            m_Text.text = sentence;

            m_RoutineHandle = StartCoroutine(FadeOutRoutine());
        }

        private IEnumerator FadeOutRoutine()
        {
            Color color = m_Text.color;
            color.a = 1.0f;

            m_Text.color = color;

            yield return new WaitForSeconds(1.0f);

            while (m_Text.color.a > 0.0f)
            {
                color.a -= Time.deltaTime;

                m_Text.color = color;
                yield return new WaitForEndOfFrame();
            }

            m_Text.enabled = false;
        }
    }
}
