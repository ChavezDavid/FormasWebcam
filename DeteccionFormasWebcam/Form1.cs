using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace DeteccionFormasWebcam
{
    public partial class Form1 : Form
    {
        VideoCapture capture;
        Image<Gray, byte> imgInput;

        public Form1()
        {
            InitializeComponent();
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 1000;
            aTimer.Enabled = true;
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            if (capture == null)
            {
                return;
            }
            DetectarForma();
        }

        private void iniciarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (capture == null)
            {
                capture = new Emgu.CV.VideoCapture(0);
            }
            capture.ImageGrabbed += CaptureImageGrabbed;
            capture.Start();
        }

        private void CaptureImageGrabbed(object sender, EventArgs e)
        {
            try
            {
                Mat m = new Mat();
                capture.Retrieve(m);
                pictureBox1.Image = m.ToImage<Gray, byte>().Bitmap;
                imgInput = new Image<Gray, byte>(m.ToImage<Gray, byte>().Bitmap);

                if (capture == null)
                {
                    return;
                }
            }
            catch (Exception)
            {

            }
        }

        private void detenerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (capture != null)
            {
                capture.Stop();
            }
        }

        private void iniciarToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (capture == null)
            {
                return;
            }

            try
            {
                var temp = imgInput.SmoothGaussian(1).Convert<Gray, byte>().ThresholdBinaryInv(new Gray(230), new Gray(255));

                VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
                Mat m = new Mat();

                CvInvoke.FindContours(temp, contours, m, RetrType.External, ChainApproxMethod.ChainApproxSimple);

                for (int i = 0; i < contours.Size; i++)
                {
                    double perimeter = CvInvoke.ArcLength(contours[i], true);
                    VectorOfPoint approx = new VectorOfPoint();
                    CvInvoke.ApproxPolyDP(contours[i], approx, 0.04 * perimeter, true);

                    CvInvoke.DrawContours(imgInput, contours, i, new MCvScalar(0, 0, 255), 2);

                    var moments = CvInvoke.Moments(contours[i]);
                    int x = (int)(moments.M10 / moments.M00);
                    int y = (int)(moments.M01 / moments.M00);

                    if (approx.Size == 3)
                    {
                        CvInvoke.PutText(imgInput, "Triangulo", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
                    }

                    if (approx.Size == 4)
                    {
                        Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);

                        double ar = (double)rect.Width / rect.Height;

                        if (ar >= 0.95 && ar <= 1.05)
                        {
                            CvInvoke.PutText(imgInput, "Cuadrado", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
                        }
                        else
                        {
                            CvInvoke.PutText(imgInput, "Rectangulo", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
                        }
                    }

                    if (approx.Size == 5)
                    {
                        CvInvoke.PutText(imgInput, "Pentagono", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
                    }

                    if (approx.Size == 6)
                    {
                        CvInvoke.PutText(imgInput, "Hexagono", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
                    }

                    if (approx.Size == 7)
                    {
                        CvInvoke.PutText(imgInput, "Heptagono", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
                    }

                    if (approx.Size > 7)
                    {
                        CvInvoke.PutText(imgInput, "Circulo", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
                    }

                    pictureBox2.Image = imgInput.Bitmap;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void DetectarForma()
        {
            if (capture == null)
            {
                return;
            }

            try
            {
                var temp = imgInput.SmoothGaussian(31).Convert<Gray, byte>().ThresholdBinary(new Gray(230), new Gray(255));

                VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
                Mat m = new Mat();

                CvInvoke.FindContours(temp, contours, m, RetrType.External, ChainApproxMethod.ChainApproxSimple);

                for (int i = 0; i < contours.Size; i++)
                {
                    double perimeter = CvInvoke.ArcLength(contours[i], true);
                    VectorOfPoint approx = new VectorOfPoint();
                    CvInvoke.ApproxPolyDP(contours[i], approx, 0.04 * perimeter, true);

                    CvInvoke.DrawContours(temp, contours, i, new MCvScalar(0, 0, 255), 2);

                    var moments = CvInvoke.Moments(contours[i]);
                    int x = (int)(moments.M10 / moments.M00);
                    int y = (int)(moments.M01 / moments.M00);

                    if (approx.Size == 3)
                    {
                        CvInvoke.PutText(temp, "Triangulo", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
                    }

                    if (approx.Size == 4)
                    {
                        Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);

                        double ar = (double)rect.Width / rect.Height;

                        if (ar >= 0.95 && ar <= 1.05)
                        {
                            CvInvoke.PutText(temp, "Cuadrado", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
                        }
                        else
                        {
                            CvInvoke.PutText(temp, "Rectangulo", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
                        }
                    }

                    if (approx.Size == 5)
                    {
                        CvInvoke.PutText(temp, "Pentagono", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
                    }

                    if (approx.Size == 6)
                    {
                        CvInvoke.PutText(temp, "Hexagono", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
                    }

                    if (approx.Size == 7)
                    {
                        CvInvoke.PutText(temp, "Heptagono", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
                    }

                    if (approx.Size > 7)
                    {
                        CvInvoke.PutText(temp, "Circulo", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
                    }

                    pictureBox2.Image = imgInput.Bitmap;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
