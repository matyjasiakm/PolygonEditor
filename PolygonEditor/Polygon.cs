using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using static System.Math;
using System.Windows.Forms;
using PolygonEditor.Properties;
using System.Diagnostics;

namespace Grafika_Komputerowa_1_ver1
{
/// <summary>
/// Klasa polygon to klasa wielokąta.
/// Wielokąt jest reprezentowany jako ciąg kolejnych wierchołków (z klasy Vertex).
/// </summary>
/// <param name="isPolygonClosed">Przechowuje informacje czy wielokąt jest zamknięty.</param>
/// <param name="list">Lista kolejnych wierchołków wielokąta.( v1.next-edge-v2.prev)</param>
/// <param name="radius">Promien wierchołka.</param>
/// <param name="range">Odległość punktu klikniecia od krawedzi tak aby załapać krawdź</param>
    public class Polygon
    {

        List<Vertex> list;
        
        public bool isPolygonClosed;
        static double radius = 5;
        static double range = 15;
        public Polygon()
        {
            list = new List<Vertex>();
            
            isPolygonClosed = false;
        }
        /// <summary>
        /// Zwraca ilość wierchołków w wielokącie.
        /// </summary>
        /// <returns>Zwraca ilość wierchołków w wielokącie.</returns>
        public int Count()
        {
            return list.Count;
        }
        /// <summary>
        /// Dodaje wierchołek o podanym punkcie. 
        /// Jesli punkt pokrywa sie z wierchołkiem poczatkowym to wierchołek nie jest dodawany ale wielokąt jest zamykany.
        /// </summary>
        /// <param name="point">Punkt w którym dodawany jest wierchołek.</param>
        public void Add(Vertex point)
        {
            if (list.Count > 2 && isPointInVertexRadius(list[0], point))
            {
                isPolygonClosed = true;
            }
            if (!isPolygonClosed)
            {
                list.Add(point);           
            }
        }
        /// <summary>
        /// Ustawia kąt w wiercholku.
        /// Odpowiednim ramionom dodaje sie ograniczenia.
        /// </summary>
        /// <param name="vertex">Wierchołek w którym ma byc ustawiony dany kąt.</param>
        /// <param name="angle">Wartość kąta w wierchołku.</param>
        public void setAngleinVertex(Vertex vertex, int angle)
        {
            double radians = (PI / 180) * angle;
            Vertex next = list[(list.IndexOf(vertex) + 1) % list.Count];
            Vertex prev = list[(list.IndexOf(vertex) - 1 + list.Count) % list.Count];

            
            vertex.nextVertex = TypeOfEdge.armOfAngle;
            vertex.prevVertex = TypeOfEdge.armOfAngle;
            next.prevVertex = TypeOfEdge.armOfAngle;
            prev.nextVertex = TypeOfEdge.armOfAngle;

            if (angle != (int)getAngleInVertex(vertex))
            {
                Vertex prevprev = list[(list.IndexOf(vertex) - 2 + list.Count) % list.Count];
                double a = (prevprev.Y - prev.Y) / (prevprev.X - prev.Y);
                double b = -a * prev.X + prev.Y;
                double a_next = (next.Y - vertex.Y) / (next.X - vertex.X);
                double a1 = (-Tan(radians) - a_next) / (a_next * Tan(radians) - 1);
                double b1 = -a1 * vertex.X + vertex.Y;
                double a2= (-Tan(radians) + a_next) / (a_next * Tan(radians) + 1);
                double b2 = -a2 * vertex.X + vertex.Y;
                double x1 = (b - b1) / (a1 - a);
                double y1 = a1 * x1 + b1;
                double x2 = (b - b2) / (a2 - a);
                double y2 = a2 * x2 + b2;
                double k1, k2;

                prev.X = x1;
                prev.Y = y1;
                k1 = Abs(angle - (int)getAngleInVertex(vertex));
                prev.X = x2;
                prev.Y = y2;
                k2= Abs(angle - (int)getAngleInVertex(vertex));

                if (k1<k2)
                {
                    prev.X = x1; prev.Y = y1;
                }

            }

            if (vertex.X != next.X)
            {
                    vertex.a_next = (vertex.Y - next.Y) / (vertex.X - next.X);
                    next.a_prev = vertex.a_next;
            }
            else
            {
                    vertex.a_next = double.NaN;
                    next.a_prev = vertex.a_next;
            }


            if (vertex.X != prev.X)
            {
                    vertex.a_prev = (vertex.Y - prev.Y) / (vertex.X - prev.X);
                    prev.a_next = vertex.a_prev;
            }
            else
            {
                    vertex.a_prev = double.NaN;
                    prev.a_next = vertex.a_prev;
            }

            vertex.isAngleSetInVertex = true;
           
        }
        /// <summary>
        /// Ustawia krawędź jako poziomą. 
        /// </summary>
        /// <param name="point">Krawędz podana w formie krotki (Vertex v1, Vertex v2) </param>
        public void makeEdgeHorizontal((Vertex p1, Vertex p2) point)
        {
            if (point.p1.prevVertex != TypeOfEdge.horizontal && point.p2.nextVertex != TypeOfEdge.horizontal)
            {
                point.p1.nextVertex = TypeOfEdge.horizontal;
                point.p2.prevVertex = TypeOfEdge.horizontal;
                point.p1.Y = point.p2.Y;
            }
        }
        /// <summary>
        /// Ustawia krawędź jako pionową.
        /// </summary>
        /// <param name="point">Krawędz podana w formie krotki (Vertex v1, Vertex v2)</param>
        public void makeEdgeVertical((Vertex p1, Vertex p2) point)
        {
            if (point.p1.prevVertex != TypeOfEdge.vertical && point.p2.nextVertex != TypeOfEdge.vertical)
            {
                point.p1.nextVertex = TypeOfEdge.vertical;
                point.p2.prevVertex = TypeOfEdge.vertical;
                point.p1.X = point.p2.X;
            }
            
        }
        /// <summary>
        /// Usuwa ograniczenia z krawedzi.
        /// </summary>
        /// <param name="point">Krawędz podana w formie krotki (Vertex v1, Vertex v2)</param>
        public void makeEdgeNormal((Vertex p1, Vertex p2) point)
        {
            point.p1.nextVertex = TypeOfEdge.none;
            point.p2.prevVertex = TypeOfEdge.none;
            
        }
        /// <summary>
        /// Usuwa ograniczenia z wierchołka (kąt).
        /// </summary>
        /// <param name="vertex">Krawędz podana w formie krotki (Vertex v1, Vertex v2)</param>
        public void makeVertexNormal(Vertex vertex)
        {
            Vertex next = list[(list.IndexOf(vertex) + 1) % list.Count];
            Vertex prev = list[(list.IndexOf(vertex) - 1 + list.Count) % list.Count];
            vertex.isAngleSetInVertex = false;
            vertex.nextVertex = TypeOfEdge.none;
            vertex.prevVertex = TypeOfEdge.none;
            next.prevVertex = TypeOfEdge.none;
            prev.nextVertex = TypeOfEdge.none;
        }
        /// <summary>
        /// Przesuwa podany wierchołek na dany punkt, a następnie od tego wierchołka poprawia wielokat.
        /// Poprawianie wielokąta odbywa sie w dwie strony.
        /// Sprawdzanie odbywa sie w rosnącym kierunku listy wierzchołków i malejacym, dlatego w zalezności od kierunku sa sprawdzane rózne parametry vertex.
        /// Sprawdzanie konczy sie jesli podany wierchołek zostłą juz poprawiony, podana krawędź nie ma ograniczen lub poprawiona została jedna z 
        /// prostych przy kącie ale tylko wtedy gdy do niej wchodzimy (Jezeli rozpoczynamy od wierchołka w którym jest kąt wtedy sprawdzanie idzie dalej i nie zatrzymuje sie na ramieniu kąta)
        /// 
        /// </summary>
        /// <param name="p">Wierchołek do zmiany</param>
        /// <param name="point">Punkt w którym ma się znaleść nowy wierchołek</param>
        public void CorrectFromVertex(Vertex p,Point point)
        {
            int i = (list.IndexOf(p) - 1 + list.Count) % list.Count;
            int j = (list.IndexOf(p) + 1 ) % list.Count;
            List<Vertex> beforeChanges=new List<Vertex>();
            bool[] verticesChecked=new bool[list.Count];

            for(int k=0;k<list.Count;k++)
            {
                beforeChanges.Add(list[k].Clone());
            }
            
            verticesChecked[list.IndexOf(p)] = true;
            p.X = point.X;
            p.Y = point.Y;
            
            bool stop1 = false, stop2 = false;

            while(true)
            {
                if (list[i].nextVertex == TypeOfEdge.none)
                    stop1 = true;
                if (list[j].prevVertex == TypeOfEdge.none) 
                    stop2 = true;
                if (stop1 == stop2 && stop2 == true)
                    break;
                if (verticesChecked[j] == true && stop1 == true || verticesChecked[i] == true && stop2 == true || (verticesChecked[i] == verticesChecked[j] && verticesChecked[i] == true))
                    break;
                if (stop1 == false)
                {
                    if (list[i].nextVertex == TypeOfEdge.horizontal)
                    {

                        list[i].Y = list[(i + 1) % list.Count].Y;
                        verticesChecked[i] = true;
                        i = (i - 1 + list.Count) % list.Count;
                    }
                    else if (list[i].nextVertex == TypeOfEdge.vertical)
                    {
                        verticesChecked[i] = true;
                        list[i].X = list[(i + 1) % list.Count].X;
                        i = (i - 1 + list.Count) % list.Count;
                    }
                    else if (list[i].nextVertex == TypeOfEdge.armOfAngle)
                    {
                        
                        double a;
                        double b;
                        double a_next;
                        double b_next;
                        double x, y;
                        bool temp = false;
                        

                            if ((beforeChanges[(i + 1) % list.Count].X - list[i].X) == 0)
                            {
                                a = list[(i + 1) % list.Count].a_prev;
                                b = -a * list[(i + 1) % list.Count].X + list[(i + 1) % list.Count].Y;
                                x = list[i].X;
                                y = a * x + b;

                                if (double.IsNaN(y))
                                    temp = true;


                            }
                            else
                            {
                                if ((list[(i - 1 + list.Count) % list.Count].X - list[i].X) == 0)
                                {
                                    a = list[(i + 1) % list.Count].a_prev;
                                    b = -a * list[(i + 1) % list.Count].X + list[(i + 1) % list.Count].Y;
                                    x = list[i].X;
                                    y = a * x + b;
                                    if (double.IsNaN(y))
                                        temp = true;
                                }
                                else
                                {
                                    a = list[i].a_next;
                                    b = -a * list[(i + 1) % list.Count].X + list[(i + 1) % list.Count].Y;
                                    a_next = ((list[(i - 1 + list.Count) % list.Count].Y - list[i].Y) / (list[(i - 1 + list.Count) % list.Count].X - list[i].X));
                                    b_next = -a_next * list[(i - 1 + list.Count) % list.Count].X + list[(i - 1 + list.Count) % list.Count].Y;
                                    x = (b - b_next) / (a_next - a);
                                    y = a_next * x + b_next;
                                    if (double.IsNaN(y))
                                        temp = true;

                                }
                            }
                        

                        if(temp ==true)
                        {
                            if (list[i].X == beforeChanges[(i + 1) % list.Count].X)
                                y = list[i].Y;
                                x= list[(i + 1) % list.Count].X;
                             
                        }
                        
                        list[i].X = x;
                        list[i].Y = y;
                        verticesChecked[i] = true;

                        if (list[i].prevVertex == TypeOfEdge.armOfAngle)
                        {
                            stop1 = true;
                        }
                        else
                        {
                            i = (i - 1 + list.Count) % list.Count;
                        }
                    }

                }
                
                if (stop2 == false)
                {
                    if (list[j].prevVertex == TypeOfEdge.horizontal)
                    {
                        
                        list[j].Y = list[(j - 1 + list.Count) % list.Count].Y;
                        verticesChecked[j] = true;
                        j = (j + 1) % list.Count;
                    }
                    else if (list[j].prevVertex == TypeOfEdge.vertical)
                    {

                        list[j].X = list[(j - 1 + list.Count) % list.Count].X;
                        verticesChecked[j] = true;
                        j = (j + 1) % list.Count;
                    }
                    else if (list[j].prevVertex == TypeOfEdge.armOfAngle)
                    {
                        
                        double a;
                        double b;
                        double a_next;
                        double b_next;
                        double x, y;
                        bool temp = false;
                        
                            if ((beforeChanges[(j - 1 + list.Count) % list.Count].X - list[j].X) == 0)
                            {
                                a = list[(j - 1 + list.Count) % list.Count].a_next;
                                b = -a * list[(j - 1 + list.Count) % list.Count].X + list[(j - 1 + list.Count) % list.Count].Y;
                                x = list[j].X;
                                y = a * x + b;

                                if (double.IsNaN(y))
                                    temp = true;


                            }
                            else
                            {
                                if ((list[(j + 1) % list.Count].X - list[j].X) == 0)
                                {
                                    a = list[(j - 1 + list.Count) % list.Count].a_next;
                                    b = -a * list[(j - 1 + list.Count) % list.Count].X + list[(j - 1 + list.Count) % list.Count].Y;
                                    x = list[j].X;
                                    y = a * x + b;
                                    if (double.IsNaN(y))
                                        temp = true;
                                }
                                else
                                {
                                    a = list[j].a_prev;
                                    b = -a * list[(j - 1 + list.Count) % list.Count].X + list[(j - 1 + list.Count) % list.Count].Y;
                                    a_next = ((list[(j + 1) % list.Count].Y - list[j].Y) / (list[(j + 1) % list.Count].X - list[j].X));
                                    b_next = -a_next * list[(j + 1) % list.Count].X + list[(j + 1) % list.Count].Y;
                                    x = (b - b_next) / (a_next - a);
                                    y = a_next * x + b_next;

                                    if (double.IsNaN(y))
                                        temp = true;

                                }
                            }
                        
                        if(temp == true)
                        {
                            if (list[j].X == beforeChanges[(j - 1+list.Count) % list.Count].X)
                                y = list[j].Y;
                                x = list[(j - 1 + list.Count) % list.Count].X;
                        }
                        
                        list[j].X = x;
                        list[j].Y = y;
                        verticesChecked[j] = true;
                        
                        if (list[j].nextVertex == TypeOfEdge.armOfAngle)
                        {
                            stop2 = true;
                        }
                        else
                        {
                            j = (j + 1) % list.Count;
                        }
                    }
                }
                
            }
        }
        /// <summary>
        /// Dodaje na danej krawędzi punkt.
        /// </summary>
        /// <param name="edge">Krawędz wielokąta</param>
        /// <param name="point">Miejsce dodania punktu</param>
        public void AddBetween((Vertex p1, Vertex p2) edge,Point point)
        {

            int p = list.IndexOf(edge.p1);
            if (p != -1)
            {
                
                list[(list.IndexOf(edge.p1) + 1) % list.Count].prevVertex = TypeOfEdge.none;
                list[(list.IndexOf(edge.p1) - 1+ list.Count) % list.Count].nextVertex = TypeOfEdge.none;
                edge.p1.nextVertex = TypeOfEdge.none;
                edge.p2.prevVertex = TypeOfEdge.none;

                if(list[(list.IndexOf(edge.p1) + 1) % list.Count].isAngleSetInVertex)
                {
                    list[(list.IndexOf(edge.p1) + 1) % list.Count].nextVertex = TypeOfEdge.none;
                    list[(list.IndexOf(edge.p1) + 2) % list.Count].prevVertex = TypeOfEdge.none;
                    list[(list.IndexOf(edge.p1) + 1) % list.Count].isAngleSetInVertex = false;
                }
                               
                if (list[(list.IndexOf(edge.p1)) % list.Count].isAngleSetInVertex)
                {
                    list[(list.IndexOf(edge.p1)) % list.Count].prevVertex = TypeOfEdge.none;
                    list[(list.IndexOf(edge.p1) - 1 + list.Count) % list.Count].nextVertex = TypeOfEdge.none;
                    list[(list.IndexOf(edge.p1)) % list.Count].isAngleSetInVertex = false;
                }
                list.Insert(((p + 1 + list.Count) % list.Count), new Vertex(point.X, point.Y));

            }
            

        }
        /// <summary>
        /// Usuwa wiercholek.
        /// </summary>
        /// <param name="point">Wierchołek do usunięcia</param>
        public void Remove(Vertex point)
        {
            if (list.Count > 3)
            {
                Vertex prev = list[(list.IndexOf(point) - 1 + list.Count) % list.Count];
                Vertex next = list[(list.IndexOf(point) + 1) % list.Count];
                if(prev.isAngleSetInVertex)
                {
                    makeVertexNormal(prev);
                }
                if(next.isAngleSetInVertex)
                {
                    makeVertexNormal(next);
                }
                next.prevVertex = TypeOfEdge.none;
                prev.nextVertex = TypeOfEdge.none;
                list.Remove(point);
            }
            
        }
        /// <summary>
        ///  Czy wielokąt jest zamknięty.
        /// </summary>
        /// <returns>Czy wielokąt jest zamknięty.</returns>
        public bool isClosed()
        {
            return isPolygonClosed;
        }
        /// <summary>
        /// Rysuje wielokąt z biblioteczną funkcja DrawLine.
        /// </summary>
        /// <param name="g">Grafika</param>
        /// <param name="vertex">Zanaczony wierchołek</param>
        /// <param name="edge">Zaznaczona krawędź</param>
        public void Draw(Graphics g,Vertex vertex, (Vertex p1, Vertex p2)edge)
        {

            Pen pen = new Pen(Color.Black);
            SolidBrush brush = new SolidBrush(Color.Black);
            Pen pen2 = new Pen(Color.Red);
            SolidBrush brush2 = new SolidBrush(Color.Red);
            
            for (int i = 0; i <list.Count; i++)
            {
                g.FillEllipse(brush, (int)(list[i].X - radius / 2), (int)(list[i].Y - radius / 2), (int)radius, (int)radius);
                if (i > 0)
                {
                    g.DrawLine(pen,(int) list[i - 1].X, (int)list[i - 1].Y, (int)list[i].X, (int)list[i].Y);
                    if (list[i - 1].nextVertex == TypeOfEdge.horizontal)
                    {
                        Image img = Resources.Horizontal;
                        g.DrawImage(img, (int)((list[i].X + list[i - 1].X) / 2 -10d), (int)(list[i].Y+10d), 20, 20);
                    }
                    else  if (list[i - 1].nextVertex == TypeOfEdge.vertical)
                    {
                            Image img = Resources.Vertical;
                            g.DrawImage(img, (int)list[i].X + 10,(int)((list[i].Y + list[i - 1].Y) / 2 - 10), 20, 20);
                    }                    
                }

                if(list[i].isAngleSetInVertex)
                {
                    Vertex next = list[(i + 1) % list.Count];
                    Vertex prev = list[(i - 1+list.Count) % list.Count];
                    (double v,double u) e1=((next.X-list[i].X)/Sqrt((next.X - list[i].X)* (next.X - list[i].X)+ (next.Y - list[i].Y)*(next.Y - list[i].Y)),(next.Y-list[i].Y)/Sqrt((next.X - list[i].X) * (next.X - list[i].X) + (next.Y - list[i].Y) * (next.Y - list[i].Y)));
                    (double v, double u) e2 = ((prev.X - list[i].X) / Sqrt((prev.X - list[i].X) * (prev.X - list[i].X) + (prev.Y - list[i].Y) * (prev.Y - list[i].Y)), (prev.Y - list[i].Y) / Sqrt((prev.X - list[i].X) * (prev.X - list[i].X) + (prev.Y - list[i].Y) * (prev.Y - list[i].Y)));
                    double l1 = giveEdgeLength((list[i], next))/4;
                    double l2 = giveEdgeLength((prev,list[i])) / 4;
                    double lt;
                    lt=l1<l2?l1: l2;
                    Point[] point = new Point[3];
                    point[0].X=  (int)(list[i].X + lt * e1.v);
                    point[0].Y = (int)(list[i].Y + lt * e1.u);
                    point[2].X = (int)(list[i].X + lt * e2.v);
                    point[2].Y = (int)(list[i].Y + lt * e2.u);
                    (double v, double u) e3 = (e1.v + e2.v, e1.u + e2.u);
                    if (!giveHull().Exists(x=>x.X==list[i].X&&x.Y==list[i].Y))
                    {
                        e3.u *= -1;
                        e3.v *= -1;
                    }
                    point[1].X = (int)(list[i].X + 0.60*lt * e3.v);
                    point[1].Y = (int)(list[i].Y + 0.60*lt * e3.u);
                    try
                    {
                        g.DrawString(((int)getAngleInVertex(list[i])).ToString() + "°", new Font("Arial", 10), brush, (int)(list[i].X + 0.25 * lt * e3.v) - 10, (int)(list[i].Y + 0.25 * lt * e3.u) - 10);
                        g.DrawCurve(pen, point);
                    }
                    catch(Exception e)
                    {

                    }
                }
            }

            if (isPolygonClosed && list.Count != 0)
            {
                g.DrawLine(pen, (int)list[0].X, (int)list[0].Y, (int)list[list.Count - 1].X, (int)list[list.Count - 1].Y);

                if (list[0].prevVertex == TypeOfEdge.horizontal)
                {

                    Image img = Resources.Horizontal;
                    g.DrawImage(img, (int)((list[0].X+list[list.Count-1].X)/2-10), (int)(list[0].Y+10),20,20);
                }
                else if (list[0].prevVertex == TypeOfEdge.vertical)
                {
                    Image img = Resources.Vertical;
                    g.DrawImage(img, (int)(list[0].X + 10), (int)((list[0].Y + list[list.Count-1].Y) / 2 - 10), 20, 20);

                }
            }
            try
            {
                if (vertex != null) g.FillEllipse(brush2, (int)(vertex.X - radius / 2), (int)(vertex.Y - radius / 2), (int)radius, (int)radius);
                if (edge.p1 != null && edge.p2 != null) g.DrawLine(pen2, (int)edge.p1.X, (int)edge.p1.Y, (int)edge.p2.X, (int)edge.p2.Y);
            }
            catch(Exception e)
            {

            }

            pen.Dispose();
            pen2.Dispose();
            brush.Dispose();
            brush2.Dispose();
        }
        /// <summary>
        /// Rysuje wielokąt za pomoca algorytmu Bersenhama rysowania lini.
        /// </summary>
        /// <param name="g">Grafika</param>
        /// <param name="vertex">Zanaczony wierchołek</param>
        /// <param name="edge">Zaznaczona krawędź</param>
        public void DrawBersenham(PictureBox pictureBox, Graphics g,Vertex vertex,(Vertex p1,Vertex p2)edge)
        {

            Pen pen = new Pen(Color.Black);
            SolidBrush brush = new SolidBrush(Color.Black);
            Pen pen2 = new Pen(Color.Red);
            SolidBrush brush2 = new SolidBrush(Color.Red);
            Bitmap bitmap = new Bitmap(pictureBox.Width,pictureBox.Height);
            
            for(int i=1;i<list.Count;i++)
            {
                DrawLineBersenham.DrawLine(bitmap, (int)list[i - 1].X, (int)list[i - 1].Y, (int)list[i].X, (int)list[i].Y);
            }
            if (isPolygonClosed && list.Count != 0)
            {
                DrawLineBersenham.DrawLine(bitmap, (int)list[0].X, (int)list[0].Y, (int)list[list.Count - 1].X, (int)list[list.Count - 1].Y);
            }
            
            g.DrawImage((Image)bitmap,0,0);

            for (int i = 0; i < list.Count; i++)
            {
                g.FillEllipse(brush, (int)(list[i].X - radius / 2), (int)(list[i].Y - radius / 2), (int)radius, (int)radius);
                if (i > 0)
                {

                    if (list[i - 1].nextVertex == TypeOfEdge.horizontal)
                    {
                        Image img = Resources.Horizontal;
                        g.DrawImage(img, (int)((list[i].X + list[i - 1].X) / 2 - 10d), (int)(list[i].Y + 10d), 20, 20);

                    }
                    else if (list[i - 1].nextVertex == TypeOfEdge.vertical)
                    {
                        Image img = Resources.Vertical;
                        g.DrawImage(img, (int)list[i].X + 10, (int)((list[i].Y + list[i - 1].Y) / 2 - 10), 20, 20);

                    }



                }

                if (list[i].isAngleSetInVertex)
                {
                    Vertex next = list[(i + 1) % list.Count];
                    Vertex prev = list[(i - 1 + list.Count) % list.Count];
                    (double v, double u) e1 = ((next.X - list[i].X) / Sqrt((next.X - list[i].X) * (next.X - list[i].X) + (next.Y - list[i].Y) * (next.Y - list[i].Y)), (next.Y - list[i].Y) / Sqrt((next.X - list[i].X) * (next.X - list[i].X) + (next.Y - list[i].Y) * (next.Y - list[i].Y)));
                    (double v, double u) e2 = ((prev.X - list[i].X) / Sqrt((prev.X - list[i].X) * (prev.X - list[i].X) + (prev.Y - list[i].Y) * (prev.Y - list[i].Y)), (prev.Y - list[i].Y) / Sqrt((prev.X - list[i].X) * (prev.X - list[i].X) + (prev.Y - list[i].Y) * (prev.Y - list[i].Y)));
                    double l1 = giveEdgeLength((list[i], next)) / 4;
                    double l2 = giveEdgeLength((prev, list[i])) / 4;
                    double lt;
                    lt = l1 < l2 ? l1 : l2;
                    Point[] point = new Point[3];
                    point[0].X = (int)(list[i].X + lt * e1.v);
                    point[0].Y = (int)(list[i].Y + lt * e1.u);
                    point[2].X = (int)(list[i].X + lt * e2.v);
                    point[2].Y = (int)(list[i].Y + lt * e2.u);
                    (double v, double u) e3 = (e1.v + e2.v, e1.u + e2.u);
                    if (!giveHull().Exists(x => x.X == list[i].X && x.Y == list[i].Y))
                    {
                        e3.u *= -1;
                        e3.v *= -1;
                    }
                    point[1].X = (int)(list[i].X + 0.60 * lt * e3.v);
                    point[1].Y = (int)(list[i].Y + 0.60 * lt * e3.u);
                    g.DrawString(((int)getAngleInVertex(list[i])).ToString() + "°", new Font("Arial", 10), brush, (int)(list[i].X + 0.25 * lt * e3.v) - 10, (int)(list[i].Y + 0.25 * lt * e3.u) - 10);
                    g.DrawCurve(pen, point);
                }
            }

            if (isPolygonClosed && list.Count != 0)
            {
                g.DrawLine(pen, (int)list[0].X, (int)list[0].Y, (int)list[list.Count - 1].X, (int)list[list.Count - 1].Y);

                if (list[0].prevVertex == TypeOfEdge.horizontal)
                {

                    Image img = Resources.Horizontal;
                    g.DrawImage(img, (int)((list[0].X + list[list.Count - 1].X) / 2 - 10), (int)(list[0].Y + 10), 20, 20);
                }
                else if (list[0].prevVertex == TypeOfEdge.vertical)
                {
                    Image img = Resources.Vertical;
                    g.DrawImage(img, (int)(list[0].X + 10), (int)((list[0].Y + list[list.Count - 1].Y) / 2 - 10), 20, 20);

                }
            }
            if (vertex != null) g.FillEllipse(brush2, (int)(vertex.X - radius / 2), (int)(vertex.Y - radius / 2), (int)radius, (int)radius);
            if (edge.p1 != null && edge.p2 != null) g.DrawLine(pen2, (int)edge.p1.X, (int)edge.p1.Y, (int)edge.p2.X, (int)edge.p2.Y);

            pen.Dispose();
            brush.Dispose();
            pen2.Dispose();
            brush2.Dispose();
        }
        /// <summary>
        /// Sprawdza czy punkt jest w obrebie wierzchołka.
        /// </summary>
        /// <param name="click">Punkt kliknięcia</param>
        /// <param name="point">Wierchołek</param>
        /// <returns></returns>
        private bool isPointInVertexRadius(Vertex click, Vertex point)
        {
            if (Math.Sqrt((point.X - click.X) * (point.X - click.X) + (point.Y - click.Y) * (point.Y - click.Y)) < radius)
                return true;
            return false;
        }
        /// <summary>
        /// Zwraca klikniety wierchołek.
        /// </summary>
        /// <param name="point">Punkt kliknięcia</param>
        /// <returns>Zwraca wierchołek kliknięty lub null</returns>
        public Vertex getClickedVertex(Point point)
        {
            Vertex vertex = new Vertex(point.X,point.Y);
            foreach(var v in list)
            {
                if (isPointInVertexRadius(vertex, v))
                    return v;
            }
            return null;
        }
        /// <summary>
        /// Zwraca kliknięta krawędź.
        /// </summary>
        /// <param name="point">Punkt kliknięcia</param>
        /// <returns>Zwraca kliknięta krawedź lub null</returns>
        public (Vertex p1, Vertex p2) getClickedEdge(Point point)
        {
            for(int i=0;i<list.Count;i++)
            {
                if(list[i].X== list[(i + 1) % list.Count].X)
                {
                    if(Math.Abs(list[i].X-point.X)<range && Min(list[i].Y, list[(i + 1) % list.Count].Y)<point.Y && Max(list[i].Y, list[(i + 1) % list.Count].Y)>point.Y)
                        return (list[i], list[(i + 1) % list.Count]);
                }
                else if (isPointInRange(list[i],list[(i+1)%list.Count],point))
                {
                    return (list[i], list[(i + 1) % list.Count]);
                }
            }
            return (null,null);
        }
        /// <summary>
        /// Sprawdza czy punkt jest w zasiegu krawedzi.
        /// </summary>
        /// <param name="p1">Punkt poczatkowy krawedzi</param>
        /// <param name="p2">Punkt koncowy krawędzi</param>
        /// <param name="point">Miejsce klikniecia</param>
        /// <returns>Czy punkt znajduje sie w zasiegu prostej</returns>
        public static bool isPointInRange(Vertex p1,Vertex p2,Point point)
        {

            if (((Abs((p2.Y - p1.Y) * point.X - (p2.X - p1.X) * point.Y - (p2.Y - p1.Y) * p1.X + (p2.X - p1.X) * p1.Y) / (Math.Sqrt(Math.Pow((p2.Y - p1.Y), 2) + Math.Pow(-(p2.X - p1.X), 2)))) <= range)&&OnRectangle(p1,p2,point))
                return true;
            return false;
        }
        /// <summary>
        /// Sprawdza czy punkt znajduje sie w prostokącie wyznaczonym przez wierchołki.
        /// </summary>
        /// <param name="p1">Wierchołek </param>
        /// <param name="p2">Wierchołek</param>
        /// <param name="p">Miejsce klikniecia</param>
        /// <returns></returns>
        public static bool OnRectangle(Vertex p1, Vertex p2,Point p)
        {
            return Min(p1.X, p2.X) <= p.X && p.X <= Max(p1.X, p2.X) && Min(p1.Y, p2.Y) <= p.Y && Min(p1.Y, p2.Y) <= p.Y;
        }
        /// <summary>
        /// Oblicza kat w wierchołku
        /// </summary>
        /// <param name="vertex">Wierchołek</param>
        /// <returns>Kąt w wierchołku</returns>
        public double getAngleInVertex(Vertex vertex)
        {
            Vertex next = list[(list.IndexOf(vertex) + 1) % list.Count];
            Vertex prev = list[(list.IndexOf(vertex) - 1 + list.Count) % list.Count];
            double x, y,angle;
            double v1 = (next.X - vertex.X);
            double u1 = (next.Y - vertex.Y);
            double v2 = (prev.X - vertex.X);
            double u2 = (prev.Y - vertex.Y);

            x = (next.X + prev.X) / 2;
            y = (prev.Y + next.X) / 2;

           if(giveHull().Exists(v=>(v.X==vertex.X)&&(v.Y==vertex.Y)))
            {
                angle = Acos((v1 * v2 + u1 * u2) / (Sqrt(v1 * v1 + u1 * u1) * Sqrt(v2 * v2 + u2 * u2))) * 360 / (2 * PI);
            }
            else
            {
                angle=360 - Acos((v1 * v2 + u1 * u2) / (Sqrt(v1 * v1 + u1 * u1) * Sqrt(v2 * v2 + u2 * u2))) * 360 / (2 * PI);
            }
            return angle;
        }
        /// <summary>
        /// Wyznacza otoczke wypukłą z punktow wielokąta.
        /// Potrzebne by ustalić czy kat jest powyżej 180 stopni;
        /// </summary>
        /// <returns>Otoczka wypukła</returns>
        private List<Vertex> giveHull()
            
        {
            List<Vertex> hull = new List<Vertex>();
            List<Vertex> sortedVertex = new List<Vertex>();
            for (int k = 0; k < list.Count; k++)
            {
                sortedVertex.Add(list[k].Clone());
            }
            Vertex pointOnHull = list.Where(p => p.X == sortedVertex.Min(min => min.X)).First();
            int a = list.IndexOf(pointOnHull);
            int orientation(Vertex p1, Vertex p2, Vertex p)
            {
                
                return Sign((p2.Y - p1.Y) * (p.X - p2.X) - (p2.X - p1.X) * (p.Y - p2.Y));
            }
            int b;
            
            do
            {
                hull.Add(list[a]);
                if(hull.Count>list.Count)
                {
                    hull.Clear();
                    break;
                }
                b = (a + 1) % list.Count;
                for (int i = 0; i < sortedVertex.Count; i++)
                {
                    if (orientation(list[a], list[i], list[b]) == -1)
                    {
                        b = i;
                    }
                }

                a = b;

            }
            while (a!=list.IndexOf(pointOnHull));

            return hull;
       
            
        }
        /// <summary>
        ///Zwraca  długość krawędzi.
        /// </summary>
        /// <param name="edge">Krawędź</param>
        /// <returns>Długość krawedzi</returns>
        public double giveEdgeLength((Vertex p1, Vertex p2)edge)
        {
            return Sqrt((edge.p1.X - edge.p2.X) * (edge.p1.X - edge.p2.X) + (edge.p1.Y - edge.p2.Y) * (edge.p1.Y - edge.p2.Y));
        }
        /// <summary>
        /// Zwraca pozycje wierchołka na liście.
        /// </summary>
        /// <param name="vertex">Wierchołek</param>
        /// <returns>Pozycja</returns>
        public int getNumberOfvertex(Vertex vertex)
        {
            return list.IndexOf(vertex);
        }
    }
}
