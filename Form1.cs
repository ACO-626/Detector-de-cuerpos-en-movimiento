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
        Image<Gray, byte> frame1;
        Image<Gray, byte> frame2;
        Image<Gray, byte> imgOut;

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
                pictureFrameVideo.Image = matriz.Bitmap;
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
                    Mat matriz = new Mat();
                    video.Read(matriz);
                    //Si la matriz está llena
                    if (!matriz.IsEmpty)
                    {
                        pictureFrameVideo.Image = matriz.Bitmap;
                        //double fps = video.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps);
                        //await Task.Delay(1000/Convert.ToInt32( fps));
                        await Task.Delay(1000 / tiempo);
                    }
                    else
                    {
                        break;
                    }
                }
                

            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            pausa = true;
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
