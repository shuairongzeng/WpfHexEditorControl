//////////////////////////////////////////////
// Apache 2.0  - 2021
// Author : Derek Tremblay (derektremblay666@gmail.com)
//
//
// NOT A TRUE PROJECT! IT'S JUST A SAMPLE FOR TESTING THE HEXEDITOR IN VARIOUS SITUATIONS... 
//////////////////////////////////////////////

using System.Windows.Media;

namespace WpfHexEditor.Sample.BinaryFilesDifference
{
    public class BlockData
    {
        public long StartPosition { get; set; }
        public int Length { get; set; }
        public SolidColorBrush   Color { get; set; }
    }
}