using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace Grafika_Komputerowa_1_ver1
{
    public static class DrawLineBersenham
    {
       public static void DrawLine(Bitmap image,int x1,int y1, int x2,int y2)
        {

            int dx = Abs(x2 - x1);
            int dy = Abs(y2 - y1);
            int k1 = Sign( x2 - x1);
            int k2 = Sign(y2 - y1);
            int incrE;
            int incrNE;
            int d;
            int x = x1;
            int y = y1;
            bool change = false;
            if(dy>dx)
            {
                int temp = dy;
                dy = dx;
                dx = temp;
                change = true;
            }
            
             d = 2 * dy-dx; 
             incrE = 2 * dy; 

             incrNE = 2 * (dy-dx);
            if (x >= 0 && x < image.Width && y >= 0 && y < image.Height)
                image.SetPixel(x, y,Color.Black);
            for(int i=1;i<dx;i++)
            {
                if(d<0)
                {
                    d += incrE;
                    if (change)
                        y += k2;
                    else
                        x += k1;
                   
                }
                else
                {
                    d += incrNE;
                    x += k1;
                    y += k2;
                }
                if(x>=0&&x<image.Width&&y>=0&&y<image.Height)
                    image.SetPixel(x, y, Color.Black);
            }

        }

        
    }
}
