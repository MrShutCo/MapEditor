using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MapEditor {

    public static class IntColor {
        public static Dictionary<string, SolidColorBrush> Colours = new Dictionary<string, SolidColorBrush>() {
            { "0", new SolidColorBrush(Colors.White) },
            { "1", new SolidColorBrush(Colors.Red) },
            { "2", new SolidColorBrush(Colors.Blue) }
        };


    }

    public class Tile {

        public string Color;

        public Tile(string color) {
            Color = color;
        }
    }
}
