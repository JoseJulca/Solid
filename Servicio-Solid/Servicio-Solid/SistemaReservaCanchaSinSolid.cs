using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinSolid
{
    public class SistemaReservaCanchaSinSolid
    {
        private string cadenaConexion = "Server=localhost\\SQLEXPRESS;Database=ReservaCanchaDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public void RealizarReserva(string nombreCliente, string tipoCancha, int horas, string tipoCliente, string metodoNotificacion)
        {
            // 1. Validación
            if (string.IsNullOrEmpty(nombreCliente))
            {
                Console.WriteLine("Error: el nombre del cliente es requerido");
                return;
            }

            // 2. Cálculo del precio según tipo de cancha
            double precioPorHora;
            if (tipoCancha == "Cancha5")
            {
                precioPorHora = 50;
            }
            else if (tipoCancha == "Cancha7")
            {
                precioPorHora = 80;
            }
            else if (tipoCancha == "Cancha11")
            {
                precioPorHora = 120;
            }
            else
            {
                Console.WriteLine("Error: tipo de cancha no válido");
                return;
            }

            double precioTotal = precioPorHora * horas;

            // 3. Aplicar descuento según tipo de cliente
            if (tipoCliente == "Estudiante")
            {
                precioTotal *= 0.9;
            }
            else if (tipoCliente == "Frecuente")
            {
                precioTotal *= 0.85;
            }
            else if (tipoCliente == "VIP")
            {
                precioTotal *= 0.7;
            }

            // 4. Guardar la reserva directamente en SQL Server
            string sql = @"
                INSERT INTO Reservas (NombreCliente, TipoCancha, Horas, PrecioTotal)
                VALUES (@NombreCliente, @TipoCancha, @Horas, @PrecioTotal);";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@NombreCliente", nombreCliente);
                comando.Parameters.AddWithValue("@TipoCancha", tipoCancha);
                comando.Parameters.AddWithValue("@Horas", horas);
                comando.Parameters.AddWithValue("@PrecioTotal", precioTotal);

                try
                {
                    conexion.Open();
                    comando.ExecuteNonQuery();
                    Console.WriteLine("Reserva guardada.");
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"Error al guardar: {ex.Message}");
                }
            }

            // 5. Enviar notificación
            if (metodoNotificacion == "Email")
            {
                Console.WriteLine($"[Email] Para {nombreCliente}: Reserva confirmada. Total: S/. {precioTotal}");
            }
            else if (metodoNotificacion == "SMS")
            {
                Console.WriteLine($"[SMS] Para {nombreCliente}: Reserva confirmada. Total: S/. {precioTotal}");
            }
            else
            {
                Console.WriteLine("Método de notificación no reconocido, no se envió aviso.");
            }

            // 6. Mostrar resumen
            Console.WriteLine($"Reserva realizada: {nombreCliente} - {tipoCancha} - {horas}h - Total: S/. {precioTotal}");
        }
    }

    public class ProgramaSinSolid
    {
        public static void Ejecutar()
        {
            var sistema = new SistemaReservaCanchaSinSolid();
            sistema.RealizarReserva("Juan Perez", "Cancha7", 2, "VIP", "Email");
        }
    }
}
