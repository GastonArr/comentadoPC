using AdminEmpleadosEntidades;
using AdminEmpleadosNegocio;

namespace AdminEmpleadosFront
{
    public partial class FrmAdminEmpleados : Form  // herencia, es que la clase hereda de formulario
    {
        List<Empleado> empleadosList = new List<Empleado>();

        public FrmAdminEmpleados()
        {
            InitializeComponent();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            buscarEmpleados();
        }
        private void buscarEmpleados()
        {
            //Obtengo el nombre y DNI ingresado por el usuario
            string textoBuscar = txtBuscar.Text.Trim().ToUpper();

            //declaro el parametro
            Empleado parametro = new Empleado();

            //asigno el nombre ingresado
            if (!String.IsNullOrEmpty(textoBuscar.Trim()))
            {
                parametro.Nombre = textoBuscar;
                parametro.Dni = textoBuscar;
            }

            //seteo el nuevo filtro de anulados usando el valor del checkbox
            parametro.anulado = chkVerAnulados.Checked;

            //Busco la lista de empleados en la capa de negocio, pasandole el parametro ingresado
            empleadosList = EmpleadosNegocio.Get(parametro);
            //Actualizo la grilla
            refreshGrid();
        }

        private void refreshGrid()
        {
            //Actualizo el Binding con la lista de empleados que viene desde la BD
            empleadoBindingSource.DataSource = null;
            empleadoBindingSource.DataSource = empleadosList;

        }

        private void txtBuscar_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Llamo al metodo buscar al presionar la tecla "Enter"
            if (e.KeyChar == (char)Keys.Enter)
            {
                buscarEmpleados();
            }
        }

        private void btnAlta_Click(object sender, EventArgs e) // evento alta
        {
            FrmEditEmpleados frm = new FrmEditEmpleados(); // se llama al otro formulario instanciando osea declarando una variable en el otro form
            // se inicia en el contructor porque aca estoy instanciando a la clase (creando un objeto), osea llamando a dicha clase
            frm.modo = EnumModoForm.Alta; // le asigno el modo ALTA
            frm.ShowDialog();//modal para no perder el foco Y se va directamente al formulario de edicion de empleado (constructor) para mostrarlo

            buscarEmpleados();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (empleadoBindingSource.Current == null)
                return;

            FrmEditEmpleados frm = new FrmEditEmpleados();

            frm.modo = EnumModoForm.Modificacion;
            frm._empleado = (Empleado)empleadoBindingSource.Current;

            frm.ShowDialog();

            buscarEmpleados();
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            if (empleadoBindingSource.Current == null) //si no tomo algunelemento relacionado a la grilla me retorna
                return;

            FrmEditEmpleados frm = new FrmEditEmpleados(); // declaro el formulario de edicion de empleado

            frm.modo = EnumModoForm.Consulta; // le seteo el modo con la enumeracion en este caso es consulta
            frm._empleado = (Empleado)empleadoBindingSource.Current; // traigo el elemento relacionado en la grilla

            frm.ShowDialog(); // lo muestro para que sea modal (no pierda el foco)

            buscarEmpleados();
        }

        private void btnBaja_Click(object sender, EventArgs e)
        {
            if (empleadoBindingSource.Current == null)
                return;

            Empleado emp = (Empleado)empleadoBindingSource.Current; // tomo el empleado seleccionado en la grilla con la propiedad .Current que devulve todo el objeto seleccionado
            // y lo guardo en la propiedad emp, todo lo que me devolvio la grilla
            //pregunto si quiere guardar los datos
            DialogResult res = MessageBox.Show("¿Confirma anular el empleado " + emp.Nombre + " ?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);// muestro el mensaje
            if (res == DialogResult.No)
            {
                return;
            }

            try
            {
                EmpleadosNegocio.Anular((int)emp.EmpleadoId); // llamo a este metodo y le mando el ID
                MessageBox.Show("El empleado " + emp.Nombre + " se anuló correctamente", "Anulación", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }

            buscarEmpleados();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkVerAnulados_CheckedChanged(object sender, EventArgs e)
        {
            //cada vez que el usuario tilda o destilda el check actualizo la grilla para respetar el filtro
            buscarEmpleados();
        }

        private void btnBorrarAnulados_Click(object sender, EventArgs e)
        {
            //primer mensaje de advertencia para evitar borrar registros por error
            DialogResult confirmarPrimeraVez = MessageBox.Show("¿Desea quitar de la BD todos los empleados anulados?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirmarPrimeraVez != DialogResult.Yes)
            {
                //si la respuesta es No salgo del metodo
                return;
            }

            //segundo mensaje de confirmacion definitiva como pide el requerimiento
            DialogResult confirmarSegundaVez = MessageBox.Show("¿Confirma que va a borrar definitivamente de la BD los empleados anulados?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirmarSegundaVez != DialogResult.Yes)
            {
                //si la respuesta es No salgo del metodo
                return;
            }

            try
            {
                //ejecuto la eliminacion definitiva y guardo cuantos empleados fueron borrados
                int cantidadEliminados = EmpleadosNegocio.DeleteAnulados();

                //informo el resultado al usuario mostrando la cantidad que salio de la capa de datos
                MessageBox.Show($"Se borraron definitivamente {cantidadEliminados} empleados anulados.", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                //muestro cualquier error inesperado
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //refresco la grilla para que desaparezcan los empleados eliminados
            buscarEmpleados();
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
