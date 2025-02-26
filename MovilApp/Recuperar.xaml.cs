﻿using MovilApp.Controllers;
using MovilApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MovilApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Recuperar : ContentPage
    {

        UserManager usuarioManager = new UserManager();
        Login usuarioActualizado = new Login();


        public Recuperar()
        {
            InitializeComponent();

            btnCambiarClave.Clicked += btnCambiarClave_Clicked;

            btnVolver.Clicked += BtnVolver_Clicked;
        }

        private void BtnVolver_Clicked(object sender, EventArgs e)
        {
            ((NavigationPage)this.Parent).PushAsync(new MainPage());
        }

        private async void btnCambiarClave_Clicked(object sender, EventArgs e)
        {

            if (txtCorreo.Text != null &&
             txtOldpass.Text != null &&
             txtPass.Text != null &&
             txtPassConfirm.Text != null &&
             txtPass.Text == txtPassConfirm.Text)
            {
                Login login = new Login()
                {
                    Correo = txtCorreo.Text,
                    Password = txtPass.Text
                };

                usuarioActualizado = await usuarioManager.cambiarClave(login);

                if (usuarioActualizado != null)
                {
                    await DisplayAlert("Cambio de Contraseña", "Cambio de Contraseña correcto, Inicie Sesion", "Ir");
                    await ((NavigationPage)this.Parent).PushAsync(new MainPage());
                }
                else
                {
                    await DisplayAlert("Actualizacion de usuario", "Ocurrio un error", "Aceptar");
                }

            }
            else if (txtPass.Text != txtPassConfirm.Text)
            {
                await DisplayAlert("Cambio de Contraseña", "Las contraseñas no coinciden", "Ir");

            }
            else
            {
                await DisplayAlert("Cambio de Contraseña", "Cambio de Contraseña Incorrecto, Revise que todos los datos esten completos", "Volver");
            }




        }


    }
}