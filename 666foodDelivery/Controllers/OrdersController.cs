using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using _666foodDelivery.Models;
using Newtonsoft.Json;
using Microsoft.Azure.ServiceBus.Core;
using _666foodDelivery.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace _666foodDelivery.Views.Orders_SB
{

    public class OrdersController : Controller
    {
        const string ServiceBusConnectionString = "Endpoint=sb://fooddelivery666-servicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=HYNGfz8LyamgvCTuRQbiRoaZr3oWzrI2DdRp2ZPtHWE=";
        const string QueueName = "foodorder";

        private readonly _666foodDeliveryNewContext _context;

        public OrdersController(_666foodDeliveryNewContext _context)
        {
            this._context = _context;
        }

        public async Task<IActionResult> Index()
        {
            var managementClient = new ManagementClient(ServiceBusConnectionString);
            var queue = await managementClient.GetQueueRuntimeInfoAsync(QueueName);
            ViewBag.MessageCount = queue.MessageCount;

            List<Food> food = new List<Food>();
            food = (from c in _context.Food select c).ToList();
            food.Insert(0, new Food { ID = 0, FoodName = "---Select A Food---" });
            ViewBag.message = food;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index([Bind("CustomerName, Address, PhoneNumber, FoodName, DeliverTime, Quantity")]Order order)
        {
            QueueClient queue = new QueueClient(ServiceBusConnectionString, QueueName);
            if (ModelState.IsValid)
            {
                var orderJSON = JsonConvert.SerializeObject(order);
                var message = new Message(Encoding.UTF8.GetBytes(orderJSON))
                {
                    MessageId = Guid.NewGuid().ToString(),
                    ContentType = "application/json"
                };
                await queue.SendAsync(message);
                return RedirectToAction("Index", "Orders", new { });
            }
            return View(order);
        }

        private static async Task CreateQueueFunctionAsync()
        {
            var managementClient = new ManagementClient(ServiceBusConnectionString);
            bool queueExists = await managementClient.QueueExistsAsync(QueueName);
            if (!queueExists)
            {
                QueueDescription qd = new QueueDescription(QueueName);
                qd.MaxSizeInMB = 1024; qd.MaxDeliveryCount = 3;
                await managementClient.CreateQueueAsync(qd);
            }
        }

        public static void Initialize()
        {
            CreateQueueFunctionAsync().GetAwaiter().GetResult();
        }

        public async Task<ActionResult> ReceivedOrders()
        {
            var managementClient = new ManagementClient(ServiceBusConnectionString);
            var queue = await managementClient.GetQueueRuntimeInfoAsync(QueueName);
            List<Order> messages = new List<Order>();
            List<long> sequence = new List<long>();
            MessageReceiver messageReceiver = new MessageReceiver(ServiceBusConnectionString, QueueName);
                for (int i = 0; i < queue.MessageCount; i++)
                {
                    Message message = await messageReceiver.PeekAsync();
                    Order result = JsonConvert.DeserializeObject<Order>(Encoding.UTF8.GetString(message.Body));
                    sequence.Add(message.SystemProperties.SequenceNumber);
                    messages.Add(result);
                }
            ViewBag.sequence = sequence;
            ViewBag.messages = messages;

            return View();
        }

        public async Task<ActionResult> Approve(long sequence, Order customerData)
        {   
            //connect to the same queue             
            var managementClient = new ManagementClient(ServiceBusConnectionString);
            var queue = await managementClient.GetQueueRuntimeInfoAsync(QueueName);
            //receive the selected message             
            MessageReceiver messageReceiver = new MessageReceiver(ServiceBusConnectionString, QueueName);
            Order result = null;
            for (int i = 0; i < queue.MessageCount; i++)
            {
                Message message = await messageReceiver.ReceiveAsync();
                string token = message.SystemProperties.LockToken;
                //to find the selected message - read and remove from the queue                 
                if (message.SystemProperties.SequenceNumber == sequence)
                {
                    result = JsonConvert.DeserializeObject<Order>(Encoding.UTF8.GetString(message.Body));               
                    await messageReceiver.CompleteAsync(token);
                    break;
                }
                
            }

            Console.Write(result);


            return View();
        }

        public async Task<ActionResult> Job(long sequence)
        {
            //connect to the same queue             
            var managementClient = new ManagementClient(ServiceBusConnectionString);
            var queue = await managementClient.GetQueueRuntimeInfoAsync(QueueName);
            //receive the selected message             
            MessageReceiver messageReceiver = new MessageReceiver(ServiceBusConnectionString, QueueName);
            Order result = null;
            for (int i = 0; i < queue.MessageCount; i++)
            {
                Message message = await messageReceiver.ReceiveAsync();
                string token = message.SystemProperties.LockToken;
                //to find the selected message - read and remove from the queue                 
                if (message.SystemProperties.SequenceNumber == sequence)
                {
                    result = JsonConvert.DeserializeObject<Order>(Encoding.UTF8.GetString(message.Body));
                    await messageReceiver.CompleteAsync(token);
                    break;
                }
            }
            return View();
        }
    }
}
