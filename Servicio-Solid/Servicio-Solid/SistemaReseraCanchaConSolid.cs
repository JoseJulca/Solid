using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace ConSolid
{
    // =========================================================
    // MODELO
    // =========================================================
    public class Reserva
    {
        public string NombreCliente { get; set; }
        public string TipoCancha { get; set; }
        public int Horas { get; set; }
        public double PrecioTotal { get; set; }
    }


    // =========================================================
    // (OCP) Abstracción de canchas
    // =========================================================
    public interface ICancha
    {
        string Nombre { get; }
        double PrecioPorHora { get; }
    }

    public class Cancha5 : ICancha
    {
        public string Nombre => "Cancha 5";
        public double PrecioPorHora => 50;
    }

    public class Cancha7 : ICancha
    {
        public string Nombre => "Cancha 7";
        public double PrecioPorHora => 80;
    }

    public class Cancha11 : ICancha
    {
        public string Nombre => "Cancha 11";
        public double PrecioPorHora => 120;
    }


    // =========================================================
    // (OCP) Estrategia de descuento
    // =========================================================
    public interface IEstrategiaDescuento
    {
        double AplicarDescuento(double precio);
    }

    public class SinDescuento : IEstrategiaDescuento
    {
        public double AplicarDescuento(double precio) => precio;
    }

    public class DescuentoEstudiante : IEstrategiaDescuento
    {
        public double AplicarDescuento(double precio) => precio * 0.9;
    }

    public class DescuentoFrecuente : IEstrategiaDescuento
    {
        public double AplicarDescuento(double precio) => precio * 0.85;
    }

    public class DescuentoVIP : IEstrategiaDescuento
    {
        public double AplicarDescuento(double precio) => precio * 0.7;
    }

    // =========================================================
    // (ISP + DIP) Notificaciones
    // =========================================================
    public interface INotificador
    {
        void Enviar(string destinatario, string mensaje);
    }

    public class EmailNotificador : INotificador
    {
        public void Enviar(string destinatario, string mensaje)
        {
            Console.WriteLine($"[Email] Para {destinatario}: {mensaje}");
        }
    }

    public class SmsNotificador : INotificador
    {
        public void Enviar(string destinatario, string mensaje)
        {
            Console.WriteLine($"[SMS] Para {destinatario}: {mensaje}");
        }
    }

    // =========================================================
    // (DIP) Persistencia
    // =========================================================
    public interface IRepositorioReservas
    {
        void Guardar(Reserva reserva);
    }

    public class ArchivoRepositorioReservas : IRepositorioReservas
    {
        private readonly string _ruta;

        public ArchivoRepositorioReservas(string ruta)
        {
            _ruta = ruta;
        }

        public void Guardar(Reserva reserva)
        {
            string linea = $"{reserva.NombreCliente},{reserva.TipoCancha},{reserva.Horas},{reserva.PrecioTotal}";
            System.IO.File.AppendAllText(_ruta, linea + Environment.NewLine);
        }
    }

    public class MemoriaRepositorioReservas : IRepositorioReservas
    {
        private readonly List<Reserva> _reservas = new();

        public void Guardar(Reserva reserva)
        {
            _reservas.Add(reserva);
            Console.WriteLine("Reserva guardada en memoria.");
        }
    }

    public class SqlServerRepositorioReservas : IRepositorioReservas
    {
        private readonly string _cadenaConexion;

        public SqlServerRepositorioReservas(string cadenaConexion)
        {
            _cadenaConexion = cadenaConexion;
        }

        public void Guardar(Reserva reserva)
        {
            const string sql = @"
                INSERT INTO Reservas (NombreCliente, TipoCancha, Horas, PrecioTotal)
                VALUES (@NombreCliente, @TipoCancha, @Horas, @PrecioTotal);";

            using (SqlConnection conexion = new SqlConnection(_cadenaConexion))
            using (SqlCommand comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@NombreCliente", reserva.NombreCliente);
                comando.Parameters.AddWithValue("@TipoCancha", reserva.TipoCancha);
                comando.Parameters.AddWithValue("@Horas", reserva.Horas);
                comando.Parameters.AddWithValue("@PrecioTotal", reserva.PrecioTotal);

                try
                {
                    conexion.Open();
                    comando.ExecuteNonQuery();
                    Console.WriteLine("Reserva guardada correctamente");
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"Error al guardar: {ex.Message}");
                }
            }
        }
    }

    // =========================================================
    // (SRP) Cálculo de precios
    // =========================================================
    public class CalculadoraPrecio
    {
        public double Calcular(ICancha cancha, int horas, IEstrategiaDescuento descuento)
        {
            double precioBase = cancha.PrecioPorHora * horas;
            return descuento.AplicarDescuento(precioBase);
        }
    }

    // =========================================================
    // (SRP + DIP) Orquestador principal
    // =========================================================
    public class GestorReservas
    {
        private readonly CalculadoraPrecio _calculadora;
        private readonly IRepositorioReservas _repositorio;
        private readonly INotificador _notificador;

        public GestorReservas(CalculadoraPrecio calculadora, IRepositorioReservas repositorio, INotificador notificador)
        {
            _calculadora = calculadora;
            _repositorio = repositorio;
            _notificador = notificador;
        }

        public void RealizarReserva(string nombreCliente, ICancha cancha, int horas, IEstrategiaDescuento descuento)
        {
            if (string.IsNullOrWhiteSpace(nombreCliente))
            {
                Console.WriteLine("Error: el nombre del cliente es requerido");
                return;
            }

            double total = _calculadora.Calcular(cancha, horas, descuento);

            var reserva = new Reserva
            {
                NombreCliente = nombreCliente,
                TipoCancha = cancha.Nombre,
                Horas = horas,
                PrecioTotal = total
            };

            _repositorio.Guardar(reserva);
            _notificador.Enviar(nombreCliente, $"Reserva confirmada en {cancha.Nombre} por S/. {total}");

            Console.WriteLine($"Reserva realizada: {reserva.NombreCliente} - {reserva.TipoCancha} - {reserva.Horas}h - Total: S/. {reserva.PrecioTotal}");
        }
    }

    public class ProgramaConSolid
    {
        public static void Ejecutar()
        {
            var calculadora = new CalculadoraPrecio();
            var notificador = new EmailNotificador();

            string cadenaConexion =
                "Server=localhost\\SQLEXPRESS;Database=ReservaCanchaDB;Trusted_Connection=True;TrustServerCertificate=True;";

            IRepositorioReservas repositorio = new SqlServerRepositorioReservas(cadenaConexion);

            var gestor = new GestorReservas(calculadora, repositorio, notificador);

            gestor.RealizarReserva("Juan Perez", new Cancha7(), 2, new DescuentoVIP());
            gestor.RealizarReserva("Maria Lopez", new Cancha5(), 1, new DescuentoEstudiante());
        }
    }
}
