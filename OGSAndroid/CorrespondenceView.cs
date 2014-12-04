using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace OGSAndroid
{
    class CorrespondenceView : SGFView
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
            OGSAPI.SendMove(mv,gid);

        }
                    
    }
}