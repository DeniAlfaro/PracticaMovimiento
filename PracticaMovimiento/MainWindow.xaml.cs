using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

// librerias para multiprocesamiento
using System.Threading;
using System.Diagnostics;

namespace PracticaMovimiento
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Stopwatch stopwatch;
        TimeSpan tiempoAnterior;

        public MainWindow()
        {
            InitializeComponent();
            miCanvas.Focus();

            stopwatch = new Stopwatch();
            stopwatch.Start();
            tiempoAnterior = stopwatch.Elapsed;
            
            // 1. establecer instrucciones
            ThreadStart threadStart = new ThreadStart(moverEnemigos);
            // 2. inicializar el Thread
            Thread threadMoverEnemigos = new Thread(threadStart);
            // 3. ejecutar el Thread
            threadMoverEnemigos.Start();

        }

        void moverEnemigos()
        {
            while (true)
            {
                Dispatcher.Invoke(
                () =>
                {
                    var tiempoActual = stopwatch.Elapsed;
                    var deltaTime = tiempoActual - tiempoAnterior;

                    double leftTumblrActual = Canvas.GetLeft(imgTumblr);
                    Canvas.SetLeft(imgTumblr, leftTumblrActual - (90 * deltaTime.TotalSeconds));
                    if(Canvas.GetLeft(imgTumblr) <= -100)
                    {
                        Canvas.SetLeft(imgTumblr, 800);
                    }
                    tiempoAnterior = tiempoActual;
                }
                );
            }
        }

        private void miCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                double topCelularActual = Canvas.GetTop(imgCelular);
                Canvas.SetTop(imgCelular, topCelularActual - 15);
            }
            if (e.Key == Key.Down)
            {
                double bottomCelularActual = Canvas.GetTop(imgCelular);
                Canvas.SetTop(imgCelular, bottomCelularActual + 15);
            }
            if (e.Key == Key.Left)
            {
                double leftCelularActual = Canvas.GetLeft(imgCelular);
                Canvas.SetLeft(imgCelular, leftCelularActual - 15);
            }
            if (e.Key == Key.Right)
            {
                double rightCelularActual = Canvas.GetLeft(imgCelular);
                Canvas.SetLeft(imgCelular, rightCelularActual + 15);
            }
        }
    }
}
