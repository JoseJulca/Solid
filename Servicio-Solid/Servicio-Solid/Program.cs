using System;
using SinSolid;


class Program
{
    static void Main()
    {
        Console.WriteLine("=== Servicio Reserva de Cancha ===");
        Console.WriteLine("1. Ejecutar versión SIN SOLID");
        Console.WriteLine("2. Ejecutar versión CON SOLID");
        Console.Write("Elige una opción: ");

        string opcion = Console.ReadLine();

        switch (opcion)
        {
            case "1":
                SinSolid.ProgramaSinSolid.Ejecutar();
                break;
            case "2":
                ConSolid.ProgramaConSolid.Ejecutar();
                break;
            default:
                Console.WriteLine("Opción no válida.");
                break;
        }
    }


}