using Android.Content;
using Android.Util;

namespace OGSAndroid
{
    internal class CorrespondenceView : SGFView
    {
        public CorrespondenceView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public void ConfirmMove()
        {
            if (!boardTouch.ConfirmStoneActive) return;
        }

        private void SendMove(Move mv)
        {
            var gid = Moves.Info.ID;
            OGSAPI.SendMove(mv, gid);
        }
    }
}