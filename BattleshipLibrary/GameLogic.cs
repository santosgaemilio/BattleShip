using BattleshipLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BattleshipLibrary
{
    // La clase es estática por que solamente se va a dedicar a los métodos
    // Es una forma rara de hacer las cosas pero me gusta
    public static class GameLogic
    {
        public static void InitializeGrid(PlayerInfoModel player)
        {
            string spots = "A,B,C,D,E";
            List<string> letters = spots.Split(',').ToList();
            List<int> numbers = new List<int> {1,2,3,4,5};
            
            foreach (var letter in letters)
            {
                foreach (var number in numbers)
                {
                    AddSpot(player, number, letter);
                }
            }
        }

        public static bool PlaceShip(PlayerInfoModel player, string location)
        {
            bool isValid = false;

            (string row, int column) = SplitShot(location);
            bool isInGrid = ValidateGridLoc(player, row.ToUpper(), column);
            (bool isOpen, _) = ValidateOpenLoc(player, row.ToUpper(), column);

            if (isInGrid && isOpen)
            {
                player.ShipLocations.Add(new GridSpotModel
                {
                    SpotLetter = row.ToUpper(),
                    SpotNumber = column,
                    Status = SpotStatus.Ship,
                });

                isValid = true;
            }
            return isValid;
        }

        private static (bool, SpotStatus) ValidateOpenLoc(PlayerInfoModel player, string row, int column)
        {
            bool valid = true;
            SpotStatus status = SpotStatus.Empty;
            foreach (var ship in player.ShipLocations)
            {
                if (ship.SpotLetter == row && ship.SpotNumber == column)
                {
                    valid = false;
                    status = ship.Status;
                    break;
                }
            }

            return (valid, status);
        }

        private static bool ValidateGridLoc(PlayerInfoModel player, string row, int column)
        {
            bool isValid = false;

            foreach (var ship in player.ShotGrid)
            {
                if (ship.SpotLetter == row && ship.SpotNumber == column)
                {
                    isValid = true;
                    break;
                }
            }
            return isValid;
        }

        public static bool PlayerAlive(PlayerInfoModel opponent)
        {
            bool isDead = true;
            foreach (var ship in opponent.ShipLocations)
            {
                if (ship.Status != SpotStatus.Sunk)
                {
                    isDead = false; 
                    break;

                }
            }
              
            return isDead;
        }

        public static int ShotCount(PlayerInfoModel winner)
        {
            int shots = 0;
            foreach (var shot in winner.ShotGrid)
            {
                if (shot.Status == SpotStatus.Hit || shot.Status == SpotStatus.Miss)
                {
                    shots += 1;
                }
            }

            return shots;
        }

        // Esto lo hace para el oponente
        public static bool ShotResultO(PlayerInfoModel opponent, string row, int column)
        {
            bool hit = false;
            (bool open, SpotStatus status) = ValidateOpenLoc(opponent, row, column); 
            if (!open && status != SpotStatus.Sunk)
            {
                hit = true;
                foreach(var ship in opponent.ShipLocations)
                {
                    if (ship.SpotLetter == row && ship.SpotNumber == column)
                    {
                        ship.Status = SpotStatus.Sunk;
                    }
                }
            }

            return hit;
        }

        // Esto lo hace para el activePlayer
        public static void ShotResultA(PlayerInfoModel activePlayer, string row, int column, bool didHit)
        {
            bool hit = didHit;
            foreach (var spot in activePlayer.ShotGrid)
            {
                if(spot.SpotLetter == row && spot.SpotNumber == column)
                {
                    if (hit) 
                    { 
                        spot.Status = SpotStatus.Hit;
                    }
                    else
                    {
                        spot.Status = SpotStatus.Miss;
                    }
                }
            }
        }

        public static (string row, int column) SplitShot(string shot)
        {
            //List<string> splits = shot.Split("").ToList();
            (string row, int column) = (shot[0].ToString().ToUpper(), int.Parse(shot[1].ToString()));
            return (row, column);
        }

        public static bool ValidateShot(PlayerInfoModel activePlayer, string row, int column, string shot)
        {
            bool isValid = false;
            
            foreach (var spot in activePlayer.ShotGrid)
            {
                if (spot.SpotLetter == row.ToUpper() && spot.SpotNumber == column)
                {
                    if (spot.Status == SpotStatus.Empty && spot.Status != SpotStatus.Miss && spot.Status != SpotStatus.Hit)
                    {
                        isValid = true;
                    }
                }
            }
            

            return isValid;
        }

        private static void AddSpot(PlayerInfoModel player, int number, string letter)
        {
            GridSpotModel spot = new GridSpotModel();
            spot.SpotNumber = number;
            spot.SpotLetter = letter;
            player.ShotGrid.Add(spot);
        }

        public static bool CheckInput(string? location)
        {
            List<string> validInputs = new List<string>()
            {
                "a1", "a2", "a3", "a4", "a5",
                "b1", "b2", "b3", "b4", "b5",
                "c1", "c2", "c3", "c4", "c5",
                "d1", "d2", "d3", "d4", "d5",
                "e1", "e2", "e3", "e4", "e5"
            };

            bool valid = validInputs.Contains(location.ToLower());

            return valid;
        }
    }
}
