using Hunting.ViewModel;
using Hunting.Model;
using Hunting.View;
using Hunting.Persistence;
namespace Hunting
{
    public partial class AppShell : Shell
    {
        private IHuntingDataAccess _dataAccess;
        private readonly HuntingGameModel _gameModel;
        private HuntingViewModel _viewModel;

        private readonly Store _store;
        private readonly StoredGameBrowserModel _gameBrowserModel;
        private readonly StoredGameBrowserViewModel _gameBrowserViewModel;

        public AppShell(Store store, IHuntingDataAccess data, HuntingGameModel model, HuntingViewModel viewModel)
        {
            InitializeComponent();

            _store = store;
            _dataAccess = data;
            _gameModel = model;
            _viewModel = viewModel;

            _gameModel.GameOver += new EventHandler<HuntingEventArgs>(Model_GameOver);

            _viewModel.LoadGame += new EventHandler(ViewModel_LoadGame);
            _viewModel.SaveGame += new EventHandler(ViewModel_SaveGame);

            _gameBrowserModel = new StoredGameBrowserModel(_store);
            _gameBrowserViewModel = new StoredGameBrowserViewModel(_gameBrowserModel);
            _gameBrowserViewModel.GameLoading += new EventHandler<StoredGameEventArgs>(StoreViewModel_GameLoading);
            _gameBrowserViewModel.GameSaving += new EventHandler<StoredGameEventArgs>(StoreViewModel_GameSaving);
        }
        private async void Model_GameOver(object? sender, HuntingEventArgs e)
        {
            if (e.HuntersWon)
            {
                await DisplayAlert("Game over!","The cats won the game!", "OK");
            }
            if (e.PreyWon)
            {
                await DisplayAlert("Game over!","The tuna won the game!", "OK");
            }
            _gameModel.NewGame();
        }
        private async void ViewModel_LoadGame(object? sender, EventArgs e)
        {
            await _gameBrowserModel.UpdateAsync();
            await Navigation.PushAsync(new LoadGamePage
            {
                BindingContext = _gameBrowserViewModel
            });
        }
        private async void ViewModel_SaveGame(object? sender, EventArgs e)
        {
            await _gameBrowserModel.UpdateAsync();
            await Navigation.PushAsync(new SaveGamePage
            {
                BindingContext = _gameBrowserViewModel
            });
        }
        private async void StoreViewModel_GameLoading(object? sender, StoredGameEventArgs e)
        {
            await Navigation.PopAsync();
            try
            {
                await _gameModel.LoadGameAsync(e.Name);
                await Navigation.PopAsync();
                await DisplayAlert("Hunting", "Successful loading.", "OK");
            }
            catch
            {
                await DisplayAlert("Error!", "An error occured during loading.", "OK");
            }
        }
        private async void StoreViewModel_GameSaving(object? sender, StoredGameEventArgs e)
        {
            await Navigation.PopAsync();

            try
            {
                await _gameModel.SaveGameAsync(e.Name);
                await DisplayAlert("Hunting", "Successful saving.", "OK");
            }
            catch
            {
                await DisplayAlert("Error!", "An error occured during saving.", "OK");
            }
        }
    }
}
