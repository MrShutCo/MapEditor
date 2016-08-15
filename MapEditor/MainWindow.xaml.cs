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

        public int X = 16;
        public int Y = 16;
        public int TileSize = 32;

        public ImageBrush CurrentImage;

        public int CurrentTile;

        public MainWindow() {
            InitializeComponent();

            Map = new Map(X, Y);
            RestartMap();
            CurrentContol = MapEditControl.Draw;
        }


        private void richTextBox_TextChanged(object sender, TextChangedEventArgs e) {

        }

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

        public void UpdateMap() {
            for (int x = 0; x < X; x++)
                for (int y = 0; y < Y; y++) {
                    //Map.TileInformation[x, y].
                }
        }

        private void ChangeMap_Click(object sender, RoutedEventArgs e) {

        }

        // Zoom on Mouse wheel

        private double zoomMax = 5;
        private double zoomMin = 0.5;
        private double zoomSpeed = 0.001;
        private double zoom = 1;
        private void MapViewer_MouseWheel(object sender, MouseWheelEventArgs e) {
            zoom += zoomSpeed * e.Delta; // Ajust zooming speed (e.Delta = Mouse spin value )
            if (zoom < zoomMin) { zoom = zoomMin; } // Limit Min Scale
            if (zoom > zoomMax) { zoom = zoomMax; } // Limit Max Scale

            Point mousePos = e.GetPosition(MapViewer);

            if (zoom > 1) {
                MapViewer.RenderTransform = new ScaleTransform(zoom, zoom, mousePos.X, mousePos.Y); // transform Canvas size from mouse position

            }
            else {
                MapViewer.RenderTransform = new ScaleTransform(zoom, zoom); // transform Canvas size
            }
        }

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
            UpdateMap();
        }

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
                Rectangle ClickedRectangle = (Rectangle)e.OriginalSource;
                ClickedRectangle.Fill = CurrentImage;
                int x = (int)Canvas.GetLeft(ClickedRectangle) / 32;
                int y = (int)Canvas.GetTop(ClickedRectangle) / 32;
                int oldTile = Map.TileInformation[x, y].TileID;
                //Map.TileInformation[x, y].TileID = CurrentTile;
                FloodFill(Map.TileInformation[x, y], oldTile, CurrentTile);
            }
        }

        void SetImageAtCoord(int x, int y) {
            IEnumerable<Rectangle> rectangles = MapViewer.Children.OfType<Rectangle>();
            Map.TileInformation[x, y].TileID = CurrentTile;
            foreach(Rectangle rect in rectangles) {
                if (Canvas.GetLeft(rect) / TileSize == x && Canvas.GetTop(rect) / TileSize == y) {
                    rect.Fill = CurrentImage;
                }
            }
        }

        private void FloodFill(Tile node, int targetID, int replaceID) {

            if (targetID == replaceID) return;
            if (node.TileID != targetID) return;

            node.TileID = replaceID;
            SetImageAtCoord(node.X, node.Y);

            if (node.Y + 1 != Map.Height) FloodFill(Map.TileInformation[node.X, node.Y + 1], targetID, replaceID);
            if (node.Y - 1 != -1) FloodFill(Map.TileInformation[node.X, node.Y - 1], targetID, replaceID);
            if (node.X - 1 != -1) FloodFill(Map.TileInformation[node.X - 1, node.Y], targetID, replaceID);
            if (node.X + 1 != Map.Width) FloodFill(Map.TileInformation[node.X + 1, node.Y], targetID, replaceID);

        }

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
            if (CurrentContol == MapEditControl.Erase) {
                if (e.LeftButton == MouseButtonState.Pressed) {
                    Rectangle ClickedRectangle = (Rectangle)e.OriginalSource;
                    ClickedRectangle.Fill = new SolidColorBrush(Colors.White);
                }
            }
        }

        private void SpriteSheet_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {


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

        private void UnlockControls() {
            DrawButton.IsEnabled = true;
            EraseButton.IsEnabled = true;
            FillButton.IsEnabled = true;
        }

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
                    // {"ExpiryDate":new Date(1230375600000),"Price":0}
                }
            }
        }

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

        private void Sprite_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Rectangle curr = (Rectangle)e.OriginalSource;
            CurrentImage = (ImageBrush)curr.Fill;
            int x = (int)Canvas.GetLeft(curr) / 32;
            int y = (int)Canvas.GetTop(curr) / 32;
            CurrentTile = y * Map.SpriteSheet.GetLength(0) + x;
        }

        


    }
}
