using System;
using System.ComponentModel.DataAnnotations;

namespace Socket
{
    public class GameRoom
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<string> Users { get; set; }
        public int[] Board { get; set; } = Enumerable.Repeat<int>(0, 9).ToArray();
        private int PlayerTurn { get; set; } = 1;
        public int Winner { get; set; } = 0;

        public string GetPlayerTurn()
        {
            return (PlayerTurn-1) < Users.Count ? Users.ElementAt(PlayerTurn-1) : throw new Exception("Invalid player turn.");
        }
        public string GetWinner()
        {
            return Winner > 0 ? Users.ElementAt(Winner - 1) : null;
        }

        public string GetStatus()
        {
            if (Winner > 0) return "Finished";
            return Users.Count < 2 ? "Waiting" : "Ready";
        }

        public string[] GetBoard()
        {
            string[] tempBoard = Enumerable.Repeat<string>("", 9).ToArray();
            for (int i = 0; i < Board.Length; i++)
            {
                if (Board[i] == 1)
                {
                    tempBoard[i] = "O";
                } else if (Board[i] == 2)
                {
                    tempBoard[i] = "X";
                }
            }
            return tempBoard;
        }
        public void PlayerMove(int pos)
        {
            if (Board[pos] == 0)
            {
                Board[pos] = PlayerTurn;
                SwapPlayerTurn();
                CalculateWin();
            }
            else
            {
                throw new Exception("Invalid move.");
            }
        }

        public void CalculateWin()
        {
            // rows.
            if (Board[0] == Board[1] && Board[1] == Board[2]) Winner = Board[0];
            else if (Board[3] == Board[4] && Board[4] == Board[5]) Winner = Board[3];
            else if (Board[6] == Board[7] && Board[7] == Board[8]) Winner = Board[6];
            // cols.
            else if (Board[0] == Board[3] && Board[3] == Board[6]) Winner = Board[0];
            else if (Board[1] == Board[4] && Board[4] == Board[7]) Winner = Board[1];
            else if (Board[2] == Board[5] && Board[5] == Board[8]) Winner = Board[2];
            // diags.
            else if (Board[0] == Board[4] && Board[4] == Board[8]) Winner = Board[0];
            else if (Board[2] == Board[4] && Board[4] == Board[6]) Winner = Board[2];
        }

        private void SwapPlayerTurn()
        {
            if (PlayerTurn == 1)
            {
                PlayerTurn = 2;
            }
            else if (PlayerTurn == 2)
            {
                PlayerTurn = 1;
            }
        }
    }
}
