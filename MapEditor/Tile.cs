using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MapEditor {

    public class Tile {

        //Unused
        public Bitmap TileImage;

        public int TileID;

        public int X, Y;

        public Tile(int tileID, int x, int y) {
            TileID = tileID;
            X = x;
            Y = y;
        }
    }
}
