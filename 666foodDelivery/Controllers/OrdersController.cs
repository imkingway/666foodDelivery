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
using _666foodDelivery.Entities.Data;
using _666foodDelivery.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace _666foodDelivery.Views.Orders_SB
{

    public class OrdersController : Controller
    {
        const string ServiceBusConnectionString = "Endpoint=sb://fooddelivery666-servicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=HYNGfz8LyamgvCTuRQbiRoaZr3oWzrI2DdRp2ZPtHWE=";
        const string QueueName = "foodorder";

        private readonly _666foodDeliveryNewContext _context;
        private readonly UserManager<_666foodDeliveryUser> _userManager;

        public OrdersController(_666foodDeliveryNewContext _context, UserManager<_666foodDeliveryUser> userManager)
        {
            _userManager = userManager;
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
        public async Task<ActionResult> Index([Bind("CustomerName, Address, PhoneNumber, FoodName, DeliverTime, Quantity")] Order order)
        {
            QueueClient queue = new QueueClient(ServiceBusConnectionString, QueueName);
            if (ModelState.IsValid)
            {
                order.DeliverTime = DateTime.Now;
                var orderJSON = JsonConvert.SerializeObject(order);
                var message = new Message(Encoding.UTF8.GetBytes(orderJSON))
                {
                    MessageId = Guid.NewGuid().ToString(),
                    ContentType = "application/json"
                };
                await queue.SendAsync(message);
                order order1 = new order();

                //order1.CustomerName = order.CustomerName;
                //order1.Address = order.Address;
                //order1.PhoneNumber = order.PhoneNumber;
                //order1.FoodName = order.FoodName;
                //order1.DeliverTime = order.DeliverTime;
                //order1.Quantity = order.Quantity;
                //_context.Order.AddRange(order);
                //_context.SaveChanges();

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

        public IActionResult ReceivedOrders()
        {
            Job asd = _context.Job.Where(x => x.DriverID.Equals(_userManager.GetUserId(User))).FirstOrDefault();
            if (asd != null)
            {
                return RedirectToAction("Accept", "Orders");
            }
            else
            {

            }
            var OrderList = _context.Order.ToList();
            var asdasd = _context.Food.ToList();
            return View(OrderList);
        }

        //public async Task<ActionResult> ReceivedOrders()
        //{
        //    var managementClient = new ManagementClient(ServiceBusConnectionString);
        //    var queue = await managementClient.GetQueueRuntimeInfoAsync(QueueName);
        //    List<Order> messages = new List<Order>();
        //    List<long> sequence = new List<long>();
        //    MessageReceiver messageReceiver = new MessageReceiver(ServiceBusConnectionString, QueueName);
        //    for (int i = 0; i < queue.MessageCount; i++)
        //    {
        //        Message message = await messageReceiver.PeekAsync();
        //        Order result = JsonConvert.DeserializeObject<Order>(Encoding.UTF8.GetString(message.Body));
        //        sequence.Add(message.SystemProperties.SequenceNumber);
        //        messages.Add(result);
        //        Console.WriteLine(result);
        //    }

        //    ViewBag.sequence = sequence;
        //    ViewBag.messages = messages;


        //    return View();
        //}

        public async Task<ActionResult> AcceptOrder(string id)
        {
            //string customerName, string address, string phoneNumber, string foodName, string deliverTime, string quantity

            ////connect to the same queue             
            //var managementClient = new ManagementClient(ServiceBusConnectionString);
            //var queue = await managementClient.GetQueueRuntimeInfoAsync(QueueName);
            ////receive the selected message             
            //MessageReceiver messageReceiver = new MessageReceiver(ServiceBusConnectionString, QueueName);
            //Order result = null;
            //for (int i = 0; i < queue.MessageCount; i++)
            //{
            //    Message message = await messageReceiver.ReceiveAsync();
            //    string token = message.SystemProperties.LockToken;
            //    string token = message.SystemProperties.LockToken;
            //    //to find the selected message - read and remove from the queue                 
            //    if (message.SystemProperties.SequenceNumber == sequence)
            //    {
            //        result = JsonConvert.DeserializeObject<Order>(Encoding.UTF8.GetString(message.Body));
            //        await messageReceiver.CompleteAsync(token);
            //        break;
            //    }

            //}
            Order order = _context.Order.Where(x => x.ID.Equals(int.Parse(id))).FirstOrDefault();
            Job job = new Job { 
                DriverID = _userManager.GetUserId(User),
                CustomerName = order.CustomerName,
                Address = order.Address,
                PhoneNumber = order.PhoneNumber,
                FoodName = order.FoodName,
                DeliverTime = order.DeliverTime.ToString(),
                Quantity = order.Quantity.ToString()
            };
            _context.Job.AddRange(job);
            _context.Order.RemoveRange(order);
            _context.SaveChanges();
            return RedirectToAction("Accept","Orders");
        }

        public IActionResult Accept() {
            Job list = _context.Job.Where(X => X.DriverID.Equals(_userManager.GetUserId(User))).FirstOrDefault();
            return View(list);
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

        public IActionResult Finish(string id) {
            if (id.Equals(string.Empty)) {
                return BadRequest();
            }
            else {
                Job job = _context.Job.Where(x => x.DriverID.Equals(id)).FirstOrDefault();
                _context.Job.RemoveRange(job);
                _context.SaveChanges();
                return RedirectToAction("Index","Driver", new { response="success"});
            }
        }
    }
}
