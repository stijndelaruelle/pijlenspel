using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ArrowCardGame
{
    //Functions as the GameManager. But I like the name "Table" in this case
    public class Table : MonoBehaviour
    {
        public static int DIR_NUM = 8;

        [SerializeField]
        private Board m_Board;


        public void EndTurn()
        {
            m_Board.Resolve();
        }

        public static Direction RotateDir(Direction dir)
        {
            Direction rotatedDir = (Direction)(((int)dir + 4) % Table.DIR_NUM);
            return rotatedDir;
        }
    }
}
