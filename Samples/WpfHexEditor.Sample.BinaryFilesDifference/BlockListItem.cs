using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfHexEditor.Sample.BinaryFilesDifference
{
    public partial class BlockListItem : UserControl
    {
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(nameof(Position), typeof(long), typeof(BlockListItem), new PropertyMetadata(0L));

        public static readonly DependencyProperty FirstFileByteProperty =
            DependencyProperty.Register(nameof(FirstFileByte), typeof(byte?), typeof(BlockListItem), new PropertyMetadata(null));

        public static readonly DependencyProperty SecondFileByteProperty =
            DependencyProperty.Register(nameof(SecondFileByte), typeof(byte?), typeof(BlockListItem), new PropertyMetadata(null));

        public long Position
        {
            get => (long)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        public byte? FirstFileByte
        {
            get => (byte?)GetValue(FirstFileByteProperty);
            set => SetValue(FirstFileByteProperty, value);
        }

        public byte? SecondFileByte
        {
            get => (byte?)GetValue(SecondFileByteProperty);
            set => SetValue(SecondFileByteProperty, value);
        }
    }
}