using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;


namespace Detector_de_cuerpos_en_movimiento
{
    public partial class Form1 : Form
    {

        #region Variables Globales
        VideoCapture video;
        bool pausa = false;
        int tiempo = 60;


        //Region de detección de movimiento
        Image<Bgr, byte> frame;
        Image<Gray, byte> frame1;
        Image<Gray, byte> frame2;
        //Image<Gray, byte> imgOut;

        Graphics papel;
        Pen pluma = new Pen(Color.Red);

        #endregion


        #region Inicialización
        public Form1()
        {
            InitializeComponent();
        }
        #endregion

        #region Importación de video
        private void importarVideoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportacionVideo();
        }

        private void ImportacionVideo()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                video = new VideoCapture(ofd.FileName);
                Mat matriz = new Mat();               
                video.Read(matriz);
                //Image<Bgr, byte> img = matriz.ToImage<Bgr, byte>();
                //img = img.Resize(pictureFrameVideo.Width, pictureFrameVideo.Height, Emgu.CV.CvEnum.Inter.Linear);
                //pictureFrameVideo.Image = matriz.Bitmap;
            }
        }
        #endregion

        #region Control de video
        private async void btnPlay_Click(object sender, EventArgs e)
        {
            pausa = false;
            if(video==null)
            {
                ImportacionVideo();
            }
            try
            {
            
                while(!pausa)
                {
                    Mat mFrame2 = new Mat();
                    Mat mFrame1 = new Mat();

                    
                    
                    video.Retrieve(mFrame1);
                video.Grab();
                video.Grab();
                video.Grab();
                video.Grab();
                //video.Grab();
                //video.Grab();
                //video.Grab();



                video.Read(mFrame2);


                frame = mFrame2.ToImage<Bgr,byte>();
                    frame = frame.Resize(pictureFrameVideo.Width,pictureFrameVideo.Height,Emgu.CV.CvEnum.Inter.Lanczos4);
                    
                    frame1 = mFrame1.ToImage<Gray, byte>();
                //frame1 = frame1.Resize(pictureFrameVideo.Width, pictureFrameVideo.Height, Emgu.CV.CvEnum.Inter.Lanczos4);
                //await Task.Delay(1000);
                frame2 = mFrame2.ToImage<Gray, byte>();
                    //frame2 = frame2.Resize(pictureFrameVideo.Width, pictureFrameVideo.Height, Emgu.CV.CvEnum.Inter.Lanczos4);
                        

                    DibujaBlobs(GeneraImagenBlob(frame2,frame1));
                    //pictureFrameVideo.Image = GeneraImagenBlob(frame2,frame1).Bitmap;


                    //Si la matriz está llena
                    if (!mFrame1.IsEmpty)
                    {
                    
                    pictureFrameVideo.Image = frame.ToBitmap();
                    //pictureFrameVideo.Image = GeneraImagenBlob(frame2, frame1).ToBitmap();
                    //    pictureFrameVideo.Image = mFrame2.Bitmap;
                    //    //pictureFrameVideo.Image = mFrame2.Bitmap;
                    //    //double fps = video.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps);
                    //    //await Task.Delay(1000/Convert.ToInt32( fps));
                        await Task.Delay(1000 / tiempo);
                    
                    }
                    else
                    {
                        video.Stop();
                        break;
                    }
                }
                

            }catch(Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            pausa = true;
        }


        #endregion

        #region GeneraImgBlob
        private Image<Gray,byte> GeneraImagenBlob(Image<Gray,byte> frameActual, Image<Gray,byte> frameAnterior)
        {
            Image<Gray, byte> imgOut = new Image<Gray, byte>(frameActual.Width,frameActual.Height);
            
            for(int i=0;i<frameActual.Height;i++)
            {
                for(int j=0;j<frameActual.Width;j++)
                {
                    imgOut[i, j] = new Gray(diferenciaPixel(frameAnterior[i, j].Intensity, frameActual[i, j].Intensity));
                }
            }
            imgOut = imgOut.ThresholdBinary(new Gray((int)numericUpDown1.Value), new Gray((int)numericUpDown2.Value));
            imgOut.Erode((int)numericErosion.Value);
            imgOut.Dilate((int)numericDilatacion.Value);
            imgOut.Resize(pictureFrameVideo.Width,pictureFrameVideo.Height, Emgu.CV.CvEnum.Inter.Lanczos4);
            return imgOut;    
        }
        #endregion

        #region DiferenciarPixel
        private double diferenciaPixel(double fondo, double frame)
        {
            double dif = 0;
            dif = fondo - frame;
            if (dif < 0)
                dif = 0;
            if (dif > 255)
                dif = 255;
            return dif;
        }

        #endregion

        #region DibujaBlobs
        private void DibujaBlobs(Image<Gray, byte> imgBlob)
        {

            Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint();
            Mat mat = new Mat();
            imgBlob = imgBlob.Resize(pictureFrameVideo.Width, pictureFrameVideo.Height, Emgu.CV.CvEnum.Inter.Lanczos4);
            CvInvoke.FindContours(imgBlob, contours, mat, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
            for (int i = 0; i < contours.Size; i++)
            {

                var area = CvInvoke.ContourArea(contours[i]);
                if (area > (int)numericTama.Value)
                {
                    Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);
                    CvInvoke.Rectangle(frame, rect, new MCvScalar(0, 0, 255), 2);

                    //papel = pictureFrameVideo.CreateGraphics();
                    //pluma.Width = 5;
                    //pluma.Color = Color.DarkBlue;
                    //papel.DrawRectangle(pluma, rect);
                }

            }



        }
        #endregion

        #region Tiempo
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            tiempo = trackBar1.Value;
            label2.Text =trackBar1.Value.ToString();
        }
        #endregion
    }
}
