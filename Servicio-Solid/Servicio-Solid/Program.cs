using System;
using Servicio_Solid;

class Program
{
    static void Main(string[] args)
    {
        var sistema = new SistemaReservaCanchaSinSolid();
        sistema.RealizarReserva("Juan Perez", "Cancha7", 2, "VIP", "Email");
    }

   
}