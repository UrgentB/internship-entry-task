using Dobrodum_modulbank_test.Models;
using Newtonsoft.Json;
using Xunit;

namespace CrestNullAPITests
{
    public class CrestNullGameTests
    {
        [Fact]
        public void Constructor_InitializesGameCorrectly()
        {
            // Паметры игры
            uint fieldSize = 3;
            uint winningLength = 3;
            uint percentage = 50;

            var game = new CrestNullGame(fieldSize, winningLength, percentage);

            Assert.Equal(fieldSize, game.FieldSize);
            Assert.Equal(winningLength, game.WinningLenght);
            Assert.Equal(percentage, game.PercentageOfOccupiedCells);
            Assert.Equal(0u, game.Round);
            Assert.Equal(GameStateEnum.Vague, game.GameState);

            var fieldState = JsonConvert.DeserializeObject<char[,]>(game.FieldState);
            for (int i = 0; i < fieldSize; i++)
            {
                for (int j = 0; j < fieldSize; j++)
                {
                    Assert.Equal('-', fieldState[i, j]);
                }
            }
        }

        [Fact]
        public void NextMove_ValidMove_UpdatesFieldState()
        {
            var game = new CrestNullGame(3, 3, 50);

            game.NextMove(1, 1, 'X');

            var fieldState = JsonConvert.DeserializeObject<char[,]>(game.FieldState);
            Assert.Equal('X', fieldState[1, 1]);
            Assert.Equal(1u, game.Round);
        }

        [Fact]
        public void NextMove_OutOfBounds_ThrowsArgumentException()
        {
            var game = new CrestNullGame(3, 3, 50);

            Assert.Throws<ArgumentException>(() => game.NextMove(3, 3, 'X'));
        }

        [Fact]
        public void NextMove_WrongSymbol_ThrowsArgumentException()
        {
            var game = new CrestNullGame(3, 3, 50);

            Assert.Throws<ArgumentException>(() => game.NextMove(3, 3, 'x'));
        }

        [Fact]
        public void NextMove_UpdateSameCell_ThrowsArgumentException()
        {
            var game = new CrestNullGame(3, 3, 50);

            game.NextMove(0, 0, 'X');
            Assert.Throws<ArgumentException>(() => game.NextMove(0, 0, 'X'));
        }

        [Fact]
        public void NextMove_SwitchesSymbol()
        {
            var game = new CrestNullGame(3, 3, 50);

            var mockRandom = new MockRandom(0);

            game.NextMove(0, 0, 'X', mockRandom);
            game.NextMove(0, 1, '0', mockRandom);
            game.NextMove(1, 0, 'X', mockRandom); 

            var fieldState = JsonConvert.DeserializeObject<char[,]>(game.FieldState);
            Assert.Equal('0', fieldState[1, 0]);
        }

        [Fact]
        public void NextMove_CheckWin_Xwin()
        {
            var game = new CrestNullGame(3, 3, 10);
            var mockRandom = new MockRandom(1);

            game.NextMove(0, 0, 'X', mockRandom);
            game.NextMove(1, 1, 'X', mockRandom);
            game.NextMove(2, 2, 'X', mockRandom);

            Assert.Equal(GameStateEnum.Xwin, game.GameState);
        }

        [Fact]
        public void NextMove_CheckWin_Owin()
        {
            var game = new CrestNullGame(3, 3, 10);
            var mockRandom = new MockRandom(1);

            game.NextMove(0, 0, '0', mockRandom);
            game.NextMove(1, 1, '0', mockRandom);
            game.NextMove(2, 2, '0', mockRandom);

            Assert.Equal(GameStateEnum.Owin, game.GameState);
        }


        [Theory]
        [InlineData(90, 90u)]
        [InlineData(70, 60u)]
        public void NextMove_CheckSpare_Spare(uint moveCount, uint percent)
        {
            var game = new CrestNullGame(10, 10, percent);

            var mockRandom = new MockRandom(1);

            int i = 0;
            for (uint n = 0u; n < game.FieldSize; n++)
            {
                for (uint m = 0u; m < game.FieldSize; m++)
                {
                    game.NextMove(n, m, (n + m) % 3 == 0 ? 'X' : '0', mockRandom);
                    if (i == moveCount)
                        break;
                }
                if (i == moveCount)
                    break;
            }

            Assert.Equal(GameStateEnum.Spare, game.GameState);
        }

        [Theory]
        [InlineData(90, 100u)]
        [InlineData(50, 60u)]
        public void NextMove_CheckSpare_NotSpare(uint moveCount, uint percent)
        {
            var game = new CrestNullGame(10, 10, percent);

            int i = 0;
            for (uint n = 0u; n < game.FieldSize; n++)
            {
                for (uint m = 0u; m < game.FieldSize; m++)
                {
                    game.NextMove(n, m, 'X');
                    if (i == moveCount)
                        break;
                }
                if (i == moveCount)
                    break;
            }
            
            Assert.NotEqual(GameStateEnum.Spare, game.GameState);
        }



        // Подмена случайности для проверки инвертирования символа и контроля проверки победы
        public class MockRandom : Random
        {
            private readonly int _valueToReturn;

            public MockRandom(int valueToReturn)
            {
                _valueToReturn = valueToReturn;
            }

            public override int Next(int maxValue)
            {
                return _valueToReturn;
            }
        }
    }
}
