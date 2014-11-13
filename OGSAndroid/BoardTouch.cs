using System;
using System.Linq;
using System.Collections;
using Android.Views;

namespace OGSAndroid
{
    public class BoardTouch
    {
        public bool ConfirmStoneActive { get; private set; }
        public Stone ConfirmStone { get; private set; }
        public int[] ConfirmStonePos { get; private set; }

        public BoardTouch(BoardView view)
        {
            ConfirmStoneActive = false;
            ConfirmStonePos = new int[2];

            view.Touch += (object sender, View.TouchEventArgs e) => 
            {
                if(e.Event.Action != MotionEventActions.Down) return;

                float[] xy = {e.Event.GetX(), e.Event.GetY()}; 
                int padding = view.Padding, spacing = view.Spacing;
                var spacingsmall = spacing/2;
                var xyi = new int[2];
                    xyi[0] = (int)Math.Ceiling(((xy[0] - padding) + spacingsmall) / spacing);
                    xyi[1] = (int)Math.Ceiling(((xy[1] - padding) + spacingsmall) / spacing);

                Console.Write("x: "+ xyi[0] + " y: "+ xyi[1] + "\n");

                if(xyi.SequenceEqual(ConfirmStonePos))
                {
                    ConfirmStoneActive = false;
                    view.PlaceStone(new Stone(view.CurrentTurn, xyi[0], xyi[1]));
                }
                else
                {
                    ConfirmStone = new Stone(view.CurrentTurn,xyi[0],xyi[1]);
                    ConfirmStonePos = xyi;
                    ConfirmStoneActive = true;
                    view.Invalidate();
                }
            };
        }

        public void Reset()
        {
            ConfirmStoneActive = false;
            ConfirmStonePos = new int[2];
        }
                           
    }
}

