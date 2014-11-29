using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Util;

namespace OGSAndroid
{
	public class BoardView : View
	{
        public int Padding { get; set; }
        public int Lines{ get; set; }
        public int Size { get; private set; }
        public int Spacing { get; private set; }
        public int ExtPad { get; private set; }

        private readonly Paint blackPaint;
        private readonly Paint whitePaint;
        private readonly Paint bgPaint;     
        private readonly BoardTouch boardTouch;       
        private bool firstDraw = true;
        private bool initialized = false;

        public Stone[,] stones;
        public Stone CurrentTurn = Stone.Black;


		public BoardView(Context context, IAttributeSet attrs) : base(context)
		{

            blackPaint = new Paint
            {
                AntiAlias = true,
                Color = Color.Black,
                StrokeWidth = 2
            };

            whitePaint = new Paint
            {
                AntiAlias = true,
                Color = new Color(180,180,180),
            };

            bgPaint = new Paint
            {
                AntiAlias = true,
                Color = Color.SandyBrown,
            };

            Bitmap bmp = BitmapFactory.DecodeResource(Resources, Resource.Drawable.woodtex);
            bgPaint.SetShader(new BitmapShader(bmp,Shader.TileMode.Repeat,Shader.TileMode.Repeat));
            boardTouch = new BoardTouch(this);          
            Padding = 20;
            Invalidate();         
		}

        public void Initialize(int lines)
        {
            Lines = lines;
            stones = new Stone[Lines, Lines];
            initialized = true;
        }

		protected override void OnDraw(Canvas canvas)
		{
			base.OnDraw (canvas);

            if (!initialized)
                throw new Exception("BoardView must be Initialize()'d");
           
            //Could remove these from every draw if we need to optimize;
            Size = Math.Min(canvas.Width, canvas.Height);
            ExtPad = this.PaddingLeft; //Make sure padding top/left are always the same.
            Spacing = (Size-((ExtPad+Padding)*2)) / (Lines-1);
            Padding = (Spacing / 2) + 2;

            DrawBoard(canvas);

            foreach (var s in stones)
            {
                if(s != null)
                    DrawStone(canvas, s);
            }

            if (boardTouch.ConfirmStoneActive)
            {
                DrawStone(canvas, boardTouch.ConfirmStone, false);
            }
                
            if (firstDraw) //Draw again to fix padding/spacing dependency on eachother.
            {
                firstDraw = false;               
                RelativeLayout.LayoutParams par = new RelativeLayout.LayoutParams(Size, Size);
                this.LayoutParameters = par;                              
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
                var xy = (Spacing * i) + Padding + ExtPad; 
                canvas.DrawLine (Padding + ExtPad, xy,ExtPad + Padding+(Lines-1)*Spacing, xy, paint);
                canvas.DrawLine (xy, Padding + ExtPad, xy,ExtPad + Padding+(Lines-1)*Spacing, paint);
			}               
		}

        private void DrawStarPoints(Canvas canvas)
        {
            if (Lines < 9)
                return;

            var radius = (Spacing / 8) + 1;

            //All boards > 9x have these.
            canvas.DrawCircle(ExtPad + Padding + (3 * Spacing), ExtPad + Padding + (3 * Spacing), radius, blackPaint); //TopLeft
            canvas.DrawCircle(ExtPad + Padding + ((Lines-4) * Spacing), ExtPad + Padding + (3 * Spacing), radius, blackPaint); //TopRight
            canvas.DrawCircle(ExtPad + Padding + (3 * Spacing), ExtPad + Padding + ((Lines-4) * Spacing), radius, blackPaint); //BottomLeft
            canvas.DrawCircle(ExtPad + Padding + ((Lines-4) * Spacing), ExtPad + Padding + ((Lines-4) * Spacing), radius, blackPaint); //BottomRight
            canvas.DrawCircle(ExtPad + Padding + ((Lines/2) * Spacing), ExtPad + Padding + ((Lines/2) * Spacing), radius, blackPaint); //Middle

            //Todo: 19x points.

        }

        private void DrawStone(Canvas canvas, Stone stone, bool alpha = true)
        {
            Paint col;
            if (stone)
            {
                col = blackPaint;
                col.SetShader(new RadialGradient(ExtPad + Padding + ((stone.x - 1.2f) * Spacing), ExtPad + Padding + ((stone.y - 1.3f) * Spacing), 40, new Color(45, 45, 45), col.Color, Shader.TileMode.Mirror));
            }
            else
            {
                col = whitePaint;
                col.SetShader(new RadialGradient(ExtPad + Padding + ((stone.x - 1.2f) * Spacing),ExtPad +  Padding + ((stone.y - 1.3f) * Spacing), 40, new Color(240, 240, 240), col.Color, Shader.TileMode.Mirror));
            }

            if (!alpha)
                col.Alpha = 100;

            col.AntiAlias = true;

            canvas.DrawCircle(ExtPad + Padding + ((stone.x-1) * Spacing), ExtPad + Padding + ((stone.y-1) * Spacing), (Spacing / 2), col);
            col.Alpha = 255;
        }
            
        public void PlaceStone(Stone s)
        {
           /* if (Stone.StoneAlreadyExists(s, stones))
            {
                stones.Remove(s);
                this.Invalidate();

                return;
            }*/
                
            CurrentTurn = CurrentTurn ? Stone.White : Stone.Black;

            stones[s.x-1, s.y-1] = s;
            Console.WriteLine(s.x +","+ s.y);

            this.Invalidate();
            CapturePass(s);
        }

        public void ClearBoard()
        {
            CurrentTurn = Stone.Black;
            boardTouch.Reset();
            stones = new Stone[Lines, Lines];
            Invalidate();
        }

        //Rules

        private void CapturePass(Stone s)
        {
            //Foreach adjacent stone get group and check for life.
            var surr = SurroundingStones(s);
            foreach (var st in surr)
            {
                if (st == null)
                    continue;

                Stone[] grp;
                var alive = GroupAlive(st, out grp);

                if (!alive)
                {
                    foreach(var dst in grp)
                    {
                        stones[dst.x-1, dst.y-1] = null;
                    }
                }
            }
        }

        private Stone[] SurroundingStones(Stone st, bool remOwn = true)
        {
            List<Stone> adj = new List<Stone>();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    var xx = x + st.x;
                    var yy = y + st.y;

                    if (InBounds(xx-1, yy-1))
                    {
                        var res = stones[xx-1, yy-1];

                        if (remOwn)
                        {
                            if (res == st)
                                continue;
                        }

                        adj.Add(res);
                    }
                }
            }

            return adj.ToArray();
        }

        private Stone[] AdjacentStones(Stone s)
        {
            List<Stone> adj = new List<Stone>();

            var st = new Stone(s, s.x-1, s.y-1);

            //adj.Add(s); //Add self too.

            if (InBounds(st.x - 1, st.y)) adj.Add(stones[st.x - 1, st.y]); //Left
            if (InBounds(st.x + 1, st.y)) adj.Add(stones[st.x + 1, st.y]); //Right
            if (InBounds(st.x, st.y - 1)) adj.Add(stones[st.x, st.y - 1]); //Up
            if (InBounds(st.x, st.y + 1)) adj.Add(stones[st.x, st.y + 1]); //Down

            return adj.ToArray();
        }

        private bool InBounds(int x, int y)
        {
            return x >= 0 && x < Lines && y >= 0 && y < Lines;
        }          

        private bool GroupAlive(Stone st, out Stone[] stgrp)
        {
            var adj = AdjacentStones(st);

            bool alive = false;

            List<Stone> grp = new List<Stone>();
            grp.Add(st);

            Queue<Stone> workGrp = new Queue<Stone>();

            foreach (var adjst in adj)
                workGrp.Enqueue(adjst);

            while(workGrp.Count > 0)
            {
                var s = workGrp.Dequeue();

                if (Stone.InList(grp,s))
                    continue;

                if (s == null)
                {
                    alive = true;
                    continue;
                }                   
                    
                if (s != st && s.Equals(st))//Same colour
                {
                    grp.Add(s);
                    foreach (var adjStone in AdjacentStones(s))
                        workGrp.Enqueue(adjStone);
                }
            }

            stgrp = grp.ToArray();

            return alive;

        }

	}
}

