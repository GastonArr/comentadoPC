using AdminEmpleadosDatos;
using AdminEmpleadosEntidades;

namespace AdminEmpleadosNegocio
{
    public class DepartamentosNegocio// aca no tengo ninguna regla de negocio pero si la tuviera la pongo aca
    {
        public static List<Departamento> Get()
        {
            return DepartamentoDatosEF.Get(); // vamos a la capa de datos.
        }
    }
}
