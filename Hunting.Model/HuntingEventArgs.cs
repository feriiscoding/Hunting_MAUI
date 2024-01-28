namespace Hunting.Model
{
    public class HuntingEventArgs : EventArgs
    {
        private int turnCount;
        private bool huntersWon;
        private bool preyWon;
        public List<int[]> player { get; private set; }
        public int TurnCount { get { return turnCount; } }
        public bool HuntersWon { get { return huntersWon; } }
        public bool PreyWon { get { return preyWon; } }
        public HuntingEventArgs(List<int[]> player, int turnCount, bool huntersWon, bool preyWon)
        {
            this.player = player;
            this.turnCount = turnCount;
            this.huntersWon = huntersWon;
            this.preyWon = preyWon;
        }
    }
}
