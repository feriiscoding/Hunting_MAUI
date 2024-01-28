using Moq;
using Hunting.Model;
using Hunting.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System;
using System.Threading.Tasks;

namespace Hunting.Test
{
    [TestClass]
    public class HuntingTest
    {
        private Mock<IHuntingDataAccess> _mock = null!;
        private HuntingGameModel _game = null!;
        private Player _player = null!;

        [TestInitialize]
        public void Initialize()
        {
            _player = new Player(3);
            _mock = new Mock<IHuntingDataAccess>();
            _mock.Setup(mock => mock.LoadAsync(It.IsAny<string>())).Returns(() => Task.FromResult(_player));
            _game = new HuntingGameModel(_mock.Object);
        }

        [TestMethod]
        public void HuntingConstructorTest()
        {
            _game.NewGame();
            Assert.AreEqual(3, _game.TableSize);
            Assert.AreEqual(1, _game.Table.Prey.Count);
            Assert.AreEqual(4, _game.Table.Hunters.Count);
            Assert.AreEqual(4, _game.Table.Empty.Count);
            int[] array = new int[] { 1, 1 };
            Assert.AreEqual(array[0], _game.Table.Prey[0][0]);
            Assert.AreEqual(array[1], _game.Table.Prey[0][1]);
            bool contains = false;
            for (int i = 0; i < 2; i += 2)
            {
                for (int j = 0; j < 2; j += 2)
                {
                    array[0] = i; array[1] = j;
                    foreach (int[] coordinate in _game.Table.Hunters)
                    {
                        if (coordinate[0] == array[0] && coordinate[1] == array[1])
                        {
                            contains = true;
                            break;
                        }
                    }
                    if (!contains) { break; }
                }
                if (!contains) { break; }
            }
            Assert.IsTrue(contains);
            contains = false;
            array = new int[] { 0, 1 };
            Assert.AreEqual(array[0], _game.Table.Empty[0][0]);
            Assert.AreEqual(array[1], _game.Table.Empty[0][1]);
            Assert.AreEqual(array[0], _game.Table.Empty[1][1]);
            Assert.AreEqual(array[1], _game.Table.Empty[1][0]);
            array = new int[] { 1, 2 };
            Assert.AreEqual(array[0], _game.Table.Empty[2][0]);
            Assert.AreEqual(array[1], _game.Table.Empty[2][1]);
            Assert.AreEqual(array[0], _game.Table.Empty[3][1]);
            Assert.AreEqual(array[1], _game.Table.Empty[3][0]);
        }
        [TestMethod]
        public void HuntingSelectNextPlayerTest()
        {
            _game.NewGame();
            if (_game.CurrentPlayer == _game.Table.Hunters)
            {
                int[] array = new int[2];
                array = _game.SelectNextPlayer(0, 0);
                Assert.AreEqual(0, array[0]);
                Assert.AreEqual(0, array[1]);
            }
            else
            {
                int[] array = new int[2];
                array = _game.SelectNextPlayer(1, 1);
                Assert.AreEqual(1, array[0]);
                Assert.AreEqual(1, array[1]);
            }
            _game.NewGame();
            try
            {
                _game.SelectNextPlayer(0, 1);
                Assert.Fail();
            }
            catch (InvalidOperationException) { }
        }
        [TestMethod]
        public void HuntingStepTest()
        {
            _game.NewGame();
            if (_game.CurrentPlayer == _game.Table.Hunters)
            {
                int[] array = _game.SelectNextPlayer(0, 0);
                _game.Step(array, 0, 1);
                Assert.AreEqual(_game.CurrentPlayer, _game.Table.Prey);
                Assert.AreEqual(array[0], _game.Table.Empty[3][0]);
                Assert.AreEqual(array[1], _game.Table.Empty[3][1]);
                array[0] = 0; array[1] = 1;
                Assert.AreEqual(array[0], _game.Table.Hunters[3][0]);
                Assert.AreEqual(array[1], _game.Table.Hunters[3][1]);
            }
            else
            {
                int[] array = _game.SelectNextPlayer(1, 1);
                _game.Step(array, 0, 1);
                Assert.AreEqual(_game.CurrentPlayer, _game.Table.Hunters);
                Assert.AreEqual(array[0], _game.Table.Empty[3][0]);
                Assert.AreEqual(array[1], _game.Table.Empty[3][1]);
                array[0] = 0; array[1] = 1;
                Assert.AreEqual(array[0], _game.Table.Prey[0][0]);
                Assert.AreEqual(array[1], _game.Table.Prey[0][1]);
            }
            _game.NewGame();
            if (_game.CurrentPlayer == _game.Table.Hunters)
            {
                int[] arr = _game.SelectNextPlayer(0, 0);
                try
                {
                    _game.Step(arr, 0, 2);
                    Assert.Fail();
                }
                catch (ArgumentOutOfRangeException) { }
            }
            else
            {
                int[] arr = _game.SelectNextPlayer(1, 1);
                try
                {
                    _game.Step(arr, 0, 2);
                    Assert.Fail();
                }
                catch (ArgumentOutOfRangeException) { }
            }
        }
        [TestMethod]
        public async Task HuntingLoadTest()
        {
            _game.NewGame();
            if (_game.CurrentPlayer == _game.Table.Hunters)
            {
                int[] array = new int[2];
                array = _game.SelectNextPlayer(0, 0);
                _game.Step(array, 0, 1);
                array = _game.SelectNextPlayer(1, 1);
                _game.Step(array, 1, 0);
                array = _game.SelectNextPlayer(2, 2);
                _game.Step(array, 1, 2);
                array = _game.SelectNextPlayer(1, 0);
                _game.Step(array, 0, 0);

                Assert.AreEqual(4, _game.TurnCount);
                try
                {
                    await _game.LoadGameAsync(String.Empty);
                    Assert.Fail();
                    _mock.Verify(dataAccess => dataAccess.LoadAsync(string.Empty), Times.Once());
                }
                catch (DataException) { }
            }
            else
            {
                int[] array = new int[2];
                array = _game.SelectNextPlayer(1, 1);
                _game.Step(array, 0, 1);
                array = _game.SelectNextPlayer(0, 0);
                _game.Step(array, 1, 0);
                array = _game.SelectNextPlayer(0, 1);
                _game.Step(array, 0, 0);
                array = _game.SelectNextPlayer(1, 0);
                _game.Step(array, 1, 1);

                Assert.AreEqual(4, _game.TurnCount);
                try
                {
                    await _game.LoadGameAsync(String.Empty);
                    Assert.Fail();
                    _mock.Verify(dataAccess => dataAccess.LoadAsync(string.Empty), Times.Once());
                }
                catch (DataException) { }
            }
        }
        [TestMethod]
        public async Task HuntingSaveTest()
        {
            _game.NewGame();
            List<int[]> currentPlayer = _game.CurrentPlayer;
            int stepCount = _game.TurnCount;
            string who = "";
            if (currentPlayer == _game.Table.Hunters)
            {
                who = "h";
            }
            else { who = "p"; }

            await _game.SaveGameAsync(String.Empty);
            Assert.AreEqual(currentPlayer, _game.CurrentPlayer);
            Assert.AreEqual(stepCount, _game.TurnCount);

            _mock.Verify(mock => mock.SaveAsync(String.Empty, It.IsAny<Player>(), stepCount, who, 3), Times.Once());
        }
        [TestMethod]
        public void HuntingCheckIfOverTest()
        {
            bool eventRaised = false;
            _game.GameOver += delegate (object? sender, HuntingEventArgs e)
            {
                eventRaised = true;
            };
            _game.NewGame();
            if (_game.CurrentPlayer == _game.Table.Hunters)
            {
                int[] array = new int[2];
                array = _game.SelectNextPlayer(0, 0);
                _game.Step(array, 0, 1);
                array = _game.SelectNextPlayer(1, 1);
                _game.Step(array, 1, 2);
                array = _game.SelectNextPlayer(0, 1);
                _game.Step(array, 1, 1);
            }
            else
            {
                int[] array = new int[2];
                array = _game.SelectNextPlayer(1, 1);
                _game.Step(array, 0, 1);
                array = _game.SelectNextPlayer(2, 0);
                _game.Step(array, 1, 0);
                array = _game.SelectNextPlayer(0, 1);
                _game.Step(array, 1, 1);
                array = _game.SelectNextPlayer(2, 2);
                _game.Step(array, 1, 2);
                array = _game.SelectNextPlayer(1, 1);
                _game.Step(array, 0, 1);
                array = _game.SelectNextPlayer(1, 0);
                _game.Step(array, 1, 1);
            }
            Assert.IsTrue(eventRaised);

        }
    }
}