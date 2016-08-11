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
        public Image TileImage;

        public int TileID;

        public Tile(int tileID) {
            TileID = tileID;
        }
    }
}
