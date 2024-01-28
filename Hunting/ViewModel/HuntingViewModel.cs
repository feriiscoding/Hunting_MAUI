using Hunting.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hunting.ViewModel;

namespace Hunting.ViewModel
{
    public class HuntingViewModel : ViewModelBase
    {
        private HuntingGameModel _model;
        private bool _isSelected;
        private int _xSelected;
        private int _ySelected;
        private string tunaTurn;
        private string catTurn;
        private int[] _selected;
        private int _gridSize;
        public int Easy { get { return 3; } }
        public int Medium { get { return 5; } }
        public int Hard { get { return 7; } }
        public string TurnCount { get { return _model.TurnCount.ToString(); } }
        public int AllTurns { get { return _model.AllTurns; } }

        public string TunaTurn { get { return tunaTurn; } }
        public string CatTurn { get { return catTurn; } }
        public string WhosTurn
        {
            get
            {
                if (_model.CurrentPlayer == _model.Table.Hunters) { return catTurn; }
                return tunaTurn;
            }
        }
        public int GridSize
        {
            get { return _gridSize; }
            set
            {
                _gridSize = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TableRowCount));
                OnPropertyChanged(nameof(TableColumnCount));
            }
        }
        public RowDefinitionCollection TableRowCount
        {
            get => new RowDefinitionCollection(Enumerable.Repeat(new RowDefinition(GridLength.Star), GridSize).ToArray());
        }
        public ColumnDefinitionCollection TableColumnCount
        {
            get => new ColumnDefinitionCollection(Enumerable.Repeat(new ColumnDefinition(GridLength.Star), GridSize).ToArray());
        }
        public DelegateCommand NewGameCommand { get; private set; }
        public DelegateCommand LoadGameCommand { get; private set; }
        public DelegateCommand SaveGameCommand { get; private set; }
        public DelegateCommand ExitGameCommand { get; private set; }
        public ObservableCollection<HuntingField> Fields { get; set; }

        public event EventHandler? ExitGame;
        public event EventHandler? LoadGame;
        public event EventHandler? SaveGame;

        public HuntingViewModel(HuntingGameModel model)
        {
            _model = model;

            _model.GameAdvanced += new EventHandler<HuntingEventArgs>(Model_GameAdvanced!);
            _model.GameOver += new EventHandler<HuntingEventArgs>(Model_GameOver!);
            _model.GameStarted += new EventHandler<HuntingEventArgs>(Model_GameStarted!);

            NewGameCommand = new DelegateCommand(OnNewGame!);
            LoadGameCommand = new DelegateCommand(param => OnLoadGame());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
            ExitGameCommand = new DelegateCommand(param => OnGameExit());

            Fields = new ObservableCollection<HuntingField>();
            _isSelected = false;
            _xSelected = 0;
            _ySelected = 0;
            tunaTurn = "It's the tuna's turn!";
            catTurn = "It's the cat's turn!";
            _selected = new int[2];
            _gridSize = _model.TableSize;

            Refresh();
            OnPropertyChanged("WhosTurn");
        }

        private void Refresh()
        {
            Fields.Clear();
            for (int i = 0; i < _gridSize; i++)
            {
                for (int j = 0; j < _gridSize; j++)
                {
                    int[] coord = new int[] { i, j };
                    foreach (int[] hunter in _model.Table.Hunters)
                    {
                        if (hunter[0] == coord[0] && hunter[1] == coord[1])
                        {
                            Fields.Add(new HuntingField
                            {
                                IsHunter = true,
                                IsEmpty = false,
                                IsPrey = false,
                                X = i,
                                Y = j,
                                ImageClickCommand = new DelegateCommand(param =>
                                {
                                    if (param is HuntingField field && _model.CurrentPlayer == _model.Table.Hunters && !_isSelected)
                                    {
                                        try
                                        {
                                            _selected = _model.SelectNextPlayer(field.X, field.Y);
                                            _xSelected = field.X;
                                            _ySelected = field.Y;
                                            _isSelected = true;
                                        }
                                        catch { }
                                    }
                                })
                            });
                        }
                    }
                    if (_model.Table.Prey[0][0] == coord[0] && _model.Table.Prey[0][1] == coord[1])
                    {
                        Fields.Add(new HuntingField
                        {
                            IsHunter = false,
                            IsEmpty = false,
                            IsPrey = true,
                            X = i,
                            Y = j,
                            ImageClickCommand = new DelegateCommand(param =>
                            {
                                if (param is HuntingField field && _model.CurrentPlayer == _model.Table.Prey && !_isSelected)
                                {
                                    try
                                    {
                                        _selected = _model.SelectNextPlayer(field.X, field.Y);
                                        _xSelected = field.X;
                                        _ySelected = field.Y;
                                        _isSelected = true;
                                    }
                                    catch { }
                                }
                            })
                        });
                    }
                    foreach (int[] empty in _model.Table.Empty)
                    {
                        if (empty[0] == coord[0] && empty[1] == coord[1])
                        {
                            Fields.Add(new HuntingField
                            {
                                IsHunter = false,
                                IsEmpty = true,
                                IsPrey = false,
                                X = i,
                                Y = j,
                                ImageClickCommand = new DelegateCommand(param =>
                                {
                                    if (param is HuntingField field)
                                    {
                                        if (field.IsEmpty && _isSelected)
                                        {
                                            try
                                            {
                                                Model_Step(field.X, field.Y);
                                                _isSelected = false;
                                            }
                                            catch { }
                                        }
                                    }
                                })
                            });
                        }
                    }

                }
            }
            OnPropertyChanged("TurnCount");
            OnPropertyChanged("WhosTurn");
        }
        public void Model_SelectNextPlayer(int x, int y)
        {
            for (int i = 0; i < _gridSize * _gridSize; i++)
            {
                if (Fields[i].X == x && Fields[i].Y == y)
                {
                    try
                    {
                        _selected = _model.SelectNextPlayer(x, y);
                        _xSelected = x;
                        _ySelected = y;
                        _isSelected = true;
                    }
                    catch { throw new InvalidOperationException("Invalid field selected"); }
                }
            }
        }
        public void Model_Step(int x, int y)
        {
            try
            {
                if ((x == _xSelected && (y == _ySelected + 1 || y == _ySelected - 1)) || (y == _ySelected && (x == _xSelected + 1 || x == _xSelected - 1)))
                {
                    foreach (HuntingField field in Fields)
                    {
                        if (field.X == x && field.Y == y)
                        {
                            if (_model.CurrentPlayer == _model.Table.Hunters)
                            {
                                field.IsEmpty = false;
                                field.IsPrey = false;
                                field.IsHunter = true;
                            }
                            else if (_model.CurrentPlayer == _model.Table.Prey)
                            {
                                field.IsEmpty = false;
                                field.IsHunter = false;
                                field.IsPrey = true;
                            }
                        }
                        if (field.X == _xSelected && field.Y == _ySelected)
                        {
                            field.IsEmpty = true;
                            field.IsHunter = false;
                            field.IsPrey = false;
                        }
                    }
                    _model.Step(_selected, x, y);
                }
                Refresh();
            }
            catch { throw new InvalidOperationException("Ivalid step."); }
        }

        private void Model_GameStarted(object sender, HuntingEventArgs e)
        {
            if (_gridSize != _model.TableSize)
            {
                GridSize = _model.TableSize;
                _model.AllTurns = _model.TableSize * 4;
            }
            Refresh();
        }
        private void Model_GameOver(object sender, HuntingEventArgs e)
        {
            Fields.Clear();
        }
        private void Model_GameAdvanced(object sender, HuntingEventArgs e)
        {
            OnPropertyChanged("TurnCount");
            OnPropertyChanged("WhosTurn");
        }
        private void OnNewGame(object param)
        {
            _model.TableSize = (int)param;
            _model.NewGame();
            Refresh();
        }
        private void OnLoadGame()
        {
            LoadGame?.Invoke(this, EventArgs.Empty);
            GridSize = _model.TableSize;
            Refresh();
        }
        private void OnSaveGame()
        {
            SaveGame?.Invoke(this, EventArgs.Empty);
        }
        private void OnGameExit()
        {
            ExitGame?.Invoke(this, EventArgs.Empty);
        }
    }
}
