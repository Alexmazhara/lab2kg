using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Markup;

namespace laba2_kg
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        int FrameCount;
        DateTime NextFPSUpdate = DateTime.Now.AddSeconds(1);
        void displayFPS()
        {
            if (DateTime.Now >= NextFPSUpdate)
            {
                this.Text = String.Format("CT Visualizer (fps={0})", FrameCount);
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
                FrameCount = 0;
            }
            FrameCount++;
        }


        class Bin
        {
            public static int X, Y, Z;
            public static short[] array;
            public Bin() { }
            public Int32 Zer() { return Z; }
            public void readBIN(string path)
            {
                if (File.Exists(path))
                {
                    BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open));
                    X = reader.ReadInt32();
                    Y = reader.ReadInt32();
                    Z = reader.ReadInt32();
                    int arraySize = X * Y * Z;
                    array = new short[arraySize + 1];
                    for (int i = 0; i < arraySize; i++)
                        array[i] = reader.ReadInt16();

                }
            }
        }
        /*
        class View
        {
            public void SetupView(int width, int height)
            {
                GL.ShadeModel(ShadingModel.Smooth);
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.Ortho(0, Bin.X, 0, Bin.Y, -1, -1);
                GL.Viewport(0, 0, width, height);
            }

            public void DrawQuads(int layerNumber)
            {
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                GL.Begin(BeginMode.Quads);

                var query = Bin.array
                    .Skip(layerNumber * Bin.X * Bin.Y)
                    .Take(Bin.X * Bin.Y)
                    .Select((value, index) =>
                    {
                        var x_coord = index % Bin.X;
                        var y_coord = index / Bin.X;
                        return (x_coord, y_coord, value);
                    });

                foreach (var (x_coord, y_coord, value) in query)
                {
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(x_coord, y_coord);
                }

                GL.End();
            }

            Color TransferFunction(short value)
            {
                const int min = 0;
                const int max = 2000;
                int newVal = Math.Min(Math.Max((value - min) * 255 / (max - min), 0), 255);
                return Color.FromArgb(255, newVal, newVal, newVal);
            }
        }
        */

        class View
        {
            public void SetupView(int wid, int hei)
            {
                GL.ShadeModel(ShadingModel.Smooth);
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.Ortho(0, Bin.X, Bin.Y, 0, -1, 1);
                GL.Viewport(0, 0, wid, hei);
            }
            public void DrawQuads(int layerNumber)
            {
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                GL.Begin(BeginMode.Quads);
                for (int x_coord = 0; x_coord < Bin.X - 1; x_coord++)
                    for (int y_coord = 0; y_coord < Bin.Y - 1; y_coord++)
                    {
                        short value;
                        //1vershina
                        value = Bin.array[x_coord + y_coord * Bin.X + layerNumber * Bin.X * Bin.Y];
                        GL.Color3(TransferFunction(value));
                        GL.Vertex2(x_coord, y_coord);
                        //2vershina
                        value = Bin.array[x_coord + (y_coord + 1) * Bin.X + layerNumber * Bin.X * Bin.Y];
                        GL.Color3(TransferFunction(value));
                        GL.Vertex2(x_coord, y_coord + 1);
                        //33vershina
                        value = Bin.array[x_coord + 1 + (y_coord + 1) * Bin.X + layerNumber * Bin.X * Bin.Y];
                        GL.Color3(TransferFunction(value));
                        GL.Vertex2(x_coord + 1, y_coord + 1);
                        //4vershina
                        value = Bin.array[x_coord + 1 + (y_coord) * Bin.X + layerNumber * Bin.X * Bin.Y];
                        GL.Color3(TransferFunction(value));
                        GL.Vertex2(x_coord + 1, y_coord);
                    }
                GL.End();
            }
            public int Clamp(int val, int min, int max)
            {
                if (val < min)
                    return min;
                if (val > max)
                    return max;
                return val;
            }
            public Color TransferFunction(short val)
            {
                int min = 0;
                int max = 2000;
                int newVal = Clamp((val - min) * 255 / (max - min), 0, 255);
                return Color.FromArgb(255, newVal, newVal, newVal);
            }


            Bitmap textureImage;
            int VBOtexture;
            public void Load2DTexture()
            {
                GL.BindTexture(TextureTarget.Texture2D, VBOtexture);
                BitmapData data = textureImage.LockBits(new System.Drawing.Rectangle(0, 0, textureImage.Width, textureImage.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                    data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                    PixelType.UnsignedByte, data.Scan0);
                textureImage.UnlockBits(data);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                    (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                    (int)TextureMinFilter.Linear);
                ErrorCode Er = GL.GetError();
                string str = Er.ToString();

            }

            public void generateTextureImage(int layerNumber)
            {
                textureImage = new Bitmap(Bin.X, Bin.Y);
                for (int i = 0; i < Bin.X; ++i)
                    for (int j = 0; j < Bin.Y; ++j)
                    {
                        int PixelNumber = i + j * Bin.X + layerNumber * Bin.X * Bin.Y;
                        textureImage.SetPixel(i, j, TransferFunction(Bin.array[PixelNumber]));
                    }
            }

            public void DrawTexture()
            {
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                GL.Enable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, VBOtexture);
                GL.Begin(BeginMode.Quads);
                GL.Color3(Color.White);
                GL.TexCoord2(0f, 0f);
                GL.Vertex2(0, 0);
                GL.TexCoord2(0f, 1f);
                GL.Vertex2(0, Bin.Y);
                GL.TexCoord2(1f, 1f);
                GL.Vertex2(Bin.X, Bin.Y);
                GL.TexCoord2(1f, 0f);
                GL.Vertex2(Bin.X, 0);
                GL.End();
                GL.Disable(EnableCap.Texture2D);
            }

        }
        

        private void InitializeComponent()
        {
            this.glControl3 = new OpenTK.GLControl();
            this.button1 = new System.Windows.Forms.Button();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.trackBar2 = new System.Windows.Forms.TrackBar();
            this.trackBar3 = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).BeginInit();
            this.SuspendLayout();
            // 
            // glControl3
            // 
            this.glControl3.BackColor = System.Drawing.Color.Black;
            this.glControl3.Location = new System.Drawing.Point(129, 58);
            this.glControl3.Name = "glControl3";
            this.glControl3.Size = new System.Drawing.Size(478, 249);
            this.glControl3.TabIndex = 0;
            this.glControl3.VSync = false;
            this.glControl3.Load += new System.EventHandler(this.glControl3_Load);
            this.glControl3.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl3_Paint_1);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(13, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "open";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(13, 313);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(727, 45);
            this.trackBar1.TabIndex = 2;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(613, 18);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(117, 17);
            this.radioButton1.TabIndex = 3;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "radioButton1 quads";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(613, 42);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(120, 17);
            this.radioButton2.TabIndex = 4;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "radioButton2 texture";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // trackBar2
            // 
            this.trackBar2.Location = new System.Drawing.Point(13, 364);
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Size = new System.Drawing.Size(727, 45);
            this.trackBar2.TabIndex = 5;
            this.trackBar2.Scroll += new System.EventHandler(this.trackBar2_Scroll);
            // 
            // trackBar3
            // 
            this.trackBar3.Location = new System.Drawing.Point(13, 427);
            this.trackBar3.Name = "trackBar3";
            this.trackBar3.Size = new System.Drawing.Size(727, 45);
            this.trackBar3.TabIndex = 6;
            this.trackBar3.Scroll += new System.EventHandler(this.trackBar3_Scroll);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(752, 473);
            this.Controls.Add(this.trackBar3);
            this.Controls.Add(this.trackBar2);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.glControl3);
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

       
        bool loaded = false;
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;
                Bin bin = new Bin();
                bin.readBIN(str);
                View view = new View();
                view.SetupView(glControl3.Width, glControl3.Height);
                loaded = true;
                glControl3.Invalidate();
                trackBar1.Maximum = bin.Zer() - 1;
            }
        }
       


        private void glControl3_Load(object sender, EventArgs e)
        {

        }

        int currentLayer = 0;
        bool needReload = false;
        bool quads = true;
        private void glControl3_Paint_1(object sender, PaintEventArgs e)
        {
            if (loaded)
            {

                View view = new View();
                if (quads == false)
                {
                    if (needReload)
                    {
                        view.generateTextureImage(currentLayer);
                        view.Load2DTexture();
                        needReload = false;
                    }
                    view.DrawTexture();
                }
                else
                    view.DrawQuads((int)currentLayer);

                glControl3.SwapBuffers();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
            needReload = true;
        }

        void Application_Idle(object sender, EventArgs e)
        {
            while (glControl3.IsIdle)
            {
                displayFPS();
                glControl3.Invalidate();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            quads = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            quads = false;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {

        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {

        }
    }
}


