using System;

namespace OGSAndroid
{
	//Game representation taken from JSON.
    public class OGSGame
    {
    	//There is a lot more but it can be grabbed from the SGF.
    	public string Name;
    	public string ID;
        public string Result;
    	public Player Black;
    	public Player White;

        public OGSGame()
        {
        }

        public struct Player
        {
        	public string Username;
            public string Country;
            public string Icon;
            public string Id;
        }

        public override string ToString()
        {
            return Name + "  -  " + Black.Username + " vs " + White.Username + "   Result: " + Result; 
        }
    }
}

