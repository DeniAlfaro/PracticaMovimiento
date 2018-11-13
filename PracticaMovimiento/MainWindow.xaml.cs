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

        enum EstadoJuego { Gameplay, Gameover};
        EstadoJuego estadoActual = EstadoJuego.Gameplay;

        enum Direccion { Arriba, Abajo, Izquierda, Derecha, Ninguna };
        Direccion direccionJugador = Direccion.Ninguna;

        double velocidadCelular = 80;

        public MainWindow()
        {
            InitializeComponent();
            miCanvas.Focus();

            stopwatch = new Stopwatch();
            stopwatch.Start();
            tiempoAnterior = stopwatch.Elapsed;
            
            // 1. establecer instrucciones
            ThreadStart threadStart = new ThreadStart(actualizar);
            // 2. inicializar el Thread
            Thread threadMoverEnemigos = new Thread(threadStart);
            // 3. ejecutar el Thread
            threadMoverEnemigos.Start();

        }

        void moverJugador(TimeSpan deltaTime)
        {
            double topCelularActual = Canvas.GetTop(imgCelular);
            double leftCelularActual = Canvas.GetLeft(imgCelular);
            switch (direccionJugador)
            {
                case Direccion.Arriba:
                    Canvas.SetTop(imgCelular, topCelularActual - (velocidadCelular * deltaTime.TotalSeconds));
                    break;
                case Direccion.Abajo:
                    Canvas.SetTop(imgCelular, topCelularActual + (velocidadCelular * deltaTime.TotalSeconds));
                    break;
                case Direccion.Izquierda:
                    //para que no se pase de la pantalla
                    if (leftCelularActual - (velocidadCelular * deltaTime.TotalSeconds) >= 0)
                    {
                        Canvas.SetLeft(imgCelular, leftCelularActual - (velocidadCelular * deltaTime.TotalSeconds));
                    }
                    break;
                case Direccion.Derecha:
                    double nuevaPosicion = leftCelularActual + (velocidadCelular * deltaTime.TotalSeconds);
                    if (nuevaPosicion + imgCelular.Width <= 800)
                    {
                        Canvas.SetLeft(imgCelular, nuevaPosicion);
                    }
                    break;
                case Direccion.Ninguna:
                    break;
                default:
                    break;
            }
        }

        void actualizar()
        {
            while (true)
            {
                Dispatcher.Invoke(
                () =>
                {
                    var tiempoActual = stopwatch.Elapsed;
                    var deltaTime = tiempoActual - tiempoAnterior;

                    //para hacerlo mas rapido tmb conforme pasa el juego
                    //velocidadCelular += 10 * deltaTime.TotalSeconds;

                    if (estadoActual == EstadoJuego.Gameplay)
                    {
                        moverJugador(deltaTime);
                        double leftTumblrActual = Canvas.GetLeft(imgTumblr);
                        Canvas.SetLeft(imgTumblr, leftTumblrActual - (90 * deltaTime.TotalSeconds));

                        if (Canvas.GetLeft(imgTumblr) <= -100)
                        {
                            Canvas.SetLeft(imgTumblr, 800);
                        }
                        //interseccion en x
                        double xTumblr = Canvas.GetLeft(imgTumblr);
                        double xCelular = Canvas.GetLeft(imgCelular);

                        if (xCelular + imgCelular.Width >= xTumblr && xCelular <= xTumblr + imgTumblr.Width)
                        {
                            lblinterseccionX.Text = "SI HAY INTERSECCION EN X!!!";
                        }
                        else
                        {
                            lblinterseccionX.Text = "No hay interseccion en X";
                        }
                        //Intersección en Y
                        double yTumblr = Canvas.GetTop(imgTumblr);
                        double yCelular = Canvas.GetTop(imgCelular);

                        if (yCelular + imgCelular.Height >= yTumblr && yCelular <= yTumblr + imgTumblr.Height)
                        {
                            lblinterseccionY.Text = "SI HAY INTERSECCION EN Y!!!";
                        }
                        else
                        {
                            lblinterseccionY.Text = "No hay interseccion en Y";
                        }
                        if (xCelular + imgCelular.Width >= xTumblr && xCelular <= xTumblr + imgTumblr.Width &&
                            yCelular + imgCelular.Height >= yTumblr && yCelular <= yTumblr + imgTumblr.Height)
                        {
                            lblcolision.Text = "HAY COLISION!";
                            estadoActual = EstadoJuego.Gameover;
                            miCanvas.Visibility = Visibility.Collapsed;
                            canvasGameOver.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            lblcolision.Text = "No hay colision";
                        }

                    } else if (estadoActual == EstadoJuego.Gameover)
                    {

                    }

                    tiempoAnterior = tiempoActual;
                }
                );
            }
        }

        private void miCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (estadoActual == EstadoJuego.Gameplay) {
                if (e.Key == Key.Up)
                {
                    direccionJugador = Direccion.Arriba;
                }
                if (e.Key == Key.Down)
                {
                    direccionJugador = Direccion.Abajo;
                }
                if (e.Key == Key.Left)
                {
                    direccionJugador = Direccion.Izquierda;
                }
                if (e.Key == Key.Right)
                {
                    direccionJugador = Direccion.Derecha;
                }
            }
        }

        //para que se mueva y en cuanto soltemos el boton deje de moverse
        private void miCanvas_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up && direccionJugador == Direccion.Arriba)
            {
                direccionJugador = Direccion.Ninguna;
            }
            if (e.Key == Key.Down && direccionJugador == Direccion.Abajo)
            {
                direccionJugador = Direccion.Ninguna;
            }
            if (e.Key == Key.Left && direccionJugador == Direccion.Izquierda)
            {
                direccionJugador = Direccion.Ninguna;
            }
            if (e.Key == Key.Right && direccionJugador == Direccion.Derecha)
            {
                direccionJugador = Direccion.Ninguna;
            }
        }
    }
}
