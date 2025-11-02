using AdminEmpleadosEntidades;
using AdminEmpleadosNegocio;
using System.ComponentModel;


namespace AdminEmpleadosFront
{
    public partial class FrmEditEmpleados : Form
    {
        public EnumModoForm modo = EnumModoForm.Alta;  // utilizo enumeraciones f12 entramos

        public Empleado _empleado = new Empleado();

        public FrmEditEmpleados()//CONSTRUCTOR: porque no devuelve nada (void / String) y se llama igual que la clase - se ejecuata apenas hago el new
        {
            InitializeComponent();
        }

        private void FrmEditEmpleados_Load(object sender, EventArgs e)
        {
            CargarComboDepartamento();

            if (modo == EnumModoForm.Alta)
            {
                LimpiarControles(); // llamo al metodo limpiar
                HabilitarControles(true);//llamo al metodo habilitar controles y es un true osea los habilitos
            }
            if (modo == EnumModoForm.Modificacion)
            {
                HabilitarControles(true);
                CargarDatos();
            }
            if (modo == EnumModoForm.Consulta)
            {
                HabilitarControles(false); // llamo al metodo habilitar controles para desabilitarlos por eso es un false
                CargarDatos(); //cargo los datos
                btnAceptar.Enabled = false;
            }
        }

        private void btnAceptar_Click(object sender, EventArgs e) // cuando acepto en el modificar de editar empleado con todos los campos con los nuevos valores
        {
            Guardar(); // llamo al guardar

        }

        private void Guardar()
        {
            try
            {
                //cargo (tomo) los datos del formulario ingresados, en un objeto empleado
                Empleado emp = new Empleado(); // lo guardo en una variable objeto de tipo empleado creando este objeto new
                emp.Salario = txtSalario.Value; // le asigno todo lo que viene de los controloes
                emp.Direccion = txtDireccion.Text.Trim();// le asigno todo lo que viene de los controloes
                emp.Dni = txtDni.Text.Trim();// le asigno todo lo que viene de los controloes
                emp.FechaIngreso = txtIngreso.Value;// le asigno todo lo que viene de los controloes
                emp.Departamento = null;// le asigno todo lo que viene de los controloes
                emp.Nombre = txtNombre.Text.Trim();// le asigno todo lo que viene de los controloes
                emp.anulado = false;

                //tomo el ID del departamento, el cual esta en el combo /// esto es para mostrar o mejor dicho guardar los departamentos 
                emp.dpto_id = (int)cmbDepartamento.SelectedValue;

                string mensajeErrores = "";
                //realizo validaciones. El mensaje va por referencia
                if (!ValidarEmpleado(ref mensajeErrores, emp))
                {
                    //si falla alguna validacion muestro el mensaje y no hago nada mas
                    MessageBox.Show("Atención: Se encontraron los siguientes errores \n" + mensajeErrores, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;

                }

                //las validaciones estan bien
                //pregunto si quiere guardar los datos
                DialogResult res = MessageBox.Show("¿Confirma guardar?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.No)
                {
                    return;
                }

                //Guardo los datos
                if (modo == EnumModoForm.Alta)
                {// me muevo a la capa EmpleadoNegocio he le inserto todo
                    int idEmp = EmpleadosNegocio.Insert(emp);  // finalmente llamo a la clase empleado negocio y al metodo INSERT (MET ESTATIC)
                    txtId.Text = idEmp.ToString();
                    MessageBox.Show("Se generó el empleado nro " + idEmp.ToString(), "Empleado creado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                if (modo == EnumModoForm.Modificacion)
                {
                    emp.EmpleadoId = Convert.ToInt32(txtId.Text);// aca tomo el Id para modificarlo este ID viene de la grilla

                    EmpleadosNegocio.Update(emp); // llamamo a la capa de negocio
                    MessageBox.Show("Se actualizaron los datos correctamente", "Empleado actualizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();

                }



                LimpiarControles();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarEmpleado(ref string mensaje, Empleado e)
        {
            mensaje = "";

            if (String.IsNullOrEmpty(e.Dni.Trim()))
            {
                mensaje += "\nError en DNI";

            }

            if (String.IsNullOrEmpty(e.Nombre.Trim()))
            {
                mensaje += "\nError en Nombre";

            }
            if (!String.IsNullOrEmpty(mensaje))
            {
                return false;
            }

            return true;

        }

        private void LimpiarControles()
        {
            txtId.Text = "";
            txtSalario.Value = 0;
            txtDireccion.Text = "";
            txtDni.Text = "";
            txtIngreso.Value = DateTime.Now;
            txtNombre.Text = "";
        }



        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void HabilitarControles(bool habilitar) // recibe un buleano
        {

            txtSalario.Enabled = habilitar;
            txtDireccion.Enabled = habilitar;
            txtDni.Enabled = habilitar;
            txtIngreso.Enabled = habilitar;
            txtNombre.Enabled = habilitar;
            cmbDepartamento.Enabled = habilitar;
        }

        private void CargarDatos() // cada control le asigno los datos que ya estan seleccionados previamente en la propiedad _empleado (bindinSource)
        {
            txtId.Text = _empleado.EmpleadoId.ToString();
            txtSalario.Value = Convert.ToDecimal(_empleado.Salario);
            txtDireccion.Text = _empleado.Direccion;
            txtDni.Text = _empleado.Dni;
            if (_empleado.FechaIngreso != null)
                txtIngreso.Value = Convert.ToDateTime(_empleado.FechaIngreso);
            txtNombre.Text = _empleado.Nombre;

            if (_empleado.dpto_id != null)
                cmbDepartamento.SelectedValue = _empleado.dpto_id;

            // tambien podria ser....             
            // cmbDepartamento.SelectedValue = _empleado.Departamento.id;

        }

        private void CargarComboDepartamento()
        {
            //envio por parametro un departamento sin datos, asi va sin filtro y trae todos los dptos            
            departamentoBindingSource.DataSource = DepartamentosNegocio.Get(); // como el combo esta asociado al BindingSource se va a actualizar
        }

        private void txt_Validating(object sender, CancelEventArgs e)
        {
            errorProvider1.Clear();
            if (String.IsNullOrEmpty(txtDni.Text.Trim()))
            {
                errorProvider1.SetError(txtDni, "Ingrese el DNI");
            }
            if (String.IsNullOrEmpty(txtNombre.Text.Trim()))
            {
                errorProvider1.SetError(txtNombre, "Ingrese el nombre");
            }
        }

    }
}
