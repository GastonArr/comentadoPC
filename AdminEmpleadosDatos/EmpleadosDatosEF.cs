using AdminEmpleadosEF;
using AdminEmpleadosEntidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AdminEmpleadosDatos
{
    public static class EmpleadosDatosEF
    {
        static AdminEmpleadosDBContext? empleadosContext;

        public static List<Empleado> Get(Empleado e)
        {
            empleadosContext = new AdminEmpleadosDBContext();

            if (empleadosContext.empleado == null)
            {
                return new List<Empleado>();
            }
            //Lazy Loading
            //List<Empleado> list = empleadosContext.empleado.ToList(); //sin departamentos

            List<Empleado> list;
            if (String.IsNullOrWhiteSpace(e.Nombre) && String.IsNullOrWhiteSpace(e.Dni))
            {
                list = empleadosContext.empleado.Include("Departamento")
                    //filtra segun el valor del parametro anulado enviado desde la capa de presentacion
                    .Where(emp => emp.anulado == e.anulado)
                    .ToList();
            }
            else
            {
                
                /*
                //con warnings, va a dar excepcion si nombre o dni estan nulos en la BD
                list = empleadosContext.empleado.Include("Departamento").Where(i =>
                    i.Nombre.Contains(e.Nombre)
                    ||
                    i.Dni.Contains(e.Dni)
                    ).ToList();
                */

                //? operador ternario (es como un IF-ELSE) 
                //?? operador de fusion de null (Asigna un valor cuando es NULL la variable de la izquierda)                
                list = empleadosContext.empleado.Include("Departamento").Where(i =>
                    (i.Nombre != null ? i.Nombre.Contains(e.Nombre ?? "") : true)
                    ||
                    (i.Dni != null ? i.Dni.Contains(e.Dni ?? "") : true)
                    )
                    //aplico nuevamente el filtro por anulados cuando el usuario esta buscando por nombre o dni // CODIGO A EXPLICAR ACA VEO LOS ANULADOS
                    .Where(emp => emp.anulado == e.anulado) // ACA VEO LOS ANULADOS Y ANTE SOLAMENTE LOS FALSOS
                    .ToList();
            }


            return list;
        }

        public static int Insert(Empleado e) //insert no busco nada en la base porque es un insert
        {
            empleadosContext = new AdminEmpleadosDBContext();// creo mi contexto de base de datos

            if (empleadosContext == null) // me fijo que sea nulo por las dudas o si no EF lo va a tomar como si fuera un update
            {
                return 0;
            }
            //seteo el ID en null para que realice el insert porque si tiene otro valor EF lo toma como un update
            e.EmpleadoId = null; // me asiguro que el id sea nulo por si aca
            e.anulado = false;
            empleadosContext.Add(e); // hago referencia al contexto y le paso el objeto (e) con los valores que vienen cargados
            empleadosContext.SaveChanges(); // y se guarda
            if (e.EmpleadoId == null)// una vez que se guarda en la BD son el SaveChanges me genera el ID nuevo
                return 0;// y me retorma el id entero generado 

            return (int)e.EmpleadoId;// se genero un nuevo ID

        }

        public static bool Update(Empleado e) // modificacion (Empleado e) contiene el Id que quiero modificar
        {
            empleadosContext = new AdminEmpleadosDBContext(); // base de datos
            // para hacer un update

            //paso 1: buscar el objeto que queremos actualizar

            var empleadoBD = empleadosContext.empleado.FirstOrDefault(c => c.EmpleadoId == e.EmpleadoId); // con el FirstOrDefault voy a buscar ese objeto en la BD con landa
            if (empleadoBD == null) // cuando lo encuestra lo guarda en empleadoBD
                return false;
            // los piso con valores que vienen por parametro con los nuevos valores osea con los que vienen por parametro
            empleadoBD.Direccion = e.Direccion;
            empleadoBD.Dni = e.Dni;
            empleadoBD.Salario = e.Salario;
            empleadoBD.FechaIngreso = e.FechaIngreso;
            empleadoBD.Nombre = e.Nombre;
            empleadoBD.dpto_id = e.dpto_id;
            empleadoBD.anulado = e.anulado;

            empleadosContext.SaveChanges(); // guardo en la base de dato con este metodo SaveChanges

            return true;
        }

        public static bool Anular(int id) // anular pasando el Id
        {
            empleadosContext = new AdminEmpleadosDBContext();
            // voy a la base de datos y busco el empleado por Id
            var empleadoBD = empleadosContext.empleado.FirstOrDefault(c => c.EmpleadoId == id);
            if (empleadoBD == null)
                return false;
            // paso 2 para anular seria setear en true el campo anulado
            empleadoBD.anulado = true;
            // paso 3: guardar los cambios
            empleadosContext.SaveChanges();

            return true;
        }
        //                                                           //                                                      // lo que tengo que explicar
        public static int DeleteAnulados()
        {
            empleadosContext = new AdminEmpleadosDBContext();

            if (empleadosContext.empleado == null)
            {
                //devuelvo cero porque no hay tabla para consultar (por ejemplo en una BD sin migraciones)
                return 0;
            }

            //busco todos los empleados marcados como anulados y los guardo en la lista solicitada
            List<Empleado> listaParaDeletear = empleadosContext.empleado
                .Where(emp => emp.anulado)
                .ToList();

            if (listaParaDeletear.Count == 0)
            {
                //si la lista esta vacia no hay nada para borrar, retorno cero
                return 0;
            }

            //elimino todos los empleados anulados usando RemoveRange como pidio el requerimiento
            empleadosContext.empleado.RemoveRange(listaParaDeletear);

            //persisto los cambios en la base de datos
            empleadosContext.SaveChanges();

            //retorno la cantidad de registros eliminados para informar al usuario
            return listaParaDeletear.Count;
        }
    }
}
