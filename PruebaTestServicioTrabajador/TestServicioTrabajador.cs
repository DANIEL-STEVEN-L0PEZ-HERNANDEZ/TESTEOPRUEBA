using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTestServicioTrabajador
{
    [TestClass]
    public class TestServicioTrabajador
    {
        private IWebDriver driver;
        private const string AppUrl = "http://frontend-beautysaly.somee.com/";

        // Propiedad para el contexto de la prueba
        public TestContext TestContext { get; set; }

        // Inicializa el controlador del navegador
        [TestInitialize]
        public void Setup()
        {
            driver = new EdgeDriver();

        }

        [TestMethod]
        public void TestLoginAdmin()
        {
            driver.Navigate().GoToUrl(AppUrl);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            try
            {
                // Asegurarse de estar en el frame principal
                driver.SwitchTo().DefaultContent();

                // Esperar que exista el input email
                wait.Until(ExpectedConditions.ElementExists(By.Id("inputemail")));

                // Ingresar datos directamente con JS (más estable que .SendKeys)
                js.ExecuteScript("document.getElementById('inputemail').value='marvinadmin@gmail.com';");
                js.ExecuteScript("document.getElementById('inputpassword').value='12345';");

                // Click al botón con JS
                js.ExecuteScript("document.getElementById('btnIngresar').click();");

                // Esperar la redirección
                wait.Until(ExpectedConditions.UrlContains("/sobrenosotros"));

                TestContext.WriteLine("✅ Login exitoso. Se redirigió a 'Sobre Nosotros'.");
            }
            catch (Exception ex)
            {
                TestContext.WriteLine("❌ Error durante el login: " + ex.Message);
                Assert.Fail("Fallo al iniciar sesión: " + ex.Message);
            }
        }

        [TestMethod]
        public void BuscarServicioTrabajadorTest()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            string idAsignacion = "3";

            driver.Navigate().GoToUrl(AppUrl);

            // LOGIN
            wait.Until(ExpectedConditions.ElementExists(By.Id("inputemail")));
            js.ExecuteScript("document.getElementById('inputemail').value='marvinadmin@gmail.com';");
            js.ExecuteScript("document.getElementById('inputpassword').value='12345';");
            js.ExecuteScript("document.getElementById('btnIngresar').click();");

            // 🔹 Esperar redirección a /sobrenosotros
            wait.Until(ExpectedConditions.UrlContains("/sobrenosotros"));

            // Validar URL
            if (!driver.Url.Contains("sobrenosotros"))
            {
                Assert.Fail("❌ No se pudo acceder a la página 'Sobre Nosotros'. URL actual: " + driver.Url);
            }

            // 🔹 Esperar el botón de asignaciones
            try
            {
                IWebElement BtnAsignacion = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("btnAsignacion")));
                js.ExecuteScript("arguments[0].click();", BtnAsignacion);
            }
            catch (WebDriverTimeoutException)
            {
                Assert.Fail("❌ No se encontró el botón 'btnAsignacion' — revisa el id en la vista o la ruta de navegación.");
            }

            // 🔹 Buscar asignación
            IWebElement InputBuscar = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("inputBuscarId")));
            InputBuscar.Clear();
            InputBuscar.SendKeys(idAsignacion);

            driver.FindElement(By.Id("btnBuscar")).Click();

            try
            {
                By selectorFilaID = By.XPath($"//table//tr[td[text()='{idAsignacion}']]");
                IWebElement filaAsignacion = wait.Until(ExpectedConditions.ElementIsVisible(selectorFilaID));
                Assert.IsTrue(filaAsignacion.Displayed);
                TestContext.WriteLine("✅ Asignación encontrada correctamente.");
            }
            catch (Exception)
            {
                Assert.Fail("❌ No se encontró la asignación buscada.");
            }
        }




        [TestCleanup]
        public void Cleanup()
        {
            driver.Quit();
        }
    }
}
