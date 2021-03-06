﻿#region Includes

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

#endregion

namespace Daishi.Pluralsight.EventHub.ConsoleApp {
    internal static class Program {
        private static void Main(string[] args) {
            MainAsync().Wait();
        }

        private static async Task MainAsync() {
            Console.ForegroundColor = ConsoleColor.Green;

            var eventHubConnectionString =
                ConfigurationManager.AppSettings["EventHubConnectionString"];

            EventHubToolbox.Instance.Connect(eventHubConnectionString);

            if (EventHubToolbox.Instance.IsConnected) {
                Console.WriteLine(@"Connected OK!");
            }

            var eventHubConnectionStringShort =
                ConfigurationManager.AppSettings["EventHubConnectionStringShort"];
            var eventHubName =
                ConfigurationManager.AppSettings["EventHubName"];
            var storageAccountName =
                ConfigurationManager.AppSettings["StorageAccountName"];
            var storageAccountKey =
                ConfigurationManager.AppSettings["StorageAccountKey"];

            var eventReceiver = new EventReceiver(TimeSpan.FromMinutes(5));
            eventReceiver.NotificationReceived += EventReceiver_NotificationReceived;
            eventReceiver.EventReceived += EventReceiver_EventReceived;

            var eventProcessorOptions = new EventProcessorOptions();
            eventProcessorOptions.ExceptionReceived += EventProcessorOptions_ExceptionReceived;

            const string eventProcessorHostName = "MyEventHost";

            await EventHubToolbox.Instance.SubscribeAsync(
                "MyEventHost",
                eventHubConnectionStringShort,
                eventHubName,
                storageAccountName,
                storageAccountKey,
                eventReceiver,
                EventHubToolbox.UnRegisterAsync,
                EventHubToolbox.RegisterAsync,
                false,
                eventProcessorOptions);

            if (EventHubToolbox.Instance.IsSubscribedToAny) {
                Console.WriteLine(@"Subscribed!");
            }

            await EventHubToolbox.Instance.SendAsync("TEST");

            var events = new List<string> {
                "Event1",
                "EVent2"
            };

            EventHubToolbox.Instance.SendBatch(events);
            await EventHubToolbox.Instance.SendBatchAsync(events);

            Console.ReadLine();
            await EventHubToolbox.Instance.UnSubscribeAsync(
                eventProcessorHostName,
                EventHubToolbox.UnRegisterAsync);
            Console.ReadLine();
        }

        private static void EventReceiver_EventReceived(object sender,
            EventReceivedEventArgs e) {
            Console.WriteLine(@"Event received: {0}", e.Event);
        }

        private static void EventReceiver_NotificationReceived(
            object sender,
            NotificationReceivedEventArgs e) {
            Console.WriteLine(@"Notification received: {0}\r\nPartition: {1}",
                e.Notificaction, e.PartitionId);
        }

        private static void EventProcessorOptions_ExceptionReceived(
            object sender,
            ExceptionReceivedEventArgs e) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.Exception.Message);
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }
}