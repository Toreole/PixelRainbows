using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelRainbows
{
    //Game progress is per-session.
    //Chapters are the 100's, panel number are 0-99. 
    //can technically be handled by a 16-bit number, but 32-bit is usually handled better on 64-bit devices.
    public static class GameProgress 
    {
        //Furthest progress in the game so far. is overriden by current if it is higher.
        //Furthest => available chapters.
        public static int Furthest { get; set; } = 0;

        //A flag set in the menu for when the furthest progress should be loaded. This gets used up by the camera controller.
        public static bool LoadFromCurrent { get; set; } = false;

        private static int current = 0;
        //Current progress in the game.
        public static int Current 
        {
            get => current;
            set
            {
                if(value > Furthest)
                    Furthest = value;
                current = value;
            }
        }
    }
}