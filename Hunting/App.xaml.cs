using Hunting.Model;
using Hunting.Persistence;
using Hunting.ViewModel;
using Hunting.View;
namespace Hunting
{
    public partial class App : Application
    {
        private readonly string SuspendedGameSavePath = "SuspendedGame";

        private readonly AppShell _appShell;
        private IHuntingDataAccess _huntingDataAccess;
        private HuntingGameModel _model;
        private readonly Store _huntingStore;
        private readonly HuntingViewModel _huntingViewModel;
        public App()
        {
            InitializeComponent();

            _huntingStore = new Store();
            _huntingDataAccess = new HuntingFileDataAccess(FileSystem.AppDataDirectory);

            _model = new HuntingGameModel(_huntingDataAccess);
            _huntingViewModel = new HuntingViewModel(_model);

            _appShell = new AppShell(_huntingStore, _huntingDataAccess, _model, _huntingViewModel)
            {
                BindingContext = _huntingViewModel
            };

            MainPage = _appShell;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            Window window = base.CreateWindow(activationState);

            window.Created += async (s, e) =>
            {
                _model.NewGame();
                try
                {
                    await _model.LoadGameAsync(SuspendedGameSavePath);
                }
                catch { }
            };
            window.Resumed += (s, e) =>
            {
                if (!File.Exists(Path.Combine(FileSystem.AppDataDirectory, SuspendedGameSavePath)))
                    return;
                Task.Run(async () => 
                {
                    try
                    {
                        await _model.LoadGameAsync(SuspendedGameSavePath);
                    }
                    catch{ }
                });
            };
            window.Stopped += (s, e) =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await _model.SaveGameAsync(SuspendedGameSavePath);
                    }
                    catch
                    {
                    }
                });
            };
            return window;
        }
    }
}
