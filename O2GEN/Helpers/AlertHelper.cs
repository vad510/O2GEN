using Microsoft.AspNetCore.Http;
using O2GEN.Models;

namespace O2GEN.Helpers
{
    public static class AlertHelper
    {
        /// <summary>
        /// Сохраняем сообщение в сессию
        /// </summary>
        /// <param name="Session"></param>
        /// <param name="AlertType"></param>
        /// <param name="Message"></param>
        public static void SaveMessage(ISession Session, AlertType AlertType, string Message)
        {
            Session.SetString("AlertType", ((int)AlertType).ToString());
            Session.SetString("AlertMessage", Message);
        }
        /// <summary>
        /// Отправлям сообщение во ViewBag
        /// </summary>
        /// <param name="Session"></param>
        /// <param name="ViewBag"></param>
        public static void DisplayMessage(ISession Session, dynamic ViewBag)
        {
            if (!string.IsNullOrEmpty(Session.GetString("AlertType")))
            {
                ViewBag.AlertType = (AlertType)int.Parse(Session.GetString("AlertType"));
                ViewBag.AlertMessage = Session.GetString("AlertMessage");
                Session.SetString("AlertType", "");
                Session.SetString("AlertMessage", "");
            }
        }
    }
}
