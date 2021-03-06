﻿#region

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

#endregion

namespace Quobject.SocketIoClientDotNet.Parser
{
    public class Packet<T>
    {
        public int Attachments;
        public T Data;
        public int Id = -1;
        public String Nsp;
        public int Type = -1;

        public Packet()
        {
        }

        public Packet(int type)
        {
            Type = type;
        }

        public Packet(int type, T data)
        {
            Type = type;
            Data = data;
        }
    }

    public class Packet
    {
        public int Attachments;
        public object Data;
        public int Id = -1;
        public String Nsp;
        public int Type = -1;

        public Packet()
        {
        }

        public Packet(int type)
            : this(type, JToken.Parse("{}"))
        {
        }

        public Packet(int type, object data)
        {
            Type = type;
            Data = data;
        }

        public override string ToString()
        {
            return string.Format("Type:{0} Id:{1} Nsp:{2} Data:{3} Attachments:{4}", Type, Id, Nsp, Data, Attachments);
        }

        public List<object> GetDataAsList()
        {
            JArray jarray;
            try{
                jarray = Data is JArray ? (JArray) Data : JArray.Parse((string) ((JValue) Data).Value);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Tried to parse unfinished packet");
                return null;
            }
            var args = new List<object>();
            foreach (var o in jarray)
            {
                if (o is JValue)
                {
                    var jval = (JValue) o;
                    if (jval != null)
                    {
                        args.Add(jval.Value);
                    }
                }
                else if (o is JToken)
                {
                    var jtoken = o;
                    if (jtoken != null)
                    {
                        args.Add(jtoken);
                        //args.Add(jtoken.ToString(Formatting.None));
                    }
                }
            }
            return args;
        }

        public static JArray Args2JArray(IEnumerable<object> _args)
        {
            var jsonArgs = new JArray();
            foreach (var o in _args)
            {
                jsonArgs.Add(o);
            }
            return jsonArgs;
        }

        public static JArray Remove(JArray a, int pos)
        {
            var na = new JArray();
            for (int i = 0; i < a.Count; i++)
            {
                if (i != pos)
                {
                    var v = a[i];
                    na.Add(v);
                }
            }
            return na;
        }
    }
}