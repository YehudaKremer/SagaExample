using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefaultValues.Consumers
{
    public class UserConsumer : IConsumer<User>
    {
        public Task Consume(ConsumeContext<User> context)
        {
            throw new Exception("Dont want this message");
            Console.WriteLine(context.Message.Name);

            return Task.CompletedTask;
        }
    }
}
