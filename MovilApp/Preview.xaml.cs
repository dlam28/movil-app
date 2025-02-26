﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MovilApp.Models;
using MovilApp.Controllers;
using Xamarin.Essentials;

namespace MovilApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Preview : ContentPage
    {
        Products products = new Products();
        Cuot miCuota = new Cuot();

        decimal Monto;

        public Preview(Cuot cuot)
        {
            InitializeComponent();

            lstPreview.ItemsSource = Products.carrito;

            miCuota.TIPO_CUOTA = cuot.TIPO_CUOTA;
            miCuota.TASA_INTERES = cuot.TASA_INTERES;

            labelTipoPlan.Text = $" Tipo de Plan: {cuot.TIPO_CUOTA}";
            labelInteres.Text = $" Tasa de Interés: {cuot.TASA_INTERES.ToString()}";

            labelTotal.Text = $"Precio total de los productos: {products.devolverPrecio()}";

            miCuota = cuot;

            Monto = calcularPrecioFinal(products.devolverPrecio(), cuot.TASA_INTERES);

            labelTotalInteres.Text = $"Total: {Monto}";


            btnTerminar.Clicked += btnTerminar_Clicked;

            BindingContext = this;

            //calcularPrecioFinal();
        }

        int meses;
        private void calcularPagoMensual()
        {

            switch (miCuota.CUOTA_ID)
            {
                case 1:
                    meses = 3;
                    break;

                case 2:
                    meses = 6;
                    break;
                case 3:
                    meses = 9;
                    break;
            }
        }



        private async void btnTerminar_Clicked(object sender, EventArgs e)
        {
            calcularPagoMensual();

            try
            {

                //Genera los datos de la factura
                FacturaManager facturaManager = new FacturaManager();
                Factura facturaIngresado = new Factura();
                Factura factura = new Factura()

                {

                    USUARIO_ID = App.usuarioSesionID,
                    PLAN_ID = miCuota.CUOTA_ID,
                    MONTO_FACTURA = Monto,
                    CANT_PRODUCTOS = Products.carrito.Count,
                    PAGO_MENSUAL = Monto / meses,
                    ESTADO = "1"

                };

                //Ingresa la factura a la base de datos
                facturaIngresado = await facturaManager.Ingresar(factura);

                App.facturaSesionID = await facturaManager.obtenerUltimoID();


                //Genera los datos de compra_producto
                Compra_productoManager compra_productoManager = new Compra_productoManager();
                Compra_producto compra_productoIngresado = new Compra_producto();


                foreach (Products product in Products.carrito)
                {
                    Compra_producto compra_producto = new Compra_producto()

                    {
                        FACTURA_ID = App.facturaSesionID,
                        PRODUCTO_ID = product.PRODUCTO_ID

                    };

                    //Ingresa la compra_producto a la base de datos
                    compra_productoIngresado = await compra_productoManager.Ingresar(compra_producto);

                }


                if (facturaIngresado != null && compra_productoIngresado != null)
                {
                    await DisplayAlert("Proceso de Compra realizado", "Redireccionando al correo para los detalles del envio de su factura", "Aceptar");
                    await Launcher.OpenAsync(new Uri($"mailto:{App.usuarioSesionEmail}?subject=MovilApp-Detalles de factura&body=Detalles de la factura\n" +
                        $"ID de Factura: {App.usuarioSesionID}\n" +
                        $"ID Plan: {miCuota.CUOTA_ID}\n" +
                        $"Monto de la factura: {Monto}\n" +
                        $"Cantidad de productos: {Products.carrito.Count}" +
                        $"Pago mensual: {Monto / meses}"));
                    await DisplayAlert("Factura generada", "Gracias por elegirnos", "Aceptar");
                    await ((NavigationPage)this.Parent).PushAsync(new DeBanco());
                }
                else
                {
                    await DisplayAlert("Proceso de Compra", "Error en la Compra del Carrito, intente denuevo", "Aceptar");
                }

            }
            catch (Exception)
            {
                await DisplayAlert("Proceso de Compra", "Error en la Compra del Carrito, intente denuevo", "Aceptar");
            }
        }






        //Generar el envio por correo



        private decimal calcularPrecioFinal(decimal totalPrecio, decimal tasaInteres)
        {
            decimal precioFinal = totalPrecio + (totalPrecio * tasaInteres);

            return precioFinal;
        }


    }
}

