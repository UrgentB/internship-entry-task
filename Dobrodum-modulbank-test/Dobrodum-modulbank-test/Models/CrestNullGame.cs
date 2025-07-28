using Microsoft.AspNetCore.Mvc.TagHelpers;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dobrodum_modulbank_test.Models
{
    public enum GameStateEnum
    {
        Xwin,
        Owin,
        Spare,
        Vague
    }

    [Table("GAME")]
    public class CrestNullGame
    {
        [Key]
        [Column("ID")]
        public uint Id { get; private set; }

        [Column("FIELD_SIZE")]
        public uint FieldSize { get; private set; }

        [Column("FIELD_STATE")]
        public string FieldState { get; private set; }

        [Column("ROUND")]
        public uint Round { get; private set; }

        [Column("WINNING_LENGHT")]
        public uint WinningLenght { get; private set; }

        [Column("PERCENTAGE_OF_OCCUPIED_CELLS")]
        public uint PercentageOfOccupiedCells { get; private set; }

        [Column("GAME_STATE")]
        public GameStateEnum GameState { get; private set; } = GameStateEnum.Vague;

        public CrestNullGame() { }

        public CrestNullGame(uint fieldSize, uint winningLength, uint percentageOfOccupiedCells)
        {
            if (fieldSize < winningLength)
                throw new ArgumentException("Размер поля не может быть меньше длинны выигрышной линии");

            FieldSize = fieldSize;

            var defaultFieldState = new char[fieldSize, fieldSize];
            for (int n = 0; n < FieldSize; n++)
                for (int m = 0; m < FieldSize; m++)
                    defaultFieldState[n, m] = '-';
            FieldState = JsonConvert.SerializeObject(defaultFieldState);

            WinningLenght = winningLength;
            PercentageOfOccupiedCells = percentageOfOccupiedCells;
        }

        public int NextMove(uint x, uint y, char symbol, Random random = null)
        {
            if (x >= FieldSize || y >= FieldSize)
                throw new ArgumentException($"Индексы ячеек ({(x, y)}) вне границ игрового поля: ({FieldSize})");

            if (symbol != 'X' && symbol != '0')
                throw new FormatException($"Недопустимый символ для игрового поля: {symbol}");

            var newFieldState = JsonConvert.DeserializeObject<char[,]>(FieldState);
            if (newFieldState[x, y] != '-')
                if (newFieldState[x, y] == symbol)
                    return 0;
                else
                    throw new ArgumentException("Запрещено делать ход для уже занятой клетки");

            Round++;
            random ??= new Random();

            if (Round % 3 == 0 && random.Next(10) == 0)
                if (symbol == 'X')
                    symbol = '0';
                else
                    symbol = 'X';
            newFieldState[x, y] = symbol;
            FieldState = JsonConvert.SerializeObject(newFieldState);

            CheckWinSpare(newFieldState, x, y, symbol);
            return 1;
        }

        private void CheckWinSpare(char[,] newFieldState, uint x, uint y, char symbol)
        {
            //Проверка условий победы
            var directions = new (int, int)[] { (0, 1), (1, 0), (1, 1), (1, -1) };
            foreach (var (dx, dy) in directions)
            {
                int count = 1;

                for (int sign = -1; sign <= 1; sign += 2)
                {
                    for (int i = 1; i <= WinningLenght; i++)
                    {
                        long nx = x + sign * i * dx;
                        long ny = y + sign * i * dy;

                        if (nx < 0 || nx >= FieldSize || ny < 0 || ny >= FieldSize)
                            break;
                        if (newFieldState[nx, ny] != symbol)
                            break;

                        count++;
                    }
                }
                if (count >= WinningLenght)
                {
                    GameState = symbol == 'X' ? GameStateEnum.Xwin : GameStateEnum.Owin;
                    return;
                }
            }

            //Проверка условия ничьи
            uint neutralCount = 0;
            for (int n = 0; n < FieldSize; n++)
                for (int m = 0; m < FieldSize; m++)
                    neutralCount += newFieldState[n, m] == '-' ? 1u : 0;
            if (PercentageOfOccupiedCells <= 100 - neutralCount * FieldSize * FieldSize / 100 )
                GameState = GameStateEnum.Spare;
        }

    }
}

