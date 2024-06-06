using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
        public double LastPageStartY { get; set; }
        public double LineHeight { get; set; }
        public Canvas RootCanvas { get; set; }
        public int PageCount { get; set; }
        public Stack<Properties> Properties { get; set; }

        public Properties PageProperties { get; set; }

        public double LeftPadding;
        public double RightPadding;
        public double TopPadding;
        public double BottomPadding;

        public FrameworkElement elementUi { get; set; }
        public ElementDrawResult()
        {
            Properties = new Stack<Properties>();
        }
    }


    public abstract class Element
    {
        public string Data { get; set; }
        public Properties Properties { get; set; }
        public List<Element> ChildElements { get; set; }

        public bool Equals(Element other)
        {
            if (other == null) return false;

            bool childElementsEqual;
            if (ChildElements == null && other.ChildElements == null)
            {
                childElementsEqual = true;
            }
            else if (ChildElements == null || other.ChildElements == null)
            {
                childElementsEqual = false;
            }
            else
            {
                childElementsEqual = ChildElements.SequenceEqual(other.ChildElements);
            }

            return Data == other.Data && childElementsEqual;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Element);
        }

        public override int GetHashCode()
        {
            return (Data, Properties, ChildElements).GetHashCode();
        }

        public Element()
        {
            Properties = new Properties(){
                new FontWeightProperrty()
                {
                    Value = FontWeights.Normal,
                },
                new FontStyleProperty()
                {
                    Value = FontStyles.Normal,
                },
                new FontSizeProperty()
                {
                    Value = 15d,
                },
                new TextAlignmentProperty()
                {
                    Value = TextAlignment.Left,
                },
                new HorizontalOptionsProperty()
                {
                    Value = HorizontalAlignment.Left,
                },
                new VerticalOptionsProperty()
                {
                    Value = VerticalAlignment.Top,
                },
                new RowProperty()
                {
                    Value = 0,
                },
                new RowSpanProperty()
                {
                    Value = 1,
                },
                new ColumnProperty
                {
                    Value = 0,
                },
                new ColumnSpanProperty
                {
                    Value = 1,
                },
                new PathProperty()
                {
                },
                new DebugProperty()
                {
                    Value = false,
                }
            };
        }


        public Element(string data) : this() { this.Data = data; }

        public static Element Create(Token token)
        {
            Element element = null;

            switch (token.RawData)
            {
                case "root":
                    element = new RootElement("root");
                    break;
                case "p":
                    element = new ParagraphElement("p");
                    break;
                case "h1":
                    element = new HeaderElement("h1");
                    break;
                case "b":
                    element = new BoldElement("b");
                    break;
                case "i":
                    element = new ItalicElement("i");
                    break;
                case "image":
                    element = new ImageElement("image");
                    break;
                case "url":
                    element = new UrlElement("url");
                    break;
                case "table":
                    element = new TableElement("table");
                    break;
                case "rows":
                    element = new RowsElement("rows");
                    break;
                case "row":
                    element = new RowElement("row");
                    break;
                case "columns":
                    element = new ColumnsElement("columns");
                    break;
                case "column":
                    element = new ColumnElement("column");
                    break;
                case "list":
                    element = new ListElement("list");
                    break;
                case "listitem":
                    element = new ListItemElement("listitem");
                    break;
                default:
                    element = new StringElement(token.RawData);
                    break;
            }
            if (token.Properties?.Count > 0)
            {
                Element.AddPropertiers(element, token.Properties);
            }
            return element;
        }

        public static void AddPropertiers(Element element, List<TokenProperty> properties)
        {
            foreach (var property in properties)
            {
                dynamic existingPropery = element.Properties.FirstOrDefault(x => x.Name == property.Name);
                if (existingPropery is not null)
                {
                    existingPropery.SetValue(property.Value);
                }
            }
        }

        public virtual void Draw(Canvas canvas, ref ElementDrawResult result)
        {
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
        public RootElement(string data) : base(data)
        {
            Properties.AddRange(new Properties()
            {
                new PageHeightProperty() {Value = 29.7d},
                new PageWidthProperty() {Value = 21.0d},
                new LeftPaddingProperty() {Value = 2.5d},
                new RightPaddingProperty() {Value = 1.5d},
                new TopPaddingProperty() {Value = 2.0d},
                new BottomPaddingProperty() {Value = 1.0d},
            });
        }

        public override void Draw(Canvas canvas, ref ElementDrawResult result)
        {
            result.Properties.Push(this.Properties);
            result.PageProperties = this.Properties;
            result.LeftPadding = (this.Properties.FirstOrDefault(x => x.Name == "LeftPadding") as LeftPaddingProperty).Value * 35.43307d;
            result.RightPadding = (this.Properties.FirstOrDefault(x => x.Name == "RightPadding") as RightPaddingProperty).Value * 35.43307d;
            result.TopPadding = (this.Properties.FirstOrDefault(x => x.Name == "TopPadding") as TopPaddingProperty).Value * 35.43307d;
            result.BottomPadding = (this.Properties.FirstOrDefault(x => x.Name == "BottomPadding") as BottomPaddingProperty).Value * 35.43307d;
            result.RootCanvas = canvas;
            result.RootCanvas.Height = 0;
            result.RootCanvas.Width = 0;

            Canvas currentPage = null;

            for (var i = 0; i < this.ChildElements.Count; i++)
            {
                var child = this.ChildElements[i];

                if (currentPage is null || (result.Y > result.CanavsHeight + result.TopPadding))
                {
                    currentPage = new Canvas();
                    result.Properties.Peek().Apply(currentPage);
                    currentPage.UpdateLayout();

                    result.CanavsWidth = currentPage.Width - (result.LeftPadding + result.RightPadding);
                    result.CanavsHeight = currentPage.Height - (result.TopPadding + result.BottomPadding);
                    result.X = result.LeftPadding;
                    result.Y = result.TopPadding;

                    Rectangle bg = new Rectangle();
                    bg.Width = currentPage.Width;
                    bg.Height = currentPage.Height;
                    bg.Fill = Brushes.White;
                    Canvas.SetLeft(bg, 0);
                    Canvas.SetTop(bg, 0);
                    
                    currentPage.Children.Add(bg);

                    Rectangle bg2 = new Rectangle();
                    bg2.Width = result.CanavsWidth;
                    bg2.Height = result.CanavsHeight;
                    bg2.Stroke = Brushes.LightPink;
                    bg.StrokeDashArray = new DoubleCollection() { 1.0d };
                    Canvas.SetLeft(bg2, result.LeftPadding);
                    Canvas.SetTop(bg2, result.TopPadding);

                    currentPage.Children.Add(bg2);

                    currentPage.UpdateLayout();

                    Canvas.SetLeft(currentPage, 0);
                    Canvas.SetTop(currentPage, result.RootCanvas.Height);
                    Canvas.SetZIndex(currentPage, 0);

                    result.RootCanvas.Children.Add(currentPage);
                    result.RootCanvas.Height += currentPage.Height + 10.0d;
                    result.RootCanvas.Width += currentPage.Width;
                    currentPage.UpdateLayout();

                }

                child.Draw(currentPage, ref result);

                if (currentPage is null || (result.Y > result.CanavsHeight + result.TopPadding))
                {
                    i--;
                    currentPage.Children.Remove(result.elementUi);

                    currentPage = new Canvas();
                    result.Properties.Peek().Apply(currentPage);
                    currentPage.UpdateLayout();

                    result.CanavsWidth = currentPage.Width - (result.LeftPadding + result.RightPadding);
                    result.CanavsHeight = currentPage.Height - (result.TopPadding + result.BottomPadding);
                    result.X = result.LeftPadding;
                    result.Y = result.TopPadding;

                    Rectangle bg = new Rectangle();
                    bg.Width = currentPage.Width;
                    bg.Height = currentPage.Height;
                    bg.Fill = Brushes.White;
                    Canvas.SetLeft(bg, 0);
                    Canvas.SetTop(bg, 0);
                    
                    currentPage.Children.Add(bg);

                    Rectangle bg2 = new Rectangle();
                    bg2.Width = result.CanavsWidth;
                    bg2.Height = result.CanavsHeight;
                    bg2.Stroke = Brushes.LightPink;
                    bg.StrokeDashArray = new DoubleCollection() { 1.0d };
                    Canvas.SetLeft(bg2, result.LeftPadding);
                    Canvas.SetTop(bg2, result.TopPadding);

                    currentPage.Children.Add(bg2);
                    currentPage.UpdateLayout();

                    Canvas.SetLeft(currentPage, 0);
                    Canvas.SetTop(currentPage, result.RootCanvas.Height);
                    Canvas.SetZIndex(currentPage, 0);

                    result.RootCanvas.Children.Add(currentPage);
                    result.RootCanvas.Height += currentPage.Height + 10.0d;
                    result.RootCanvas.Width += currentPage.Width;
                    currentPage.UpdateLayout();

                   
                }

            }



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
            text.MaxHeight = Math.Floor(result.CanavsHeight);
            result.elementUi = text;
            text.Text = this.Data;
            result.Properties.Peek().Apply(text);

            Canvas.SetLeft(text, result.LeftPadding);
            Canvas.SetTop(text, 0);
            Canvas.SetZIndex(text, 2);
            canvas.Children.Add(text);
            canvas.UpdateLayout();

            if (result.X + text.ActualWidth > result.CanavsWidth)
            {
                //result.Y += text.ActualHeight;
                result.X = result.LeftPadding;
            }
            Canvas.SetLeft(text, result.X);
            Canvas.SetTop(text, result.Y);

            result.X += text.ActualWidth + 10;
            //result.Y += text.ActualHeight;
            result.LineHeight = text.ActualHeight + 10;
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

            result.X = result.LeftPadding;
            result.Y += result.LineHeight;

            result.Properties.Pop();
        }
    }

    public class HeaderElement : Element
    {
        public HeaderElement(string data) : base(data)
        {
            ((dynamic)Properties.FirstOrDefault(x => x.Name == "FontSize")).Value = 20d;
            ((dynamic)Properties.FirstOrDefault(x => x.Name == "FontWeight")).Value = FontWeights.Bold;
            ((dynamic)Properties.FirstOrDefault(x => x.Name == "HorizontalOptions")).Value = HorizontalAlignment.Center;
            ((dynamic)Properties.FirstOrDefault(x => x.Name == "TextAlignment")).Value = TextAlignment.Center;
        }

        public override void Draw(Canvas canvas, ref ElementDrawResult result)
        {
            result.Properties.Push(this.Properties);
            base.Draw(canvas, ref result);

            //var blockWidth = ChildElements.Sum(x => x.elementUi.ActualWidth + 10);
            //var start = result.CanavsWidth / 2 - blockWidth / 2;
            //foreach (var element in ChildElements)
            //{
            //    Canvas.SetLeft(element.elementUi, start);
            //    start += element.elementUi.ActualWidth + 10;
            //}
            canvas.UpdateLayout();

            result.X = result.LeftPadding;
            result.Y += result.LineHeight;

            result.Properties.Pop();
        }
    }

    public class BoldElement : Element
    {
        public BoldElement(string data) : base(data)
        {
            ((dynamic)Properties.FirstOrDefault(x => x.Name == "FontWeight")).Value = FontWeights.Bold;
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
            ((dynamic)Properties.FirstOrDefault(x => x.Name == "FontStyle")).Value = FontStyles.Italic;
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
            var pathprop = Properties.FirstOrDefault(x => x.Name == "Path") as PathProperty;
            if (pathprop is null || pathprop?.Value is null)
            {
                return;
            }
            myBitmapImage.UriSource = new Uri(pathprop.Value);
            myBitmapImage.DecodePixelWidth = 200;
            myBitmapImage.EndInit();
            //set image source
            image.Source = myBitmapImage;
            canvas.Children.Add(image);
            canvas.UpdateLayout();

            result.X = result.LeftPadding;
            result.Y += image.ActualHeight;
            result.elementUi = image;


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
            grid.ShowGridLines = false;
            canvas.Children.Add(grid);
            Canvas.SetLeft(grid, result.X);
            Canvas.SetTop(grid, result.Y);

            ColumnsElement columns = (ColumnsElement)ChildElements.FirstOrDefault(x => x is ColumnsElement);
            foreach (ColumnElement column in columns.ChildElements)
            {
                var width = new GridLength(Double.Parse(column.ChildElements[0].Data), GridUnitType.Pixel);
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = width });

            }
            RowsElement rows = (RowsElement)ChildElements.FirstOrDefault(x => x is RowsElement);
            foreach (RowElement row in rows.ChildElements)
            {
                var height = new GridLength(Double.Parse(row.ChildElements[0].Data), GridUnitType.Pixel);
                grid.RowDefinitions.Add(new RowDefinition() { Height = height });
            }

            for (int i = 0; i < grid.ColumnDefinitions.Count; i++)
            {
                for (int j = 0; j < grid.RowDefinitions.Count; j++)
                {
                    var border = new Border();
                    border.BorderBrush = new SolidColorBrush(Colors.Black);
                    border.BorderThickness = new Thickness(
                        i == 0 ? 2 : 0,
                        j == 0 ? 2 : 0,
                        2,
                        2);
                    Grid.SetColumn(border, i);
                    Grid.SetRow(border, j);
                    grid.Children.Add(border);
                }
            }

            canvas.UpdateLayout();

            foreach (var element in ChildElements.Where(x => x is not RowsElement && x is not ColumnsElement))
            {
                var rowProperty = element.Properties.FirstOrDefault(x => x is RowProperty) as RowProperty;
                var rowSpanProperty = element.Properties.FirstOrDefault(x => x is RowSpanProperty) as RowSpanProperty;
                var elementRow = rowProperty.Value;

                var columnProperty = element.Properties.FirstOrDefault(x => x is ColumnProperty) as ColumnProperty;
                var columnSpanProperty = element.Properties.FirstOrDefault(x => x is ColumnSpanProperty) as ColumnSpanProperty;
                var elementColumn = columnProperty.Value;

                element.Draw(canvas, ref result);

                Grid.SetColumn(result.elementUi, columnProperty.Value);
                Grid.SetRow(result.elementUi, elementRow);
                Grid.SetColumnSpan(result.elementUi, columnSpanProperty.Value);
                Grid.SetRowSpan(result.elementUi, rowSpanProperty.Value);

                var columnWidth = grid.ColumnDefinitions[elementColumn].ActualWidth;
                var rowHeight = grid.RowDefinitions[elementRow].ActualHeight;
                result.elementUi.MaxHeight = rowHeight;
                result.elementUi.MaxWidth = columnWidth;
                canvas.Children.Remove(result.elementUi);
                grid.Children.Add(result.elementUi);




            }

            result.Y += grid.ActualHeight + 10;


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

    public class ListElement : Element
    {
        public ListElement(string data) : base(data)
        {

        }

        public override void Draw(Canvas canvas, ref ElementDrawResult result)
        {

        }
    }


    public class ListItemElement : Element
    {
        public ListItemElement(string data) : base(data)
        {

        }
    }


}
