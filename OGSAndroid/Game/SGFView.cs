#region

using System;
using System.Linq;
using Android.Content;
using Android.Util;
using Android.Widget;
using Android.Graphics;
using System.Timers;

#endregion

namespace OGSAndroid.Game
{
    // ReSharper disable once InconsistentNaming
    public class SGFView : BoardView
    {
        private int currentMove = 1;
        public TextView MoveNumberText;
        public SGF<Stone> Moves = new SGF<Stone>();

        public Timer GCSuppressTimer = new Timer();

        public SGFView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            GCSuppressTimer.Interval = 2000;
            GCSuppressTimer.AutoReset = false;
            GCSuppressTimer.Elapsed += (o, e) =>
            {
                GC.Collect();
                GC.Collect(0);
            };
        }

        public int CurrentMove
        {
            get { return currentMove; }

            set
            {
                if (MoveNumberText == null)
                    throw new Exception("SGFView needs a MoveNumber TextView set on MoveNumberText");
                currentMove = value;
                MoveNumberText.Text = value.ToString();
            }
        }

        public void SetSGF(SGF<Stone> s)
        {
            Moves = s;
            CurrentMove = Convert.ToInt32(s.Info.Handicap);
            Initialize(Convert.ToInt32(s.Info.Size));
            ToStart();
        }

        public void PlaceUpTo(int max)
        {
            ClearBoard();
            for (var i = 0; i < max; i++)
            {
                if (i > Moves.Tree.Nodes.Count - 1)
                {
                    CurrentMove = Moves.Tree.Nodes.Count - 1;
                    return;
                }

                var node = Moves.Tree.Nodes[i];

                PlaceStone(node.Data);
            }
            Invalidate();           
        }

        public void ToEnd()
        {
            GC.SuppressFinalize(this);
            PlaceUpTo(Moves.Tree.Nodes.Count);
            CurrentMove = Moves.Tree.Nodes.Count;
            GC.Collect();
            GC.Collect(0);
        }

        public void ToStart()
        {
            CurrentMove = Convert.ToInt32(Moves.Info.Handicap);
            PlaceUpTo(CurrentMove);         
        }

        public void Next()
        {
            //Next means the user will probably keep clicking it, we don't want the GC_Major to act or it will stutter.
            //Devices without much memory will still stutter, but not as much.
            GC.SuppressFinalize(this);
            GCSuppressTimer.Stop();
            GCSuppressTimer.Start();

            if (currentMove > Moves.Tree.Nodes.Count() - 1)
                return;
            PlaceStone(Moves.Tree.Nodes[CurrentMove].Data, true);
            CurrentMove++;
            Invalidate();
        }

        public void Previous()
        {
            if (CurrentMove == 0) return;

            CurrentMove--;
            PlaceUpTo(CurrentMove);
        }

        protected override void OnDraw(Android.Graphics.Canvas canvas)
        {
            base.OnDraw(canvas);
            if(CurrentMove != 0)
                DrawLastStoneCircle(canvas, CurrentTurn);
        }

        private void DrawLastStoneCircle(Canvas canvas, Stone stone)
        {
            Paint col = stone ? blackPaint : whitePaint;
            col.StrokeWidth = 3;
            col.SetStyle(Paint.Style.Stroke);
            canvas.DrawCircle(ExtPad + Padding + ((stone.x - 1)*Spacing), ExtPad + Padding + ((stone.y - 1)*Spacing),
                (Spacing/3), col);
            col.SetStyle(Paint.Style.Fill);
            col.StrokeWidth = 2;
        }
    }
}