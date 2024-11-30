using System.Net.Http;
using System.Text;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography;
using System.Net;

namespace factura_afip_test
{
    public partial class Form1 : Form
    {
        string signedFilePathR = string.Empty;

        public Form1()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();

            // Crear el elemento raíz <TRA>
            XmlElement root = doc.CreateElement("TRA", "http://ar.gov.afip.dia.wsaa");
            doc.AppendChild(root);

            // Crear el Header
            XmlElement header = doc.CreateElement("Header", "http://ar.gov.afip.dia.wsaa");
            root.AppendChild(header);

            // Añadir elementos al Header
            XmlElement uniqueId = doc.CreateElement("uniqueId", "http://ar.gov.afip.dia.wsaa");
            uniqueId.InnerText = "123456"; // Identificador único
            header.AppendChild(uniqueId);

            XmlElement generationTime = doc.CreateElement("generationTime", "http://ar.gov.afip.dia.wsaa");
            generationTime.InnerText = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            header.AppendChild(generationTime);

            XmlElement expirationTime = doc.CreateElement("expirationTime", "http://ar.gov.afip.dia.wsaa");
            expirationTime.InnerText = DateTime.UtcNow.AddHours(12).ToString("yyyy-MM-ddTHH:mm:ssZ");
            header.AppendChild(expirationTime);

            // Añadir el elemento <service>
            XmlElement service = doc.CreateElement("service", "http://ar.gov.afip.dia.wsaa");
            service.InnerText = "wsfe"; // Servicio solicitado
            root.AppendChild(service);

            // Guardar el archivo TRA con encoding UTF-8
            string filePath = "TRA.xml";
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                doc.Save(writer);
            }

            MessageBox.Show("Archivo TRA generado correctamente: " + filePath);
        }

        private void FirmarTRA_Click(object sender, EventArgs e)
        {
            string traFilePath = "TRA.xml"; // Ruta del archivo TRA
            XmlDocument doc = new XmlDocument();
            doc.Load(traFilePath);

            try
            {
                // Cargar el certificado y clave privada
                X509Certificate2 cert = new X509Certificate2("certificado.pfx", "1528");
                RSA rsa = cert.GetRSAPrivateKey();

                SignedXml signedXml = new SignedXml(doc);
                signedXml.SigningKey = rsa;

                // Crear una referencia al nodo raíz del XML
                Reference reference = new Reference();
                reference.Uri = "";
                reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
                reference.AddTransform(new XmlDsigC14NTransform());
                signedXml.AddReference(reference);

                // Agregar la información del certificado al objeto SignedXml
                KeyInfo keyInfo = new KeyInfo();
                keyInfo.AddClause(new KeyInfoX509Data(cert));
                signedXml.KeyInfo = keyInfo;

                // Firmar el XML
                signedXml.ComputeSignature();

                // Añadir la firma al documento
                XmlElement xmlSignature = signedXml.GetXml();
                doc.DocumentElement.AppendChild(doc.ImportNode(xmlSignature, true));

                // Guardar el archivo firmado
                string signedFilePath = "TRA_firmado.xml";
                signedFilePathR = signedFilePath;
                doc.Save(signedFilePath);

                MessageBox.Show("Archivo TRA firmado y guardado: " + signedFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al firmar el TRA: " + ex.Message);
            }
        }

        private void solicitudSoap_Click(object sender, EventArgs e)
        {
            string wsaaUrl = "https://wsaahomo.afip.gov.ar/ws/services/LoginCms"; // URL del servicio WSAA
            string traFilePath = signedFilePathR;

            try
            {
                // Leer el archivo TRA firmado y convertirlo a Base64
                byte[] fileBytes = File.ReadAllBytes("TRA_firmado.xml");

                // Convertir a Base64
                string base64Tra = Convert.ToBase64String(fileBytes);

                // Incluir en el cuerpo del mensaje SOAP
                string soapEnvelope = $@"
    <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/""
                      xmlns:wsaa=""http://ar.gov.afip.dia.wsaa"">
       <soapenv:Header/>
       <soapenv:Body>
          <wsaa:loginCms>
             <wsaa:in0>{base64Tra}</wsaa:in0>
          </wsaa:loginCms>
       </soapenv:Body>
    </soapenv:Envelope>";

                byte[] byteArray = Encoding.UTF8.GetBytes(soapEnvelope);

                // Configurar la solicitud HTTP
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(wsaaUrl);
                request.Method = "POST";
                request.ContentType = "text/xml; charset=utf-8";
                request.ContentLength = byteArray.Length;

                // **Agregar el encabezado SOAPAction**
                request.Headers.Add("SOAPAction", "http://ar.gov.afip.dia.wsaa/LoginCms");

                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                // Obtener la respuesta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string responseXml = reader.ReadToEnd();
                    ProcessResponse(responseXml);
                }

                MessageBox.Show("Todo correcto, kpo");
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        string errorResponse = reader.ReadToEnd();
                        MessageBox.Show("Error WSAA: " + errorResponse);
                    }
                }
                else
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error general: " + ex.Message);
            }
        }


        private void ProcessResponse(string responseXml)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(responseXml);

                XmlNode tokenNode = doc.SelectSingleNode("//Token");
                XmlNode signNode = doc.SelectSingleNode("//Sign");

                if (tokenNode != null && signNode != null)
                {
                    string token = tokenNode.InnerText;
                    string sign = signNode.InnerText;
                    MessageBox.Show($"Token: {token}\nSign: {sign}");
                }
                else
                {
                    MessageBox.Show("Error: No se encontró el Token o el Sign en la respuesta.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar la respuesta: " + ex.Message);
            }
        }
    }
}
