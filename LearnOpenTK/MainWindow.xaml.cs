﻿using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using LearnOpenTK.Render;

using Size = System.Drawing.Size;

namespace LearnOpenTK
{
    public partial class MainWindow : Window
    {
        public static string HOME = @"C:\wisemountain\wise.scratchpad\LearnOpenTK";

        private WriteableBitmap backbuffer;

        private FrameBufferHandler framebufferHandler;

        private int frames;

        private DateTime lastMeasureTime;

        private Renderer renderer;

        private Scene scene = new Scene();

        private MeshRenderer meshRenderer = new MeshRenderer();

        public Render.Scene Scene {  get { return scene; } }

        public MainWindow()
        {
            this.InitializeComponent();

            this.renderer = new Renderer(new Size(400, 400));
            this.framebufferHandler = new FrameBufferHandler(scene);

            CameraInfo info = new CameraInfo();
            info.Position = new Vector3(500.0f, -500.0f, 500.0f);
            info.LookAt = new Vector3(0, 0, 0);
            info.Fov = MathHelper.PiOver4;
            info.Far = 10000.0f;
            info.Near = 2.0f;
            info.Width = 800;
            info.Height = 600;

            scene.SetupCamera(info);

            CreateSampleCube();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += this.TimerOnTick;
            timer.Start();
        } 

        private void Render()
        {
            if (this.image.ActualWidth <= 0 || this.image.ActualHeight <= 0)
            {
                return;
            }

            // Framebuffer 생성. Viewport 설정
			this.framebufferHandler.Prepare( new Size( (int)this.imageContainer.ActualWidth , (int)this.imageContainer.ActualHeight ) );

            scene.Draw(meshRenderer);

            GL.Finish();

            this.framebufferHandler.Cleanup(ref this.backbuffer);

            if (this.backbuffer != null)
            {
                this.image.Source = this.backbuffer;
            }

            this.frames++;
        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            if (DateTime.Now.Subtract(this.lastMeasureTime) > TimeSpan.FromSeconds(1))
            {
                this.Title = this.frames + "fps";
                this.frames = 0;
                this.lastMeasureTime = DateTime.Now;
            }

            this.Render();
        }

        private void CreateSampleCube()
        {
            ShaderManager.Instance.Load(
                new ShaderManager.ShaderConf() { 
                    Name = "diffuse", 
                    VsPath = $"{HOME}/Assets/Shader/diffuse.vert", 
                    FsPath = $"{HOME}/Assets/Shader/diffuse.frag" 
                });

            var mat = new MaterialDiffuse() { 
                ShaderProgram = "diffuse", 
                Tex = $"{HOME}/Assets/Tex/penguine.jpg" 
            };

            var meshCube = Shape.CreateCube();
            var meshPlane = Shape.CreatePlane();

            // cube
            {
                var sn = new Scene.Node() { Name = "sampleCube", Material = mat, Mesh = meshCube };
                //scene.Add(sn);

                sn.Transform.Position = new Vector3(0, 0, 0);
                sn.Transform.Scale = new Vector3(100, 100, 50);
                sn.Transform.Update();
            }

            // plane 
            {
                var sn = new Scene.Node() { Name = "samplePlane", Material = mat, Mesh = meshPlane};
                scene.Add(sn);

                sn.Transform.Position = new Vector3(0, 0, 20);
                sn.Transform.Scale = new Vector3(300, 300, 50);
                sn.Transform.Update();
            }
        }
    }
}