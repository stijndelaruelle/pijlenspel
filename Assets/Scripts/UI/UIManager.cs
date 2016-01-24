using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ArrowCardGame
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_EndGamePanel;

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
            //m_EndGamePanel.SetActive(false);
        }

        private void OnGameEnd(int playerID, PlayerType playerType)
        {
            //m_EndGamePanel.SetActive(true);
        }
    }
}
