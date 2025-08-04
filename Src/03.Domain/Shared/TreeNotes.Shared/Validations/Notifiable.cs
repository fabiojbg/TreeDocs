using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain.Shared.Validations
{
    //baseado no projeto https://github.com/andrebaltieri/flunt/wiki
    public abstract class Notifiable
    {
        private readonly List<Notification> _notifications;

        public Notifiable() => _notifications = new List<Notification>();

        public IReadOnlyCollection<Notification> Notifications => _notifications;

        public IReadOnlyCollection<Notification> AddNotification(string message)
        {
            _notifications.Add(new Notification("InvalidProperty", message));
            return Notifications;
        }

        public IReadOnlyCollection<Notification> AddNotification(string property, string message)
        {
            _notifications.Add(new Notification(property, message));
            return Notifications;
        }

        public IReadOnlyCollection<Notification> AddNotification(Notification notification)
        {
            _notifications.Add(notification);
            return Notifications;
        }

        public IReadOnlyCollection<Notification> AddNotifications(IEnumerable<Notification> notifications)
        {
            _notifications.AddRange(notifications);
            return Notifications;
        }

        public IReadOnlyCollection<Notification> AddNotifications(Notifiable item)
        {
            AddNotifications(item.Notifications);
            return Notifications;
        }

        public IReadOnlyCollection<Notification> AddNotifications(params Notifiable[] items)
        {
            foreach (var item in items)
                AddNotifications(item);
            return Notifications;
        }

        public IReadOnlyCollection<Notification> AddNotificationsWithNewName(string newName, IEnumerable<Notification> notifications)
        {
            foreach( var notification in notifications)
                _notifications.Add(new Notification(newName, notification.Message));
            return Notifications;
        }

        public IReadOnlyCollection<Notification> AddNotificationsWithNewName(string newName, Notifiable item)
        {
            AddNotificationsWithNewName(newName, item.Notifications);
            return Notifications;
        }

        public void ClearNotifications()
        {
            _notifications.Clear();
        }

        public bool Invalid => _notifications.Any();
        public bool Valid => !Invalid;
        public bool HasNotifications => _notifications.Any();
    }
}
