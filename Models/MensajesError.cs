using MySqlConnector;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public static class MensajesError
    {
        public static string AlGuardar(Exception ex, string mensajeDuplicado)
        {
            if (ex is MySqlException mysql && mysql.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
            {
                return mensajeDuplicado;
            }

            return "Ocurrió un error al guardar. Verifique los datos e intente nuevamente.";
        }

        public static string AlEliminar()
        {
            return "No se pudo completar la baja. Intente nuevamente.";
        }
    }
}
