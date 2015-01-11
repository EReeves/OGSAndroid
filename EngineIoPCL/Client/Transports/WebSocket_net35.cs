#region

using System;
using System.Collections.Generic;
using Quobject.EngineIoClientDotNet.Modules;
using Quobject.EngineIoClientDotNet.Parser;
using SuperSocket.ClientEngine;
using WebSocket4Net;

#endregion

namespace Quobject.EngineIoClientDotNet.Client.Transports
{
    public class WebSocket : Transport
    {
        public static readonly string NAME = "websocket";
        private WebSocket4Net.WebSocket ws;
        private readonly List<KeyValuePair<string, string>> Cookies;

        public WebSocket(Options opts)
            : base(opts)
        {
            Name = NAME;
            Cookies = new List<KeyValuePair<string, string>>();
            foreach (var cookie in opts.Cookies)
            {
                Cookies.Add(new KeyValuePair<string, string>(cookie.Key, cookie.Value));
            }
        }

        protected override void DoOpen()
        {
            var log = LogManager.GetLogger(Global.CallerName());
            log.Info("DoOpen uri =" + Uri());

            ws = new WebSocket4Net.WebSocket(Uri(), "", Cookies);
            ws.EnableAutoSendPing = false;
            if (ServerCertificate.Ignore)
            {
                ws.AllowUnstrustedCertificate = true;
            }
            ws.Opened += ws_Opened;
            ws.Closed += ws_Closed;
            ws.MessageReceived += ws_MessageReceived;
            ws.DataReceived += ws_DataReceived;
            ws.Error += ws_Error;
            ws.Open();
        }

        private void ws_DataReceived(object sender, DataReceivedEventArgs e)
        {
            var log = LogManager.GetLogger(Global.CallerName());
            log.Info("ws_DataReceived " + e.Data);
            OnData(e.Data);
        }

        private void ws_Opened(object sender, EventArgs e)
        {
            var log = LogManager.GetLogger(Global.CallerName());
            log.Info("ws_Opened " + ws.SupportBinary);
            OnOpen();
        }

        private void ws_Closed(object sender, EventArgs e)
        {
            var log = LogManager.GetLogger(Global.CallerName());
            log.Info("ws_Closed");
            ws.Opened -= ws_Opened;
            ws.Closed -= ws_Closed;
            ws.MessageReceived -= ws_MessageReceived;
            ws.DataReceived -= ws_DataReceived;
            ws.Error -= ws_Error;
            OnClose();
        }

        private void ws_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var log = LogManager.GetLogger(Global.CallerName());
            log.Info("ws_MessageReceived e.Message= " + e.Message);
            OnData(e.Message);
        }

        private void ws_Error(object sender, ErrorEventArgs e)
        {
            OnError("websocket error", e.Exception);
        }

        protected override void Write(List<Packet> packets)
        {
            Writable = false;
            foreach (var packet in packets)
            {
                Parser.Parser.EncodePacket(packet, new WriteEncodeCallback(this));
            }

            // fake drain
            // defer to next tick to allow Socket to clear writeBuffer
            //EasyTimer.SetTimeout(() =>
            //{
            Writable = true;
            Emit(EVENT_DRAIN);
            //}, 1);
        }

        protected override void DoClose()
        {
            if (ws != null)
            {
                try
                {
                    ws.Close();
                }
                catch (Exception e)
                {
                    var log = LogManager.GetLogger(Global.CallerName());
                    log.Info("DoClose ws.Close() Exception= " + e.Message);
                }
            }
        }

        public string Uri()
        {
            Dictionary<string, string> query = null;
            query = Query == null ? new Dictionary<string, string>() : new Dictionary<string, string>(Query);
            string schema = Secure ? "wss" : "ws";
            string portString = "";

            if (TimestampRequests)
            {
                query.Add(TimestampParam, DateTime.Now.Ticks + "-" + Timestamps++);
            }

            string _query = ParseQS.Encode(query);

            if (Port > 0 && (("wss" == schema && Port != 443)
                             || ("ws" == schema && Port != 80)))
            {
                portString = ":" + Port;
            }

            if (_query.Length > 0)
            {
                _query = "?" + _query;
            }

            return schema + "://" + Hostname + portString + Path + _query;
        }

        public class WriteEncodeCallback : IEncodeCallback
        {
            private readonly WebSocket webSocket;

            public WriteEncodeCallback(WebSocket webSocket)
            {
                this.webSocket = webSocket;
            }

            public void Call(object data)
            {
                //var log = LogManager.GetLogger(Global.CallerName());

                if (data is string)
                {
                    webSocket.ws.Send((string) data);
                }
                else if (data is byte[])
                {
                    var d = (byte[]) data;

                    //try
                    //{
                    //    var dataString = BitConverter.ToString(d);
                    //    //log.Info(string.Format("WriteEncodeCallback byte[] data {0}", dataString));
                    //}
                    //catch (Exception e)
                    //{
                    //    log.Error(e);
                    //}

                    webSocket.ws.Send(d, 0, d.Length);
                }
            }
        }
    }
}