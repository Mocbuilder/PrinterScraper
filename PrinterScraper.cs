// @nuget: HtmlAgilityPack

using System;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace ScrapeTest
{
    public class Program
    {
		public static bool displayZittau = false;
		
        public static void Main()
        {
            var web = new HtmlWeb();

            //placeholder, should be "var document = web.Load("redacted");"
            //var document = web.Load("redacted");
            var document = new HtmlDocument();
            document.Load("redacted");

            var printers = new List<Printer>();

            //get zittau element to filter out zittau printers
            var zittauElement = document.DocumentNode.SelectSingleNode("//div[@class='v-subheader theme--dark' and contains(text(), 'Zittau')]");

			var printerElements = document.DocumentNode.SelectNodes("//div[@role='listitem' and contains(@class, 'v-list-item')]");

            if (printerElements != null)
            {
                foreach (var printerElement in printerElements)
                {

                    if (displayZittau == false && zittauElement != null && printerElement.StreamPosition > zittauElement.StreamPosition)
                    {
                        break;
                    }

                    var titleNode = printerElement.SelectSingleNode(".//div[@class='v-list-item__title']");
                    var subtitleNode = printerElement.SelectSingleNode(".//div[@class='v-list-item__subtitle']");

                    var tonerPercentNode = printerElement.SelectSingleNode(".//div[contains(@class, 'supply-black')]/span");

                    var title = titleNode != null ? HtmlEntity.DeEntitize(titleNode.InnerText) : string.Empty;
                    var subtitle = subtitleNode != null ? HtmlEntity.DeEntitize(subtitleNode.InnerText) : string.Empty;

                    int tonerPercent = 0;
                    
                    if (tonerPercentNode != null)
                    {
                        var percentText = tonerPercentNode.InnerText.TrimEnd('%');
                        tonerPercent = Convert.ToInt32(percentText);
                    }
					else
					{
						//filters out
						tonerPercent = 666;
					}
					
                    printers.Add(new Printer(title, subtitle, tonerPercent));
                }
            }

            // Print output to verify
            foreach (var printer in printers)
            {
                Console.WriteLine("Title: " + printer.Title + ", SubTitle: " + printer.SubTitle + ", Toner Percent: " + printer.TonerPercent + "%");
            }

            GetPrintersToService(printers);

            Console.ReadLine();
        }

        public static void GetPrintersToService(List<Printer> printers)
        {
            foreach (Printer printer in printers)
            {
                if (printer.TonerPercent <= 5)
                {
                    Console.WriteLine(printer.Title + " | Toner: " + printer.TonerPercent + "%");
                }
            }
        }
    }

    public class Printer
    {
        public string Title { get; set; }

        public string SubTitle { get; set; }

        public int TonerPercent { get; set; }

        public Printer(string title, string subTitle, int tonerPercent)
        {
            Title = title;
            SubTitle = subTitle;
            TonerPercent = tonerPercent;
        }
    }
}
