using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MapEditor {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public enum MapEditControl {
        Draw,
        Erase,
        Line,
        Sqaure,
        Fill
    }

    public partial class MainWindow : Window {

        public MapEditControl CurrentContol;

        public Map Map;

        public int X = 32;
        public int Y = 32;
        public int TileSize = 32;

        //Just painting stuff for rectangles
        public ImageBrush CurrentImage;

        //This is used to fill in Map.TileInformation, a 2D array
        public int CurrentTile;

        public MainWindow() {
            InitializeComponent();

            Map = new Map(X, Y);
            CheckTileIDs(-1);
            RestartMap();
            CheckTileIDs(-1);
            //CurrentContol = MapEditControl.Draw;
        }






        #region MapControls

        //This actaully starts up the map, creating all the rectangles, and adding to Canvas
        void RestartMap() {
            for (int x = 0; x < X; x++)
                for (int y = 0; y < Y; y++) {
                    Rectangle rect = new Rectangle() { Width = TileSize, Height = TileSize };
                    rect.Fill = new SolidColorBrush(Colors.White);
                    rect.StrokeThickness = 1;
                    rect.Stroke = new SolidColorBrush(Colors.Black);
                    MapViewer.Children.Add(rect);
                    Canvas.SetTop(rect, x * TileSize);
                    Canvas.SetLeft(rect, y * TileSize);
                }
        }

        //Zoom function, used with scroll wheel
        private double zoomMax = 3;
        private double zoomMin = 0.2;
        private double zoomSpeed = 0.001;
        private double zoom = 1;
        private void MapViewer_MouseWheel(object sender, MouseWheelEventArgs e) {

            zoom += zoomSpeed * e.Delta; // Ajust zooming speed (e.Delta = Mouse spin value )
            if (zoom < zoomMin) { zoom = zoomMin; } // Limit Min Scale
            if (zoom > zoomMax) { zoom = zoomMax; } // Limit Max Scale

            Point mousePos = e.GetPosition(MapViewer);
            st.ScaleX = zoom;
            st.ScaleY = zoom;

            if (zoom != zoomMin && zoom != zoomMax) {
                st.CenterX = mousePos.X;
                st.CenterY = mousePos.Y;
            }

        }

        #endregion

        #region Buttons&Stuff

        private void richTextBox_TextChanged(object sender, TextChangedEventArgs e) {

        }

        private void ChangeMap_Click(object sender, RoutedEventArgs e) {

        }

        private void ClearButton_Click(object sender, RoutedEventArgs e) {
            RestartMap();
        }

        private void DrawButton_Click(object sender, RoutedEventArgs e) {
            UnlockControls();
            CurrentContol = MapEditControl.Draw;
            DrawButton.IsEnabled = false;
        }

        private void FillButton_Click(object sender, RoutedEventArgs e) {
            UnlockControls();
            CurrentContol = MapEditControl.Fill;
            FillButton.IsEnabled = false;
        }

        private void EraseButton_Click(object sender, RoutedEventArgs e) {
            UnlockControls();
            CurrentContol = MapEditControl.Erase;
            EraseButton.IsEnabled = false;
        }

        private void LineButton_Click(object sender, RoutedEventArgs e) {
            UnlockControls();
            CurrentContol = MapEditControl.Line;
            LineButton.IsEnabled = false;
        }

        private void RoomButton_Click(object sender, RoutedEventArgs e) {
            UnlockControls();
            CurrentContol = MapEditControl.Sqaure;
            RoomButton.IsEnabled = false;
        }

        //Saves Map.TileInformation to a JSON object
        private void Savebutton_Click(object sender, RoutedEventArgs e) {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.DefaultExt = ".json";
            saveFile.Filter = "JSON Files (.json)|*.json";

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = saveFile.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true) {
                // Open document
                string filename = saveFile.FileName;
                JsonSerializer serial = new JsonSerializer();
                int[,] mapData = new int[X, Y];
                for (int x = 0; x < X; x++) {
                    for (int y = 0; y < Y; y++) {
                        mapData[x, y] = Map.TileInformation[x, y].TileID;
                    }
                }
                var map = JsonConvert.SerializeObject(Map.TileInformation);
                using (StreamWriter sw = new StreamWriter(filename))
                using (JsonWriter writer = new JsonTextWriter(sw)) {
                    serial.Serialize(writer, mapData);
                }
            }
        }

        //Load a tilesheet in (should be 32x32), example included on top folder
        private void LoadImageButton_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog image = new OpenFileDialog();
            image.DefaultExt = ".png";
            image.Filter = "PNG Files (.png)|*.png";

            Nullable<bool> result = image.ShowDialog();

            if (result == true) {
                string fileName = image.FileName;
                Map.LoadTileSheet(new BitmapImage(new Uri(fileName)));
            }

            for (int y = 0; y < Map.TileSheet.Height / 32; y++) {
                for (int x = 0; x < Map.TileSheet.Width / 32; x++) {
                    Rectangle rect = new Rectangle() { Width = TileSize, Height = TileSize };
                    rect.Fill = new ImageBrush(Map.ToBitmapImage(Map.SpriteSheet[x, y]));
                    rect.StrokeThickness = 0;
                    rect.Stroke = new SolidColorBrush(Colors.Black);
                    //MapInformation[x, y].Color = 
                    Sprite.Children.Add(rect);
                    Canvas.SetTop(rect, y * TileSize);
                    Canvas.SetLeft(rect, x * TileSize);
                }
            }
            Sprite.Width = Map.TileSheet.Width;
            Sprite.Height = Map.TileSheet.Height;


        }

        #endregion

        #region Map Interaction

        //TODO: Probably delete, It's not too useful now, no functionality
        private void ChangeMap_Click_1(object sender, RoutedEventArgs e) {
            string textBox = new TextRange(MapData.Document.ContentStart, MapData.Document.ContentEnd).Text;
            int placeHolder = 0;
            for (int x = 0; x < X; x++)
                for (int y = 0; y < Y; y++) {
                    if (Char.IsDigit(textBox[placeHolder])) {
                        //Map.TileInformation[x, y].C = textBox[placeHolder].ToString();
                        placeHolder++;
                    }
                }
        }


        //When you just click on a tile, it runs this. Other functionality commented due to possible errors (none so far)
        private void MapViewer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (CurrentContol == MapEditControl.Draw) {
                Rectangle ClickedRectangle = (Rectangle)e.OriginalSource;
                ClickedRectangle.Fill = CurrentImage;
                int x = (int)Canvas.GetLeft(ClickedRectangle) / 32;
                int y = (int)Canvas.GetTop(ClickedRectangle) / 32;
                Map.TileInformation[x, y].TileID = CurrentTile;
                //UpdateMap();
            }
            if (CurrentContol == MapEditControl.Erase) {
                Rectangle ClickedRectangle = (Rectangle)e.OriginalSource;
                ClickedRectangle.Fill = new SolidColorBrush(Colors.White);
            }
            if (CurrentContol == MapEditControl.Fill) {
                CheckTileIDs(-1);
                //All the good stuff for converting Canvas rect location to Map.TileInformation
                Rectangle ClickedRectangle = (Rectangle)e.OriginalSource;
                ClickedRectangle.Fill = CurrentImage;
                int x = (int)Canvas.GetLeft(ClickedRectangle) / 32;
                int y = (int)Canvas.GetTop(ClickedRectangle) / 32;
                int oldTile = Map.TileInformation[x, y].TileID;
                //Map.TileInformation[x, y].TileID = CurrentTile;

                //SetTileIDs(-1);
                CheckTileIDs(-1);
                //The big problem
                FloodFill2(Map.TileInformation[x, y], oldTile, CurrentTile);
                CheckTileIDs(0);
            }


            if (CurrentContol == MapEditControl.Line) {

                Rectangle ClickedRectangle = (Rectangle)e.OriginalSource;
                ClickedRectangle.Fill = CurrentImage;


                if (!lineStarted) {
                    x1 = (int)Canvas.GetLeft(ClickedRectangle) / 32;
                    y1 = (int)Canvas.GetTop(ClickedRectangle) / 32;
                    lineStarted = true;
                }
                else {
                    lineStarted = false;
                    x2 = (int)Canvas.GetLeft(ClickedRectangle) / 32;
                    y2 = (int)Canvas.GetTop(ClickedRectangle) / 32;
                    LinePlace(x1,y1,x2,y2);
                    x1 = x2 = y1 = y2 = 0;
                }
            }

            if (CurrentContol == MapEditControl.Sqaure) {

                Rectangle ClickedRectangle = (Rectangle)e.OriginalSource;
                ClickedRectangle.Fill = CurrentImage;

                if (!roomStarted) {
                    x1 = (int)Canvas.GetLeft(ClickedRectangle) / 32;
                    y1 = (int)Canvas.GetTop(ClickedRectangle) / 32;
                    roomStarted = true;
                }
                else {
                    roomStarted = false;
                    x2 = (int)Canvas.GetLeft(ClickedRectangle) / 32;
                    y2 = (int)Canvas.GetTop(ClickedRectangle) / 32;
                    RoomPlace(x1,y1,x2,y2);
                    x1 = x2 = y1 = y2 = 0;
                }
            }

        }

        bool lineStarted = false;
        bool roomStarted = false;
        int x1, x2, y1, y2;
        //This is for when you click and drag things, only applies to Draw, Erase, and panning around the Canvas
        private void MapViewer_MouseMove(object sender, MouseEventArgs e) {


            if (CurrentContol == MapEditControl.Draw) {
                if (e.LeftButton == MouseButtonState.Pressed) {
                    Rectangle ClickedRectangle = (Rectangle)e.OriginalSource;
                    ClickedRectangle.Fill = CurrentImage;
                    int x = (int)Canvas.GetLeft(ClickedRectangle) / 32;
                    int y = (int)Canvas.GetTop(ClickedRectangle) / 32;
                    Map.TileInformation[x, y].TileID = CurrentTile;
                    //UpdateMap();
                }
            }
            //TODO: This doesn't clear the tileID
            if (CurrentContol == MapEditControl.Erase) {
                if (e.LeftButton == MouseButtonState.Pressed) {
                    Rectangle ClickedRectangle = (Rectangle)e.OriginalSource;
                    ClickedRectangle.Fill = new SolidColorBrush(Colors.White);

                }
            }
            if (isDragged == false) return;

            if (e.RightButton == MouseButtonState.Pressed) {
                var pos = e.GetPosition(this);

                var matrix = mt.Matrix;
                matrix.Translate((pos.X - lastPos.X) * 2, (pos.Y - lastPos.Y) * 2);
                mt.Matrix = matrix;
                lastPos = pos;
            }

        }


        //Stop panning
        private void MapViewer_MouseRightButtonUp(object sender, MouseButtonEventArgs e) {
            MapViewer.ReleaseMouseCapture();
            isDragged = false;
        }

        //Start panning
        private void MapViewer_MouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            MapViewer.CaptureMouse();
            lastPos = e.GetPosition(this);
            isDragged = true;
        }

        //Gets the rectangle from the bottom right spritesheet, sets CurrentImage and CurrentTile
        private void Sprite_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Rectangle curr = (Rectangle)e.OriginalSource;
            CurrentImage = (ImageBrush)curr.Fill;
            int x = (int)Canvas.GetLeft(curr) / 32;
            int y = (int)Canvas.GetTop(curr) / 32;
            CurrentTile = y * Map.SpriteSheet.GetLength(0) + x;
            CheckTileIDs(-1);
        }

        //Oops
        private void SpriteSheet_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {


        }

        #endregion


        #region Helper Functions

        //Tester, for making sure the tiles are the same (when run before FloodFill, it works)
        private void SetTileIDs(int tileID) {
            foreach (Tile t in Map.TileInformation) {
                t.TileID = tileID;
            }
        }

        //Tester, to determine where the pesky tileID is set (should return 256 on first fill)
        private int CheckTileIDs(int tileID) {
            int UniqueTiles = 0;

            foreach (Tile t in Map.TileInformation) {
                if (t.TileID == tileID) UniqueTiles++;
                else {
                    //throw new Exception();
                }
            }
            //TODO: Maybe add some print statement here

            //Commmon breakpoint, useful for debugging
            return UniqueTiles;
        }

        //The code for lookup on the Canvas, based on Map.TileInformation[x,y]
        void SetImageAtCoord(int x, int y) {
            IEnumerable<Rectangle> rectangles = MapViewer.Children.OfType<Rectangle>();
            Map.TileInformation[x, y].TileID = CurrentTile;
            bool hit = false;
            foreach (Rectangle rect in rectangles) {
                int X = (int)Canvas.GetLeft(rect) / 32;
                int Y = (int)Canvas.GetTop(rect) / 32;

                if (X == x && Y == y) {
                    hit = true;
                    rect.Fill = CurrentImage;
                }


            }
            if (!hit) throw new Exception();
        }

        int TilesHit = 0;
    
        //TODO: This only works in one direction
        private void RoomPlace(int x, int y, int x2, int y2) {

            int dx, dy;
            dx = dy = 0;
            int w = x2 - x;
            int h = y2 - y;

            if (w < 0) dx = -1; else if (w > 0) dx = 1;
            if (h < 0) dy = -1; else if (h > 0) dy = 1;
            //if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;

            if (fillRoom.IsChecked.Value) {
                for (int i = x; i <= x2; i += dx) {
                    for (int j = y; j <= y2; j+= dy) {
                        Map.TileInformation[i, j].TileID = CurrentTile;
                        SetImageAtCoord(i, j);
                    }
                }
            }
            else {
                for (int i = x; i <= x2; i++) {
                    //No clue why this is needed, but w/e
                    Map.TileInformation[i, y].TileID = CurrentTile;
                    SetImageAtCoord(i, y);
                    Map.TileInformation[i, y2].TileID = CurrentTile;
                    SetImageAtCoord(i, y2);
                }
                for (int j = y; j <= y2; j++) {
                    Map.TileInformation[x, j].TileID = CurrentTile;
                    SetImageAtCoord(x, j);
                    Map.TileInformation[x2, j].TileID = CurrentTile;
                    SetImageAtCoord(x2, j);
                }
            }
        }

        //Hoop doop da woosh over the head
        //Thank the interwebs
        private void LinePlace(int x, int y, int x2, int y2) {
            int w = x2 - x;
            int h = y2 - y;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            if (!(longest > shortest)) {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            int numerator = longest >> 1;
            for (int i = 0; i <= longest; i++) {
                Map.TileInformation[x, y].TileID = CurrentTile;
                SetImageAtCoord(x, y);
                numerator += shortest;
                if (!(numerator < longest)) {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                }
                else {
                    x += dx2;
                    y += dy2;
                }
            }
        }

        //Note: Not used, this was the stack version, and same problem occurs
        private void FloodFill2(Tile tile, int targetID, int replaceID) {
            if (targetID == replaceID) return;
            Queue<Tile> Q = new Queue<Tile>();
            Map.ProcessNull();
            Q.Enqueue(tile);

            while (Q.Count != 0) {
                var n = Q.First();
                Q.Dequeue();
                if (n.TileID == targetID) {
                    n.TileID = replaceID;
                    n.proccessed = true;
                    if (n.X == 2 && n.Y == 7) {
                        int i = 0;
                    }
                    SetImageAtCoord(n.X, n.Y);
                    if (!Map.TileInformation[n.X - 1, n.Y].proccessed) Q.Enqueue(Map.TileInformation[n.X - 1, n.Y]);
                    if (!Map.TileInformation[n.X + 1, n.Y].proccessed) Q.Enqueue(Map.TileInformation[n.X + 1, n.Y]);
                    if (!Map.TileInformation[n.X, n.Y - 1].proccessed) Q.Enqueue(Map.TileInformation[n.X, n.Y - 1]);
                    if (!Map.TileInformation[n.X, n.Y + 1].proccessed) Q.Enqueue(Map.TileInformation[n.X, n.Y + 1]);

                }
            }
        }

        //Actual flood fill
        private void FloodFill(Tile node, int targetID, int replaceID) {

            if (targetID == replaceID) return;
            if (node.TileID != targetID) return;

            node.TileID = replaceID;
            SetImageAtCoord(node.X, node.Y);
            TilesHit++;

            if (node.Y + 1 != Map.Height) FloodFill(Map.TileInformation[node.X, node.Y + 1], targetID, replaceID);
            if (node.Y - 1 != -1) FloodFill(Map.TileInformation[node.X, node.Y - 1], targetID, replaceID);
            if (node.X - 1 != -1) FloodFill(Map.TileInformation[node.X - 1, node.Y], targetID, replaceID);
            if (node.X + 1 != Map.Width) FloodFill(Map.TileInformation[node.X + 1, node.Y], targetID, replaceID);
        }

        #endregion




        

        Point lastPos;
        bool isDragged = false;







        private void UnlockControls() {
            DrawButton.IsEnabled = true;
            EraseButton.IsEnabled = true;
            FillButton.IsEnabled = true;
            LineButton.IsEnabled = true;
            RoomButton.IsEnabled = true;
        }


    }
}
