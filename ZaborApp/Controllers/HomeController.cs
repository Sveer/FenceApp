using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZaborApp.Models;

namespace ZaborApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        /// <summary>
        /// Метод отрисовывает картинку
        /// </summary>
        /// <param name="formula">Строка вида 1;1,2;3,5, где ";" разделяет новый ряд забора, а "," каждую доску в ряду. При этом длина доски 1 означает разрыв, толщиной с доску</param>
        /// <returns></returns>
        public IActionResult Picture(string formula)
        {
            int dlength = 18;
            double deltaWidth = Math.Sqrt((dlength * dlength) /2);

            
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(1000, 1000);
            System.Drawing.Pen p = new System.Drawing.Pen(System.Drawing.Color.Red, 18);
            System.Drawing.Pen p2 = new System.Drawing.Pen(System.Drawing.Color.Olive, 18);
            System.Drawing.Pen pWhite = new System.Drawing.Pen(System.Drawing.Color.White, 18);

            var maxLength = formula.Split(";", StringSplitOptions.RemoveEmptyEntries).Max(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries).Sum(y=>int.Parse(y)));
            var maxHeight =  Math.Sqrt((maxLength * maxLength * dlength * dlength) / 2);

            using (var g = System.Drawing.Graphics.FromImage(bmp))
            {
                double x = 0;
                int iter = 0;
                var stipToBottom = false;

                foreach (var line in formula.Split(";", StringSplitOptions.RemoveEmptyEntries))
                {
                    iter++;

                    var l =  line.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(z=>int.Parse(z));
                    var sum = l.Sum(s => s);
                    //sum - Длина первой линии. 
                    if (iter == 1)
                    {
                        x = Math.Sqrt((sum * dlength) * (sum * dlength) / 2)*4;
                    }

                    var lastX = 0;


                    var curLength = l.Sum(s => s);
                    stipToBottom |= maxLength == curLength; //Привязываем после самых длинных меток линии к нжнему краю и серхнему соотв.
                    var delta = (int)( stipToBottom ? Math.Sqrt(((maxLength - curLength)* dlength * (maxLength - curLength) * dlength)/2) :0);

                    foreach (var zx in l) //3, 5,1,5
                    {

                       
                        var y1 = (int)lastX + 10; //stipToBottom ? (int)maxHeight - lastX + 10 : 
                        var y2 = (int)maxHeight - lastX + 10;// !stipToBottom? (int)maxHeight - lastX + 10 : (int)lastX + 10;

                        var newPoint1 = new System.Drawing.Point((int)((int)x - lastX) - delta, y1 + delta); //0-0
                        var newPoint12 = new System.Drawing.Point((int)((int)x - lastX) - delta, y2 - delta); //0-0

                        lastX += (int)(deltaWidth * zx); //x = 3 * 18;
                        y1 = (int)lastX + 10; //stipToBottom ? (int)maxHeight - lastX + 10 : 
                        y2 = (int)maxHeight - lastX + 10;// !stipToBottom? (int)maxHeight - lastX + 10 : (int)lastX + 10;

                        var newPoint2 = new System.Drawing.Point((int)((int)x - lastX) - delta, y1 + delta);
                        var newPoint22 = new System.Drawing.Point((int)((int)x - lastX) - delta, y2 - delta);

                        if (zx == 1)
                        {
                            g.DrawLine(pWhite, newPoint2, newPoint1);
                            g.DrawLine(pWhite, newPoint22, newPoint12);
                        }
                        else
                        {
                            g.DrawLine(p, newPoint2, newPoint1);
                            g.DrawLine(p2, newPoint22, newPoint12);
                        }
                        
                    }
                    Console.WriteLine($"i: {iter}");

                    // x += 6.25* iter;
                  
                    x += deltaWidth*4;
                }
                g.Save();
            }
            ViewData["Message"] = "Your contact page.";
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return File(ms.ToArray(), "image/png");
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
