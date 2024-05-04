using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

namespace TextEditorWPF.Model
{
    public class Properties : List<Property>
    {

        public Properties() { }

        public void Apply(object uiElement)
        {
            this.ForEach(x => x.Apply(uiElement));
        }
    }

    public abstract class Property
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public virtual void Apply(object uiElement) { }
        public virtual bool CanApply<E>(object uiElement) where E : class
        {
            return uiElement is E;
        }
    }

    public class TextBlockProperty<E> : Property where E : TextBlock
    {

    }

    public class FontSizeProperty<E> : TextBlockProperty<E> where E : TextBlock
    {
        public override void Apply(object uiElement)
        {
            if (!CanApply<E>(uiElement)) { return; }
            ((E)uiElement).FontSize = (double)Value;
        }
    }

    public class FontWeightProperrty<E> : TextBlockProperty<E> where E : TextBlock
    {
        public override void Apply(object uiElement)
        {
            if (!CanApply<E>(uiElement)) { return; }
            ((E)uiElement).FontWeight = (FontWeight)Value;
        }
    }

    public class FontStyleProperty<E> : TextBlockProperty<E> where E : TextBlock
    {
        public override void Apply(object uiElement)
        {
            if (!CanApply<E>(uiElement)) { return; }
            ((E)uiElement).FontStyle = (FontStyle)Value;
        }
    }

    public class FrameworkElementProperty<E> : Property where E : FrameworkElement
    {
    }



    public class HorizontalOptionsProperty<E> : FrameworkElementProperty<E> where E : FrameworkElement
    {
        public override void Apply(object uiElement)
        {
            if (!CanApply<E>(uiElement)) { return; }
            ((E)uiElement).HorizontalAlignment = (HorizontalAlignment)Value;
        }
    }

    public class Debug<E> : TextBlockProperty<E> where E : TextBlock
    {

        private Random rnd = new Random();
        public override void Apply(object uiElement)
        {
            if (!CanApply<E>(uiElement)) { return; }
            if ((bool)Value)
            {
                ((E)uiElement).Background = new SolidColorBrush(Color.FromRgb((byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256)));
            }
        }
    }


    public class GridProperty<E>
}
