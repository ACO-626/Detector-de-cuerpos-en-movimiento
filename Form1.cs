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

        VideoCapture video;
        bool pausa = false;

        public Form1()
        {
            InitializeComponent();
        }

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
                        double fps = video.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps);
                        //await Task.Delay(1000/Convert.ToInt32( fps));
                        await Task.Delay(1000 / 60);
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

        
    }
}
