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
        public int Padding { get; private set; }
        public int Lines { get; set; }
        public int Size { get; private set; }
        public int Spacing { get; private set; }

        private readonly Paint blackPaint;
        private readonly Paint whitePaint;
        private readonly Paint bgPaint;

        private readonly BoardTouch boardTouch;
        public readonly List<Stone> stones = new List<Stone>();

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
                Color = new Color(235,235,235),
            };

            bgPaint = new Paint
            {
                AntiAlias = true,
                Color = Color.SandyBrown,
            };
            Bitmap bmp = BitmapFactory.DecodeResource(Resources, Resource.Drawable.woodtex);

            bgPaint.SetShader(new BitmapShader(bmp,Shader.TileMode.Repeat,Shader.TileMode.Repeat));
    

            boardTouch = new BoardTouch(this);
		}

		protected override void OnDraw(Canvas canvas)
		{
			base.OnDraw (canvas);   

            Console.WriteLine("Color" + Color.SandyBrown);
 
			DrawBoard(canvas, Lines, 40);

            foreach (var s in stones)
            {
                DrawStone(canvas, s);
            }

            if (boardTouch.ConfirmStoneActive)
            {
                DrawStone(canvas, boardTouch.ConfirmStone, false);
            }

		}

        private void DrawBoard(Canvas canvas, int _lines, int _padding)
		{
            Lines = _lines;
            Padding = _padding;

            Size = Math.Min(canvas.Width, canvas.Height);
            Spacing = (Size-(Padding*2)) / (Lines-1);
                    
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
                var xy = (Spacing * i) + Padding;
                canvas.DrawLine (Padding, xy, Padding+(Lines-1)*Spacing, xy, paint);
                canvas.DrawLine (xy, Padding, xy, Padding+(Lines-1)*Spacing, paint);
			}
                
		}

        private void DrawStarPoints(Canvas canvas)
        {
            if (Lines < 9)
                return;

            var radius = Spacing / 8;

            //All boards > 9x have these.
            canvas.DrawCircle(Padding + (3 * Spacing), Padding + (3 * Spacing), radius, blackPaint); //TopLeft
            canvas.DrawCircle(Padding + ((Lines-4) * Spacing), Padding + (3 * Spacing), radius, blackPaint); //TopRight
            canvas.DrawCircle(Padding + (3 * Spacing), Padding + ((Lines-4) * Spacing), radius, blackPaint); //BottomLeft
            canvas.DrawCircle(Padding + ((Lines-4) * Spacing), Padding + ((Lines-4) * Spacing), radius, blackPaint); //BottomRight
            canvas.DrawCircle(Padding + ((Lines/2) * Spacing), Padding + ((Lines/2) * Spacing), radius, blackPaint); //Middle

            //Todo: 19x points.


        }
            
        public void PlaceStone(Stone s)
        {
            if (Stone.StoneAlreadyExists(s, stones))
            {
                stones.Remove(s);
                this.Invalidate();

                return;
            }
                

            CurrentTurn = CurrentTurn ? Stone.White : Stone.Black;

            stones.Add(s);

            CapturePass(s);
            this.Invalidate();
        }

        public void ClearBoard()
        {
            stones.Clear();
            CurrentTurn = Stone.Black;
            boardTouch.Reset();
            Invalidate();
        }

        public void CapturePass(Stone s)
        {
            foreach (var st in s.AdjacentStones(Lines))
            {
                if (Stone.GetStone(st.x, st.y, stones) == null)
                    continue;

                Stone[] grp;
                var alive = st.GroupAlive(stones, Lines,out grp);

                if (!alive)
                {
                    foreach(var dst in grp)
                    {
                        for (int i = 0; i < stones.Count; i++)
                        {
                            if(dst.EqualsExact(stones[i]))
                            {
                                stones.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void DrawStone(Canvas canvas, Stone stone, bool alpha = true)
        {
            Paint col;
            if (stone)
            {
                col = blackPaint;
                col.SetShader(new RadialGradient(Padding + ((stone.x - 1.2f) * Spacing), Padding + ((stone.y - 1.3f) * Spacing), 40, new Color(30, 30, 30), col.Color, Shader.TileMode.Mirror));
            }
            else
            {
                col = whitePaint;
                col.SetShader(new RadialGradient(Padding + ((stone.x - 1.2f) * Spacing), Padding + ((stone.y - 1.3f) * Spacing), 40, new Color(255, 255, 255), col.Color, Shader.TileMode.Mirror));
            }

            if (!alpha)
                col.Alpha = 100;

            canvas.DrawCircle(Padding + ((stone.x-1) * Spacing), Padding + ((stone.y-1) * Spacing), Spacing / 2.1f, col);
            col.Alpha = 255;
        }
                        


	}
}

