using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClipboardBookmark.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipboardBookmark.Model.Tests
{
    [TestClass()]
    public class BufferModelTests
    {
        private const string BUFFER_RESULT = "BufferBody|20|22\r\n";

        [TestMethod()]
        public void BufferToClipboardTest()
        {
            var bufferModel = new BufferModel();
            bufferModel.bufferBody = "BufferBody";
            bufferModel.topPosition = 20;
            bufferModel.bottomPosition = 22;
            bufferModel.BufferToClipboard();

            string clipboard = System.Windows.Forms.Clipboard.GetText();
            Assert.AreEqual(BUFFER_RESULT, clipboard);
        }

        [TestMethod()]
        public void ClipboardToBufferTest()
        {
            System.Windows.Forms.Clipboard.SetText(BUFFER_RESULT);

            var bufferModel = new BufferModel();
            bool extracted = bufferModel.ClipboardToBuffer();

            Assert.IsTrue(extracted);
            Assert.AreEqual("BufferBody", bufferModel.bufferBody);
            Assert.AreEqual(20, bufferModel.topPosition);
            Assert.AreEqual(22, bufferModel.bottomPosition);
        }
    }
}