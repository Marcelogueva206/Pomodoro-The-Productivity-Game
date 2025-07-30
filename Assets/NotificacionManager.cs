using Unity.Notifications.Android;
using UnityEngine;

public class NotificacionManager : MonoBehaviour
{
    public static NotificacionManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Para que no se destruya al cambiar de escena
        }
        else
        {
            Destroy(gameObject);
        }

        var channel = new AndroidNotificationChannel()
        {
            Id = "pomodoro_channel",
            Name = "Pomodoro Notificaciones",
            Importance = Importance.Default,
            Description = "Canal para recordatorios de sesiones pomodoro",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }


    public void ProgramarNotificacion(int segundos)
    {
        var notification = new AndroidNotification();
        notification.Title = "¡Fin de la sesión!";
        notification.Text = "Tu sesión de Pomodoro ha terminado 🕒.";
        notification.FireTime = System.DateTime.Now.AddSeconds(segundos);

        AndroidNotificationCenter.SendNotification(notification, "pomodoro_channel");
    }

    public void CancelarTodasNotificaciones()
    {
        AndroidNotificationCenter.CancelAllScheduledNotifications();
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
    }

}
