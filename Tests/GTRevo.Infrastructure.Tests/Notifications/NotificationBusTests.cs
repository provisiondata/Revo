﻿using System.Threading.Tasks;
using GTRevo.Infrastructure.Notifications;
using NSubstitute;
using Xunit;

namespace GTRevo.Infrastructure.Tests.Notifications
{
    public class NotificationBusTests
    {
        private readonly NotificationBus sut;
        private readonly INotificationChannel notificationChannel1;
        private readonly INotificationChannel notificationChannel2;

        public NotificationBusTests()
        {
            notificationChannel1 = Substitute.For<INotificationChannel>();
            notificationChannel1.NotificationTypes.Returns(new[] {typeof(Notification1)});
            notificationChannel2 = Substitute.For<INotificationChannel>();
            notificationChannel2.NotificationTypes.Returns(new[] {typeof(Notification2)});
            sut = new NotificationBus(new[] {notificationChannel1, notificationChannel2});
        }

        [Fact]
        public async Task PushNotification_SendsToCorrectChannels()
        {
            Notification1 n1 = new Notification1();
            Notification2 n2 = new Notification2();
            await sut.PushNotification(n1);
            await sut.PushNotification(n2);

            notificationChannel1.Received(1).SendNotificationAsync(n1);
            notificationChannel2.Received(1).SendNotificationAsync(n2);
        }

        [Fact]
        public async Task PushNotification_SendsDerivedToCorrectChannel()
        {
            Notification2Derived n2 = new Notification2Derived();
            await sut.PushNotification(n2);

            notificationChannel1.ReceivedWithAnyArgs(0).SendNotificationAsync(null);
            notificationChannel2.Received(1).SendNotificationAsync(n2);
        }

        [Fact]
        public async Task PushNotifications_SendsToCorrectChannels()
        {
            Notification1 n1First = new Notification1();
            Notification1 n1Second = new Notification1();
            Notification2 n2 = new Notification2();
            await sut.PushNotifications(new INotification[] {n1First, n2, n1Second});

            notificationChannel1.Received(1).SendNotificationAsync(n1First);
            notificationChannel1.Received(1).SendNotificationAsync(n1Second);
            notificationChannel2.Received(1).SendNotificationAsync(n2);
        }

        [Fact]
        public async Task PushNotifications_SendsDerivedToCorrectChannel()
        {
            Notification2Derived n2 = new Notification2Derived();
            await sut.PushNotifications(new INotification[] { n2 });

            notificationChannel1.ReceivedWithAnyArgs(0).SendNotificationAsync(null);
            notificationChannel2.Received(1).SendNotificationAsync(n2);
        }

        public class Notification1 : INotification
        {
        }

        public class Notification2 : INotification
        {
        }

        public class Notification2Derived : Notification2
        { 
        }
    }
}
