using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ArrowCardGame
{
    public class Table : MonoBehaviour
    {
        public static int DIR_NUM = 8;

        public static Direction RotateDir(Direction dir)
        {
            Direction rotatedDir = (Direction)(((int)dir + 4) % Table.DIR_NUM);
            return rotatedDir;
        }
    }
}
