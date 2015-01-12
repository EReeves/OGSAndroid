#region

using System;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;

#endregion

namespace OGSAndroid.Game
{
    public class BoardView : View
    {
        public Stone CurrentTurn = Stone.Black;
        private bool firstDraw = true;
        private bool initialized;
        public HiddenReference<Stone[,]> stones;
        protected readonly Paint bgPaint;
        protected readonly Paint blackPaint;
        public readonly BoardTouch boardTouch;
        protected readonly Paint whitePaint;

        public BoardView(Context context, IAttributeSet attrs) : base(context)
        {
            InitPaints(out blackPaint, out whitePaint, out bgPaint);

            var bmp = BitmapFactory.DecodeResource(Resources, Resource.Drawable.woodtex);
            bgPaint.SetShader(new BitmapShader(bmp, Shader.TileMode.Repeat, Shader.TileMode.Repeat));
            boardTouch = new BoardTouch(this);
            Padding = 20;
            Invalidate();

            stones = new HiddenReference<Stone[,]>();
        }

        public int Padding { get; set; }
        public int Lines { get; set; }
        public int Size { get; private set; }
        public int Spacing { get; private set; }
        public int ExtPad { get; private set; }

        private static void InitPaints(out Paint black, out Paint white, out Paint bg)
        {
            black = new Paint {AntiAlias = true, Color = Color.Black, StrokeWidth = 2};
            white = new Paint {AntiAlias = true, Color = new Color(180, 180, 180)};
            bg = new Paint {AntiAlias = true, Color = Color.SandyBrown, StrokeWidth = 3};
        }

        public void Initialize(int lines)
        {
            Lines = lines;
            stones.Value = new Stone[Lines, Lines];
            initialized = true;
        }

        public void SubmitMove()
        {
            boardTouch.SubmitMove();
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            if (!initialized)
                return;

            //Could remove these from every draw if we need to optimize;
            Size = Math.Min(canvas.Width, canvas.Height);
            ExtPad = PaddingLeft; //Make sure padding top/left are always the same.
            Spacing = (Size - ((Padding)*2))/(Lines - 1);
            Padding = (Spacing/2) + 2;

            DrawBoard(canvas);


            foreach (var s in stones.Value)
            {
                if (s != null && s.Active)
                    DrawStone(canvas, s);
            }
                
            if (boardTouch.ConfirmStoneActive)
            {
                DrawStone(canvas, boardTouch.ConfirmStone, false);
            }

            if (firstDraw) //Draw again to fix padding/spacing dependency on eachother.
            {
                firstDraw = false;
                var par = new RelativeLayout.LayoutParams(Size, Size);
                LayoutParameters = par;
                OnDraw(canvas);
            }
        }

        private void DrawBoard(Canvas canvas)
        {
            //Draw background.
            var rect = new Rect(0, 0, Size, Size);
            canvas.DrawRect(rect, bgPaint);

            DrawGrid(canvas, blackPaint);
            DrawStarPoints(canvas);
        }

        private void DrawGrid(Canvas canvas, Paint paint)
        {
            for (var i = 0; i < Lines; i++)
            {
                var xy = (Spacing*i) + Padding + ExtPad;
                canvas.DrawLine(Padding + ExtPad, xy, ExtPad + Padding + (Lines - 1)*Spacing, xy, paint);
                canvas.DrawLine(xy, Padding + ExtPad, xy, ExtPad + Padding + (Lines - 1)*Spacing, paint);
            }
        }

        private void DrawStarPoints(Canvas canvas)
        {
            if (Lines < 9)
                return;

            var radius = (Spacing/8) + 1;

            //All boards > 9x have these.
            canvas.DrawCircle(ExtPad + Padding + (3*Spacing), ExtPad + Padding + (3*Spacing), radius, blackPaint);
            //TopLeft
            canvas.DrawCircle(ExtPad + Padding + ((Lines - 4)*Spacing), ExtPad + Padding + (3*Spacing), radius,
                blackPaint); //TopRight
            canvas.DrawCircle(ExtPad + Padding + (3*Spacing), ExtPad + Padding + ((Lines - 4)*Spacing), radius,
                blackPaint); //BottomLeft
            canvas.DrawCircle(ExtPad + Padding + ((Lines - 4)*Spacing), ExtPad + Padding + ((Lines - 4)*Spacing), radius,
                blackPaint); //BottomRight
            canvas.DrawCircle(ExtPad + Padding + ((Lines/2)*Spacing), ExtPad + Padding + ((Lines/2)*Spacing), radius,
                blackPaint); //Middle

            //Todo: 19x points.
        }
           

        private void DrawStone(Canvas canvas, Stone stone, bool alpha = true)
        {
            Paint col;
            if (stone)
            {
                col = blackPaint;
                col.SetShader(new RadialGradient(ExtPad + Padding + ((stone.x - 1.2f)*Spacing),
                    ExtPad + Padding + ((stone.y - 1.3f)*Spacing), 40, new Color(45, 45, 45), col.Color,
                    Shader.TileMode.Mirror));
            }
            else
            {
                col = whitePaint;
                col.SetShader(new RadialGradient(ExtPad + Padding + ((stone.x - 1.2f)*Spacing),
                    ExtPad + Padding + ((stone.y - 1.3f)*Spacing), 40, new Color(240, 240, 240), col.Color,
                    Shader.TileMode.Mirror));
            }

            if (!alpha)
                col.Alpha = 100;

            col.AntiAlias = true;

            canvas.DrawCircle(ExtPad + Padding + ((stone.x - 1)*Spacing), ExtPad + Padding + ((stone.y - 1)*Spacing),
                (Spacing/2), col);
            col.Alpha = 255;
        }

        public virtual void PlaceStone(Stone s, bool collect = false)
        {
            s.val = CurrentTurn.val;
            s.Active = true;
            stones.Value[s.x - 1, s.y - 1] = s;
            Console.WriteLine(s.x + "," + s.y);

            CapturePass(s);

            if (collect)
                GC.Collect(0);

            CurrentTurn = CurrentTurn ? Stone.White : Stone.Black;
            CurrentTurn.x = s.x;
            CurrentTurn.y = s.y;
        }

        public void ClearBoard()
        {
            CurrentTurn = Stone.Black;
            boardTouch.Reset();
            stones.Value = new Stone[Lines, Lines];
            Invalidate();
        }

        //Rules

        private void CapturePass(Stone s)
        {
            //Foreach adjacent stone get group and check for life.
            var surr = AdjacentStones(s);
            foreach (var st in surr)
            {
                if (st == null)
                    continue;

                List<Stone> grp;
                var alive = GroupAlive(st, out grp);

                if (!alive)
                {
                    foreach (var dst in grp)
                    {
                        stones.Value[dst.x - 1, dst.y - 1].Active = false;
                    }
                }
                    
            }
        }

        private List<Stone> SurroundingStones(Stone st, bool remOwn = true)
        {
            var adj = new List<Stone>();
            for (var x = -1; x <= 1; x++)
            {
                for (var y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    var xx = x + st.x;
                    var yy = y + st.y;

                    if (!InBounds(xx - 1, yy - 1)) continue;

                    var res = stones.Value[xx - 1, yy - 1];

                    if (remOwn)
                    {
                        // ReSharper disable once PossibleUnintendedReferenceComparison
                        if (res == st)
                            continue;
                    }

                    adj.Add(res);
                }
            }

            return adj;
        }

        private List<Stone> AdjacentStones(Stone s)
        {
            var adj = new List<Stone>();

            var st = new Stone(s, s.x - 1, s.y - 1);

            if (InBounds(st.x - 1, st.y) && NullOrActiveExist(st.x - 1, st.y)) adj.Add(stones.Value[st.x - 1, st.y]); //Left
            if (InBounds(st.x + 1, st.y) && NullOrActiveExist(st.x + 1, st.y)) adj.Add(stones.Value[st.x + 1, st.y]); //Right
            if (InBounds(st.x, st.y - 1) && NullOrActiveExist(st.x, st.y - 1)) adj.Add(stones.Value[st.x, st.y - 1]); //Up
            if (InBounds(st.x, st.y + 1) && NullOrActiveExist(st.x, st.y + 1)) adj.Add(stones.Value[st.x, st.y + 1]); //Down

            return adj;
        }

        private bool NullOrActiveExist(int x, int y)
        {
            return stones.Value[x, y] == null || stones.Value[x, y].Active;
        }

        private bool InBounds(int x, int y)
        {
            return x >= 0 && x < Lines && y >= 0 && y < Lines;
        }

        private bool GroupAlive(Stone st, out List<Stone> stgrp)
        {
            var adj = AdjacentStones(st);

            var alive = false;

            var grp = new List<Stone> {st};

            var workGrp = new Queue<Stone>();

            foreach (var adjst in adj)
                workGrp.Enqueue(adjst);

            while (workGrp.Count > 0)
            {
                var s = workGrp.Dequeue();

                if (Stone.InList(grp, s))
                    continue;

                if (s == null)
                {
                    stgrp = null;
                    return true;
                    //alive = true;
                    //continue;
                }

                if (s != st && s.Equals(st)) //Same colour
                {
                    grp.Add(s);
                    foreach (var adjStone in AdjacentStones(s))
                        workGrp.Enqueue(adjStone);
                }
            }

            stgrp = grp;

            return alive;
        }
    }
}