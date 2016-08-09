using System;
using System.Collections.Generic;
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

        public Tile[,] MapInformation;
        public MapEditControl CurrentContol;
        SolidColorBrush CurrentColor;

        public MainWindow() {
            InitializeComponent();

            MapInformation = new Tile[16, 16];
            for (int x = 0; x < 16; x++)
                for (int y = 0; y < 16; y++)
                    MapInformation[x, y] = new Tile("0");
            UpdateMap();
            CurrentContol = MapEditControl.Draw;
            CurrentColor = new SolidColorBrush(Colors.Red);
        }


        private void richTextBox_TextChanged(object sender, TextChangedEventArgs e) {

        }

        public void UpdateMap() {
            for (int x = 0; x < 16; x++)
                for (int y = 0; y < 16; y++) {
                    Rectangle rect = new Rectangle() { Width = 16, Height = 16 };
                    rect.Fill = IntColor.Colours[MapInformation[x, y].Color];
                    rect.StrokeThickness = 1;
                    rect.Stroke = new SolidColorBrush(Colors.Black);
                    MapViewer.Children.Add(rect);
                    Canvas.SetTop(rect, x * 16);
                    Canvas.SetLeft(rect, y * 16);
                }
        }

        private void ChangeMap_Click(object sender, RoutedEventArgs e) {

        }

        private void ChangeMap_Click_1(object sender, RoutedEventArgs e) {
            string textBox = new TextRange(MapData.Document.ContentStart, MapData.Document.ContentEnd).Text;
            int placeHolder = 0;
            for (int x = 0; x < 16; x++)
                for (int y = 0; y < 16; y++) {
                    if (Char.IsDigit(textBox[placeHolder])) {
                        MapInformation[x, y].Color = textBox[placeHolder].ToString();
                        placeHolder++;
                    }
                }
            UpdateMap();
        }

        private void MapViewer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (CurrentContol == MapEditControl.Draw) {
                Rectangle ClickedRectangle = (Rectangle)e.OriginalSource;
                ClickedRectangle.Fill = CurrentColor;
            }
            if (CurrentContol == MapEditControl.Erase) {
                Rectangle ClickedRectangle = (Rectangle)e.OriginalSource;
                ClickedRectangle.Fill = new SolidColorBrush(Colors.White);
            }
        }

        private void MapViewer_MouseMove(object sender, MouseEventArgs e) {
            if (CurrentContol == MapEditControl.Draw) {
                if (e.LeftButton == MouseButtonState.Pressed) {
                    Rectangle ClickedRectangle = (Rectangle)e.OriginalSource;
                    ClickedRectangle.Fill = CurrentColor;
                }
            }
            if (CurrentContol == MapEditControl.Erase) {
                if (e.LeftButton == MouseButtonState.Pressed) {
                    Rectangle ClickedRectangle = (Rectangle)e.OriginalSource;
                    ClickedRectangle.Fill = new SolidColorBrush(Colors.White);
                }
            }
        }

        private void DrawButton_Click(object sender, RoutedEventArgs e) {
            UnlockControls();
            CurrentContol = MapEditControl.Draw;
            DrawButton.IsEnabled = false;
        }

        private void EraseButton_Click(object sender, RoutedEventArgs e) {
            UnlockControls();
            CurrentContol = MapEditControl.Erase;
            EraseButton.IsEnabled = false;
        }

        private void UnlockControls() {
            DrawButton.IsEnabled = true;
            EraseButton.IsEnabled = true;
        }
    }
}
