using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Windows.Controls.Image;

namespace TextEditorWPF.Model
{
    public class ElementDrawResult
    {

        public double X { get; set; }
        public double Y { get; set; }
        public double CanavsWidth { get; set; }
        public double CanavsHeight { get; set; }
        public double LineHeight { get; set; }
        public Stack<Properties> Properties { get; set; }
        public ElementDrawResult()
        {
            Properties = new Stack<Properties>();
        }
    }


    public abstract class Element
    {
        public string Data { get; set; }
        public FrameworkElement elementUi { get; set; }
        public Properties Properties { get; set; }
        public List<Element> ChildElements { get; set; }

        public Element()
        {
            Properties = new Properties(){
                new FontWeightProperrty<TextBlock>()
                {
                    Name = "FontWeight",
                    Value = FontWeights.Normal,
                },
                new FontStyleProperty<TextBlock>()
                {
                    Name = "FontStyle",
                    Value = FontStyles.Normal,
                },
                new FontSizeProperty<TextBlock>()
                {
                    Name = "FontSize",
                    Value = 15d,
                },
                new HorizontalOptionsProperty<FrameworkElement>()
                {
                    Name = "HorizontalOptions",
                    Value = HorizontalAlignment.Left,
                },
                new Debug<TextBlock>()
                {
                    Name = "Debug",
                    Value = true,
                }
            };
        }


        public Element(string data) : this() { this.Data = data; }

        public static Element Create(Token token)
        {
            switch (token.RawData)
            {
                case "root":
                    return new RootElement("root");
                case "p":
                    return new ParagraphElement("p");
                case "h1":
                    return new HeaderElement("h1");
                case "b":
                    return new BoldElement("b");
                case "i":
                    return new ItalicElement("i");
                case "image":
                    return new ImageElement("image");
                case "url":
                    return new UrlElement("url");
                case "table":
                    return new TableElement("table");
                case "rows":
                    return new RowsElement("rows");
                case "row":
                    return new RowElement("row");
                case "columns":
                    return new ColumnsElement("columns");
                case "column":
                    return new ColumnElement("column");
                    
                default:
                    return new StringElement(token.RawData);

            }

            return null;
        }

        public virtual void Draw(Canvas canvas, ref ElementDrawResult result)
        {
            result.CanavsWidth = canvas.ActualWidth;
            result.CanavsHeight = canvas.ActualHeight;
            if (ChildElements is not null && ChildElements.Count > 0)
            {
                foreach (var element in ChildElements)
                {
                    element.Draw(canvas, ref result);
                }
            }
        }
    }


    public class RootElement : Element
    {
        public RootElement(string data) : base(data) { }

        public override void Draw(Canvas canvas, ref ElementDrawResult result)
        {
            result.Properties.Push(this.Properties);
            base.Draw(canvas, ref result);
            result.Properties.Pop();
        }
    }

    public class StringElement : Element
    {
        public StringElement(string data) : base(data) { }

        public override void Draw(Canvas canvas, ref ElementDrawResult result)
        {
            TextBlock text = new TextBlock();
            text.TextWrapping = TextWrapping.Wrap;
            text.MaxWidth = result.CanavsWidth;
            this.elementUi = text;
            text.Text = this.Data;
            result.Properties.Peek().Apply(text);
            Canvas.SetLeft(text, 0);
            Canvas.SetTop(text, 0);
            Canvas.SetZIndex(text, 2);
            canvas.Children.Add(text);
            canvas.UpdateLayout();

            if (result.X + text.ActualWidth > canvas.ActualWidth)
            {
                result.Y += text.ActualHeight;
                result.X = 0;
            }
            Canvas.SetLeft(text, result.X);
            Canvas.SetTop(text, result.Y);
            result.X += text.ActualWidth + 10;
            result.LineHeight = text.ActualHeight;
            canvas.UpdateLayout();
        }
    }

    public class ParagraphElement : Element
    {
        public ParagraphElement(string data) : base(data) { }

        public override void Draw(Canvas canvas, ref ElementDrawResult result)
        {
            result.Properties.Push(this.Properties);

            base.Draw(canvas, ref result);

            result.X = 0;
            result.Y += result.LineHeight;

            result.Properties.Pop();
        }
    }

    public class HeaderElement : Element
    {
        public HeaderElement(string data) : base(data)
        {
            Properties.FirstOrDefault(x => x.Name == "FontSize").Value = 20d;
            Properties.FirstOrDefault(x => x.Name == "HorizontalOptions").Value = HorizontalAlignment.Center;
        }

        public override void Draw(Canvas canvas, ref ElementDrawResult result)
        {
            result.Properties.Push(this.Properties);
            base.Draw(canvas, ref result);

            var blockWidth = ChildElements.Sum(x => x.elementUi.ActualWidth + 10);
            var start = result.CanavsWidth / 2 - blockWidth / 2;
            foreach (var element in ChildElements)
            {
                Canvas.SetLeft(element.elementUi, start);
                start += element.elementUi.ActualWidth + 10;
            }
            canvas.UpdateLayout();

            result.X = 0;
            result.Y += result.LineHeight;

            result.Properties.Pop();
        }
    }

    public class BoldElement : Element
    {
        public BoldElement(string data) : base(data)
        {
            Properties.FirstOrDefault(x => x.Name == "FontWeight").Value = FontWeights.Bold;
        }

        public override void Draw(Canvas canvas, ref ElementDrawResult result)
        {
            result.Properties.Push(this.Properties);
            base.Draw(canvas, ref result);
            result.Properties.Pop();
        }
    }

    public class ItalicElement : Element
    {
        public ItalicElement(string data) : base(data)
        {
            Properties.FirstOrDefault(x => x.Name == "FontStyle").Value = FontStyles.Italic;
        }

        public override void Draw(Canvas canvas, ref ElementDrawResult result)
        {
            result.Properties.Push(this.Properties);
            base.Draw(canvas, ref result);
            result.Properties.Pop();
        }
    }

    public class ImageElement : Element
    {
        public ImageElement(string data) : base(data) { }

        public override void Draw(Canvas canvas, ref ElementDrawResult result)
        {
            result.Properties.Push(this.Properties);

            Image image = new Image();
            Canvas.SetLeft(image, result.X);
            Canvas.SetTop(image, result.Y);
            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = new Uri(((UrlElement)ChildElements[0]).GetUrl());
            myBitmapImage.DecodePixelWidth = 200;
            myBitmapImage.EndInit();
            //set image source
            image.Source = myBitmapImage;
            canvas.Children.Add(image);
            canvas.UpdateLayout();

            result.X = 0;
            result.Y += image.ActualHeight;



            result.Properties.Pop();
        }
    }

    public class UrlElement : Element
    {
        public UrlElement(string data) : base(data) { }

        public override void Draw(Canvas canvas, ref ElementDrawResult result)
        {

        }

        public string GetUrl()
        {
            return ChildElements[0].Data;
        }
    }


    public class TableElement : Element
    {
        public TableElement(string data) : base(data)
        {
        }

        public override void Draw(Canvas canvas, ref ElementDrawResult result)
        {
            var grid = new Grid();


            Canvas.SetLeft(grid, result.X);
            Canvas.SetTop(grid, result.Y);

        }
    }


    public class ColumnElement : Element
    {
        public ColumnElement(string data) : base(data)
        {
        }
    }

    public class ColumnsElement : Element
    {
        public ColumnsElement(string data) : base(data)
        {
        }
    }

    public class RowElement : Element
    {
        public RowElement(string data) : base(data)
        {
        }
    }

    public class RowsElement : Element
    {
        public RowsElement(string data) : base(data)
        {
        }
    }

}
