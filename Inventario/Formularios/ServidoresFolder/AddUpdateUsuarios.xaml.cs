﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DaoProject.Dao;
using DaoProject.Model;
using DaoProject.Singleton;
using DaoProject.Utilities;
using ScjnUtilities;

namespace Inventario.Formularios.ServidoresFolder
{
    /// <summary>
    /// Interaction logic for AddUpdateUsuarios.xaml
    /// </summary>
    public partial class AddUpdateUsuarios
    {
        private ServidoresPublicos servidor;
        private readonly bool isUpdating;

        public AddUpdateUsuarios()
        {
            InitializeComponent();
            servidor = new ServidoresPublicos();
            ChkDesactivar.Visibility = System.Windows.Visibility.Hidden;
        }

        public AddUpdateUsuarios(ServidoresPublicos servidor)
        {
            InitializeComponent();
            this.servidor = servidor;
            isUpdating = true;

        }

        private void RadWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RbtnBuscar.Visibility = (isUpdating) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            RbtnAceptar.Content = (isUpdating) ? "Actualizar" : "Aceptar";

            this.DataContext = servidor;
            this.RcbTitulo.DataContext = TitulosSingleton.Titulos;
            this.RcbTitulo.SelectedValue = servidor.IdTitulo;
            this.RcbArea.DataContext = AreasSingleton.Areas;
            this.RcbArea.SelectedValue = servidor.IdArea;
            this.RcbUbicacion.DataContext = UbicacionesSingleton.Ubicaciones;
            this.RcbUbicacion.SelectedValue = servidor.IdUbicacion;
        }

        private void RbtnBuscar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RbtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            servidor.IdTitulo = Convert.ToInt32(RcbTitulo.SelectedValue);
            servidor.IdArea = Convert.ToInt32(RcbArea.SelectedValue);
            servidor.IdUbicacion = Convert.ToInt32(RcbUbicacion.SelectedValue);

            if (isUpdating)
            {
                ServidoresModel model = new ServidoresModel(servidor);

                if (ChkDesactivar.IsChecked == true)
                    model.DesactivarUsuario();
                else
                    model.ActualizaInfoServidores();
                this.Close();
            }
            else
            {
             int userId =  new ServidoresModel(servidor).SetNewServidor();

             switch (userId)
             {
                 case -1:
                     MessageBox.Show("Ya existe el número de inventario.\\nVerifique por favor.");
                     break;
                 case -2:
                     MessageBox.Show("El servidor público que intenta registrar, ya se encuentra registrado");
                     break;
                 default:
                     MessageBox.Show("Registro exitoso.\\nExpediente: " + userId.ToString());
                     ServidoresSingleton.Servidores.Add(servidor);
                     this.Close();
                     break;
             }

            }
            
        }

        private void RbtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TxtExtension_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = StringUtilities.IsTextAllowed(e.Text);
        }

        private void ChkDesactivar_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ChkDesactivar_Unchecked(object sender, RoutedEventArgs e)
        {

        }


    }
}