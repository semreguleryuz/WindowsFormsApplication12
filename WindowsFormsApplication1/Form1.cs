using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using AForge;
using AForge.Imaging.Filters;
using AForge.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Math.Geometry;

//Remove ambiguousness between AForge.Image and System.Drawing.Image
using Point = System.Drawing.Point; //Remove ambiguousness between AForge.Point and System.Drawing.Point

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private FilterInfoCollection VideoCapTureDevices;
        private VideoCaptureDevice Finalvideo;
        int i = 0;

        public Form1()
        {
            InitializeComponent();
        }

        int R; //Trackbarın değişkeneleri
        int G;
        int B;
        
       
        private void Form1_Load(object sender, EventArgs e)
        {
            VideoCapTureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo VideoCaptureDevice in VideoCapTureDevices)
            {

                comboBox1.Items.Add(VideoCaptureDevice.Name);
            }

            comboBox1.SelectedIndex = 0;

        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            Finalvideo = new VideoCaptureDevice(VideoCapTureDevices[comboBox1.SelectedIndex].MonikerString);
            Finalvideo.NewFrame += new NewFrameEventHandler(Finalvideo_NewFrame);
            Finalvideo.DesiredFrameRate = 20;//saniyede kaç görüntü alsın istiyorsanız. FPS
            Finalvideo.DesiredFrameSize = new Size(320, 240);//görüntü boyutları
            Finalvideo.Start();




        }

        void Finalvideo_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {

            Bitmap image = (Bitmap)eventArgs.Frame.Clone();
            Bitmap image1 = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = image;

            

            if (rdiobtnKirmizi.Checked)
            {
                // create filter
                EuclideanColorFiltering filter = new EuclideanColorFiltering();
                // set center colol and radius
                filter.CenterColor = new RGB(Color.FromArgb(215, 0, 0));
                filter.Radius = 100;
                // apply the filter
                filter.ApplyInPlace(image1);
                nesnebul(image1);  



            }

            if (rdiobtnMavi.Checked)
            {
                // create filter
                EuclideanColorFiltering filter = new EuclideanColorFiltering();
                // set center color and radius
                filter.CenterColor = new RGB(Color.FromArgb(30, 144, 255));
                filter.Radius = 100;
                // apply the filter
                filter.ApplyInPlace(image1);
                nesnebul(image1);
            }
            if(rdiobtnYesil.Checked)
            {
                // create filter
                EuclideanColorFiltering filter = new EuclideanColorFiltering();
                // set center color and radius
                filter.CenterColor = new RGB(Color.FromArgb(0, 215, 0));
                filter.Radius = 100;
                // apply the filter
                filter.ApplyInPlace(image1);
                nesnebul(image1);
            }

            if (rdiobtnBeyaz.Checked)
            {
                // create filter
                EuclideanColorFiltering filter = new EuclideanColorFiltering();
                // set center color and radius
                filter.CenterColor = new RGB(Color.FromArgb(255, 255, 255));
                filter.Radius = 100;
                // apply the filter
                filter.ApplyInPlace(image1);
                nesnebul(image1);
             } 
            

            if (rdbtnElleBelirleme.Checked)
            {                 
                 // create filter
                EuclideanColorFiltering filter = new EuclideanColorFiltering();
                // set center colol and radius
                filter.CenterColor = new RGB(Color.FromArgb(R, G, B));
                filter.Radius = 100;
                // apply the filter
                filter.ApplyInPlace(image1);
                nesnebul(image1);
            }
        }
  
        public void nesnebul(Bitmap image)
        {
            BlobCounter blobCounter = new BlobCounter();
            //blobCounter.MaxWidth = 100; //bu degerleri uygun degerlerle elle degistirebilirsin
            blobCounter.MinWidth = 5; //algilanması istenen cismin min boyu
            blobCounter.MinHeight = 5;
            blobCounter.FilterBlobs = true;
            blobCounter.ObjectsOrder = ObjectsOrder.Size;
            //Grayscale griFiltre = new Grayscale(0.2125, 0.7154, 0.0721);
            //Grayscale griFiltre = new Grayscale(0.2, 0.2, 0.2);
            //Bitmap griImage = griFiltre.Apply(image);

            BitmapData objectsData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            // grayscaling
            Grayscale grayscaleFilter = new Grayscale(0.2125, 0.7154, 0.0721);
            UnmanagedImage grayImage = grayscaleFilter.Apply(new UnmanagedImage(objectsData));
            // unlock image
            image.UnlockBits(objectsData);


            blobCounter.ProcessImage(image);
            Rectangle[] rects = blobCounter.GetObjectsRectangles();
            Blob[] blobs = blobCounter.GetObjectsInformation();
            pictureBox2.Image = image;



            if (rdiobtnTekCisimTakibi.Checked)
            {
                //Tekli cisim Takibi Single Tracking--------

                foreach (Rectangle recs in rects)
                {
                    if (rects.Length > 0)
                    {
                        Rectangle objectRect = rects[0];
                        //Graphics g = Graphics.FromImage(image);
                        Graphics g = pictureBox1.CreateGraphics();
                        using (Pen pen = new Pen(Color.FromArgb(252, 3, 26), 2))
                        {
                            g.DrawRectangle(pen, objectRect);
                        }
                        timer1.Enabled = true;
                        timer1.Interval = 10000; // hızını ayarlayabiliriz
                        timer1.Start();
                        
                        //Cizdirilen Dikdörtgenin Koordinatlari aliniyor.
                        int objectX = objectRect.X + (objectRect.Width / 2) + 40;
                        int objectY = objectRect.Y + (objectRect.Height / 2);
                        g.Dispose();

                      
                        if (true)
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                if (true) //on kameradan goruntu alindigindan ters yani sag
                                {
                                    int tempx = objectX+12-56;
                                    //double santimetre = (double)tempx * 0.0264583333333334;//1 pixel (X) = 0.0264583333333334 centimeter [cm]

                                    //int tempy = 287;
                                    int tempy = 407;
                                    pictureBox3.Location=new Point(tempx,tempy);
                                    richTextBox4.Text = tempx.ToString() + ""; //"\ncm olarak:";+santimetre.ToString();
                             }

                            });
                        }

                        if (chkKoordinatiGoster.Checked){
                        this.Invoke((MethodInvoker)delegate
                        {
                            richTextBox1.Text = objectX.ToString() + "," + objectY.ToString();
                            //richTextBox1.Text = objectRect.Location.ToString() + "\n" + richTextBox1.Text + "\n";
                        });
                        }

                    }
                }
            }

            

            if (rdiobtnCokCisimTakibi.Checked)
            {
                //Multi tracking Çoklu cisim Takibi-------

                for (int i = 0; rects.Length > i; i++)
                {
                    Rectangle objectRect = rects[i];
                    //Graphics g = Graphics.FromImage(image);
                    Graphics g = pictureBox1.CreateGraphics();
                    using (Pen pen = new Pen(Color.FromArgb(252, 3, 26), 2))
                    {
                        g.DrawRectangle(pen, objectRect);
                        g.DrawString((i + 1).ToString(), new Font("Arial", 12), Brushes.Red, objectRect);
                    }
                    //Cizdirilen Dikdörtgenin Koordinatlari aliniyor.
                    int objectX = objectRect.X + (objectRect.Width / 2);
                    int objectY = objectRect.Y + (objectRect.Height / 2);
                    //  g.DrawString(objectX.ToString() + "X" + objectY.ToString(), new Font("Arial", 12), Brushes.Red, new System.Drawing.Point(250, 1));
                                     
                                  
                    if(chkboxMesafeOlcer.Checked){

                        if (rects.Length > 1)
                        {
                            for (int j = 0; j < rects.Length - 1; j++)
                            {
                                int ilkx = (rects[j].Right + rects[j].Left) / 2;
                                int ilky = (rects[j].Top + rects[j].Bottom) / 2;

                                int ikix = (rects[j + 1].Right + rects[j + 1].Left) / 2;
                                int ikiy = (rects[j + 1].Top + rects[j + 1].Bottom) / 2;

                                g = pictureBox1.CreateGraphics();
                                //g.DrawLine(Pens.Red, rects[j].Location, rects[j + 1].Location);
                                //g.DrawLine(Pens.Blue, rects[0].Location, rects[rects.Length - 1].Location);
                                g.DrawLine(Pens.Red, ilkx, ilky, ikix, ikiy);

                            }
                        }

                        if(rects.Length==2){

                        Rectangle ilk = rects[0];
                        Rectangle iki = rects[1];

                        int ilkX = ilk.X + (ilk.Width / 2);
                        int ilkY = ilk.Y + (ilk.Height / 2);

                        int ikiX = iki.X + (iki.Width / 2);
                        int ikiY = iki.Y + (iki.Height / 2);

                         //1 pixel (X) = 0.0264583333333334 centimeter [cm]

                        double formul = Math.Floor((Math.Sqrt((Math.Pow((ilkX - ikiX), 2)) + Math.Pow((ilkY - ikiY), 2))) * 0.0264) ;

                        string uzaklikY = "Y-" + Convert.ToString(ilkX - ikiX);
                        string uzaklikX = "X-" + Convert.ToString(ilkY - ikiY);

                        string distance = uzaklikX + " " + uzaklikY;

                        AForge.Imaging.Drawing.Line(objectsData, new IntPoint((int)ilkX, (int)ilkY), new IntPoint((int)ikiX, (int)ikiY), Color.Blue);
                                                   
                                                    
                        this.Invoke((MethodInvoker)delegate
                        {
                            richTextBox2.Text = formul.ToString() + " cm\n" + richTextBox2.Text + " cm\n"; ;
                        });


                        if (chkboxMesafeKordinati.Checked)
                        {

                            this.Invoke((MethodInvoker)delegate
                            {
                                richTextBox3.Text = distance.ToString() + "\n" + richTextBox3.Text + "\n"; ;
                            });
                       }
                        
                     }
                        
                    }

                    
                    g.Dispose();

               //     this.Invoke((MethodInvoker)delegate
               //{
               //    richTextBox1.Text = objectRect.Location.ToString() + "\n" + richTextBox1.Text + "\n"; ;
               //});

                    }
            }



            if (rdiobtnGeoSekil.Checked)
            {

                SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

                Graphics g = pictureBox1.CreateGraphics();
                Pen yellowPen = new Pen(Color.Yellow, 2); // circles
                Pen redPen = new Pen(Color.Red, 2);       // quadrilateral
                Pen brownPen = new Pen(Color.Brown, 2);   // quadrilateral with known sub-type
                Pen greenPen = new Pen(Color.Green, 2);   // known triangle
                Pen bluePen = new Pen(Color.Blue, 2);     // triangle

                for (int i = 0, n = blobs.Length; i < n; i++)
                {
                    List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blobs[i]);

                    AForge.Point center;
                    float radius;

                    // is circle ?
                    if (shapeChecker.IsCircle(edgePoints, out center, out radius))
                    {
                        g.DrawEllipse(yellowPen,
                            (float)(center.X - radius), (float)(center.Y - radius),
                            (float)(radius * 2), (float)(radius * 2));
                    }
                    else
                    {
                        List<IntPoint> corners;

                        // is triangle or quadrilateral
                        if (shapeChecker.IsConvexPolygon(edgePoints, out corners))
                        {
                            // get sub-type
                            PolygonSubType subType = shapeChecker.CheckPolygonSubType(corners);

                            Pen pen;

                            if (subType == PolygonSubType.Unknown)
                            {
                                pen = (corners.Count == 4) ? redPen : bluePen;
                            }
                            else
                            {
                                pen = (corners.Count == 4) ? brownPen : greenPen;
                            }

                            g.DrawPolygon(pen, ToPointsArray(corners));
                        }
                    }
                }

                yellowPen.Dispose();
                redPen.Dispose();
                greenPen.Dispose();
                bluePen.Dispose();
                brownPen.Dispose();
                g.Dispose();

    }
         }

        // Conver list of AForge.NET's points to array of .NET points
        private Point[] ToPointsArray(List<IntPoint> points)
        {
            Point[] array = new Point[points.Count];

            for (int i = 0, n = points.Count; i < n; i++)
            {
                array[i] = new Point(points[i].X, points[i].Y);
            }

            return array;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (Finalvideo.IsRunning)
            {
                Finalvideo.Stop();
                
            }
        }

        
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            R = trackBar1.Value;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            G = trackBar2.Value;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            B = trackBar3.Value;
        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (Finalvideo.IsRunning)
            {
                Finalvideo.Stop();

            }

            Application.Exit();
        }



        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F6) {

                // burada gizli bolum acilacak bizim icin detay alani

                if (i % 2 == 0)
                {
                    panel1.Visible = true;
                }else
                {
                    panel1.Visible = false;
                }

                i++;
            }
            if (e.KeyCode == Keys.F1)
            {

                // burada gizli bolum acilacak bizim icin detay alani

                textBox1.Text = "100";
                textBox4.Text = "90";
                textBox6.Text = "70";
                textBox8.Text = "65";
                textBox10.Text = "20";

                textBox2.Text = "90";
                textBox3.Text = "70";
                textBox5.Text = "40";
                textBox7.Text = "30";
                textBox9.Text = "10";
               

            }
        }


        int[] hiz = new int[] { 0, 0, 0, 0, 0,0 };
        int[] mesafe = new int[] { 0, 0, 0, 0, 0 };
        int v = 0; //hizi atamak icin gerekli
        int guvenli_m; // Guvenli mesafe


        private void btn_mesafe_hesapla_Click(object sender, EventArgs e)
        {
            try
            {
                
          

                hiz[0] = Convert.ToInt32(textBox1.Text);
                hiz[1] = Convert.ToInt32(textBox4.Text);
                hiz[2] = Convert.ToInt32(textBox6.Text);
                hiz[3] = Convert.ToInt32(textBox8.Text);
                hiz[4] = Convert.ToInt32(textBox10.Text);
                
                mesafe[0] = Convert.ToInt32(textBox2.Text);
                mesafe[1] = Convert.ToInt32(textBox3.Text);
                mesafe[2] = Convert.ToInt32(textBox5.Text);
                mesafe[3] = Convert.ToInt32(textBox7.Text);
                mesafe[4] = Convert.ToInt32(textBox9.Text);


                for (int i = 0; i < 5; i++) {
                    v = hiz[i];
                    guvenli_m = (v / 2);
                    if (mesafe[i] < guvenli_m)
                    {
                        
                        v = mesafe[i] * 2;
                        hiz[i + 1] = v;
                    }
                    
                }

                timer2.Start();
         
            }
            catch
            {
            }
        }
        int k = 0;
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (k < 5)
            {
                label17.Text = hiz[k+1].ToString() + "km/s";
                label18.Text = mesafe[k].ToString() + " metre";
                k++;
            }
            else { timer2.Stop(); k = 0; }
        }
    }

}


