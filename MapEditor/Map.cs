using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;

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
                    TileInformation[x, y] = new Tile(-1, x, y);
        }

        //TODO: Add a error handler
        public void LoadTileSheet(BitmapImage bit) {
            TileSheet = BitmapImage2Bitmap(bit);
            SplitIntoSheet();
        }


        //Might not even work
        public int FindImage(Bitmap bit) {
            for(int x = 0; x < SpriteSheet.GetLength(0); x++) {
                for(int y = 0; y < SpriteSheet.GetLength(1); y++) {
                    if(SpriteSheet[x,y] == bit) {
                        return TileInformation[x, y].TileID;
                    }
                }
            }
            
            return 0;
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

        public Bitmap BitmapSourceToBitmap2(BitmapSource srs) {
            int width = srs.PixelWidth;
            int height = srs.PixelHeight;
            int stride = width * ((srs.Format.BitsPerPixel + 7) / 8);
            IntPtr ptr = IntPtr.Zero;
            try {
                ptr = Marshal.AllocHGlobal(height * stride);
                srs.CopyPixels(new Int32Rect(0, 0, width, height), ptr, height * stride, stride);
                using (var btm = new System.Drawing.Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format1bppIndexed, ptr)) {
                    // Clone the bitmap so that we can dispose it and
                    // release the unmanaged memory at ptr
                    return new System.Drawing.Bitmap(btm);
                }
            }
            finally {
                if (ptr != IntPtr.Zero)
                    Marshal.FreeHGlobal(ptr);
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
