using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenGL;
using System.Runtime.InteropServices; 

namespace myOpenGL
{
    public partial class Form1 : Form
    {
        cOGL cGL;

        public Form1()
        {

            InitializeComponent();
            cGL = new cOGL(panel1);
            //apply the bars values as cGL.ScrollValue[..] properties 
                                         //!!!
            hScrollBarScroll(hScrollBar1, null);
            hScrollBarScroll(hScrollBar2, null);
            hScrollBarScroll(hScrollBar3, null);
            hScrollBarScroll(hScrollBar4, null);
            hScrollBarScroll(hScrollBar5, null);
            hScrollBarScroll(hScrollBar6, null);
            hScrollBarScroll(hScrollBar7, null);
            hScrollBarScroll(hScrollBar8, null);
            hScrollBarScroll(hScrollBar9, null);
            
           


            timer2.Start();
        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            cGL.Draw();
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
           // cGL.OnResize();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.BringToFront();
            this.Focus();
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
        }

        private void hScrollBarScroll(object sender, ScrollEventArgs e)
        {
            cGL.intOptionC = 0;
            HScrollBar hb = (HScrollBar)sender;
            int n = int.Parse(hb.Name.Substring(hb.Name.Length - 1));
            cGL.ScrollValue[n - 1] = (hb.Value - 100) / 10.0f;
            if (e != null)
                cGL.Draw();
        }

        public float[] oldPos = new float[7];

        private void numericUpDownValueChanged(object sender, EventArgs e)
        {
            NumericUpDown nUD = (NumericUpDown)sender;
            int i = int.Parse(nUD.Name.Substring(nUD.Name.Length - 1));
            int pos = (int)nUD.Value; 
            switch(i)
            {
                case 1:
                    if (pos > oldPos[i - 1])
                    {
                        cGL.xShift += 0.25f;
                        cGL.intOptionC = 4;
                    }
                    else
                    {
                        cGL.xShift -= 0.25f;
                        cGL.intOptionC = -4;
                    }
                    break;
                case 2:
                    if (pos > oldPos[i - 1])
                    {
                        cGL.yShift += 0.25f;
                        cGL.intOptionC = 5;
                    }
                    else
                    {
                        cGL.yShift -= 0.25f;
                        cGL.intOptionC = -5;
                    }
                    break;
                case 3:
                    if (pos > oldPos[i - 1])
                    {
                        cGL.zShift += 0.25f;
                        cGL.intOptionC = 6;
                    }
                    else
                    {
                        cGL.zShift -= 0.25f;
                        cGL.intOptionC = -6;
                    }
                    break;
                case 4:
                    if (pos > oldPos[i - 1])
                    {
                        cGL.xAngle += 5;
                        cGL.intOptionC = 1;
                        Console.WriteLine("4");
                    }
                    else
                    {
                        cGL.xAngle -= 5;
                        cGL.intOptionC = -1;
                       
                    }
                    break;
                case 5:
                    if (pos > oldPos[i - 1])
                    {
                        cGL.yAngle += 5;
                        cGL.intOptionC = 2;
                      
                    }
                    else
                    {
                        cGL.yAngle -= 5;
                        cGL.intOptionC = -2;
                    }
                    break;
                case 6: 
	                if (pos>oldPos[i-1]) 
	                {
		                cGL.zAngle+=5;
		                cGL.intOptionC=3;
                      
	                }
	                else
	                {
                        cGL.zAngle -= 5;
                        cGL.intOptionC = -3;
                    }
                    break;
            }
            cGL.Draw();
            oldPos[i - 1] = pos;
            cGL.intOptionC = 0;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            cGL.trans += 0.03f;

            cGL.right_upper_ARM_yangle = 10 - Math.Abs(70.0f * (float)Math.Cos(cGL.trans));
            cGL.left_upper_ARM_yangle = 10 - Math.Abs(70.0f * (float)Math.Sin(cGL.trans));
            cGL.left_upper_LEG_angle = 40 - Math.Abs(80.0f * (float)Math.Sin(cGL.trans));
            cGL.left_lower_LEG_angle = -Math.Abs(30.0f * (float)Math.Sin(cGL.trans));
            cGL.right_upper_LEG_angle = 40 - Math.Abs(80.0f * (float)Math.Cos(cGL.trans));
            cGL.right_lower_LEG_angle = -Math.Abs(30.0f * (float)Math.Cos(cGL.trans));
            cGL.updn = -Math.Abs(0.1f * (float)Math.Cos(2 * cGL.trans));
            cGL.hghg += 0.05f;
            if (cGL.hghg > 4.0f) cGL.hghg = 0;
            
            
            cGL.Draw();
        }


      
  
        
      
       
       
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            cGL.ballZMovement += 0.05f;   
            cGL.ballZtranslate += 5f;
            switch(e.KeyCode)
            {

                case Keys.A:
                    {
                        cGL.alfa += 5;
                        cGL.Draw();
                        break;
                    }
                case Keys.D:
                    {
                        cGL.alfa -= 5;
                        cGL.Draw();
                        break;
                    }
                case Keys.W:
                    {
                        cGL.trans += 0.08f;

                        cGL.right_upper_ARM_yangle = 30 - Math.Abs(70.0f * (float)Math.Cos(cGL.trans));
                        cGL.left_upper_ARM_yangle = 30 - Math.Abs(70.0f * (float)Math.Sin(cGL.trans));
                        cGL.left_upper_LEG_angle = 40 - Math.Abs(80.0f * (float)Math.Sin(cGL.trans));
                        cGL.left_lower_LEG_angle = -Math.Abs(30.0f * (float)Math.Sin(cGL.trans));
                        cGL.right_upper_LEG_angle = 40 - Math.Abs(80.0f * (float)Math.Cos(cGL.trans));
                        cGL.right_lower_LEG_angle = -Math.Abs(30.0f * (float)Math.Cos(cGL.trans));
                        cGL.updn = -Math.Abs(0.1f * (float)Math.Cos(2 * cGL.trans));
                        cGL.right_lower_ARM_angle = 90;
                        cGL.left_lower_ARM_angle = 90;

                        cGL.xExisOrigin += 0.2 * Math.Round((float)Math.Cos(cGL.alfa * ((float)Math.PI / (float)180.0f)), 2);
                        cGL.yExisOrigin += 0.2 * Math.Round((float)Math.Sin(cGL.alfa * ((float)Math.PI / (float)180.0f)), 2);
                        cGL.xExis += 0.2 * Math.Round((float)Math.Cos(cGL.alfa * ((float)Math.PI / (float)180.0f)), 2);
                        cGL.yExis += 0.2 * Math.Round((float)Math.Sin(cGL.alfa * ((float)Math.PI / (float)180.0f)), 2);

                        if (cGL.xExis > 4.0f || cGL.xExis < -4.0f) cGL.xExis = 0;
                        if (cGL.yExis > 4.0f || cGL.yExis < -4.0f) cGL.yExis = 0;
                        
                        cGL.Draw();
                        break;
                    }
                case Keys.Space:
                    {
                        int counter = 0;
                        while (counter < 1)
                        {
                            
                            cGL.trans += 0.03f;
                            cGL.bodyAngle = -30 + Math.Abs(25f * (float)Math.Cos(cGL.trans));
                            cGL.right_lower_ARM_angle = 0;
                            cGL.left_lower_ARM_angle = 0;
                            cGL.right_upper_ARM_yangle = -15 + Math.Abs(175.0f * (float)Math.Cos(cGL.trans));
                            cGL.left_upper_ARM_yangle = -15 + Math.Abs(175.0f * (float)Math.Cos(cGL.trans));
                            cGL.left_upper_LEG_angle = 80 - Math.Abs(80.0f * (float)Math.Cos(cGL.trans));
                            cGL.left_lower_LEG_angle = -90 + Math.Abs(80.0f * (float)Math.Cos(cGL.trans));
                            cGL.right_upper_LEG_angle = 80 - Math.Abs(80.0f * (float)Math.Cos(cGL.trans));
                            cGL.right_lower_LEG_angle = -90 + Math.Abs(80.0f * (float)Math.Cos(cGL.trans));
                            cGL.updn = 1.5f - Math.Abs(4f * (float)Math.Cos(cGL.trans));
                            cGL.Draw();
                            if (cGL.bodyAngle < -29.3)
                                counter++;
                        }
                        cGL.straightRobotBody();
                        cGL.Draw();
                        break;

                    }
                   
            }
        }

      

        private void timer2_Tick(object sender, EventArgs e)
        {
            
            cGL.ballZMovement += 0.05f;               
            cGL.ballZtranslate += 5f;
            cGL.Draw();
        }

 

        private void hScrollBar11_Scroll(object sender, ScrollEventArgs e)
        {
            
            cGL.ScrollValue[11] = e.NewValue;
            Console.WriteLine(cGL.ScrollValue[11]);
            cGL.Draw();
        }

        private void hScrollBar12_Scroll(object sender, ScrollEventArgs e)
        {
            cGL.ScrollValue[12] = e.NewValue;
            Console.WriteLine(cGL.ScrollValue[12]);
            cGL.Draw();
        }

        private void hScrollBar13_Scroll(object sender, ScrollEventArgs e)
        {
            cGL.ScrollValue[13] = e.NewValue;
            Console.WriteLine(cGL.ScrollValue[13]);
            cGL.Draw();
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }



       

        
        

    }
}