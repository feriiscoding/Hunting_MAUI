using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hunting.Persistence;

namespace Hunting.Model
{
    public class HuntingGameModel
    {
        #region Private Fields
        private Player _table;
        private List<int[]> _currentPlayer;
        private int tableSize;
        private int allturns;
        private int _turnCount;
        private IHuntingDataAccess _persistence;
        #endregion

        #region Public Properties
        public Player Table { get { return _table; } }
        public List<int[]> CurrentPlayer { get { return _currentPlayer; } }
        public int TableSize { get { return tableSize; } set { tableSize = value; } }
        public int AllTurns { get { return allturns; } set { allturns = value; } }
        public int TurnCount { get { return _turnCount; } }
        #endregion

        #region Events
        public event EventHandler<HuntingEventArgs>? GameAdvanced;
        public event EventHandler<HuntingEventArgs>? GameOver;
        public event EventHandler<HuntingEventArgs>? GameStarted;
        #endregion

        #region Constructor
        public HuntingGameModel(IHuntingDataAccess persistence)
        {
            _currentPlayer = new List<int[]>();
            tableSize = 3;
            allturns = tableSize * 4;
            _turnCount = 1;
            _persistence = persistence;
            _table = new Player(tableSize);
        }
        #endregion

        #region Public Methods
        public void NewGame()
        {
            _turnCount = 0;
            _table = new Player(tableSize);
            Random random = new Random();
            if (random.Next(0, 2) == 0)
            {
                _currentPlayer = _table.Hunters;
            }
            else { _currentPlayer = _table.Prey; }
            OnGameStarted();
        }
        public void AdvanceGame()
        {
            _turnCount++;
            OnGameAdvanced();
        }

        public int[] SelectNextPlayer(int x, int y)
        {
            int[] array = new int[2];
            array[0] = x;
            array[1] = y;
            foreach (int[] i in _currentPlayer)
            {
                if (i[0] == array[0] && i[1] == array[1])
                {
                    return array;
                }
            }
            throw new InvalidOperationException("Your player is not on this field!");

        }

        public void Step(int[] arr, int x, int y)
        {
            int[] next = new int[2];
            next[0] = x;
            next[1] = y;
            bool contains = false;
            if (x < 0 || x >= TableSize) { throw new ArgumentOutOfRangeException("Bad column index!"); }
            if (y < 0 || y >= TableSize) { throw new ArgumentOutOfRangeException("Bad row index!"); }
            if (_turnCount >= allturns) { throw new InvalidOperationException("The game has ended!"); }
            if (!(x == arr[0] && y == arr[1] + 1) && !(x == arr[0] && y == arr[1] - 1) && !(x == arr[0] + 1 && y == arr[1]) && !(x == arr[0] - 1 && y == arr[1])) { throw new ArgumentOutOfRangeException("The field is not next your figure!"); }
            foreach (int[] i in _table.Empty)
            {
                if (i[0] == x && i[1] == y)
                {
                    contains = true;
                    break;
                }
            }
            if (!contains) { throw new InvalidOperationException("The field is not empty!"); }
            _table.Empty.Add(arr);
            foreach (int[] i in _table.Empty)
            {
                if (i[0] == x && i[1] == y)
                {
                    _table.Empty.Remove(i);
                    break;
                }
            }
            if (_currentPlayer == _table.Hunters)
            {
                _table.Hunters.Add(next);
                foreach (int[] i in _table.Hunters)
                {
                    if (i[0] == arr[0] && i[1] == arr[1])
                    {
                        _table.Hunters.Remove(i);
                        break;
                    }
                }
                _currentPlayer = _table.Prey;
            }
            else
            {
                _table.Prey.Add(next);
                foreach (int[] i in _table.Prey)
                {
                    if (i[0] == arr[0] && i[1] == arr[1])
                    {
                        _table.Prey.Remove(i);
                        break;
                    }
                }
                _currentPlayer = _table.Hunters;
            }

            _turnCount++;
            OnGameAdvanced();
            CheckIfOver();
        }
        private void OnGameAdvanced()
        {
            GameAdvanced?.Invoke(this, new HuntingEventArgs(CurrentPlayer, _turnCount, false, false));
        }
        private void OnGameOver(bool huntersWon, bool preyWon)
        {
            GameOver?.Invoke(this, new HuntingEventArgs(_currentPlayer, _turnCount, huntersWon, preyWon));
        }
        private void OnGameStarted()
        {
            GameStarted?.Invoke(this, new HuntingEventArgs(_currentPlayer, _turnCount, false, false));
        }
        public async Task LoadGameAsync(String path)
        {
            if (_persistence == null) { return; }
            Player loadPlayer = await _persistence.LoadAsync(path);
            int size = (int)Math.Sqrt(loadPlayer.Empty.Count + 5);
            _table = new Player(size);
            _table.Empty = new List<int[]>();
            _table.Hunters = new List<int[]>();
            _table.Prey = new List<int[]>();
            _currentPlayer = new List<int[]>();
            this.tableSize = size;
            allturns = size * 4;
            if (loadPlayer.Empty.Count == 0 || loadPlayer.Hunters.Count == 0 || loadPlayer.Prey.Count == 0 || _persistence.TurnCount >= size * 4 || (_persistence.Who != "h" && _persistence.Who != "p"))
            {
                throw new DataException("The game could not load properly!");
            }
            _turnCount = _persistence.TurnCount;
            _table.Empty = loadPlayer.Empty;
            _table.Hunters = loadPlayer.Hunters;
            _table.Prey = loadPlayer.Prey;
            if (_persistence.Who == "h") { _currentPlayer = _table.Hunters; }
            else { _currentPlayer = _table.Prey; }

            OnGameStarted();
            CheckIfOver();
        }

        public async Task SaveGameAsync(String path)
        {
            string who = "";
            if (_currentPlayer == _table.Hunters) { who = "h"; }
            else { who = "p"; }
            await _persistence.SaveAsync(path, _table, _turnCount, who, tableSize);
        }
        #endregion

        #region Private Methods
        private void CheckIfOver()
        {
            bool broken = false;
            if (_turnCount >= allturns) { OnGameOver(false, true); }
            else
            {
                foreach (int[] fields in _table.Prey)
                {
                    int row = fields[0];
                    int col = fields[1];
                    foreach (int[] i in _table.Empty)
                    {
                        if ((i[0] == row && i[1] == col + 1) || (i[0] == row && i[1] == col - 1) || (i[0] == row + 1 && i[1] == col) || (i[0] == row - 1 && i[1] == col))
                        {
                            broken = true;
                            break;
                        }
                    }
                    if (broken) break;
                }
                if (!broken)
                {
                    OnGameOver(true, false);
                }
            }
        }
        #endregion
    }
}
