using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicio_Solid
{
    public  class SistemaReservaCanchaSinSolid
    {
        public SistemaReservaCanchaSinSolid()
        {

        }

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

            // 3. Aplicar descuento según tipo de cliente


            // 4. Guardar la reserva DIRECTAMENTE en SQL Server


            // 5. Enviar notificación


            // 6. Mostrar resumen

        }
    }
}
