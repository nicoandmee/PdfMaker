using PdfFileWriter;
using System;
using System.Drawing;
using System.IO;
using PassKitSharp;
using System.Text;

namespace PassAPI
{

    public class TicketPdfMaker
    {
        private PdfFont ArialNormal;
        private PdfFont ArialBold;
        private PdfFont ArialItalic;
        private PdfFont ArialBoldItalic;
        private PdfFont TimesNormal;
        private PdfFont Comic;
        private PdfTilingPattern WaterMark;
        private PdfDocument Document;
        private PdfPage Page;
        private PdfContents Contents;
        private PassKit pk;

        public TicketPdfMaker(string pkpassfile)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            pk = PassKit.Parse(pkpassfile);
            Console.WriteLine(pk.Barcode.Message);

        }

        public void Test
                (
                Boolean Debug,
                String FileName
                )
        {
            Console.WriteLine("Start");
            // Step 1: Create empty document
            Document = new PdfDocument(PaperType.Letter, false, UnitOfMeasure.Inch, FileName);
            Document.Debug = Debug;

            PdfInfo Info = PdfInfo.CreatePdfInfo(Document);
            Info.Title("Ticket");

            // Step 2: create resources
            DefineFontResources();

            // Step 3: Add new page
            Page = new PdfPage(Document);

            // Step 4: Add contents to page
            Contents = new PdfContents(Page);

            // Step 5: add graphices and text contents to the contents object
            DrawFrameAndBackgroundWaterMark();

            // DrawIcon();
            DrawLogo();
            DrawStrip();
            // DrawTime();
            DrawAuxiliaryFields();
            DrawBarcode();

            // Step 6: create pdf file
            Document.CreateFile();

            Console.WriteLine($"[+] Output File: {FileName}");

            // exit
            return;
        }

        // Stop images from losing transparency
        private Image byteArrayToImage(byte[] byteArrayIn)
        {
            Bitmap src = (Bitmap)((new ImageConverter()).ConvertFrom(byteArrayIn));
            Bitmap target = new Bitmap(src.Size.Width, src.Size.Height);
            Graphics g = Graphics.FromImage(target);
            g.Clear(Color.FromArgb(pk.BackgroundColor.Red, pk.BackgroundColor.Green, pk.BackgroundColor.Blue));
            g.DrawImage(src, 0, 0);
            return target;
        }

        // Define Font Resources
        private void DefineFontResources()
        {
            // Define font resources
            // Arguments: PdfDocument class, font family name, font style, embed flag
            // Font style (must be: Regular, Bold, Italic or Bold | Italic) All other styles are invalid.
            // Embed font. If true, the font file will be embedded in the PDF file.
            // If false, the font will not be embedded
            String FontName1 = "Arial";
            String FontName2 = "Times New Roman";

            ArialNormal = PdfFont.CreatePdfFont(Document, FontName1, FontStyle.Regular, true);
            ArialBold = PdfFont.CreatePdfFont(Document, FontName1, FontStyle.Bold, true);
            ArialItalic = PdfFont.CreatePdfFont(Document, FontName1, FontStyle.Italic, true);
            ArialBoldItalic = PdfFont.CreatePdfFont(Document, FontName1, FontStyle.Bold | FontStyle.Italic, true);
            TimesNormal = PdfFont.CreatePdfFont(Document, FontName2, FontStyle.Regular, true);
            Comic = PdfFont.CreatePdfFont(Document, "Comic Sans MS", FontStyle.Bold, true);
            return;
        }

        private void DrawFrameAndBackgroundWaterMark()
        {
            Contents.SaveGraphicsState();
            Contents.SetColorNonStroking(Color.FromArgb(pk.BackgroundColor.Red, pk.BackgroundColor.Green, pk.BackgroundColor.Blue));
            Contents.DrawRectangle(0.0, 0.0, 8.5, 11, PaintOp.Fill);
            Contents.RestoreGraphicsState();
            return;
        }

        private void CreateLogoFile()
        {
            using (var imageFile = new FileStream($"temps/{pk.Logo.HighResFilename}", FileMode.Create))
            {
                imageFile.Write(pk.Logo.HighResData, 0, pk.Logo.HighResData.Length);
                imageFile.Flush();
            }
            return;
        }

        private void DrawLogo()
        {
            if (pk.Logo != null)
            {
                PdfImage LogoImage = new PdfImage(Document);
                LogoImage.Resolution = 96.0;
                LogoImage.ImageQuality = 100;
                LogoImage.LoadImage(byteArrayToImage(pk.Logo.HighResData));
                Contents.SaveGraphicsState();

                // translate coordinate origin to the center of the picture
                Contents.Translate(0, 10);

                // adjust image size an preserve aspect ratio
                PdfRectangle NewSize = LogoImage.ImageSizePosition(3.1, 1.2, ContentAlignment.MiddleCenter);

                // clipping path
                // Contents.DrawOval(NewSize.Left, NewSize.Bottom, NewSize.Width, NewSize.Height, PaintOp.Fill);

                // draw image
                Contents.DrawImage(LogoImage, NewSize.Left, NewSize.Bottom, NewSize.Width, NewSize.Height);

                // restore graphics state
                Contents.RestoreGraphicsState();
            }

            return;
        }

        private void CreateIconFile()
        {
            using (var imageFile = new FileStream($"temps/{pk.Icon.HighResFilename}", FileMode.Create))
            {
                imageFile.Write(pk.Icon.HighResData, 0, pk.Icon.HighResData.Length);
                imageFile.Flush();
            }
            return;
        }

        private void DrawIcon()
        {
            if (pk.Icon != null)
            {
                PdfImage IconImage = new PdfImage(Document);
                IconImage.Resolution = 96.0;
                IconImage.ImageQuality = 100;
                IconImage.LoadImage(byteArrayToImage(pk.Icon.HighResData));
                Contents.SaveGraphicsState();

                // translate coordinate origin to the center of the picture
                Contents.Translate(0, 10);

                // adjust image size an preserve aspect ratio
                PdfRectangle NewSize = IconImage.ImageSizePosition(3, 1.1, ContentAlignment.MiddleCenter);

                // clipping path
                // Contents.DrawOval(NewSize.Left, NewSize.Bottom, NewSize.Width, NewSize.Height, PaintOp.Fill);
                // Contents.SetBlendMode(BlendMode.Difference);

                // draw image
                Contents.DrawImage(IconImage, NewSize.Left, NewSize.Bottom, NewSize.Width, NewSize.Height);

                // restore graphics state
                Contents.RestoreGraphicsState();
            }

            return;
        }


        private void DrawProperty(string label, string value, TextJustify textalign, double posX, double posY)
        {
            Color labelColor = Color.FromArgb(pk.LabelColor.Red, pk.LabelColor.Green, pk.LabelColor.Blue);
            Contents.SetColorNonStroking(labelColor);

            Contents.DrawText(ArialNormal, 16.0, posX, posY, textalign, label);
            Contents.SaveGraphicsState();

            // change nonstroking (fill) color to purple
            Contents.SetColorNonStroking(Color.White);

            // Draw second line of heading text
            // Text Justify: Center (text center will be at X position)
            Contents.DrawText(ArialNormal, 24.0, posX, posY - 0.4, textalign, value);

            // restore graphics sate (non stroking color will be restored to default)
            Contents.RestoreGraphicsState();
            return;
        }

        private void CreateStripFile()
        {
            using (var imageFile = new FileStream($"temps/{pk.Strip.HighResFilename}", FileMode.Create))
            {
                imageFile.Write(pk.Strip.HighResData, 0, pk.Strip.HighResData.Length);
                imageFile.Flush();
            }
            return;
        }

        private void DrawStrip()
        {
            if (pk.Strip != null)
            {
                PdfImage StripImage = new PdfImage(Document);
                StripImage.Resolution = 96.0;
                StripImage.ImageQuality = 100;
                StripImage.LoadImage(byteArrayToImage(pk.Strip.HighResData));
                Contents.SaveGraphicsState();

                // translate coordinate origin to the center of the picture
                Contents.Translate(0, 7.8);

                // adjust image size an preserve aspect ratio
                PdfRectangle NewSize = StripImage.ImageSizePosition(8.5, 2.3, ContentAlignment.MiddleCenter);

                // draw image
                Contents.DrawImage(StripImage, NewSize.Left, NewSize.Bottom, NewSize.Width, NewSize.Height);

                // Contents.DrawImage(StripImage, 0, 7.8, 8.5, 2.3);

                Contents.RestoreGraphicsState();
            }
            return;
        }

        private PKPassField FindPKPassField(PKPassFieldSet source, string key)
        {
            foreach (PKPassField item in source)
            {
                if (item.Key == key)
                {
                    return item;
                }
            }
            return null;
        }

        private void DrawAuxiliaryFields()
        {
            PKPassStringField eventName = (PKPassStringField)FindPKPassField(pk.SecondaryFields, "eventName");
            DrawProperty(eventName?.Label, eventName?.Value, TextJustify.Left, 0.5, 7.6);

            var section = FindPKPassField(pk.AuxiliaryFields, "section");
            if (section is PKPassStringField sectionString)
            {
                DrawProperty(section?.Label, sectionString?.Value, TextJustify.Left, 0.5, 6.6);
            }
            if (section is PKPassNumberField sectionNum)
            {
                DrawProperty(section?.Label, sectionNum?.Value.ToString(), TextJustify.Left, 0.5, 6.6);
            }

            PKPassNumberField row = (PKPassNumberField)FindPKPassField(pk.AuxiliaryFields, "row");
            DrawProperty(row?.Label, row?.Value.ToString(), TextJustify.Left, 1.8, 6.6);

            PKPassNumberField seat = (PKPassNumberField)FindPKPassField(pk.AuxiliaryFields, "seat");
            DrawProperty(seat?.Label, seat?.Value.ToString(), TextJustify.Left, 2.5, 6.6);

            var level = FindPKPassField(pk.AuxiliaryFields, "level");
            if (level is PKPassStringField levelString)
            {
                DrawProperty(levelString?.Label, levelString?.Value, TextJustify.Left, 3.5, 6.6);
            }
            if (level is PKPassNumberField levelNum)
            {
                DrawProperty(levelNum?.Label, levelNum?.Value.ToString(), TextJustify.Left, 3.5, 6.6);
            }

            PKPassStringField entryinfo = (PKPassStringField)FindPKPassField(pk.AuxiliaryFields, "entryinfo");
            DrawProperty(entryinfo?.Label, entryinfo?.Value, TextJustify.Right, 7.9, 6.6);

            return;
        }

        private void DrawBarcode()
        {
            // save graphics state
            Contents.SaveGraphicsState();
            string barcode = pk.Barcode.Message;

            // create QRCode barcode
            QREncoder QREncoder = new QREncoder();

            // set error correction code (default is M)
            QREncoder.ErrorCorrection = ErrorCorrection.M;

            // encode your text or byte array
            QREncoder.Encode(barcode);

            // convert QRCode to PdfImage in black and white
            PdfImage BarcodeImage = new PdfImage(Document);
            BarcodeImage.LoadImage(QREncoder);

            Contents.DrawImage(BarcodeImage, 1.75, 0.5, 5);
            Contents.DrawText(ArialNormal, 24, 4.25, 0.7, TextJustify.Center, barcode);

            // restore graphics sate
            Contents.RestoreGraphicsState();
            return;
        }
    }
}
