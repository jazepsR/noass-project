using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UnityEngine;
using Aspose.Pdf;


public class PdfGenerator 
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("P"))
        {
            GenerateImagePDF();
        }
    }


    public void GenerateImagePDF()
    {
        
        Aspose.Pdf.Document doc = new Document();

        // Access image files in the folder
        string[] fileList = Directory.GetFiles(@"D:/images/");

        foreach (System.String file in fileList)
        {
            // Add a page to pages collection of document
            var page = doc.Pages.Add();

            // Load image into stream
            FileStream imageStream = new FileStream(file, FileMode.Open);

            // Set margins so image will fit, etc.
            page.PageInfo.Margin.Bottom = 0;
            page.PageInfo.Margin.Top = 0;
            page.PageInfo.Margin.Left = 0;
            page.PageInfo.Margin.Right = 0;
            page.CropBox = new Aspose.Pdf.Rectangle(0, 0, 400, 400);

            // Create an image object
            Image image1 = new Image();

            // Add the image into paragraphs collection of the section
            page.Paragraphs.Add(image1);

            // Set the image file stream
            image1.ImageStream = imageStream;
        }

        // Save resultant PDF file
        doc.Save("D:/document.pdf");
    }
}
