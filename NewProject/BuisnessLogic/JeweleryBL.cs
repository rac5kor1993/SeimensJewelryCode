using NewProject.Interface;
using System;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text;
using System.IO;

namespace NewProject.BuisnessLogic
{
    public class JeweleryBL : IJeweleryBL
    {
        private readonly IJeweleryBr _JewelryBr;

        public JeweleryBL(IJeweleryBr JewelryBr)
        {

            _JewelryBr = JewelryBr;
        }

        public bool GetUser(string username)
        {
            var res = _JewelryBr.GetUser(username);
            if (res.Name.Equals(username))
            {
                return true;
            }
            else
                return false;
        }

        public GoldPriceCalculation GetTotalGoldPrice(GoldPriceCalculation goldprice, string userrole)
        {

            if (userrole == "Priveleged")
            {
                goldprice.Discount = 2;

            }
            goldprice.GetTotalValue(goldprice.Discount);

            return goldprice;
        }

        public MemoryStream CreatePdf(GoldPriceCalculation goldprice)
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created   
            string strPDFFileName = string.Format("GOLDCompany" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(0, 0, 0, 0);
            //Create PDF Table with 5 columns  
            PdfPTable tableLayout = new PdfPTable(5);
            doc.SetMargins(0, 0, 0, 0);
            //Create PDF Table  

            //file will created in this path  
            string strAttachment = Path.Combine("~/Downloads/" + strPDFFileName);


            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();

            //Add Content to PDF   
            doc.Add(Add_Content_To_PDF(tableLayout, goldprice));

            // Closing the document  
            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;
            return workStream;
        }
        protected PdfPTable Add_Content_To_PDF(PdfPTable tableLayout, GoldPriceCalculation goldprice)
        {

            float[] headers = { 50, 24, 45, 35, 50 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            //Add Title to the PDF file at the top  

            ////Add header  

            AddCellToHeader(tableLayout, "Gold Price(per gram)");
            AddCellToHeader(tableLayout, "Weight");
            AddCellToHeader(tableLayout, "Discount");
            AddCellToHeader(tableLayout, "Total Price");

            ////Add body  


            AddCellToBody(tableLayout, goldprice.GetGoldPrice.ToString());
            AddCellToBody(tableLayout, goldprice.GetWeight.ToString());
            AddCellToBody(tableLayout, goldprice.GetDiscount.ToString());
            AddCellToBody(tableLayout, goldprice.GetTotalPrice.ToString());




            return tableLayout;
        }
        // Method to add single cell to the Header  
        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.YELLOW)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5,
                BackgroundColor = new iTextSharp.text.BaseColor(128, 0, 0)
            });
        }

        // Method to add single cell to the body  
        private static void AddCellToBody(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });
        }
    }
}
