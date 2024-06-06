using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

namespace TextEditorWPF.Model
{
    public class Properties : List<IProperty>
    {

        public Properties() { }

        public void Apply(object uiElement)
        {
            foreach(var x in this)
            {
                dynamic property = x;
                property.Apply(uiElement);
            }
        }
    }

    public interface IProperty {

        public string Name { get; }
    }

    public abstract class Property<E, T> : IProperty
    {
        public string Name { get; set; }
        public T Value { get; set; }
        public virtual void Apply(object uiElement) { }
        public virtual bool CanApply(object uiElement)
        {
            return uiElement is E;
        }

        protected Property(string name)
        {
            Name = name;
        }

        public virtual E ConvertElement(object uiElement)
        {
            return (E)uiElement;
        }

        public virtual void SetValue(string value) 
        {
            Value = ConvertFromString(value);
        }

        protected virtual T ConvertFromString(string value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null)
            {
                return (T)converter.ConvertFromString(value);
            }
            return default(T);
        }
    }

    public class TextBlockProperty<T> : Property<TextBlock, T>
    {
        public TextBlockProperty(string name) : base(name)
        {
        }
    }

    public class FontSizeProperty : TextBlockProperty<double>
    {
        public FontSizeProperty() : base("FontSize") { }

        public override void Apply(object uiElement)
        {
            if (!CanApply(uiElement)) { return; }
            ConvertElement(uiElement).FontSize = Value;
        }
    }

    public class FontWeightProperrty : TextBlockProperty<FontWeight>
    {
        public FontWeightProperrty() : base("FontWeight")
        {
        }

        public override void Apply(object uiElement)
        {
            if (!CanApply(uiElement)) { return; }
            ConvertElement(uiElement).FontWeight = Value;
        }
    }

    public class FontStyleProperty : TextBlockProperty<FontStyle>
    {
        public FontStyleProperty() : base("FontStyle")
        {
        }

        public override void Apply(object uiElement)
        {
            if (!CanApply(uiElement)) { return; }
            ConvertElement(uiElement).FontStyle = Value;
        }
    }

    public class TextAlignmentProperty : TextBlockProperty<TextAlignment>
    {
        public TextAlignmentProperty() : base("TextAlignment")
        {
        }

        public override void Apply(object uiElement)
        {
            if (!CanApply(uiElement)) { return; }
            ConvertElement(uiElement).TextAlignment = Value;
        }
        protected override TextAlignment ConvertFromString(string value)
        {
            switch (value)
            {
                case "Left":
                    return TextAlignment.Left;
                case "Right":
                    return TextAlignment.Right;
                case "Center":
                    return TextAlignment.Center;
                case "Justify":
                    return TextAlignment.Justify;
                default:
                    return TextAlignment.Left;
            }
        }
    }

    public class FrameworkElementProperty<T> : Property<FrameworkElement, T>
    {
        public FrameworkElementProperty(string name) : base(name)
        {
        }
    }



    public class HorizontalOptionsProperty : FrameworkElementProperty<HorizontalAlignment>
    {
        public HorizontalOptionsProperty() : base("HorizontalOptions")
        {
        }

        public override void Apply(object uiElement)
        {
            if (!CanApply(uiElement)) { return; }
            ConvertElement(uiElement).HorizontalAlignment = Value;
        }

        protected override HorizontalAlignment ConvertFromString(string value)
        {
            switch (value)
            {
                case "Left":
                    return HorizontalAlignment.Left;
                case "Right":
                    return HorizontalAlignment.Right;
                case "Center":
                    return HorizontalAlignment.Center;
                default:
                    return HorizontalAlignment.Left;
            }
        }
    }

    public class VerticalOptionsProperty : FrameworkElementProperty<VerticalAlignment>
    {
        public VerticalOptionsProperty() : base("VerticalOptions")
        {
        }

        public override void Apply(object uiElement)
        {
            if (!CanApply(uiElement)) { return; }
            ConvertElement(uiElement).VerticalAlignment = Value;
        }

        protected override VerticalAlignment ConvertFromString(string value)
        {
            switch (value)
            {
                case "Top":
                    return VerticalAlignment.Top;
                case "Right":
                    return VerticalAlignment.Bottom;
                case "Center":
                    return VerticalAlignment.Center;
                default:
                    return VerticalAlignment.Top;
            }
        }
    }

    public class DebugProperty : TextBlockProperty<bool>
    {

        private Random rnd = new Random();

        public DebugProperty() : base("Debug")
        {
        }

        public override void Apply(object uiElement)
        {
            if (!CanApply(uiElement)) { return; }
            if (Value)
            {
                ConvertElement(uiElement).Background = new SolidColorBrush(Color.FromRgb((byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256)));
            }
        }
    }

    public class GridProperty<T> : Property<Grid, T>
    {
        public GridProperty(string name) : base(name)
        {
        }
    }


    public class RowProperty : GridProperty<int>
    {
        public RowProperty() : base("Row")
        {
        }
    }

    public class RowSpanProperty : GridProperty<int>
    {
        public RowSpanProperty() : base("RowSpan")
        {
        }
    }

    public class ColumnProperty : GridProperty<int>
    {
        public ColumnProperty() : base("Column")
        {
        }
    }
    public class ColumnSpanProperty : GridProperty<int>
    {
        public ColumnSpanProperty() : base("ColumnSpan")
        {
        }
    }

    public class ImageProperty<T> : Property<Image, T>
    {
        public ImageProperty(string name) : base(name)
        {
        }
    }

    public class PathProperty : ImageProperty<string>
    {
        public PathProperty() : base("Path")
        {
        }
    }

    public class PagePropertry<T> : Property<Canvas, T>
    {
        public PagePropertry(string name) : base(name)
        {

        }
    }

    public class PageHeightProperty : PagePropertry<double>
    {
        public PageHeightProperty() : base("PageHeight")
        {
            
        }

        public override void Apply(object uiElement)
        {
            if (!CanApply(uiElement)) { return; }
            ConvertElement(uiElement).Height = Value * 35.43307d;
        }
    }


    public class PageWidthProperty : PagePropertry<double>
    {
        public PageWidthProperty() : base("PageWidth")
        {

        }

        public override void Apply(object uiElement)
        {
            if (!CanApply(uiElement)) { return; }
            ConvertElement(uiElement).Width = Value * 35.43307d;
        }
    }

    public class LeftPaddingProperty : PagePropertry<double>
    {
        public LeftPaddingProperty() : base("LeftPadding")
        {

        }
    }

    public class RightPaddingProperty : PagePropertry<double>
    {
        public RightPaddingProperty() : base("RightPadding")
        {

        }
    }

    public class TopPaddingProperty : PagePropertry<double>
    {
        public TopPaddingProperty() : base("TopPadding")
        {

        }
    }

    public class BottomPaddingProperty : PagePropertry<double>
    {
        public BottomPaddingProperty() : base("BottomPadding")
        {

        }
    }






}
