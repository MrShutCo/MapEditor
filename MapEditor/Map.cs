using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System.Drawing.Imaging;

namespace MapEditor {
    public class Map {

        public Bitmap TileSheet;

        public Bitmap[,] SpriteSheet;

        public Tile[,] TileInformation;

        public int Width;
        public int Height;
        public int TileSize = 32;

        public Map(int width, int height) {
            Height = height;
            Width = width;
            TileInformation = new Tile[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    TileInformation[x, y] = new Tile(0);
        }

        //TODO: Add a error handler
        public void LoadTileSheet(BitmapImage bit) {
            TileSheet = BitmapImage2Bitmap(bit);
            SplitIntoSheet();
        }

        void SplitIntoSheet() {
            SpriteSheet = new Bitmap[TileSheet.Width / 32, TileSheet.Height / 32];
            for (int y = 0; y < TileSheet.Height / 32; y++) {
                for (int x = 0; x < TileSheet.Width / 32; x++) {
                    SpriteSheet[x,y] = TileSheet.Clone(new System.Drawing.Rectangle(x * 32, y * 32, 32, 32), PixelFormat.DontCare); 
                }
            }
        }

        public BitmapImage ToBitmapImage(Bitmap bitmap) {
            using (var memory = new MemoryStream()) {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage) {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream()) {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }
    }
}
