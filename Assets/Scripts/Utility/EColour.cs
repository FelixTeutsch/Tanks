using UnityEngine;

namespace Utility
{
    public enum EColour
    {
        Blue,
        Cyan,
        Green,
        Magenta,
        Purple,
        Red,
        Orange,
        White,
        Black
    }

    public static class EColourExtensions
    {
        public static Color GetColour(this EColour colour)
        {
            string hex;
            switch (colour)
            {
                case EColour.Red:
                    hex = "#FF0000";
                    break;
                case EColour.Green:
                    hex = "#00FF00";
                    break;
                case EColour.Blue:
                    hex = "#0000FF";
                    break;
                case EColour.Cyan:
                    hex = "#00FFFF";
                    break;
                case EColour.Magenta:
                    hex = "#FF00FF";
                    break;
                case EColour.Purple:
                    hex = "#800080";
                    break;
                case EColour.Orange:
                    hex = "#FFA500";
                    break;
                case EColour.Black:
                    hex = "#000000";
                    break;
                default:
                    hex = "#FFFFFF";
                    break;
            }

            ColorUtility.TryParseHtmlString(hex, out var color);
            return color;
        }
    }
}