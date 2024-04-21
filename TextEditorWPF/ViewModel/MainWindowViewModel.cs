using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TextEditorWPF.Model;
using static System.Net.Mime.MediaTypeNames;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace TextEditorWPF.ViewModel
{
    public partial class MainWindowViewModel : ObservableObject
    {
        #region Propertiues

        public string Text;
        public Canvas Canvas;

        #endregion

        public MainWindowViewModel()
        {

        }

        public async Task TextChanged(string text)
        {
            Text = text;
            Debug.WriteLine(Text);
            var document = ParserCore.ProccesText(Text);
            CnavasDrawable.Draw(Canvas, document);
        }
    }


    public static class CnavasDrawable
    {
    
        public static void Draw(Canvas canvas, Document document)
        {
            canvas.Children.Clear();

            Rectangle rect = new Rectangle();
            rect.Fill = Brushes.White;
            Canvas.SetLeft(rect, 0);
            Canvas.SetTop(rect, 0);
            Canvas.SetZIndex(rect, 0);

            rect.Width = canvas.ActualWidth;
            rect.Height = canvas.ActualHeight;

            canvas.Children.Add(rect);

            ElementDrawResult result = new ElementDrawResult();
            document.Elements[0].Draw(canvas, ref result);


            //TextBlock textBox = new TextBlock();
            //textBox.Text = "absdefg";
            //textBox.FontSize = 40;
            //Canvas.SetLeft(textBox, 0);
            //Canvas.SetTop(textBox, 0);
            //Canvas.SetZIndex(textBox, 2);
            //canvas.Children.Add(textBox);
            //canvas.UpdateLayout();

            //Rectangle rect2 = new Rectangle();
            //rect2.Fill = Brushes.LightPink;
            //Canvas.SetLeft(rect2, 0);
            //Canvas.SetTop(rect2, 0);
            //Canvas.SetZIndex(rect2, 1);
            //rect2.Width = textBox.ActualWidth;
            //rect2.Height = textBox.ActualHeight;


            //canvas.Children.Add(rect2);
        }
    }

}
