namespace OGSAndroid.Game
{
    //Game representation taken from JSON.
    public class OGSGame
    {
        //There is a lot more but it can be grabbed from the SGF.
        public Player Black;
        public string ID;
        public string Name;
        public string Result;
        public string TimeControl;
        public Player White;

        public override string ToString()
        {
            return Name + "  -  " + Black.Username + " vs " + White.Username + "   Result: " + Result;
        }

        public struct Player
        {
            public string Country;
            public string Icon;
            public string Id;
            public string Rank;
            public string Username;
        }
    }
}