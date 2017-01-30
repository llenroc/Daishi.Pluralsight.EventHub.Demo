﻿using Microsoft.ServiceBus.Messaging;

namespace Daishi.Pluralsight.EventHub
{
    public class BridgeEventProcessorFactory : IEventProcessorFactory
    {
        private readonly IEventProcessor _eventReceiver;

        public BridgeEventProcessorFactory(IEventProcessor eventReceiver)
        {
            _eventReceiver = eventReceiver;
        }

        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {
            return _eventReceiver;
        }
    }
}