using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Data;
using System.IO;
using System.Threading;

namespace NotificacionPostgreSQL
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread HiloS = new Thread(SuscribirseALosCambiosDeLaTablaListado);
            HiloS.Start();
            Console.WriteLine("Ahh perro!!!");
            string entrada = Console.ReadLine();
        }

        public static void SuscribirseALosCambiosDeLaTablaListado()
        {
            const string connString = "host=localhost;Database=db;Username=postgres;Password=pass";
            using (var conn=new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("LISTEN datachange", conn))
                {
                    cmd.ExecuteNonQuery();
                    conn.Notification += Personas_Cambio;
                }
                while (true)
                    conn.Wait();
            }

        }

        public static void Personas_Cambio(object sender, NpgsqlNotificationEventArgs e)
        {
            string mensaje = ObtenerMensajeAMostrar(e);
            Console.WriteLine(mensaje);
            Console.WriteLine(e.Payload);
            SuscribirseALosCambiosDeLaTablaListado();
        }

        public static string ObtenerMensajeAMostrar(NpgsqlNotificationEventArgs e)
        {
            string actionName = e.Payload;
            if (actionName.Contains("DELETE"))
                return "delete";
            else if (actionName.Contains("UPDATE"))
                return "update";
            else if (actionName.Contains("INSERT"))
                return "insert";
            return "";
        }


    }
}
