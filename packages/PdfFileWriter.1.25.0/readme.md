# PdfFileWriter


The PDF File Writer C# class library allows you to create PDF files directly from your .net application. The library shields you from the details of the PDF file structure.

Version 1.25.0 enhancements: Support for font collections and for non-ASCII font names. This mainly applies to Japanese, Chinese and Korean fonts.

The full article is published at CodeProject website. <a href="https://www.codeproject.com/Articles/570682/PDF-File-Writer-Csharp-Class-Library-Version">PDF File Writer C# Class Library (Version 1.25.0)</a>. The article includes documentation and test/demo application.


Creating a PDF is a six steps process.

Step 1: Create one document object <code>PdfDocument</code>.
Step 2: Create resource objects such as fonts or images (i.e.PdfFont or PdfImage).
Step 3: Create page object PdfPage.
Step 4: Create contents object PdfContents.
Step 5: Add text and graphics to the contents object (using PdfContents methods).
Repeat steps 3, 4 and 5 for additional pages
Step 6: Create your PDF document file by calling CreateFile method of PdfDocument.





