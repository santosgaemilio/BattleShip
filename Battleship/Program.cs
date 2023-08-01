using BattleshipLibrary;
using BattleshipLibrary.Models;

internal class Program
{
    private static void Main(string[] args)
    {
        HelloMessage();
        Console.ReadLine();
        PlayerInfoModel activePlayer = CreatePlayer("Player 1");
        PlayerInfoModel opponent = CreatePlayer("Player 2");
        PlayerInfoModel winner = null;
        // Loop logic
        do
        {
            //Display grid
            Console.Clear();
            DisplayShotGrid(activePlayer);
            Console.WriteLine();
            Console.WriteLine();
            // Ask actvePlayer for a shot
            RecordPlayerShot(activePlayer, opponent);
            // Determine if it was a valid shot
            // Determine if the game is over
            bool gameContinue = !GameLogic.PlayerAlive(opponent);
            // If it's over, set activePlayer as winner
            // else, swap positions (activePlayer to opponent)
            if (gameContinue)
            {
                (activePlayer, opponent) = (opponent, activePlayer);
            }
            else
            {
                winner = activePlayer;
            }

        } while (winner == null);

        FinishGame(winner);
    }

    private static void FinishGame(PlayerInfoModel winner)
    {
        Console.WriteLine($"Congratulations to {winner.Name} for winning");
        Console.WriteLine($"{winner.Name} took {GameLogic.ShotCount(winner)} shots to win");
    }

    private static void RecordPlayerShot(PlayerInfoModel activePlayer, PlayerInfoModel opponent)
    {
        // Ask for a shot (we ask for "B2)
        // Determine what row and column that is - split apart
        // Determine if that is a valid shot
        // Go back if its not

        // Determine shot results
        // Record results
        bool flag = false;
        string row = "";
        int column = 0;
        do
        {
            (string shot, bool inputValid) = AskShot(activePlayer);
            if (inputValid)
            {
                (row, column) = GameLogic.SplitShot(shot);
                flag = GameLogic.ValidateShot(activePlayer, row, column, shot);
            }
            if (!flag)
            {
                Console.WriteLine("Not a valid shot, do it again");
                Console.ReadLine();
                Console.Clear();
            }
        } while (!flag);

        bool didHit = GameLogic.ShotResultO(opponent, row, column);
        if (didHit)
        {
            Console.WriteLine("Hit!!");
            Console.ReadLine();
        }
        else
        {
            Console.WriteLine("Miss!!");
            Console.ReadLine();
        }
        GameLogic.ShotResultA(activePlayer, row, column, didHit);
    }

    private static (string,bool) AskShot(PlayerInfoModel activePlayer)
    {
        Console.WriteLine($"{activePlayer.Name}! please enter a shot spot: ");
        string shot = Console.ReadLine();
        bool valid = GameLogic.CheckInput(shot);

        return (shot, valid);
    }

    private static void DisplayShotGrid(PlayerInfoModel activePlayer)
    {
        string currentRow = activePlayer.ShotGrid[0].SpotLetter;
        foreach (var gridSpot in activePlayer.ShotGrid)
        {
            if (gridSpot.SpotLetter != currentRow)
            {
                Console.WriteLine();
                currentRow = gridSpot.SpotLetter;
            }
            if (gridSpot.Status == SpotStatus.Empty)
            {
                Console.Write($"{gridSpot.SpotLetter}{gridSpot.SpotNumber} ");
            }
            else if (gridSpot.Status == SpotStatus.Hit)
            {
                Console.Write(" X ");
            }
            else if (gridSpot.Status == SpotStatus.Miss)
            {
                Console.Write(" O ");
            }
            else if (gridSpot.Status == SpotStatus.Ship)
            {
                Console.Write(" S ");
            }
            else if (gridSpot.Status == SpotStatus.Sunk)
            {
                Console.Write(" D ");
            }

        }
    }

    private static void HelloMessage()
    {
        Console.WriteLine("---------------------------");
        Console.WriteLine("WELCOME TO BATTLESHIP");
        Console.WriteLine("Created by Emilio Santos");
        Console.WriteLine("---------------------------");
        Console.WriteLine();
        Console.WriteLine("PRESS ENTER TO START PLAYER 1");
    }

    private static PlayerInfoModel CreatePlayer(string playerNum)
    {
        Console.WriteLine($"BEGIN {playerNum}");
        PlayerInfoModel player = new PlayerInfoModel();
        // Ask for name
        player.Name = AskPlayerName();
            // Make grid
        GameLogic.InitializeGrid(player);        
            // Ask for ship placements
        PlaceShips(player);
        // DEBUG QUITAR ANTES DE TERMINAR!!!
        Console.WriteLine("These are your ships: ");
        foreach (var ship in player.ShipLocations)
        {
            Console.Write($"{ship.SpotLetter}{ship.SpotNumber} ");
        }
        Console.ReadLine();
        /////////////////////////////
            // Clear
        Console.Clear();
        return player;
    }
    private static string AskPlayerName()
    {
        Console.Clear();
        Console.WriteLine("What's your name dude?: ");
        return Console.ReadLine();
    }

    private static void ShowGrid()
    {
        Console.WriteLine("Here is the grid!");
        Console.WriteLine();
        string grid = "  A1   |   A2   |   A3   |   A4   |   A5   \n-----------------------------------------\n  B1   |   B2   |   B3   |   B4   |   B5   \n-----------------------------------------\n  C1   |   C2   |   C3   |   C4   |   C5   \n-----------------------------------------\n  D1   |   D2   |   D3   |   D4   |   D5   \n-----------------------------------------\n  E1   |   E2   |   E3   |   E4   |   E5   ";

        Console.WriteLine(grid);

    }
    private static void PlaceShips(PlayerInfoModel player)
    {
        do
        {
            Console.Clear();
            Console.WriteLine($"Where do you want to place your ship {player.ShipLocations.Count + 1}?: ");
            ShowGrid();
            Console.Write("------------> ");
            string location = Console.ReadLine();
            bool isValidInput = GameLogic.CheckInput(location);
            bool isValidLoc = false;
            if (isValidInput)
            {
                isValidLoc = GameLogic.PlaceShip(player, location);
            }            
            if (!isValidLoc)
            {
                Console.WriteLine("Not a valid location. Press enter to try again");
                Console.ReadLine();
            }
        } while (player.ShipLocations.Count < 5);
    }
}