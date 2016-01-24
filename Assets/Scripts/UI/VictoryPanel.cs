using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ArrowCardGame
{
    public class VictoryPanel : MonoBehaviour
    {
        [SerializeField]
        private Text m_Text;

        private bool m_AllPlayersAreHuman;
        private bool m_AllPlayersAreAI;

        private void Start()
        {
            Table.Instance.StartGameEvent += OnStartGame;
            Table.Instance.EndGameEvent += OnGameEnd;
        }

        private void OnDestroy()
        {
            if (Table.Instance == null)
                return;

            Table.Instance.StartGameEvent -= OnStartGame;
            Table.Instance.EndGameEvent -= OnGameEnd;
        }

        private void OnStartGame(PlayerType playerType1, PlayerType playerType2)
        {
            m_AllPlayersAreHuman = false;
            m_AllPlayersAreAI = false;

            if (playerType1 == PlayerType.Human && playerType2 == PlayerType.Human)
                m_AllPlayersAreHuman = true;

            if (playerType1 == PlayerType.AI && playerType2 == PlayerType.AI)
                m_AllPlayersAreAI = true;

            gameObject.SetActive(false);
        }

        private void OnGameEnd(int playerID, PlayerType playerType)
        {
            string sentence = "";
            if (m_AllPlayersAreHuman)
            {
                sentence = "Player " + playerID + " won!";
            }
            else if (m_AllPlayersAreAI)
            {
                sentence = "Computer " + playerID + " won!";
            }
            else
            {
                if (playerType == PlayerType.Human)
                {
                    sentence = "You won!";
                }
                else
                {
                    sentence = "You lost!";
                }
            }

            gameObject.SetActive(true);
            m_Text.text = sentence;
        }

    }
}
