using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;



namespace GK1
{
    public partial class Form1 : Form
    {
        
        List<Vertex> vertexslist;
        
        Vertex activeVertex;
        (Vertex p1, Vertex p2) edge;
        Polygon polygon;
        Point whereClicked;
        public Form1()
        {
            InitializeComponent();
            vertexslist = new List<Vertex>();
            activeVertex = null;
            polygon = new Polygon();
            edge = (null, null);
            whereClicked = new Point(-1,-1);
            radioButton1.Checked = true;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            polygon = new Polygon();
            Vertex v1 = new Vertex(100, 300);
            Vertex v2 = new Vertex(200, 200);
            Vertex v3 = new Vertex(400, 200);
            Vertex v4 = new Vertex(400, 400);
            Vertex v5 = new Vertex(200, 400);
            polygon.Add(v1);
            polygon.Add(v2);
            polygon.Add(v3);
            polygon.Add(v4);
            polygon.Add(v5);
            polygon.setAngleinVertex(v1, (int)polygon.getAngleInVertex(v1));
            polygon.makeEdgeHorizontal((v2, v3));
            polygon.CorrectFromVertex(v2, new Point((int)v2.X,(int)v2.Y));
            polygon.makeEdgeHorizontal((v4, v5));
            polygon.CorrectFromVertex(v4, new Point((int)v4.X, (int)v4.Y));
            polygon.makeEdgeVertical((v3, v4));
            polygon.CorrectFromVertex(v3, new Point((int)v3.X, (int)v3.Y));
            polygon.isPolygonClosed = true;
            pictureBox1.Refresh();
        }



        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right:
                    {
                        if (!polygon.isClosed()) break;
                        Point pt = pictureBox1.PointToScreen(e.Location);
                        whereClicked = new Point(e.X, e.Y);
                        activeVertex = polygon.getClickedVertex(whereClicked);
                        edge = polygon.getClickedEdge(whereClicked);
                        contextMenuStrip1.Items["RemoveVertex"].Enabled = true;
                        if (activeVertex != null)
                        {

                            edge.p1 = null;
                            edge.p2 = null;
                        }
                        pictureBox1.Refresh();
                        if (activeVertex != null)
                        {
                            if (polygon.Count() <= 3)
                            
                            {
                                contextMenuStrip1.Items["RemoveVertex"].Enabled = false;



                            }
                            ((ToolStripMenuItem)contextMenuStrip1.Items["setAngle"]).Checked = false;
                            contextMenuStrip1.Items["setAngle"].Enabled = true;

                            if (activeVertex.isAngleSetInVertex)
                                ((ToolStripMenuItem)contextMenuStrip1.Items["setAngle"]).Checked = true;
                            else if (activeVertex.nextVertex != TypeOfEdge.none || activeVertex.prevVertex!=TypeOfEdge.none)
                                    contextMenuStrip1.Items["setAngle"].Enabled = false;
                            
                            

                            contextMenuStrip1.Show(pt);
                        }
                        else if(edge.p1!=null && edge.p2!=null)
                        {
                            edgeToolStrip.Items["Horizontal"].Enabled = true;
                            edgeToolStrip.Items["Vertical"].Enabled = true;
                            ((ToolStripMenuItem)edgeToolStrip.Items["Vertical"]).Checked = false;
                            ((ToolStripMenuItem)edgeToolStrip.Items["Horizontal"]).Checked = false;
                            if (edge.p1.prevVertex == TypeOfEdge.horizontal || edge.p2.nextVertex == TypeOfEdge.horizontal)
                                edgeToolStrip.Items["Horizontal"].Enabled = false;
                            if (edge.p1.prevVertex == TypeOfEdge.vertical || edge.p2.nextVertex == TypeOfEdge.vertical)
                                edgeToolStrip.Items["Vertical"].Enabled = false;
                            if (edge.p1.nextVertex == TypeOfEdge.armOfAngle || edge.p2.prevVertex == TypeOfEdge.armOfAngle)
                            {
                                edgeToolStrip.Items["Vertical"].Enabled = false;
                                edgeToolStrip.Items["Horizontal"].Enabled = false;
                            }

                            if (edge.p1.nextVertex==TypeOfEdge.horizontal)
                            {
                                var item = (ToolStripMenuItem)edgeToolStrip.Items["Horizontal"];
                                edgeToolStrip.Items["Vertical"].Enabled = false;
                                item.Checked = true;
                            }
                            else if(edge.p1.nextVertex == TypeOfEdge.vertical)
                            {
                                var item = (ToolStripMenuItem)edgeToolStrip.Items["Vertical"];
                                edgeToolStrip.Items["Horizontal"].Enabled = false;
                                item.Checked = true;

                            }
                             if(edge.p1.nextVertex==TypeOfEdge.armOfAngle || edge.p2.prevVertex==TypeOfEdge.armOfAngle)
                            {

                            }
                            
                            edgeToolStrip.Show(pt);

                        }
                    break;
                }

                case MouseButtons.Left:
                {
                        whereClicked = new Point(e.X, e.Y);
                        if (polygon.isPolygonClosed)
                        {
                            activeVertex = polygon.getClickedVertex(whereClicked);
                            edge = polygon.getClickedEdge(whereClicked);

                            if (activeVertex != null)
                            {

                                edge.p1 = null;
                                edge.p2 = null;

                            }
                        }
                        
                        if (!polygon.isClosed())
                        polygon.Add(new Vertex(e.X, e.Y));
                    
                    pictureBox1.Refresh();
                    break;
                }
            }
        }

        
        private void setLabels()
        {
            label9.Text = "";
            label11.Text = "";
            label13.Text = "";
            label15.Text = "";
            label7.Text = "";
            label3.Text = "";
            label5.Text = "";
            label1.Text = "No selected edge.";
            if (activeVertex!=null)
            {
                label9.Text = "(" + (int)activeVertex.X + ", " + (int)activeVertex.Y + ")";
                label11.Text = ""+polygon.getNumberOfvertex(activeVertex);
                label13.Text = ""+(int)polygon.getAngleInVertex(activeVertex);
                label15.Text = ""+activeVertex.isAngleSetInVertex;

            }
            else if(edge.p1!=null && edge.p2!=null)
            {
                if (edge.p1.nextVertex == TypeOfEdge.horizontal)
                    label1.Text = "Horizontal edge.";
                else if (edge.p1.nextVertex == TypeOfEdge.vertical)
                    label1.Text = "Vertical edge.";
                else if (edge.p1.nextVertex == TypeOfEdge.armOfAngle)
                    label1.Text = "Arm of angle.";
                label7.Text = "" + Math.Round( polygon.giveEdgeLength(edge),2);
                label3.Text = "(" + (int)edge.p1.X + ", " + (int)edge.p1.Y + ")";
                label5.Text = "(" + (int)edge.p2.X + ", " + (int)edge.p2.Y + ")";

            }
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            setLabels();
            if (radioButton1.Checked == true)
            {
                polygon.Draw(e.Graphics,activeVertex,edge);
            }
            else if (radioButton2.Checked == true)
            {
                polygon.DrawBersenham(pictureBox1, e.Graphics,activeVertex,edge);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            polygon = new Polygon();
            activeVertex = null;
            edge = (null, null);
            pictureBox1.Refresh();
           
        }


        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {

            switch(e.Button)
            {
                case MouseButtons.Left:
                    {
                        if (polygon.isClosed())
                        {
                            activeVertex = polygon.getClickedVertex(new Point(e.X, e.Y));
                        }
                        break;
                    }
            }

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    {
                        if (activeVertex != null)
                        {
                            //activeVertex.point.X = e.X;
                            //activeVertex.point.Y = e.Y;
                            polygon.CorrectFromVertex(activeVertex,new Point(e.X,e.Y));
                            pictureBox1.Refresh();
                        }
                        break;
                    }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            switch(e.Button)
            {
                case MouseButtons.Left:
                    {
                        activeVertex = null;
                        break;
                    }
            }
            
        }

        private void removeVertexToolStripMenuItem_Click(object sender, EventArgs e)
        {

            edge = (null, null);
            polygon.Remove(activeVertex);
            activeVertex = null;
                pictureBox1.Refresh();
            
        }

        private void addVertexToolStripMenuItem_Click(object sender, EventArgs e)
        {

            polygon.AddBetween(edge, whereClicked);
            edge = (null, null);
            activeVertex = polygon.getClickedVertex(whereClicked);
            pictureBox1.Refresh();
        }

        private void horizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (edge.p1.nextVertex == TypeOfEdge.none)
            {
                polygon.makeEdgeHorizontal(edge);
                polygon.CorrectFromVertex(edge.p1, new Point((int)edge.p1.X, (int)edge.p1.Y));
            }
            else
            {
                polygon.makeEdgeNormal(edge);
            }


            pictureBox1.Refresh();
        }

        private void verticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (edge.p1.nextVertex == TypeOfEdge.none)
            {
                polygon.makeEdgeVertical(edge);
                polygon.CorrectFromVertex(edge.p1, new Point((int)edge.p1.X, (int)edge.p1.Y));
            }
            else
            {
                polygon.makeEdgeNormal(edge);
            }
            pictureBox1.Refresh();
        }

        private void setAngle_Click(object sender, EventArgs e)
        {
            if (setAngle.Checked == true)
            {
                polygon.makeVertexNormal(activeVertex);
                setAngle.Checked = false;
            }
            else
            {
                int deegre = (int)polygon.getAngleInVertex(activeVertex);
                AngleWindow angleWindow = new AngleWindow(deegre);
                angleWindow.StartPosition = FormStartPosition.CenterParent;
                angleWindow.ShowDialog();
                if (angleWindow.DialogResult == DialogResult.OK)
                {
                    deegre = angleWindow.d;
                    polygon.setAngleinVertex(activeVertex, deegre);
                    setAngle.Checked = true;
                }
                
            }
            pictureBox1.Refresh();
        }    

        private void button1_Click_1(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
        }

        
        

        
        
    }
}
