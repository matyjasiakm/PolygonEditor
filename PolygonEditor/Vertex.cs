using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace Grafika_Komputerowa_1_ver1
{
    ///<summary>
    ///TypeOfEdge okresla typ krawędzi czy jest ona pozioma, pionowa, jest częścią tworzonego kąta lub jest zwyczajna krawedzią bez ograniczenia.
    ///</summary>
    public enum TypeOfEdge{
        none,
        vertical,
        horizontal,
        armOfAngle
        
    };
    /// <summary>
    /// Klasa Vertex to klasa wierchołka wielokata.
    /// </summary>
    /// <param name="X">Współrzędna x wierchołka</param> 
    /// <param name="Y">Wspólrzędna y wierchołka</param>
    /// <param name="a_next">Wspólczynnik kierunkowy prostej tworzonej przez ten wierchołek i wierchołek nastepny </param>
    /// <param name="a_prev">Wspólczynnik kierunkowy prostej tworzonej przez ten wierchołek i wierchołek poprzedni</param>
    public class Vertex
    {
        
        public double X { get; set; }
        public double Y { get; set; }
        public double a_next { get; set; }
        public double a_prev { get; set; }       
        public bool isAngleSetInVertex { get; set; }
        public TypeOfEdge prevVertex { get; set; }
        public TypeOfEdge nextVertex { get; set; }
        
        
       
        public Vertex(double x, double y)
        {
            
            X = x;
            Y = y;
            
            

        }
        Vertex(double x, double y , TypeOfEdge prev, TypeOfEdge next, bool isAngleInVertex,double a_next,double a_prev)
        {
            this.X = x;
            this.Y = y;
            this.prevVertex = prev;
            nextVertex = next;
            this.a_next = a_next;
            this.a_prev = a_prev;
            
            this.isAngleSetInVertex = isAngleSetInVertex;

        }
        public Vertex Clone()
        {
            return new Vertex(X, Y, prevVertex, nextVertex,isAngleSetInVertex,a_next,a_prev);
        }

    }
}
