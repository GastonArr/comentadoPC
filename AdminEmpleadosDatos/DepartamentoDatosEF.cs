using AdminEmpleadosEF;
using AdminEmpleadosEntidades;

namespace AdminEmpleadosDatos
{
    public  class DepartamentoDatosEF
    {
        static AdminEmpleadosDBContext? empleadosContext;

        public static List<Departamento> Get()
        {
            empleadosContext = new AdminEmpleadosDBContext(); // creo el contexto

            if (empleadosContext.departamento == null)
            {
                return new List<Departamento>();
            }
            
            List<Departamento> list = empleadosContext.departamento.ToList();// devuelvo la lista

            return list; // vuelvo hasta el front
        }
    }
}
