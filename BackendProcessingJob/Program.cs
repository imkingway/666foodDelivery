using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BackendProcessingJob
{
    class Program
    {
        const string ServiceBusConnectionString = "Endpoint=sb://fooddelivery666-servicebus.servicebus.windows.net/;SharedAccessKeyName=666food;SharedAccessKey=qpeG4lyDhA5G5p8ou2sqioBH8g1e8pg4Hj8Ylb4/Qk8=";
        const string QueueName = "foodorder";
        static IQueueClient queueClient;

        static void Main()
        {
            var config = new JobHostConfiguration();

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            var host = new JobHost(config);
            ProcessMsg();
            host.RunAndBlock();
        }

        public static void insertIntoDB(string query) {
            using (SqlConnection conn = new SqlConnection("Server=tcp:666fooddelivery.database.windows.net,1433;Initial Catalog=666foodDelivery;Persist Security Info=False;User ID=user123;Password=Pass@word0*;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;")) {
                conn.Open();
                using(SqlCommand cmd = new SqlCommand(query, conn))
                {
                    int row = cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        public static async Task ProcessMsg()
        {
            await Task.Factory.StartNew(() =>
            {
                queueClient = new QueueClient(ServiceBusConnectionString, QueueName, ReceiveMode.PeekLock);
                var options = new MessageHandlerOptions(ExceptionMethod)
                {
                    MaxConcurrentCalls = 1,
                    AutoComplete = false
                };
                queueClient.RegisterMessageHandler(ExecuteMessageProcessing, options);
            });
        }

        private static async Task ExecuteMessageProcessing(Message message, CancellationToken arg2)
        {
            Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");
            string item = Encoding.UTF8.GetString(message.Body);
            Order order = JsonConvert.DeserializeObject<Order>(item);          

            string query = $"INSERT INTO [dbo].[Order] (CustomerName, Address, PhoneNumber, FoodName, DeliverTime, quantity) VALUES('{order.CustomerName}','{order.Address}','{order.PhoneNumber}','{order.FoodName}',GETDATE(),'{order.Quantity}')";
            insertIntoDB(query);
            await queueClient.CompleteAsync(message.SystemProperties.LockToken);

        }

        private static async Task ExceptionMethod(ExceptionReceivedEventArgs arg)
        {
           await Task.Run(() =>
            Console.WriteLine($"Error occured. Error is {arg.Exception.Message}")
           );
        }
    }
}
