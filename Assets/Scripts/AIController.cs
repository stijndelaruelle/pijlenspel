using UnityEngine;
using System.Collections;

namespace ArrowCardGame
{
    public class AIController : MonoBehaviour
    {
        [SerializeField]
        private Board m_Board;

        [SerializeField]
        private Board m_Hand;

        public void Process()
        {
            //Values of the best move
            int cardID = 0;
            int boardID = 0;
            bool rotated = false;

            //For every card in my hand, I'm going to place it on every open position on the board. Both regular & rotated.


        }
    }
}
