using AdminEmpleadosDatos;
using AdminEmpleadosEntidades;

namespace AdminEmpleadosNegocio
{
    public class EmpleadosNegocio // Esta clase no tiene propiedades, solo tiene comportamientos
    {
        public static List<Empleado> Get(Empleado e)
        {
            return EmpleadosDatosEF.Get(e);
        }

        public static int DeleteAnulados()
        {
            try
            {
                //delegamos la eliminacion definitiva a la capa de datos y devolvemos la cantidad borrada
                return EmpleadosDatosEF.DeleteAnulados();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static int Insert(Empleado e) // en el insert me trae aca
        { // con sus validaciones
            if (String.IsNullOrEmpty(e.Nombre)) // que el nombre no este vacio 
            {
                return 0; // si esta vacio retorno un 0 y asi con todo
            }
            if (String.IsNullOrEmpty(e.Dni))
            {
                return 0;
            }
            if (e.FechaIngreso == null)
            {
                e.FechaIngreso = DateTime.Now;
            }

            try
            {
                return EmpleadosDatosEF.Insert(e); //si va todo bien llamo a la capa de datos con el objeto que recibi (e)       
            }
            catch (Exception)
            {
                throw;
            }

        }

        public static bool Update(Empleado e) // para hacer una validaciones osea REGLAS DE NEGOCIO 
        {
            if (String.IsNullOrEmpty(e.Nombre))
            {
                return false;
            }
            if (String.IsNullOrEmpty(e.Dni))
            {
                return false;
            }
            if (e.FechaIngreso == null)
            {
                e.FechaIngreso = DateTime.Now;
            }

            try    // Si todo esta bien llamo a los datos
            {
                return EmpleadosDatosEF.Update(e); // lamo a la capa de datos        
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool Anular(int id) // aca puedo hacer algunas validaciones pero no hay nada
        {
            try
            {
                return EmpleadosDatosEF.Anular(id); // llamo la capa de datos
            }
            catch (Exception)
            {
                throw;
            }
 
        }
    }
}
