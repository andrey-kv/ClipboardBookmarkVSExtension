using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipboardBookmark.Model
{
    public class BufferModel
    {
        public string bufferBody { get; set; }
        public int topPosition { get; set; }
        public int bottomPosition { get; set; }

        public void BufferToClipboard()
        {
            System.Windows.Forms.Clipboard.SetText(bufferBody
                + "|" + topPosition
                + "|" + bottomPosition + "\r\n");
        }

        public bool ClipboardToBuffer()
        {
            string buffer = System.Windows.Forms.Clipboard.GetText();
            bool positionExtracted = false;
            if (buffer.Contains("|") && buffer.Length < 500)
            {
                var spl = buffer.Split('|');
                for (int i = 0; i < spl.Length; i++)
                {
                    if (i == 0)
                    {
                        bufferBody = spl[i];
                        positionExtracted = true;
                    }
                    else
                    {
                        int posValue = 0;
                        if (Int32.TryParse(spl[i], out posValue))
                        {
                            switch (i)
                            {
                                case 1:
                                    topPosition = posValue;
                                    bottomPosition = posValue;
                                    break;
                                case 2:
                                    bottomPosition = posValue;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            return positionExtracted;
        }
    }
}
