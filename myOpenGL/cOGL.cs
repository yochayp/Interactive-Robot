using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Media;
//2
using System.Drawing;

namespace OpenGL
{
    class cOGL
    {
        Control p;
        int Width;
        int Height;
        
        public struct Star
        {
            public float X;
            public float Y;
            public Boolean toDraw;

            public void setX(float x)
            {
                this.X = x;
            }
            public void setY(float y)
            {
                this.Y = y;
            }
            public void setDraw(Boolean bol)
            {
                this.toDraw = bol;
            }

        }

        
        GLUquadric obj;

        public cOGL(Control pb)
        {
            p=pb;
            Width = p.Width;
            Height = p.Height;


            ground[0, 0] = 1;
            ground[0, 1] = 1;
            ground[0, 2] = -0.5f;

            ground[1, 0] = 0;
            ground[1, 1] = 1;
            ground[1, 2] = -0.5f;

            ground[2, 0] = 1;
            ground[2, 1] = 0;
            ground[2, 2] = -0.5f;

            pos[0] = -4;
            pos[1] = -4;
            pos[2] = 15;
            pos[3] = 1;

            
            Random rnd = new Random();
            
            for (int i = 0; i < 20; i++)
            {
                arr[i].setX (rnd.Next(-50,50));
                arr[i].setY ( rnd.Next(-50,50));
                arr[i].toDraw = true;
            }

            
            InitializeGL();
            obj = GLU.gluNewQuadric(); //!!!
            
        }

        ~cOGL()
        {
            GLU.gluDeleteQuadric(obj); //!!!
            WGL.wglDeleteContext(m_uint_RC);
        }

		uint m_uint_HWND = 0;

        public uint HWND
		{
			get{ return m_uint_HWND; }
		}
		
        uint m_uint_DC   = 0;

        public uint DC
		{
			get{ return m_uint_DC;}
		}
		uint m_uint_RC   = 0;

        public uint RC
		{
			get{ return m_uint_RC; }
		}


        float[] planeCoeff = { 1, 1, 1, 1 };
        float[,] ground = new float[3, 3];//{ { 1, 1, -0.5 }, { 0, 1, -0.5 }, { 1, 0, -0.5 } };
        float[,] wall = new float[3, 3];//{ { -15, 3, 0 }, { 15, 3, 0 }, { 15, 3, 15 } };

        // Reduces a normal vector specified as a set of three coordinates,
        // to a unit normal vector of length one.
        void ReduceToUnit(float[] vector)
        {
            float length;

            // Calculate the length of the vector		
            length = (float)Math.Sqrt((vector[0] * vector[0]) +
                                (vector[1] * vector[1]) +
                                (vector[2] * vector[2]));

            // Keep the program from blowing up by providing an exceptable
            // value for vectors that may calculated too close to zero.
            if (length == 0.0f)
                length = 1.0f;

            // Dividing each element by the length will result in a
            // unit normal vector.
            vector[0] /= length;
            vector[1] /= length;
            vector[2] /= length;
        }

        const int x = 0;
        const int y = 1;
        const int z = 2;
        
        private void playSimpleSound()
        {
            SoundPlayer simpleSound = new SoundPlayer(@"c:\Windows\Media\chimes.wav");
            simpleSound.Play();
        }
        // Points p1, p2, & p3 specified in counter clock-wise order
        void calcNormal(float[,] v, float[] outp)
        {
            float[] v1 = new float[3];
            float[] v2 = new float[3];

            // Calculate two vectors from the three points
            v1[x] = v[0, x] - v[1, x];
            v1[y] = v[0, y] - v[1, y];
            v1[z] = v[0, z] - v[1, z];

            v2[x] = v[1, x] - v[2, x];
            v2[y] = v[1, y] - v[2, y];
            v2[z] = v[1, z] - v[2, z];

            // Take the cross product of the two vectors to get
            // the normal vector which will be stored in out
            outp[x] = v1[y] * v2[z] - v1[z] * v2[y];
            outp[y] = v1[z] * v2[x] - v1[x] * v2[z];
            outp[z] = v1[x] * v2[y] - v1[y] * v2[x];

            // Normalize the vector (shorten length to one)
            ReduceToUnit(outp);
        }

        float[] cubeXform = new float[16];

        // Creates a shadow projection matrix out of the plane equation
        // coefficients and the position of the light. The return value is stored
        // in cubeXform[,]
        void MakeShadowMatrix(float[,] points)
        {
            float[] planeCoeff = new float[4];
            float dot;

            // Find the plane equation coefficients
            // Find the first three coefficients the same way we
            // find a normal.
            calcNormal(points, planeCoeff);

            // Find the last coefficient by back substitutions
            planeCoeff[3] = -(
                (planeCoeff[0] * points[2, 0]) + (planeCoeff[1] * points[2, 1]) +
                (planeCoeff[2] * points[2, 2]));


            // Dot product of plane and light position
            dot = planeCoeff[0] * pos[0] +
                    planeCoeff[1] * pos[1] +
                    planeCoeff[2] * pos[2] +
                    planeCoeff[3];

            // Now do the projection
            // First column
            cubeXform[0] = dot - pos[0] * planeCoeff[0];
            cubeXform[4] = 0.0f - pos[0] * planeCoeff[1];
            cubeXform[8] = 0.0f - pos[0] * planeCoeff[2];
            cubeXform[12] = 0.0f - pos[0] * planeCoeff[3];

            // Second column
            cubeXform[1] = 0.0f - pos[1] * planeCoeff[0];
            cubeXform[5] = dot - pos[1] * planeCoeff[1];
            cubeXform[9] = 0.0f - pos[1] * planeCoeff[2];
            cubeXform[13] = 0.0f - pos[1] * planeCoeff[3];

            // Third Column
            cubeXform[2] = 0.0f - pos[2] * planeCoeff[0];
            cubeXform[6] = 0.0f - pos[2] * planeCoeff[1];
            cubeXform[10] = dot - pos[2] * planeCoeff[2];
            cubeXform[14] = 0.0f - pos[2] * planeCoeff[3];

            // Fourth Column
            cubeXform[3] = 0.0f - pos[3] * planeCoeff[0];
            cubeXform[7] = 0.0f - pos[3] * planeCoeff[1];
            cubeXform[11] = 0.0f - pos[3] * planeCoeff[2];
            cubeXform[15] = dot - pos[3] * planeCoeff[3];
        }
        

       

        public double xExis = 0;
        public double xExisOrigin = 0;
        public double yExis = 0;
        public double yExisOrigin = 0;
        public float hghg = 0;
        public float depth = 0;
        void DrawFloor3()
        {
            
            float[] WHITE = {1f, 1f, 1f};
            float[]RED = {1, 0, 0};
            float[] GREEN= {0, 1, 0};
            float[] MAGENTA = {1, 0, 1};
 
            int width = 45;
            int depth = 45;
            GL.glEnable(GL.GL_LIGHTING);
            GL.glBegin(GL.GL_QUADS);

            for (int x = -width ; x < width; x+=2)
            {
                for (int z = -depth; z < depth ; z+=2)
                {
                    if ((x + z) % 4 == 0) 
                        GL.glColor4d(1, 0, 0, 0.5); 
                    else 
                        GL.glColor4d(1, 1, 1, 0.5);

                    GL.glVertex3d(x + hghg + xExis, z + yExis, ground[0, 2] - 0.05);
                    GL.glVertex3d(x + hghg + 2 + xExis, z + yExis, ground[0, 2] - 0.05);
                    GL.glVertex3d(x + 2 + hghg + xExis, z + 2 + yExis, ground[0, 2] - 0.05);
                    GL.glVertex3d(x + hghg + xExis, z + 2 + yExis, ground[0, 2] - 0.05);
                    
                }
            }

         
            GL.glEnd();
            
        }
        public float ballZMovement = 0;
        public float ballZtranslate = 0;
        void drawStars(bool isForShade)
        {
           
            for (int i = 0; i < 20; i++)
            {
              
                if ((arr[i].X + (float)xExisOrigin) < 2.5 &
                    (arr[i].X + (float)xExisOrigin) > -2.5 & 
                    (arr[i].Y + (float)yExisOrigin) > -2.5 & 
                    (arr[i].Y + (float)yExisOrigin) < 2.5)
                    
                {
                    playSimpleSound();
                    arr[i].toDraw = false;
                }
               
                drawStar(arr[i],isForShade);
            }
        }
       
       
        void drawStar(Star star,bool isForShade)
        {
            if (star.toDraw == true)
            {
                GL.glPushMatrix();

                
                    



                    GL.glTranslatef(0f, 0f, 1f);
                    GL.glTranslatef(star.X + (float)xExisOrigin, star.Y + (float)yExisOrigin, 3 - Math.Abs(2f * (float)Math.Cos(ballZMovement)));
                    GL.glRotatef(ballZtranslate, 0, 0, 1f);

                   
                    if (!isForShade)
                    {
                        GL.glEnable(GL.GL_TEXTURE_2D);
                        GL.glColor3f(1.0f, 1.0f, 1.0f);
                        GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[6]);
                        GL.glEnable(GL.GL_TEXTURE_GEN_S);

                        GL.glEnable(GL.GL_TEXTURE_GEN_T);
                        GL.glTexGeni(GL.GL_S, GL.GL_TEXTURE_GEN_MODE, (int)GL.GL_SPHERE_MAP);
                        GL.glTexGeni(GL.GL_T, GL.GL_TEXTURE_GEN_MODE, (int)GL.GL_SPHERE_MAP);
                    }
                    else
                        GL.glColor3d(0.2, 0.2, 0.2);
                for (int i = 0; i < 5; i++)
                {
                    GL.glRotatef(72, 1, 0, 0);
                    GL.glBegin(GL.GL_QUADS);

                    GL.glVertex3f(0f, 0f, 0);
                    GL.glVertex3f(0f, (float)Math.Sin(36), 1);
                    GL.glVertex3f(0f, 0, 3f);
                    GL.glVertex3f(0f, -(float)Math.Sin(36), 1);

                    GL.glEnd();
                }
                if (!isForShade)
                {
                    GL.glDisable(GL.GL_TEXTURE_GEN_S);
                    GL.glDisable(GL.GL_TEXTURE_GEN_T);
                    GL.glDisable(GL.GL_TEXTURE_2D);
                }
                GL.glPopMatrix();
            }
        }
      
    
        public void DrawFigures()
        {
            GL.glPushMatrix();


          
            GL.glLightfv(GL.GL_LIGHT0, GL.GL_POSITION, pos);
            
            //Draw Light Source
            GL.glDisable(GL.GL_LIGHTING);
            GL.glTranslatef(pos[0], pos[1], pos[2]);
            //Yellow Light source
            GL.glColor3f(1, 1, 0);
            GLUT.glutSolidSphere(0.05, 8, 8);
            GL.glTranslatef(-pos[0], -pos[1], -pos[2]);

            //main System draw
            GL.glEnable(GL.GL_LIGHTING);

            DrawRobot(false);
            drawStars(false);
            //end of regular show
            //!!!!!!!!!!!!!
            GL.glPopMatrix();
            //!!!!!!!!!!!!!

            //SHADING begin
            //we'll define cubeXform matrix in MakeShadowMatrix Sub
            // Disable lighting, we'll just draw the shadow
            //else instead of shadow we'll see stange projection of the same objects
            GL.glDisable(GL.GL_LIGHTING);
            // wall shadow
            //!!!!!!!!!!!!!
            GL.glPushMatrix();
            //!!!!!!!!!!!!       
            MakeShadowMatrix(ground);
            GL.glMultMatrixf(cubeXform);
            DrawRobot(true);
            drawStars(true);
            //!!!!!!!!!!!!!
            GL.glPopMatrix();
            //!!!!!!!!!!!!!
            
        }
       
        //! TEXTURE CUBE e
        float[] chrome_ambient = { 0.19225f, 0.19225f, 0.19225f, 1.0f };
        float[] chrome_diffuse = { 0.50754f, 0.50754f, 0.50754f, 1.0f };
        float[] chrome_specular = { 0.508273f, 0.508273f, 0.508273f, 1.0f };
        public Star[] arr = new Star[20];
        public float SHOULDER_angle = 2;
        public float ROBOT_angle = 0;
        public float trans = 0;
        public float alfa = 0;
        public float right_upper_ARM_yangle = 10;
        public float right_upper_ARM_xangle = -7,
                     right_lower_ARM_angle = 90,
                     left_upper_ARM_yangle = -10,
                     left_upper_ARM_xangle = 7,
                     left_lower_ARM_angle = 90,
                     right_upper_LEG_angle = 0,
                     right_lower_LEG_angle = 30,
                     left_upper_LEG_angle = 0,
                     left_lower_LEG_angle = 30, updn =0,bodyAngle =0;
        public void DrawRobot(bool isForShades)
        {
            GL.glPushMatrix();
            if (!isForShades)
            {
                GL.glColor4f(0f, 0f, 0.4f, 1f);
                GL.glEnable(GL.GL_COLOR_MATERIAL);
                GL.glMaterialf(GL.GL_FRONT_AND_BACK, GL.GL_SHININESS, 51.2f);
                GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT, chrome_ambient);
                GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_DIFFUSE, chrome_diffuse);
                GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_SPECULAR, chrome_specular);
            }
            
            GL.glTranslated(0, 0, 7.5);
            
            GL.glRotatef(180, 0, 1, 0);
            GL.glRotatef(-alfa, 0, 0, 1);
            GL.glTranslated(0, 0, updn);
            GL.glRotatef(bodyAngle, 0, 1, 0);
            GL.glPushMatrix();

            
            //torso
            if(isForShades)
                GL.glColor3f(0.2f, 0.2f, 0.2f);
           
            GLU.gluCylinder(obj, 1.0, 1.0, 4.0, 50, 3);
            GLU.gluDisk(obj, 0, 1, 40, 20);
            GL.glTranslated(0, 0, 4);
            GLU.gluDisk(obj, 0, 1, 40, 20);
            GL.glTranslated(0, 0, -4);
            //head
            GL.glTranslated(0, 0, -r * 3);
            GLU.gluSphere(obj, r * 3, 20, 20);
          
            GL.glPopMatrix();
           
            GL.glPushMatrix();
            //right_upper_ARM
            
            GL.glTranslated(0, 1.3, 0.3);
            
            GL.glRotatef(right_upper_ARM_yangle, 0, 1, 0);
            GL.glRotatef(right_upper_ARM_xangle, 1, 0, 0);
            if (!isForShades)
                GL.glColor3f(0.4f, 0.6f, 0f);
            else
                GL.glColor3f(0.2f, 0.2f, 0.2f);
            GLU.gluSphere(obj, r * 1.4, 20, 20);
            if (!isForShades)
                GL.glColor3f(0, 0, 1f);
            else
                GL.glColor3f(0.2f, 0.2f, 0.2f);
            GLU.gluCylinder(obj, 0.3, 0.3, 1.7, 50, 3);
            //right_lower_ARM
            GL.glTranslated(0, 0, 1.7);
            GL.glRotatef(right_lower_ARM_angle, 0, 1, 0);
            if (!isForShades)
                GL.glColor3f(0.4f, 0.6f, 0f);
            else
                GL.glColor3f(0.2f, 0.2f, 0.2f);
            GLU.gluSphere(obj, r * 1.1, 20, 20);
            if (!isForShades)
                GL.glColor3f(0, 0, 1f);
            else
                GL.glColor3f(0.2f, 0.2f, 0.2f);
            GLU.gluCylinder(obj, 0.3, 0.3, 1.7, 50, 3);

            GL.glPopMatrix();

            GL.glPushMatrix();
            
            //left_upper_ARM
           
            GL.glTranslated(0, -1.3, 0.3);
            GL.glRotatef(left_upper_ARM_yangle, 0, 1, 0);
            GL.glRotatef(left_upper_ARM_xangle, 1, 0, 0);
            if (!isForShades)
                GL.glColor3f(0.4f, 0.6f, 0f);
            else
                GL.glColor3f(0.2f, 0.2f, 0.2f);
            GLU.gluSphere(obj, r * 1.4, 20, 20);
            if (!isForShades)
                GL.glColor3f(0, 0, 1f);
            else
                GL.glColor3f(0.2f, 0.2f, 0.2f);
            GLU.gluCylinder(obj, 0.3, 0.3, 1.7, 50, 3);
           
            //left_lower_ARM
         
            /////
            GL.glTranslated(0, 0, 1.7);
            GL.glRotatef(left_lower_ARM_angle, 0, 1, 0);
            if (!isForShades)
                GL.glColor3f(0.4f, 0.6f, 0f);
            else
                GL.glColor3f(0.2f, 0.2f, 0.2f);
            GLU.gluSphere(obj, r * 1.1, 20, 20);
            if (!isForShades)
                GL.glColor3f(0, 0, 1f);
            else
                GL.glColor3f(0.2f, 0.2f, 0.2f);
            GLU.gluCylinder(obj, 0.3, 0.3, 1.7, 50, 3);
           
            GL.glPopMatrix();

            GL.glPushMatrix();

            //left_LEG
           
            GL.glTranslated(0, -0.7, 4.2);
            GL.glRotatef(right_upper_LEG_angle, 0, 1, 0);
            if (!isForShades)
                GL.glColor3f(0.4f, 0.6f, 0f);
            else
                GL.glColor3f(0.2f, 0.2f, 0.2f);
            GLU.gluSphere(obj, r * 1.5, 20, 20);
            if (!isForShades)
                GL.glColor3f(0, 0, 1f);
            else
                GL.glColor3f(0.2f, 0.2f, 0.2f);
            GLU.gluCylinder(obj, 0.3, 0.3, 1.7, 50, 3);
            GL.glTranslated(0, 0, 1.7);
        
            // right_lower_LEG
           
            GL.glRotatef(right_lower_LEG_angle, 0, 1, 0);
            if (!isForShades)
                GL.glColor3f(0.4f, 0.6f, 0f);
            else
                GL.glColor3f(0.2f, 0.2f, 0.2f);
            GLU.gluSphere(obj, r * 1.1, 20, 20);
            if (!isForShades)
                GL.glColor3f(0, 0, 1f);
            else
                GL.glColor3f(0.2f, 0.2f, 0.2f);
            GLU.gluCylinder(obj, 0.3, 0.3, 1.7, 50, 3);
            
            GL.glPopMatrix();

            GL.glPushMatrix();
          
            //right_LEG

            GL.glTranslated(0, 0.7, 4.2);
            GL.glRotatef(left_upper_LEG_angle, 0, 1, 0);
            if (!isForShades)
                GL.glColor3f(0.4f, 0.6f, 0f);
            else
                GL.glColor3f(0.2f, 0.2f, 0.2f);
            GLU.gluSphere(obj, r * 1.5, 20, 20);
            if (!isForShades)
                GL.glColor3f(0, 0, 1f);
            else
                GL.glColor3f(0.2f, 0.2f, 0.2f);
            GLU.gluCylinder(obj, 0.3, 0.3, 1.7, 50, 3);
            GL.glTranslated(0, 0, 1.7);
            //left_lower_LEG
            GL.glRotatef(left_lower_LEG_angle, 0, 1, 0);
            if (!isForShades)
                GL.glColor3f(0.4f, 0.6f, 0f);
            else
                GL.glColor3f(0.2f, 0.2f, 0.2f);
            GLU.gluSphere(obj, r * 1.1, 20, 20);
            if (!isForShades)
                GL.glColor3f(0, 0, 1f);
            else
                GL.glColor3f(0.2f, 0.2f, 0.2f);
            GLU.gluCylinder(obj, 0.3, 0.3, 1.7, 50, 3);

            GL.glPopMatrix();

            GL.glPopMatrix();
            
           
           
        }
        public void straightRobotBody()
        {
            bodyAngle = 0;
            right_upper_ARM_yangle = 10;
            right_upper_ARM_xangle = -7;
            right_lower_ARM_angle = 0;
            left_upper_ARM_yangle = -10;
            left_upper_ARM_xangle = 7;
            left_lower_ARM_angle = 0;
            right_upper_LEG_angle = 0;
            right_lower_LEG_angle = 30;
            left_upper_LEG_angle = 0;
            left_lower_LEG_angle = 30;
            updn = 0;
            bodyAngle = 0;
        }

        //
        // texture
        public int viewAngle; 
         // 
        // </summary>

        public float[] pos = new float[4];
        public int intOptionB = 1;

        public float[] ScrollValue = new float[14];
        public float zShift = 0.0f;
        public float yShift = 0.0f;
        public float xShift = 0.0f;
        public float zAngle = 0.0f;
        public float yAngle = 0.0f;
        public float xAngle = 0.0f;
        public int intOptionC = 0;
        double[] AccumulatedRotationsTraslations = new double[16];
        
        public void Draw()
        {

            pos[0] = -4+(float)xExisOrigin + (float)ScrollValue[11];
            pos[1] = 15+(float)yExisOrigin + (float)ScrollValue[12];
            pos[2] = 15 + (float)ScrollValue[13]; 


            if (m_uint_DC == 0 || m_uint_RC == 0)
                return;

            GL.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT | GL.GL_STENCIL_BUFFER_BIT);
            
            GL.glLoadIdentity();	
			 
          
            // not trivial
            double []ModelVievMatrixBeforeSpecificTransforms=new double[16];
            double []CurrentRotationTraslation=new double[16];
                     
            GLU.gluLookAt (ScrollValue[0], ScrollValue[1], ScrollValue[2], 
	                   ScrollValue[3], ScrollValue[4], ScrollValue[5],
		               ScrollValue[6],ScrollValue[7],ScrollValue[8]);
            GL.glTranslatef(0.0f, 0.0f, -30.0f);

            GL.glRotatef(105,  0, 0, 1);
            GL.glRotatef(70,  0,1, 0);
            GL.glRotatef(15, 1, 0, 0);
            //save current ModelView Matrix values
            //in ModelVievMatrixBeforeSpecificTransforms array
            //ModelView Matrix ========>>>>>> ModelVievMatrixBeforeSpecificTransforms
            GL.glGetDoublev (GL.GL_MODELVIEW_MATRIX, ModelVievMatrixBeforeSpecificTransforms);
            //ModelView Matrix was saved, so
            GL.glLoadIdentity(); // make it identity matrix
             
            //make transformation in accordance to KeyCode
            float delta;
            if (intOptionC != 0)
            {
                delta = 5.0f * Math.Abs(intOptionC) / intOptionC; // signed 5

                switch (Math.Abs(intOptionC))
                {
                    case 1:
                        GL.glRotatef(delta, 1, 0, 0);
                        break;
                    case 2:
                        GL.glRotatef(delta, 0, 1, 0);
                        break;
                    case 3:
                        GL.glRotatef(delta, 0, 0, 1);
                        break;
                    case 4:
                        GL.glTranslatef(delta / 20, 0, 0);
                        break;
                    case 5:
                        GL.glTranslatef(0, delta / 20, 0);
                        break;
                    case 6:
                        GL.glTranslatef(0, 0, delta / 20);
                        break;
                }
            }
            //as result - the ModelView Matrix now is pure representation
            //of KeyCode transform and only it !!!

            //save current ModelView Matrix values
            //in CurrentRotationTraslation array
            //ModelView Matrix =======>>>>>>> CurrentRotationTraslation
            GL.glGetDoublev(GL.GL_MODELVIEW_MATRIX, CurrentRotationTraslation);

            //The GL.glLoadMatrix function replaces the current matrix with
            //the one specified in its argument.
            //The current matrix is the
            //projection matrix, modelview matrix, or texture matrix,
            //determined by the current matrix mode (now is ModelView mode)
            GL.glLoadMatrixd(AccumulatedRotationsTraslations); //Global Matrix

            //The GL.glMultMatrix function multiplies the current matrix by
            //the one specified in its argument.
            //That is, if M is the current matrix and T is the matrix passed to
            //GL.glMultMatrix, then M is replaced with M • T
            GL.glMultMatrixd(CurrentRotationTraslation);

            //save the matrix product in AccumulatedRotationsTraslations
            GL.glGetDoublev(GL.GL_MODELVIEW_MATRIX, AccumulatedRotationsTraslations);

            //replace ModelViev Matrix with stored ModelVievMatrixBeforeSpecificTransforms
            GL.glLoadMatrixd(ModelVievMatrixBeforeSpecificTransforms);
            //multiply it by KeyCode defined AccumulatedRotationsTraslations matrix
            GL.glMultMatrixd(AccumulatedRotationsTraslations);


            //REFLECTION//DrawAxes();

            //REFLECTION b    	
            intOptionB += 1; //for rotation
            intOptionC += 10; //for rotation
            // without REFLECTION was only DrawAll(); 
            // now
            //!!!!------sky box
            GL.glPushMatrix();
            GL.glPushAttrib(GL.GL_CURRENT_BIT);
            GL.glColor4f(1.0f, 1.0f, 1.0f, 0.5f);


            GL.glEnable(GL.GL_TEXTURE_2D);
            GL.glDisable(GL.GL_BLEND);
            GL.glRotatef(90, 1, 0, 0);
            DrawTexturedCube();
            GL.glPopAttrib();
            GL.glPopMatrix();
            /////
            GL.glEnable(GL.GL_BLEND);
            GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);


            //only floor, draw only to STENCIL buffer
            GL.glEnable(GL.GL_STENCIL_TEST);
            GL.glStencilOp(GL.GL_REPLACE, GL.GL_REPLACE, GL.GL_REPLACE);
            GL.glStencilFunc(GL.GL_ALWAYS, 1, 0xFFFFFFFF); // draw floor always
            GL.glColorMask((byte)GL.GL_FALSE, (byte)GL.GL_FALSE, (byte)GL.GL_FALSE, (byte)GL.GL_FALSE);
            GL.glDisable(GL.GL_DEPTH_TEST);

            DrawFloor3();

            // restore regular settings
            GL.glColorMask((byte)GL.GL_TRUE, (byte)GL.GL_TRUE, (byte)GL.GL_TRUE, (byte)GL.GL_TRUE);
            GL.glEnable(GL.GL_DEPTH_TEST);

            // reflection is drawn only where STENCIL buffer value equal to 1
            GL.glStencilFunc(GL.GL_EQUAL, 1, 0xFFFFFFFF);
            GL.glStencilOp(GL.GL_KEEP, GL.GL_KEEP, GL.GL_KEEP);

            GL.glEnable(GL.GL_STENCIL_TEST);

            // draw reflected scene
            GL.glPushMatrix();
            GL.glScalef(1, 1,-1); //swap on Z axis
            GL.glEnable(GL.GL_CULL_FACE);
            GL.glCullFace(GL.GL_BACK);
            DrawFigures();
  
            GL.glCullFace(GL.GL_FRONT);
            DrawFigures();
            
            GL.glDisable(GL.GL_CULL_FACE);
            GL.glPopMatrix();

            // really draw floor 
            //( half-transparent ( see its color's alpha byte)))
            // in order to see reflected objects 
            GL.glDepthMask((byte)GL.GL_FALSE);
            
            DrawFloor3();

            GL.glDepthMask((byte)GL.GL_TRUE);
            // Disable GL.GL_STENCIL_TEST to show All, else it will be cut on GL.GL_STENCIL
            GL.glDisable(GL.GL_STENCIL_TEST);



              
           DrawFigures();
        
         
            
            GL.glDisable(GL.GL_TEXTURE_2D);

            GL.glFlush();
            WGL.wglSwapBuffers(m_uint_DC);
            
        }

		protected virtual void InitializeGL()
		{
			m_uint_HWND = (uint)p.Handle.ToInt32();
			m_uint_DC   = WGL.GetDC(m_uint_HWND);

            // Not doing the following WGL.wglSwapBuffers() on the DC will
			// result in a failure to subsequently create the RC.
			WGL.wglSwapBuffers(m_uint_DC);

			WGL.PIXELFORMATDESCRIPTOR pfd = new WGL.PIXELFORMATDESCRIPTOR();
			WGL.ZeroPixelDescriptor(ref pfd);
			pfd.nVersion        = 1; 
			pfd.dwFlags         = (WGL.PFD_DRAW_TO_WINDOW |  WGL.PFD_SUPPORT_OPENGL |  WGL.PFD_DOUBLEBUFFER); 
			pfd.iPixelType      = (byte)(WGL.PFD_TYPE_RGBA);
			pfd.cColorBits      = 32;
			pfd.cDepthBits      = 32;
			pfd.iLayerType      = (byte)(WGL.PFD_MAIN_PLANE);

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            //for Stencil support 

            pfd.cStencilBits = 32;

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

			int pixelFormatIndex = 0;
			pixelFormatIndex = WGL.ChoosePixelFormat(m_uint_DC, ref pfd);
			if(pixelFormatIndex == 0)
			{
				MessageBox.Show("Unable to retrieve pixel format");
				return;
			}

			if(WGL.SetPixelFormat(m_uint_DC,pixelFormatIndex,ref pfd) == 0)
			{
				MessageBox.Show("Unable to set pixel format");
				return;
			}
			//Create rendering context
			m_uint_RC = WGL.wglCreateContext(m_uint_DC);
			if(m_uint_RC == 0)
			{
				MessageBox.Show("Unable to get rendering context");
				return;
			}
			if(WGL.wglMakeCurrent(m_uint_DC,m_uint_RC) == 0)
			{
				MessageBox.Show("Unable to make rendering context current");
				return;
			}

          
       
            initRenderingGL();
        }

        public void OnResize()
        {
            Width = p.Width;
            Height = p.Height;
            GL.glViewport(0, 0, Width, Height);
            Draw();
        }

        protected virtual void initRenderingGL()
		{
			if(m_uint_DC == 0 || m_uint_RC == 0)
				return;
			if(this.Width == 0 || this.Height == 0)
				return;
            GL.glShadeModel(GL.GL_SMOOTH);
            GL.glClearColor(0.0f, 0.0f, 0.0f, 0.5f);
            GL.glClearDepth(1.0f);


            GL.glEnable(GL.GL_LIGHT0);
            GL.glEnable(GL.GL_COLOR_MATERIAL);
            GL.glColorMaterial(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE);

            GL.glEnable(GL.GL_DEPTH_TEST);
            GL.glDepthFunc(GL.GL_LEQUAL);
            GL.glHint(GL.GL_PERSPECTIVE_CORRECTION_Hint, GL.GL_NICEST);	

            GL.glViewport(0, 0, this.Width, this.Height);
			GL.glMatrixMode ( GL.GL_PROJECTION );
			GL.glLoadIdentity();
            
            //nice 3D
			GLU.gluPerspective( 45.0,  1.0, 0.4,  100.0);

            //! TEXTURE 1a 
            GL.glEnable(GL.GL_COLOR_MATERIAL);
            float[] emis = { 0.3f, 0.3f, 0.3f, 1 };
            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_EMISSION, emis);
            //! TEXTURE 1a 



            GL.glShadeModel(GL.GL_SMOOTH);
            GLU.gluPerspective(viewAngle, (float)Width / (float)Height, 0.45f, 30.0f);
            
            GL.glMatrixMode ( GL.GL_MODELVIEW );
			GL.glLoadIdentity();

            //! TEXTURE 1a 
            GenerateTextures();
            //! TEXTURE 1b 
            //save the current MODELVIEW Matrix (now it is Identity)
            GL.glGetDoublev(GL.GL_MODELVIEW_MATRIX, AccumulatedRotationsTraslations);
		}


        //! TEXTURE b
        public uint[] Textures = new uint[8];

        void GenerateTextures()
        {
            GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
            GL.glGenTextures(6, Textures);
            string[] imagesName ={ "front.bmp","back.bmp",
		                            "left.bmp","right.bmp","top.bmp","bottom.bmp","golden2.bmp"};
            for (int i = 0; i < 7; i++)
            {
                Bitmap image = new Bitmap(imagesName[i]);
                image.RotateFlip(RotateFlipType.RotateNoneFlipY); //Y axis in Windows is directed downwards, while in OpenGL-upwards
                System.Drawing.Imaging.BitmapData bitmapdata;
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

                bitmapdata = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[i]);
                //2D for XYZ
                GL.glTexImage2D(GL.GL_TEXTURE_2D, 0, (int)GL.GL_RGB8, image.Width, image.Height,
                                                              0, GL.GL_BGR_EXT, GL.GL_UNSIGNED_byte, bitmapdata.Scan0);
                GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, (int)GL.GL_LINEAR);
                GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, (int)GL.GL_LINEAR);

                image.UnlockBits(bitmapdata);
                image.Dispose();
            }
        }

        //! TEXTURE CUBE b
        //Draws our textured cube, VERY simple.  Notice that the faces are constructed
        //in a counter-clockwise order.  If they were done in a clockwise order you would
        //have to use the glFrontFace() function.  
        void DrawTexturedCube()
        {
            // front
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[0]);
            GL.glBegin(GL.GL_QUADS);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-40.0f, -40.0f, 40.0f);
            GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(40.0f, -40.0f, 40.0f);
            GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(40.0f, 40.0f, 40.0f);
            GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(-40.0f, 40.0f, 40.0f);
            GL.glEnd();
            // back
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[1]);
            GL.glBegin(GL.GL_QUADS);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(40.0f, -40.0f, -40.0f);
            GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(-40.0f, -40.0f, -40.0f);
            GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(-40.0f, 40.0f, -40.0f);
            GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(40.0f, 40.0f, -40.0f);
            GL.glEnd();
            // left
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[2]);
            GL.glBegin(GL.GL_QUADS);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-40.0f, -40.0f, -40.0f);
            GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(-40.0f, -40.0f, 40.0f);
            GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(-40.0f, 40.0f, 40.0f);
            GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(-40.0f, 40.0f, -40.0f);
            GL.glEnd();
            // right
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[3]);
            GL.glBegin(GL.GL_QUADS);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(40.0f, -40.0f, 40.0f);
            GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(40.0f, -40.0f, -40.0f);
            GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(40.0f, 40.0f, -40.0f);
            GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(40.0f, 40.0f, 40.0f);
            GL.glEnd();
            // top
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[4]);
            GL.glBegin(GL.GL_QUADS);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-40.0f, 40.0f, 40.0f);
            GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(40.0f, 40.0f, 40.0f);
            GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(40.0f, 40.0f, -40.0f);
            GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(-40.0f, 40.0f, -40.0f);
            GL.glEnd();
            // bottom
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[5]);
            GL.glBegin(GL.GL_QUADS);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-40.0f, -40.0f, -40.0f);
            GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(40.0f, -40.0f, -40.0f);
            GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(40.0f, -40.0f, 40.0f);
            GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(-40.0f, -40.0f, 40.0f);
            GL.glEnd();

        }
       


        
        


            
            float r = 0.3f;
         
              
           

    
    }



}


