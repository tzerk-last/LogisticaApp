using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace LogisticaApp.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var emailSettings = _config.GetSection("EmailSettings");
                var smtpHost = emailSettings["SmtpHost"];
                var smtpPort = int.Parse(emailSettings["SmtpPort"] ?? "587");
                var smtpUser = emailSettings["SmtpUser"];
                var smtpPass = emailSettings["SmtpPassword"];
                var fromEmail = emailSettings["FromEmail"];
                var fromName = emailSettings["FromName"];

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, fromEmail));
                message.To.Add(new MailboxAddress("", to));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = body };
                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(smtpUser, smtpPass);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                _logger.LogInformation($"Email enviado a {to} - Asunto: {subject}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al enviar email: {ex.Message}");
            }
        }

        public async Task SendShipmentCreatedAsync(string email, string userName, string guideNumber)
        {
            var subject = "✅ Envío Creado - LogisticaApp";
            var body = $@"
                <h2>Hola {userName},</h2>
                <p>Tu envío ha sido <strong>creado exitosamente</strong>.</p>
                <p><strong>Número de Guía:</strong> {guideNumber}</p>
                <p>Pronto será asignado a un conductor. Recibirás una notificación cuando esto suceda.</p>
                <p>Consulta el estado en: <a href='http://localhost:5126'>LogisticaApp</a></p>
                <br/>
                <p>Gracias por usar LogisticaApp.</p>
            ";
            await SendEmailAsync(email, subject, body);
        }

        public async Task SendShipmentAssignedAsync(string email, string userName, string guideNumber, string driverName)
        {
            var subject = "🚗 Envío Asignado - LogisticaApp";
            var body = $@"
                <h2>Hola {userName},</h2>
                <p>Tu envío <strong>{guideNumber}</strong> ha sido asignado.</p>
                <p><strong>Conductor:</strong> {driverName}</p>
                <p>El conductor iniciará su ruta próximamente. Puedes seguir el estado en tiempo real.</p>
                <br/>
                <p>Gracias por usar LogisticaApp.</p>
            ";
            await SendEmailAsync(email, subject, body);
        }

        public async Task SendDeliveryCompletedAsync(string email, string userName, string guideNumber)
        {
            var subject = "✔️ Entrega Realizada - LogisticaApp";
            var body = $@"
                <h2>Hola {userName},</h2>
                <p>Tu envío <strong>{guideNumber}</strong> ha sido <strong>entregado</strong>.</p>
                <p>El destinatario ha confirmado la recepción. Descarga tu comprobante de entrega.</p>
                <br/>
                <p>Gracias por usar LogisticaApp.</p>
            ";
            await SendEmailAsync(email, subject, body);
        }

        public async Task SendIncidentReportedAsync(string email, string driverName, string guideNumber, string incidentType)
        {
            var subject = "⚠️ Incidencia Reportada - LogisticaApp";
            var body = $@"
                <h2>Notificación de Incidencia</h2>
                <p>El conductor <strong>{driverName}</strong> ha reportado una incidencia.</p>
                <p><strong>Guía:</strong> {guideNumber}</p>
                <p><strong>Tipo:</strong> {incidentType}</p>
                <p>Por favor, revisa el caso y toma las acciones necesarias.</p>
                <br/>
                <p>LogisticaApp</p>
            ";
            await SendEmailAsync(email, subject, body);
        }

        public async Task SendValidationResultAsync(string email, string userName, string guideNumber, bool approved)
        {
            var subject = approved ? "✅ Entrega Validada" : "❌ Entrega Rechazada";
            var body = approved ? $@"
                <h2>Hola {userName},</h2>
                <p>Tu entrega <strong>{guideNumber}</strong> ha sido <strong>validada</strong>.</p>
                <p>Los datos de la evidencia fueron confirmados correctamente.</p>
                <br/>
                <p>Gracias por usar LogisticaApp.</p>
            " : $@"
                <h2>Hola {userName},</h2>
                <p>Tu entrega <strong>{guideNumber}</strong> requiere <strong>revisión manual</strong>.</p>
                <p>La validación automática detectó inconsistencias. Un operador revisará pronto.</p>
                <br/>
                <p>Gracias por usar LogisticaApp.</p>
            ";
            await SendEmailAsync(email, subject, body);
        }
    }
}
